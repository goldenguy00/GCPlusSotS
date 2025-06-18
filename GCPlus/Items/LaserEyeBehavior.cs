using GoldenCoastPlusRevived.Buffs;
using GoldenCoastPlusRevived.Modules;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Items
{
	public class LaserEyeBehavior : CharacterBody.ItemBehavior
    {
        private float timer;

        public void Awake()
        {
            base.enabled = false; 
        }

        private void OnEnable()
        {
            if (this.body && this.body.master)
                this.body.master.OnGoldCollected += Master_OnGoldCollected;
        }

        private void OnDisable()
        {
            if (this.body)
            {
                if (0 < this.body.GetBuffCount(LaserEyeCharge.BuffIndex))
                    this.body.ClearTimedBuffs(LaserEyeCharge.BuffIndex);

                if (this.body.master)
                    this.body.master.OnGoldCollected -= Master_OnGoldCollected;
            }
        }

        private void FixedUpdate()
		{
            if (!this.body || this.body.GetBuffCount(LaserEyeCharge.BuffIndex) < LaserEye.EyeStacksRequired.Value)
                return;

            this.timer -= Time.fixedDeltaTime;
            if (this.timer <= 0f)
            {
                this.timer = 0.25f;
                this.FireLaser();
            }
        }

        private void RefreshTimedBuffs(CharacterBody body, BuffIndex buffIndex, float duration)
        {
            for (int l = 0; l < body.timedBuffs.Count; l++)
            {
                var timedBuff = body.timedBuffs[l];
                if (timedBuff.buffIndex == buffIndex)
                {
                    if (timedBuff.timer < duration)
                    {
                        timedBuff.timer = duration;
                        timedBuff.totalDuration = duration;
                    }
                }
            }
            this.body.AddTimedBuff(buffIndex, duration);
        }

        private void Master_OnGoldCollected(float _)
        {
            if (!this.body)
                return;

            if (this.body.GetBuffCount(LaserEyeCharge.BuffIndex) < LaserEye.EyeStacksRequired.Value)
            {
                this.RefreshTimedBuffs(this.body, LaserEyeCharge.BuffIndex, LaserEye.EyeStackTimer.Value);
            }
        }

        private void FireLaser()
        {
            var bs = new BullseyeSearch
            {
                teamMaskFilter = TeamMask.all,
                filterByDistinctEntity = true,
                filterByLoS = true,
                sortMode = BullseyeSearch.SortMode.Distance,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                maxDistanceFilter = 100f,
                viewer = this.body,
                searchOrigin = this.body.corePosition
            };
            bs.teamMaskFilter.RemoveTeam(this.body.teamComponent.teamIndex);
            bs.RefreshCandidates();

            int enemiesSmited = 0;
            foreach (var enemy in bs.GetResults())
            {
                if (enemiesSmited > base.stack * 2)
                    break;

                enemiesSmited++;
                new BlastAttack
                {
                    attacker = this.gameObject,
                    teamIndex = base.body.teamComponent.teamIndex,
                    procCoefficient = LaserEye.EyeBlastProcCoeff.Value,
                    baseDamage = base.body.damage * LaserEye.EyeDamage.Value,
                    position = enemy.transform.position,
                    baseForce = 500f,
                    radius = 1f,
                    falloffModel = BlastAttack.FalloffModel.None
                }.Fire();

                EffectData effectData = new EffectData
                {
                    origin = enemy.transform.position,
                    start = base.body.corePosition
                };
                Transform modelTransform = base.body.modelLocator?.modelTransform;
                if (modelTransform)
                {
                    ChildLocator component = modelTransform.GetComponent<ChildLocator>();
                    if (component)
                        effectData.SetChildLocatorTransformReference(base.body.gameObject, component.FindChildIndex("Chest"));
                }
                EffectManager.SpawnEffect(GCPAssets.tracerGolem, effectData, true);
                EffectManager.SpawnEffect(GCPAssets.tracerGolem2, effectData, true);
            }
            if (enemiesSmited > 0)
            {
                Util.PlaySound(EntityStates.GolemMonster.FireLaser.attackSoundString, this.gameObject);
                this.body.ClearTimedBuffs(LaserEyeCharge.BuffIndex);
            }
		}
	}
}
