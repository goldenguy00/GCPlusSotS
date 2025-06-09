using System;
using GoldenCoastPlusRevived.Buffs;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Items
{
	public class LaserEyeBehavior : CharacterBody.ItemBehavior
    {
        private int goldDifference;
        private int previousGold;

        private void FixedUpdate()
		{
			var currentGold = (int)this.body.master.money;
			this.goldDifference = Math.Max(0, currentGold - this.previousGold);
            this.previousGold = currentGold;

            if (this.goldDifference > 0)
			{
				this.RefreshTimedBuffs(this.body, LaserEyeCharge.BuffIndex, 5f);
			}

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

        private void FireLaser()
		{
			bool flag = false;
			TeamIndex val = (TeamIndex)0;
			while ((int)val < 5)
			{
				if (val != base.body.teamComponent.teamIndex)
				{
					foreach (TeamComponent teamMember in TeamComponent.GetTeamMembers(val))
					{
						bool flag3 = (teamMember.transform.position - this.body.transform.position).sqrMagnitude <= 900f;
						if (flag3)
						{
							flag = true;
							new BlastAttack
							{
								attacker = ((Component)base.body).gameObject,
								inflictor = null,
								teamIndex = base.body.teamComponent.teamIndex,
								baseDamage = base.body.damage * 25f * (float)base.stack,
								baseForce = 5000f,
								position = ((Component)teamMember).transform.position,
								radius = 1f,
								falloffModel = 0
							}.Fire();
							EffectData val3 = new EffectData
							{
								origin = ((Component)teamMember).transform.position,
								start = base.body.transform.position
							};
							Transform modelTransform = base.body.modelLocator.modelTransform;
							ChildLocator component = ((Component)modelTransform).GetComponent<ChildLocator>();
							int num = component.FindChildIndex("Chest");
							val3.SetChildLocatorTransformReference(((Component)base.body).gameObject, num);
							EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/tracers/TracerGolem"), val3, true);
							EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/ExplosionGolem"), val3, true);
						}
					}
				}
				val = (TeamIndex)(sbyte)(val + 1);
			}
			if (flag)
			{
				Util.PlaySound(EntityStates.GolemMonster.FireLaser.attackSoundString, ((Component)base.body).gameObject);
			}

			/*bool flag = false;
			for (TeamIndex teamIndex = 0; true; teamIndex++)
			{
				bool flag2 = teamIndex != this.body.teamComponent.teamIndex;
				if (flag2)
				{
					foreach (TeamComponent teamComponent in TeamComponent.GetTeamMembers(teamIndex))
					{
						bool flag3 = (teamComponent.transform.position - this.body.transform.position).sqrMagnitude <= 900f;
						if (flag3)
						{
							flag = true;
							new BlastAttack
							{
								attacker = this.body.gameObject,
								inflictor = null,
								teamIndex = this.body.teamComponent.teamIndex,
								baseDamage = this.body.damage * 25f * (float)this.stack,
								baseForce = 5000f,
								procCoefficient = GoldenCoastPlusPlugin.EyeBlastProcCoeff.Value,
								position = teamComponent.transform.position,
								radius = 1f,
								falloffModel = 0
							}.Fire();
							EffectData effectData = new EffectData
							{
								origin = teamComponent.transform.position,
								start = this.body.transform.position
							};
							Transform modelTransform = this.body.modelLocator.modelTransform;
							ChildLocator component = modelTransform.GetComponent<ChildLocator>();
							int num = component.FindChildIndex("Chest");
							effectData.SetChildLocatorTransformReference(this.body.gameObject, num);
							EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/tracers/TracerGolem"), effectData, true);
							EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/ExplosionGolem"), effectData, true);
						}
					}
				}
			}
			bool flag4 = flag;
			if (flag4)
			{
				Util.PlaySound(EntityStates.GolemMonster.FireLaser.attackSoundString, this.body.gameObject);
			}*/
		}
	}
}
