using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2BepInExPack.GameAssetPaths.Version_1_35_0;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GoldenCoastPlusRevived.Items
{
    public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>
    {
        public static T instance { get; private set; }
        public static ItemIndex ItemIndex => instance?.itemDef?.itemIndex ?? ItemIndex.None;

        public ItemBase(bool enabled) : base(enabled)
        {
            if (instance != null)
                throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ItemBase was instantiated twice");

            instance = this as T;
        }
    }

    public abstract class ItemBase
    {
        internal static readonly List<ItemBase> instances = [];

        public ItemBase(bool enabled)
        {
            if (!enabled)
                return;

            instances.Add(this);

            AddTokens();
            AddItem();
            AddHooks();
        }

        internal abstract string name { get; }
        internal abstract string pickup { get; }
        internal abstract string description { get; }
        internal abstract string lore { get; }
        internal abstract string token { get; }
        internal abstract GameObject modelPrefab { get; }
        internal abstract Sprite iconSprite { get; }
        internal abstract ItemTier Tier { get; }
        internal virtual ItemTag[] ItemTags { get; set; } = [];
        internal abstract bool hidden { get; }

        public ItemDef itemDef;

        private LanguageAPI.LanguageOverlay descriptionOverlay;

        internal virtual void AddTokens()
        {
            LanguageAPI.Add(this.token.ToUpper() + "_NAME", this.name);
            LanguageAPI.Add(this.token.ToUpper() + "_PICKUP", this.pickup);
            LanguageAPI.Add(this.token.ToUpper() + "_LORE", this.lore);
            RefreshDescription();
        }

        /// <summary>
        /// Descriptions are built from config values, so they are added as an overlay that gets rebuilt whenever the config changes.
        /// LanguageAPI.Add never overwrites an existing token, which left in-game config changes out of the logbook and tooltips.
        /// </summary>
        internal void RefreshDescription()
        {
            descriptionOverlay?.Remove();
            descriptionOverlay = LanguageAPI.AddOverlay(this.token.ToUpper() + "_DESC", this.description);
        }

        internal static void RefreshAllDescriptions()
        {
            foreach (var item in instances)
                item.RefreshDescription();
        }

        internal virtual void AddItem()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = this.token;
            itemDef.nameToken = this.token.ToUpper() + "_NAME";
            itemDef.pickupToken = this.token.ToUpper() + "_PICKUP";
            itemDef.descriptionToken = this.token.ToUpper() + "_DESC";
            itemDef.loreToken = this.token.ToUpper() + "_LORE";
            itemDef.pickupModelPrefab = this.modelPrefab;
            itemDef.pickupIconSprite = this.iconSprite;
            itemDef.deprecatedTier = Tier;
            itemDef.tags = ItemTags;
            itemDef.hidden = this.hidden;

            ItemAPI.Add(new CustomItem(itemDef, this.AddItemDisplays()));
        }

        internal void AddToAurelioniteDropTable(ConfigEntry<float> dropWeight)
        {
            if (dropWeight.Value <= 0f)
                return;

            var dt = Addressables.LoadAssetAsync<ExplicitPickupDropTable>(RoR2_Base_Titan.dtBossTitanGold_asset).WaitForCompletion();
            HG.ArrayUtils.ArrayAppend(ref dt.pickupEntries, new ExplicitPickupDropTable.PickupDefEntry
            {
                pickupWeight = dropWeight.Value,
                pickupDef = itemDef
            });
        }

        internal virtual void AddHooks() { }

        internal virtual ItemDisplayRuleDict AddItemDisplays() => new ItemDisplayRuleDict(Array.Empty<ItemDisplayRule>());
    }
}
