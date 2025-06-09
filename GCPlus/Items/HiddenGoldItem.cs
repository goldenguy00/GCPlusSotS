using GoldenCoastPlusRevived.Modules;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Items
{
    internal class HiddenGoldItem : ItemBase<HiddenGoldItem>
	{
        public HiddenGoldItem() : base(PluginConfig.FightChanges.Value) { }

        internal override string name => "Aurelionite's Blessing";
        internal override string token => "HiddenGoldBuffItem";
        internal override string pickup => "The Guardian of the Golden Coast has blessed you.";
        internal override string description => "Gain <style=cShrine>10%</style> <style=cStack>(+10% per stack)</style> <style=cShrine>more gold</style>.";
        internal override string lore => "";
        internal override GameObject modelPrefab => null;
        internal override Sprite iconSprite => GCPAssets.HiddenGoldBuffIcon;
        internal override ItemTier Tier => ItemTier.NoTier;
        internal override ItemTag[] ItemTags => new ItemTag[] { ItemTag.BrotherBlacklist, ItemTag.CannotDuplicate };
        internal override bool hidden => true;


        internal override void AddHooks()
        {
            On.RoR2.CharacterMaster.GiveMoney += CharacterMaster_GiveMoney;
        }

        private static void CharacterMaster_GiveMoney(On.RoR2.CharacterMaster.orig_GiveMoney orig, CharacterMaster self, uint amount)
        {
            if (amount > 0)
            {
                var itemCount = self.inventory?.GetItemCount(HiddenGoldItem.ItemIndex) ?? 0;
                if (itemCount > 0)
                {
                    var addedPercent = itemCount * PluginConfig.AurelioniteBlessingGoldGain.Value;
                    amount += (uint)Mathf.CeilToInt(amount * addedPercent);
                }
            }

            orig(self, amount);
        }
    }
}
