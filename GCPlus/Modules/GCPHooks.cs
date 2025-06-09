using EntityStates.Missions.Goldshores;
using EntityStates.TitanMonster;
using GoldenCoastPlusRevived.Buffs;
using GoldenCoastPlusRevived.Items;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Modules
{
    internal static class GCPHooks
    {
        internal static void Init()
        {
            if (PluginConfig.FightChanges.Value)
            {
                ApplyEncounterChanges();
            }
        }


        private static void ApplyEncounterChanges()
        {
            On.RoR2.GoldshoresMissionController.Awake += GoldshoresMissionController_Start;
            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.OnEnter += GoldshoresBossfight_OnEnter;
            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.OnExit += GoldshoresBossfight_OnExit;
            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.ServerFixedUpdate += GoldshoresBossfight_ServerFixedUpdate;
            On.EntityStates.TitanMonster.FireGoldFist.PlacePredictedAttack += FireGoldFist_PlacePredictedAttack;

            GoldshoresBossfight.shieldRemovalDuration = 20f;
        }

        private static void FireGoldFist_PlacePredictedAttack(On.EntityStates.TitanMonster.FireGoldFist.orig_PlacePredictedAttack orig, FireGoldFist self)
        {
            self.GetAimRay();
            var flag = FireGoldFist.fistCount == 998;
            if (flag)
            {
                var num = UnityEngine.Random.Range(0f, 360f);
                for (var i = 0; i < 4; i++)
                {
                    var num2 = 0;
                    for (var j = 0; j < 6; j++)
                    {
                        var vector = Quaternion.Euler(0f, num + 90f * i, 0f) * Vector3.forward;
                        var vector2 = self.predictedTargetPosition + vector * FireGoldFist.distanceBetweenFists * j;
                        var num3 = 60f;
                        RaycastHit raycastHit;
                        var flag2 = Physics.Raycast(new Ray(vector2 + Vector3.up * (num3 / 2f), Vector3.down), out raycastHit, num3, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
                        if (flag2)
                            vector2 = raycastHit.point;
                        self.PlaceSingleDelayBlast(vector2, FireGoldFist.delayBetweenFists * num2);
                        num2++;
                    }
                }
            }
            else
            {
                var flag3 = FireGoldFist.fistCount == 999;
                if (flag3)
                {
                    var num4 = UnityEngine.Random.Range(0f, 360f);
                    for (var k = 0; k < 8; k++)
                    {
                        var num5 = 0;
                        for (var l = 0; l < 6; l++)
                        {
                            var vector3 = Quaternion.Euler(0f, num4 + 45f * k, 0f) * Vector3.forward;
                            var vector4 = self.predictedTargetPosition + vector3 * FireGoldFist.distanceBetweenFists * l;
                            var num6 = 60f;
                            RaycastHit raycastHit2;
                            var flag4 = Physics.Raycast(new Ray(vector4 + Vector3.up * (num6 / 2f), Vector3.down), out raycastHit2, num6, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
                            if (flag4)
                                vector4 = raycastHit2.point;
                            self.PlaceSingleDelayBlast(vector4, FireGoldFist.delayBetweenFists * num5);
                            num5++;
                        }
                    }
                }
                else
                {
                    orig.Invoke(self);
                }
            }
        }

        private static void GoldshoresBossfight_OnExit(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_OnExit orig, GoldshoresBossfight self)
        {
            FireGoldFist.fistCount = 6;
            FireGoldMegaLaser.projectileFireFrequency = 8f;
            RechargeRocks.rockControllerPrefab.GetComponent<TitanRockController>().fireInterval = 1f;
            orig.Invoke(self);
        }

        private static void GoldshoresBossfight_ServerFixedUpdate(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_ServerFixedUpdate orig, GoldshoresBossfight self)
        {
            var flag = self.fixedAge >= GoldshoresBossfight.transitionDuration;
            if (flag)
            {
                self.missionController.ExitTransitionIntoBossfight();
                var flag2 = !self.hasSpawnedBoss;
                if (flag2)
                    self.SpawnBoss();
                else
                {
                    var flag3 = self.scriptedCombatEncounter.combatSquad.readOnlyMembersList.Count == 0;
                    if (flag3)
                    {
                        self.outer.SetNextState(new Exit());
                        foreach (var characterMaster in CharacterMaster.readOnlyInstancesList)
                        {
                            var flag4 = characterMaster.teamIndex == TeamIndex.Player;
                            if (flag4)
                                characterMaster.inventory.GiveItem(HiddenGoldItem.ItemIndex, 2 - self.serverCycleCount);
                        }
                        var flag5 = self.serverCycleCount < 2;
                        if (flag5)
                            Chat.AddMessage("<style=cShrine>The Guardian blesses you...</style>");
                        return;
                    }
                }
            }
            bool flag6 = self.scriptedCombatEncounter;
            if (flag6)
            {
                var flag7 = !self.bossImmunity;
                if (flag7)
                {
                    var hasPassed = self.bossInvulnerabilityStartTime.hasPassed;
                    if (hasPassed)
                    {
                        var flag8 = (double)self.scriptedCombatEncounter.combatSquad.readOnlyMembersList[0].bodyInstanceObject.GetComponent<HealthComponent>().combinedHealthFraction <= 0.666666666666 && self.serverCycleCount == 0;
                        var flag9 = (double)self.scriptedCombatEncounter.combatSquad.readOnlyMembersList[0].bodyInstanceObject.GetComponent<HealthComponent>().combinedHealthFraction <= 0.333333333333 && self.serverCycleCount == 1;
                        var flag10 = flag9;
                        if (flag10)
                        {
                            self.ExtinguishBeacons();
                            self.SetBossImmunity(true);
                            self.scriptedCombatEncounter.combatSquad.readOnlyMembersList[0].bodyInstanceObject.GetComponent<CharacterBody>().inventory.GiveItem(RoR2Content.Items.AlienHead.itemIndex, 1);
                            RechargeRocks.rockControllerPrefab.GetComponent<TitanRockController>().fireInterval /= 2f;
                            FireGoldFist.fistCount = 999;
                            self.serverCycleCount++;
                        }
                        else
                        {
                            var flag11 = flag8;
                            if (flag11)
                            {
                                self.ExtinguishBeacons();
                                self.SetBossImmunity(true);
                                FireGoldFist.fistCount = 998;
                                FireGoldMegaLaser.projectileFireFrequency *= 1.25f;
                                RechargeRocks.rockControllerPrefab.GetComponent<TitanRockController>().fireInterval /= 2f;
                                self.serverCycleCount++;
                            }
                        }
                    }
                }
                else
                {
                    var flag12 = self.missionController.beaconsActive >= self.missionController.beaconsToSpawnOnMap;
                    if (flag12)
                    {
                        self.SetBossImmunity(false);
                        var flag13 = self.serverCycleCount > 0;
                        if (flag13)
                            self.scriptedCombatEncounter.combatSquad.readOnlyMembersList[0].bodyInstanceObject.GetComponent<CharacterBody>().AddTimedBuff(TitanGoldArmorBroken.BuffIndex, 10f);
                        self.bossInvulnerabilityStartTime = Run.FixedTimeStamp.now + GoldshoresBossfight.shieldRemovalDuration;
                    }
                }
            }
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
        /*
        private static void GoldTitanManager_CalcTitanPowerAndBestTeam(ILContext il)
        {
            var ilcursor = new ILCursor(il);
            var ilcursor2 = ilcursor;
            var array = new Func<Instruction, bool>[6];
            array[0] = (x) => x.MatchLdloc(2);
            array[1] = (x) => x.MatchLdsfld("RoR2.GoldTitanManager", "goldTitanItemIndex");
            array[2] = (x) => x.MatchLdcI4(1);
            array[3] = (x) => x.MatchLdcI4(1);
            array[4] = (x) => x.MatchCallOrCallvirt("RoR2.Util", "GetItemCountForTeam");
            array[5] = (x) => x.MatchStloc(3);
            ilcursor2.GotoNext(array);
            ilcursor.Index += 5;
            ilcursor.Emit(OpCodes.Ldc_I4, TitanGoldMultiplier.Value);
            ilcursor.Emit(OpCodes.Mul);
        }*/
        /*
        private static void GoldTitanManager_TryStartChannelingTitansServer(ILContext il)
        {
            var ilcursor = new ILCursor(il);
            var ilcursor2 = ilcursor;
            var array = new Func<Instruction, bool>[5];
            array[0] = (x) => x.MatchLdloc(1);
            array[1] = (x) => x.MatchConvR4();
            array[2] = (x) => x.MatchLdcR4(0.5f);
            array[3] = (x) => x.MatchCall<Mathf>("Pow");
            array[4] = (x) => x.MatchMul();
            ilcursor2.GotoNext(array);
            ilcursor.Index += 2;
            ilcursor.Next.Operand = 1f;
        }*/
    }
}
