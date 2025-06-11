using GoldenCoastPlusRevived.Modules;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
    internal class TitanGoldArmorBroken : BuffBase<TitanGoldArmorBroken>
	{
        public TitanGoldArmorBroken() : base(PluginConfig.FightChanges.Value) { }

        internal override string name => "TitanGoldArmorBroken";
        internal override Sprite icon => GCPAssets.TitanGoldArmorBrokenIcon;
        internal override Color color => Color.yellow;
        internal override bool canStack => false;
        internal override bool isDebuff => true;
        internal override EliteDef eliteDef => null;

        internal override void AddHooks()
        {
            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }

        private static void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self.body && self.body.HasBuff(TitanGoldArmorBroken.BuffIndex))
                damageInfo.damage *= PluginConfig.BossArmorBrokenMult.Value;

            orig(self, damageInfo);
        }
    }
}
