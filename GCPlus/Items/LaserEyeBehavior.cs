using GoldenCoastPlusRevived.Buffs;
using GoldenCoastPlusRevived.Modules;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Items
{
	public class LaserEyeBehavior : CharacterBody.ItemBehavior
    {
        private void Start()
        {
            if (this.body)
                this.body.master.OnGoldCollected += this.Master_OnGoldCollected;
        }

        private void OnDisable()
        {
            if (this.body && this.body.master)
                this.body.master.OnGoldCollected -= Master_OnGoldCollected;
        }

        private void FixedUpdate()
		{
            if (!this.body)
                return;

			if (this.body.GetBuffCount(LaserEyeCharge.BuffIndex) >= LaserEye.EyeStacksRequired.Value)
			{
				this.body.ClearTimedBuffs(LaserEyeCharge.BuffIndex);
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
            this.body.AddTimedBuff(buffIndex, 5f);
        }

        private void Master_OnGoldCollected(float amount)
        {
            if (!this.body)
                return;

            if (this.body.GetBuffCount(LaserEyeCharge.BuffIndex) < LaserEye.EyeStacksRequired.Value)
            {
                this.RefreshTimedBuffs(this.body, LaserEyeCharge.BuffIndex, 5f);
            }
        }

        private void FireLaser()
		{
			bool hasShot = false;
            BullseyeSearch bs = new BullseyeSearch
            {
                searchOrigin = this.body.corePosition,
                teamMaskFilter = TeamMask.all,
                filterByDistinctEntity = true,
                filterByLoS = true,
                sortMode = BullseyeSearch.SortMode.Distance,
                viewer = this.body,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                maxDistanceFilter = 100f,
            };
            bs.teamMaskFilter.RemoveTeam(this.body.teamComponent.teamIndex);
            bs.RefreshCandidates();
			foreach (var enemy in bs.GetResults())
			{
				hasShot = true;
				new BlastAttack
				{
					attacker = base.body.gameObject,
					inflictor = null,
					teamIndex = base.body.teamComponent.teamIndex,
					baseDamage = base.body.damage * 25f * base.stack,
					baseForce = 500f,
					position = enemy.transform.position,
					radius = 1f,
					falloffModel = BlastAttack.FalloffModel.Linear
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

			if (hasShot)
			{
				Util.PlaySound(EntityStates.GolemMonster.FireLaser.attackSoundString, ((Component)base.body).gameObject);
			}
		}
	}
}
