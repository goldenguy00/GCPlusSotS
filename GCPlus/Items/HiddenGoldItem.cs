using System;
using BepInEx.Configuration;
using GoldenCoastPlusRevived.Buffs;
using GoldenCoastPlusRevived.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace GoldenCoastPlusRevived.Items
{
    internal class HiddenGoldItem : ItemBase<HiddenGoldItem>
	{
        public HiddenGoldItem() : base(FightChanges.EnableFightChanges.Value && EnableBlessing.Value) { }

        internal override string name => "Aurelionite's Blessing";
        internal override string token => "HiddenGoldBuffItem";
        internal override string pickup => "The Guardian of the Golden Coast has blessed you.";
        internal override string description => $"Gain <style=cShrine>{AurelioniteBlessingGoldGain.Value}%</style> <style=cStack>(+{AurelioniteBlessingGoldGain.Value}% per stack)</style> <style=cShrine>more gold</style>.";
        internal override string lore => "";
        internal override GameObject modelPrefab => null;
        internal override Sprite iconSprite => GCPAssets.HiddenGoldBuffIcon;
        internal override ItemTier Tier => ItemTier.NoTier;
        internal override ItemTag[] ItemTags => [ItemTag.BrotherBlacklist, ItemTag.CannotDuplicate, ItemTag.WorldUnique, ItemTag.CannotSteal, ItemTag.AIBlacklist];
        internal override bool hidden => true;

        public static ConfigEntry<bool> EnableBlessing { get; set; }
        public static ConfigEntry<bool> ShowBlessingBuff { get; set; }
        public static ConfigEntry<int> AurelioniteBlessingGoldGain { get; set; }

        internal override void AddItem()
        {
            base.AddItem();

            // keeps mithrix, scrappers and printers from taking the blessing
            itemDef.canRemove = false;
        }

        internal override void AddHooks()
        {
            On.RoR2.CharacterMaster.GiveMoney_uint_bool += CharacterMaster_GiveMoney;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (NetworkServer.active && body.inventory && ShowBlessingBuff.Value)
            {
                body.SetBuffCount(HiddenGoldBuff.BuffIndex, body.inventory.GetItemCountEffective(HiddenGoldItem.ItemIndex));
            }
        }

        private static void CharacterMaster_GiveMoney(On.RoR2.CharacterMaster.orig_GiveMoney_uint_bool orig, CharacterMaster self, uint amount, bool bypassStatManager)
        {
            if (amount > 0 && self.inventory)
            {
                var itemCount = self.inventory.GetItemCountEffective(HiddenGoldItem.ItemIndex);
                if (itemCount > 0)
                {
                    var addedPercent = itemCount * (AurelioniteBlessingGoldGain.Value * 0.01f);
                    amount += Math.Max(0, (uint)Mathf.CeilToInt(amount * addedPercent));
                }
            }

            orig(self, amount, bypassStatManager);
        }
    }
}
