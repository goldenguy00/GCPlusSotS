using System.Linq;
using EntityStates.Missions.Goldshores;
using EntityStates.TitanMonster;
using GoldenCoastPlusRevived.Buffs;
using GoldenCoastPlusRevived.Items;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Modules
{
    internal static class GCPHooks
    {
        internal static void Init()
        {
            if (!PluginConfig.FightChanges.Value)
                return;

            On.RoR2.GoldshoresMissionController.Awake += GoldshoresMissionController_Start;
            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.OnEnter += GoldshoresBossfight_OnEnter;
            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.OnExit += GoldshoresBossfight_OnExit;
            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.SetBossImmunity += GoldshoresBossfight_SetBossImmunity;
            IL.EntityStates.Missions.Goldshores.GoldshoresBossfight.ServerFixedUpdate += GoldshoresBossfight_ServerFixedUpdate;
            On.EntityStates.TitanMonster.FireGoldFist.PlacePredictedAttack += FireGoldFist_PlacePredictedAttack;

            GoldshoresBossfight.shieldRemovalDuration = 20f;
        }

        private static void GoldshoresBossfight_OnExit(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_OnExit orig, GoldshoresBossfight self)
        {
            FireGoldFist.fistCount = 6;
            FireGoldMegaLaser.projectileFireFrequency = 8f;
            RechargeRocks.rockControllerPrefab.GetComponent<TitanRockController>().fireInterval = 1f;
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
            var buffCount = 2 - self.serverCycleCount;
            if (buffCount <= 0)
                return;

            foreach (var master in CharacterMaster.instancesList)
            {
                if (master?.inventory)
                    master.inventory.GiveItem(HiddenGoldItem.ItemIndex, buffCount);
            }

            Chat.AddMessage("<style=cShrine>The Guardian blesses you...</style>");
        }


        private static bool GoldshoresBossfight_ControlVulnerability(bool hasPassed, GoldshoresBossfight self)
        {
            if (!self.scriptedCombatEncounter || self.serverCycleCount > 1)
                return hasPassed;

            var mainMaster = self.scriptedCombatEncounter.combatSquad.readOnlyMembersList.FirstOrDefault();
            var mainBody = mainMaster ? mainMaster.GetBody() : null;
            if (!mainBody || !mainBody.healthComponent)
                return hasPassed;

            var combinedHealthFraction = mainBody.healthComponent.combinedHealthFraction;
            bool isP1 = combinedHealthFraction <= (2f/3f) && self.serverCycleCount == 0;
            bool isP2 = combinedHealthFraction <= (1f/3f) && self.serverCycleCount == 1;

            if (isP1)
            {
                foreach (var master in self.scriptedCombatEncounter.combatSquad.readOnlyMembersList)
                {
                    master?.inventory?.GiveItem(RoR2Content.Items.AlienHead.itemIndex, 1);
                }
                RechargeRocks.rockControllerPrefab.GetComponent<TitanRockController>().fireInterval /= 2f;
                FireGoldFist.fistCount = 999;
                hasPassed = true;
            }
            else if (isP2)
            {
                FireGoldFist.fistCount = 998;
                FireGoldMegaLaser.projectileFireFrequency *= 1.25f;
                RechargeRocks.rockControllerPrefab.GetComponent<TitanRockController>().fireInterval /= 2f;
                hasPassed = true;
            }

            return hasPassed;
        }

        private static void GoldshoresBossfight_OnEnter(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_OnEnter orig, GoldshoresBossfight self)
        {
            orig.Invoke(self);
            self.bossInvulnerabilityStartTime = Run.FixedTimeStamp.now + GoldshoresBossfight.shieldRemovalDuration;
        }

        private static void GoldshoresMissionController_Start(On.RoR2.GoldshoresMissionController.orig_Awake orig, GoldshoresMissionController self)
        {
            self.beaconsToSpawnOnMap = 4;
            orig.Invoke(self);
        }

        private static void FireGoldFist_PlacePredictedAttack(On.EntityStates.TitanMonster.FireGoldFist.orig_PlacePredictedAttack orig, FireGoldFist self)
        {
            bool hit4 = FireGoldFist.fistCount == 998;
            bool hit8 = FireGoldFist.fistCount == 999;
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
