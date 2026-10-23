using System.Linq;
using BepInEx.Configuration;
using EntityStates.Missions.Goldshores;
using EntityStates.TitanMonster;
using GoldenCoastPlusRevived.Buffs;
using GoldenCoastPlusRevived.Items;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Skills;
using RoR2BepInExPack.GameAssetPathsBetter;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GoldenCoastPlusRevived.Modules
{
    internal static class FightChanges
    {
        public static ConfigEntry<bool> EnableFightChanges { get; set; }
        public static ConfigEntry<float> BossHealthMult { get; set; }
        public static ConfigEntry<bool> UseAdaptiveArmor { get; set; }
        public static ConfigEntry<bool> UseGildedAffix { get; set; }

        private static float origMinDuration;
        private static float origMaxDuration;
        private static int origFistCount;
        private static float origProjectileFrequency;

        /// <summary>
        /// 1 = full health, 2 = below 2/3 health, 3 = below 1/3 health (enraged)
        /// </summary>
        private static int currentPhase = 1;

        internal static void Init()
        {
            if (!EnableFightChanges.Value)
                return;

            ModifyAssets();

            On.RoR2.GoldshoresMissionController.SpawnBeacons += GoldshoresMissionController_SpawnBeacons;
            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.OnEnter += GoldshoresBossfight_OnEnter;
            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.OnExit += GoldshoresBossfight_OnExit;
            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.SetBossImmunity += GoldshoresBossfight_SetBossImmunity;
            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.SpawnBoss += GoldshoresBossfight_SpawnBoss;
            IL.EntityStates.Missions.Goldshores.GoldshoresBossfight.ServerFixedUpdate += GoldshoresBossfight_ServerFixedUpdate;
            On.EntityStates.TitanMonster.FireGoldFist.PlacePredictedAttack += FireGoldFist_PlacePredictedAttack;
        }

        private static void GoldshoresMissionController_SpawnBeacons(On.RoR2.GoldshoresMissionController.orig_SpawnBeacons orig, GoldshoresMissionController self)
        {
            self.beaconsToSpawnOnMap = 4;

            orig(self);
        }

        private static void ModifyAssets()
        {
            var body = Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Titan.TitanGoldBody_prefab).WaitForCompletion().GetComponent<CharacterBody>();
            body.levelMaxHealth *= BossHealthMult.Value;

            Addressables.LoadAssetAsync<GameObject>(RoR2_Base_goldshores.GoldshoresBeacon_prefab).WaitForCompletion().GetComponent<PurchaseInteraction>().cost = 0;
        }

        private static void GoldshoresBossfight_SpawnBoss(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_SpawnBoss orig, GoldshoresBossfight self)
        {
            orig(self);
            currentPhase = 1;

            if (!self.scriptedCombatEncounter)
                return;

            foreach (var readOnlyMembers in self.scriptedCombatEncounter.combatSquad.readOnlyMembersList)
            {
                var body = readOnlyMembers.GetBody();
                if (body)
                {
                    body.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 2f);
                }
            }
        }

        private static void GoldshoresBossfight_OnExit(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_OnExit orig, GoldshoresBossfight self)
        {
            currentPhase = 1;
            FireGoldFist.fistCount = origFistCount;
            FireGoldMegaLaser.projectileFireFrequency = origProjectileFrequency;
            FireGoldMegaLaser.minimumDuration = origMinDuration;
            FireGoldMegaLaser.maximumDuration = origMaxDuration;

            orig(self);
        }

        private static void GoldshoresBossfight_OnEnter(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_OnEnter orig, GoldshoresBossfight self)
        {
            origFistCount = FireGoldFist.fistCount;
            FireGoldFist.fistCount = 6;

            origProjectileFrequency = FireGoldMegaLaser.projectileFireFrequency;
            FireGoldMegaLaser.projectileFireFrequency = 8f;

            origMinDuration = FireGoldMegaLaser.minimumDuration;
            FireGoldMegaLaser.minimumDuration = 1.5f;

            origMaxDuration = FireGoldMegaLaser.maximumDuration;
            FireGoldMegaLaser.maximumDuration = 3f;

            GoldshoresBossfight.shieldRemovalDuration = int.MaxValue;

            orig(self);
        }


        private static void GoldshoresBossfight_SetBossImmunity(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_SetBossImmunity orig, GoldshoresBossfight self, bool newBossImmunity)
        {
            var oldBossImmunity = self.bossImmunity;

            orig(self, newBossImmunity);

            if (!self.scriptedCombatEncounter || newBossImmunity == oldBossImmunity)
                return;

            foreach (var master in self.scriptedCombatEncounter.combatSquad.readOnlyMembersList)
            {
                var body = master.GetBody();
                if (body)
                {
                    if (newBossImmunity)
                    {
                        // disable laser while invuln
                        body.skillLocator?.special?.SetBlockedCooldownSkillState(true);
                    }
                    else
                    {
                        body.AddTimedBuff(TitanGoldArmorBroken.BuffIndex, 10f);
                        body.skillLocator?.special?.SetBlockedCooldownSkillState(false);
                    }
                }
            }
        }

        private static void GoldshoresBossfight_ServerFixedUpdate(ILContext il)
        {
            var c = new ILCursor(il);

            if (c.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt<EntityStateMachine>(nameof(EntityStateMachine.SetNextState))))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate(GoldshoresBossfight_GiveBuff);
            }
            else Log.Error("Patch failed for GoldshoresBossfight_ServerFixedUpdate #1");

            if (c.TryGotoNext(MoveType.After,
                    x => x.MatchCallOrCallvirt(AccessTools.PropertyGetter(typeof(Run.FixedTimeStamp), nameof(Run.FixedTimeStamp.hasPassed)))
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate(GoldshoresBossfight_ControlVulnerability);
            }
            else Log.Error("Patch failed for GoldshoresBossfight_ServerFixedUpdate #2");
        }

        private static void GoldshoresBossfight_GiveBuff(GoldshoresBossfight self)
        {
            if (HiddenGoldItem.ItemIndex == ItemIndex.None)
                return;

            foreach (var master in PlayerCharacterMasterController.instances)
            {
                if (master.master && master.master.inventory)
                    master.master.inventory.GiveItem(HiddenGoldItem.ItemIndex, 1);
            }

            Chat.AddMessage("<style=cShrine>The Guardian blesses you...</style>");
        }


        private static bool GoldshoresBossfight_ControlVulnerability(bool hasPassed, GoldshoresBossfight self)
        {
            if (!self.scriptedCombatEncounter)
                return true;

            var mainMaster = self.scriptedCombatEncounter.combatSquad.readOnlyMembersList.FirstOrDefault();
            var mainBody = mainMaster ? mainMaster.GetBody() : null;
            if (!mainBody || !mainBody.healthComponent)
                return true;

            var combinedHealthFraction = mainBody.healthComponent.combinedHealthFraction;
            var targetPhase = combinedHealthFraction <= (1f / 3f) ? 3 : combinedHealthFraction <= (2f / 3f) ? 2 : 1;
            if (targetPhase <= currentPhase)
                return false;

            currentPhase = targetPhase;
            if (currentPhase == 3)
            {
                // enrage: cooldown reduction, and the gilded affix which adds its own ring of gold spikes on top of the swords
                foreach (var master in self.scriptedCombatEncounter.combatSquad.readOnlyMembersList)
                {
                    if (!master.inventory)
                        continue;

                    master.inventory.GiveItemPermanent(RoR2Content.Items.AlienHead.itemIndex, 1);

                    var body = master.GetBody();
                    if (body && UseGildedAffix.Value)
                        GiveAffix(body, master.inventory, DLC2Content.Elites.Aurelionite);
                }
            }

            return true;
        }

        internal static void GiveAffix(CharacterBody body, Inventory inventory, EliteDef elite)
        {
            if (!elite)
                return;

            if (elite.eliteEquipmentDef)
            {
                if (elite.eliteEquipmentDef.equipmentIndex != EquipmentIndex.None)
                {
                    //Fill in first empty equipment slot
                    for (uint i = 0; i <= inventory.GetEquipmentSlotCount(); i++)
                    {
                        if (inventory.GetEquipment(i, 0).equipmentIndex == EquipmentIndex.None ||
                            inventory.GetEquipment(i, 0).equipmentIndex == elite.eliteEquipmentDef.equipmentIndex)
                        {
                            inventory.SetEquipmentIndexForSlot(elite.eliteEquipmentDef.equipmentIndex, i, 0);
                            inventory.SetActiveEquipmentSlot((byte)i);
                            inventory.SetActiveEquipmentSet(0);
                            break;
                        }
                    }
                }
                if (elite.eliteEquipmentDef.passiveBuffDef.buffIndex != BuffIndex.None)
                    body.SetBuffCount(elite.eliteEquipmentDef.passiveBuffDef.buffIndex, 1);
            }
        }

        private static void FireGoldFist_PlacePredictedAttack(On.EntityStates.TitanMonster.FireGoldFist.orig_PlacePredictedAttack orig, FireGoldFist self)
        {
            // phase 1: a single line (vanilla), phase 2: a cross, phase 3: a six pointed star
            if (currentPhase <= 1)
            {
                orig(self);
                return;
            }

            var rnd = Random.Range(0f, 360f);
            int iMax = currentPhase >= 3 ? 6 : 4;
            float angle = (360f / iMax);
            int fistCount = FireGoldFist.fistCount + currentPhase - 1;

            for (var i = 0; i < iMax; i++)
            {
                // the center blast is shared by every direction, only place it once
                for (var j = i == 0 ? 0 : 1; j < fistCount; j++)
                {
                    var vector = Quaternion.Euler(0f, rnd + (angle * i), 0f) * Vector3.forward;
                    var vector2 = self.predictedTargetPosition + (vector * FireGoldFist.distanceBetweenFists * j);

                    if (Physics.Raycast(new Ray(vector2 + (Vector3.up * 30f), Vector3.down), out var raycastHit, 60f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                        vector2 = raycastHit.point;

                    self.PlaceSingleDelayBlast(vector2, FireGoldFist.delayBetweenFists * j);
                }
            }
        }
    }
}
