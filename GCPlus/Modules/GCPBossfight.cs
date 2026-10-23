using EntityStates;
using EntityStates.Interactables.GoldBeacon;
using EntityStates.Missions.Goldshores;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace GoldenCoastPlusRevived.Modules
{
    public class GCPBossfight : EntityState
    {
        public GoldshoresMissionController missionController;

        public static float shieldRemovalDuration;

        public bool hasSpawnedBoss;

        public int serverCycleCount;

        public Run.FixedTimeStamp bossInvulnerabilityStartTime;

        public ScriptedCombatEncounter scriptedCombatEncounter;

        public bool bossImmunity;

        public bool bossShouldBeInvulnerable => missionController.beaconsActive < missionController.beaconsToSpawnOnMap;

        public override void OnEnter()
        {
            base.OnEnter();
            missionController = GetComponent<GoldshoresMissionController>();
            bossInvulnerabilityStartTime = Run.FixedTimeStamp.negativeInfinity;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                ServerFixedUpdate();
            }
        }

        public void SetBossImmunity(bool newBossImmunity)
        {
            if (!scriptedCombatEncounter || newBossImmunity == bossImmunity)
            {
                return;
            }

            bossImmunity = newBossImmunity;
            foreach (CharacterMaster readOnlyMembers in scriptedCombatEncounter.combatSquad.readOnlyMembersList)
            {
                CharacterBody body = readOnlyMembers.GetBody();
                if ((bool)body)
                {
                    if (bossImmunity)
                    {
                        body.AddBuff(RoR2Content.Buffs.Immune);
                        continue;
                    }

                    EffectManager.SpawnEffect(GoldshoresBossfight.shieldRemovalEffectPrefab, new EffectData
                    {
                        origin = body.coreTransform.position
                    }, transmit: true);
                    body.RemoveBuff(RoR2Content.Buffs.Immune);
                }
            }
        }

        public void ExtinguishBeacons()
        {
            foreach (GameObject beaconInstance in missionController.beaconInstanceList)
            {
                beaconInstance.GetComponent<EntityStateMachine>().SetNextState(new NotReady());
            }
        }

        public void ServerFixedUpdate()
        {
            if (base.fixedAge >= GoldshoresBossfight.transitionDuration)
            {
                missionController.ExitTransitionIntoBossfight();
                if (!hasSpawnedBoss)
                {
                    SpawnBoss();
                }
                else if (scriptedCombatEncounter.combatSquad.readOnlyMembersList.Count == 0)
                {
                    outer.SetNextState(new Exit());
                    if (serverCycleCount < 1)
                    {
                        //GoldshoresBossfight.onOneCycleGoldTitanKill?.Invoke();
                    }

                    return;
                }
            }

            if (!scriptedCombatEncounter)
            {
                return;
            }

            if (!bossImmunity)
            {
                if (bossInvulnerabilityStartTime.hasPassed)
                {
                    ExtinguishBeacons();
                    SetBossImmunity(newBossImmunity: true);
                    serverCycleCount++;
                }
            }
            else if (missionController.beaconsActive >= missionController.beaconsToSpawnOnMap)
            {
                SetBossImmunity(newBossImmunity: false);
                bossInvulnerabilityStartTime = Run.FixedTimeStamp.now + shieldRemovalDuration;
            }
        }

        public void SpawnBoss()
        {
            if (!hasSpawnedBoss)
            {
                if (!scriptedCombatEncounter)
                {
                    scriptedCombatEncounter = UnityEngine.Object.Instantiate(GoldshoresBossfight.combatEncounterPrefab.gameObject).GetComponent<ScriptedCombatEncounter>();
                    scriptedCombatEncounter.GetComponent<BossGroup>().dropPosition = missionController.bossSpawnPosition;
                    NetworkServer.Spawn(scriptedCombatEncounter.gameObject);
                }

                scriptedCombatEncounter.BeginEncounter();
                hasSpawnedBoss = scriptedCombatEncounter.hasSpawnedServer;
                if (hasSpawnedBoss)
                {
                    SetBossImmunity(newBossImmunity: true);
                }
            }
        }
    }
}
