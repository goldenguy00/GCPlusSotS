using System;
using R2API;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Items
{
    public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>
    {
        public static T instance { get; private set; }
        public static ItemIndex ItemIndex => ItemCatalog.FindItemIndex(instance?.name);

        public ItemBase(bool enabled) : base(enabled)
        {
            if (instance != null)
                throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ItemBase was instantiated twice");

            instance = this as T;
        }
    }

    public abstract class ItemBase
    {
        public ItemBase(bool enabled)
        {
            if (!enabled)
                return;

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
        internal virtual ItemTag[] ItemTags { get; set; } = new ItemTag[] { };
        internal abstract bool hidden { get; }

        public ItemDef itemDef;

        internal virtual void AddTokens()
        {
            LanguageAPI.Add(this.token.ToUpper() + "_NAME", this.name);
            LanguageAPI.Add(this.token.ToUpper() + "_PICKUP", this.pickup);
            LanguageAPI.Add(this.token.ToUpper() + "_DESC", this.description);
            LanguageAPI.Add(this.token.ToUpper() + "_LORE", this.lore);
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

        internal virtual void AddHooks() { }

        internal virtual ItemDisplayRuleDict AddItemDisplays() => new ItemDisplayRuleDict(Array.Empty<ItemDisplayRule>());
    }
}
