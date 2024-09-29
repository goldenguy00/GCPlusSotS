using System;
using R2API;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Items
{
	internal abstract class ItemBase
	{
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

		internal ItemDef AddItem()
		{
			LanguageAPI.Add(this.token.ToUpper() + "_NAME", this.name);
			LanguageAPI.Add(this.token.ToUpper() + "_PICKUP", this.pickup);
			LanguageAPI.Add(this.token.ToUpper() + "_DESC", this.description);
			LanguageAPI.Add(this.token.ToUpper() + "_LORE", this.lore);
			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = this.token;
			itemDef.nameToken = this.token.ToUpper() + "_NAME";
			itemDef.pickupToken = this.token.ToUpper() + "_PICKUP";
			itemDef.descriptionToken = this.token.ToUpper() + "_DESC";
			itemDef.loreToken = this.token.ToUpper() + "_LORE";
			itemDef.pickupModelPrefab = this.modelPrefab;
			itemDef.pickupIconSprite = this.iconSprite;
			itemDef.deprecatedTier = Tier;
			if (ItemTags.Length > 0) { itemDef.tags = ItemTags; }
			itemDef.hidden = this.hidden;
			ItemDisplayRuleDict itemDisplayRules = this.AddItemDisplays();
			ItemAPI.Add(new CustomItem(itemDef, itemDisplayRules));
			bool flag = !this.hidden;
			if (flag)
			{
				GoldenCoastPlusPlugin.newItemList.Add(itemDef);
			}
			return itemDef;
		}
		
		internal virtual ItemDisplayRuleDict AddItemDisplays()
		{
			return new ItemDisplayRuleDict(Array.Empty<ItemDisplayRule>());
		}
	}
}
