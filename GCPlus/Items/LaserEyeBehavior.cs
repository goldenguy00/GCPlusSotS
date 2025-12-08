using System.Linq;
using GoldenCoastPlusRevived.Buffs;
using GoldenCoastPlusRevived.Modules;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Items
{
	public class LaserEyeBehavior : CharacterBody.ItemBehavior
    {
        private float timer;
        private float goldTarget = 25;
        private float goldCollected;

        public void Awake()
        {
            base.enabled = false; 
        }

        private void OnEnable()
        {
            goldTarget = (int)CharacterMaster.costOfSmallChest * LaserEye.EyeStacksMultiplier.Value;
            if (goldTarget < 0)
                goldTarget = int.MaxValue;

            if (this.body)
            {
                if (this.body.master)
                    this.body.master.OnGoldCollected += Master_OnGoldCollected;
            }
        }

        private void OnDisable()
        {
            if (this.body)
            {
                this.body.SetBuffCount(LaserEyeCharge.BuffIndex, 0);
                this.body.SetBuffCount(LaserEyeReady.BuffIndex, 0);
            }

            if (!ReferenceEquals(this.body?.master, null))
                this.body.master.OnGoldCollected -= Master_OnGoldCollected;
        }

        private void FixedUpdate()
		{
            if (!this.body || !this.body.inventory)
                return;

            this.timer -= Time.fixedDeltaTime;
            if (this.timer <= 0f)
            {
                this.timer = 0.2f;

                if (this.body.HasBuff(LaserEyeReady.BuffIndex))
                    this.FireLaser();

                UpdateBuffCount();
            }
        }
        private void UpdateBuffCount()
        {
            var currentCooldown = this.body.GetBuffCount(LaserEyeCharge.BuffIndex);
            var targetCooldown = (int)Util.Remap(goldCollected, 0f, goldTarget, 25f, 0f);

            if (currentCooldown < targetCooldown)
            {
                this.body.SetBuffCount(LaserEyeCharge.BuffIndex, targetCooldown);
            }
            else if (currentCooldown > targetCooldown)
            {
                this.body.RemoveBuff(LaserEyeCharge.BuffIndex);
            }

            if (this.body.GetBuffCount(LaserEyeCharge.BuffIndex) == 0)
            {
                if (!this.body.HasBuff(LaserEyeReady.BuffIndex))
                {
                    this.body.AddBuff(LaserEyeReady.BuffIndex);
                    EffectManager.SpawnEffect(GCPAssets.LaserEyeReady, new EffectData
                    {
                        origin = body.coreTransform.position
                    }, transmit: true);

                    Util.PlaySound("Play_UI_obj_casinoChest_swap", this.body.gameObject);
                }
            }
            else if (this.body.HasBuff(LaserEyeReady.BuffIndex))
                this.body.RemoveBuff(LaserEyeReady.BuffIndex);
        }

        private void Master_OnGoldCollected(float scaledAmount)
        {
            if (goldCollected < goldTarget)
            {
                goldCollected += scaledAmount;
            }
        }

        private void FireLaser()
        {
            int maxTargets = this.body.inventory.GetItemCountEffective(LaserEye.instance.itemDef) * LaserEye.EyeTargetsPerStack.Value;
            if (maxTargets == 0)
                return;

            var bs = new BullseyeSearch
            {
                teamMaskFilter = TeamMask.all,
                filterByDistinctEntity = true,
                filterByLoS = true,
                sortMode = BullseyeSearch.SortMode.Distance,
                maxDistanceFilter = LaserEye.EyeMaxRange.Value,
                viewer = this.body,
                searchOrigin = this.body.corePosition
            };
            bs.teamMaskFilter.RemoveTeam(this.body.teamComponent.teamIndex);
            bs.RefreshCandidates();

            int childIndex = -1;
            var model = this.body.modelLocator ? this.body.modelLocator.modelTransform : null;
            if (model && model.TryGetComponent<ChildLocator>(out var childLoc))
            {
                childIndex = childLoc.FindChildIndex("Head");
            }

            int enemiesSmited = 0;
            foreach (var enemy in bs.GetResults())
            {
                if (maxTargets <= enemiesSmited)
                    break;

                enemiesSmited++;

                new BlastAttack
                {
                    attacker = this.body.gameObject,
                    teamIndex = base.body.teamComponent.teamIndex,
                    procCoefficient = LaserEye.EyeBlastProcCoeff.Value,
                    baseDamage = base.body.damage * LaserEye.EyeDamage.Value,
                    position = enemy.transform.position,
                    baseForce = 500f,
                    radius = 1f,
                    falloffModel = BlastAttack.FalloffModel.None
                }.Fire();

                var effectData = new EffectData
                {
                    origin = enemy.transform.position,
                    start = this.body.aimOrigin
                };
                effectData.SetChildLocatorTransformReference(this.body.gameObject, childIndex);

                EffectManager.SpawnEffect(GCPAssets.LaserEyeBeam, effectData, transmit: true);
                EffectManager.SpawnEffect(GCPAssets.LaserEyeExplosion, effectData, transmit: true);
                goldCollected = 0f;
            }

            if (enemiesSmited > 0)
                Util.PlaySound(EntityStates.GolemMonster.FireLaser.attackSoundString, this.body.gameObject);
        }
	}
}
