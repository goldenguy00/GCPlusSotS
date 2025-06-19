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
using RoR2BepInExPack.GameAssetPaths;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GoldenCoastPlusRevived.Modules
{
    internal static class FightChanges
    {
        public static ConfigEntry<bool> EnableFightChanges { get; set; }
        public static ConfigEntry<float> BossHealthMult { get; set; }
        public static ConfigEntry<bool> UseAdaptiveArmor { get; set; }

        private static float origMinDuration;
        private static float origMaxDuration;
        private static int origFistCount;
        private static float origProjectileFrequency;

        private static bool hasBuffed1;
        private static bool hasBuffed2;

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
            Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Titan.TitanGoldBody_prefab).Completed += (task) =>
            {
                var body = task.Result.GetComponent<CharacterBody>();
                body.levelMaxHealth *= BossHealthMult.Value;
            };
        }

        private static void GoldshoresBossfight_SpawnBoss(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_SpawnBoss orig, GoldshoresBossfight self)
        {
            orig(self);
            hasBuffed1 = false;
            hasBuffed2 = false;

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
            FireGoldMegaLaser.maximumDuration = 2f;

            GoldshoresBossfight.shieldRemovalDuration = int.MaxValue;

            orig(self);
        }


        private static void GoldshoresBossfight_SetBossImmunity(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_SetBossImmunity orig, GoldshoresBossfight self, bool newBossImmunity)
        {
            var oldBossImmunity = self.bossImmunity;

            orig(self, newBossImmunity);

            if (!self.scriptedCombatEncounter || newBossImmunity || newBossImmunity == oldBossImmunity)
                return;

            foreach (var master in self.scriptedCombatEncounter.combatSquad.readOnlyMembersList)
            {
                var body = master.GetBody();
                if (body)
                {
                    body.AddTimedBuff(TitanGoldArmorBroken.BuffIndex, 10f);
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
            if (combinedHealthFraction <= (1f / 3f))
            {
                if (!hasBuffed2)
                {
                    hasBuffed2 = true;

                    foreach (var master in self.scriptedCombatEncounter.combatSquad.readOnlyMembersList)
                    {
                        var body = master.GetBody();
                        if (body && master.inventory)
                        {
                            GiveAffix(body, master.inventory, DLC2Content.Elites.Aurelionite);
                        }
                    }
                    FireGoldFist.fistCount = 998;
                    return true;
                }
            }
            else if (combinedHealthFraction <= (2f / 3f))
            {
                if (!hasBuffed1)
                {
                    hasBuffed1 = true;
                    foreach (var master in self.scriptedCombatEncounter.combatSquad.readOnlyMembersList)
                    {
                        if (master.inventory)
                            master.inventory.GiveItem(RoR2Content.Items.AlienHead.itemIndex, 1);
                    }
                    FireGoldFist.fistCount = 999;
                    return true;
                }
            }

            return false;
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
                        if (inventory.GetEquipment(i).equipmentIndex == EquipmentIndex.None ||
                            inventory.GetEquipment(i).equipmentIndex == elite.eliteEquipmentDef.equipmentIndex)
                        {
                            inventory.SetEquipmentIndexForSlot(elite.eliteEquipmentDef.equipmentIndex, i);
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
            bool hit4 = FireGoldFist.fistCount == 999;
            bool hit8 = FireGoldFist.fistCount == 998;
            if (!hit4 && !hit8)
            {
                orig(self);
                return;
            }

            var rnd = Random.Range(0f, 360f);
            int iMax = hit4 ? 4 : 8;
            float angle = hit4 ? 90f : 45f;

            for (var i = 0; i < iMax; i++)
            {
                for (var j = 0; j < 6; j++)
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
