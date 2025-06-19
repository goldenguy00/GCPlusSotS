using BepInEx.Configuration;
using GoldenCoastPlusRevived.Modules;
using RoR2;
using RoR2BepInExPack.GameAssetPaths;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GoldenCoastPlusRevived.Buffs
{
    internal class TitanGoldArmorBroken : BuffBase<TitanGoldArmorBroken>
	{
        public TitanGoldArmorBroken() : base(FightChanges.EnableFightChanges.Value) { }

        internal override string name => "TitanGoldArmorBroken";
        internal override Sprite icon => Object.Instantiate(Addressables.LoadAssetAsync<BuffDef>(RoR2_Base_ArmorReductionOnHit.bdPulverized_asset).WaitForCompletion().iconSprite);
        internal override Color color => Color.yellow;
        internal override bool canStack => false;
        internal override bool isDebuff => true;
        internal override bool isCooldown => false;
        internal override EliteDef eliteDef => null;

        public static ConfigEntry<float> BossArmorBrokenMult { get; set; }
        
        internal override void AddBuff()
        {
            base.AddBuff();
            this.buffDef.flags = BuffDef.Flags.ExcludeFromNoxiousThorns;
        }

        internal override void AddHooks()
        {
            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
            if (FightChanges.UseAdaptiveArmor.Value)
            {
                On.RoR2.CharacterBody.OnBuffFirstStackGained += CharacterBody_OnBuffFirstStackGained;
                On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
            }
        }

        private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            orig(self, buffDef);

            if (buffDef == this.buffDef && self.inventory && self.inventory.GetItemCount(RoR2Content.Items.AdaptiveArmor) != 1)
            {
                self.inventory.ResetItem(RoR2Content.Items.AdaptiveArmor);
                self.inventory.GiveItem(RoR2Content.Items.AdaptiveArmor);
            }
        }

        private void CharacterBody_OnBuffFirstStackGained(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buffDef)
        {
            orig(self, buffDef);

            if (buffDef == this.buffDef && self.inventory && self.inventory.GetItemCount(RoR2Content.Items.AdaptiveArmor) > 0)
            {
                self.inventory.ResetItem(RoR2Content.Items.AdaptiveArmor);
            }
        }

        private void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self.body && self.body.HasBuff(this.buffDef))
                damageInfo.damage *= BossArmorBrokenMult.Value;

            orig(self, damageInfo);
        }
    }
}
