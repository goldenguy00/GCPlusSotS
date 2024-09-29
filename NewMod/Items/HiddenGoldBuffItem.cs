using RoR2;
using System;
using UnityEngine;

namespace GoldenCoastPlusRevived.Items
{
	internal class HiddenGoldBuffItem : ItemBase
	{
		internal override string name
		{
			get
			{
				return "Aurelionite's Blessing";
			}
		}

		internal override string pickup
		{
			get
			{
				return "The Guardian of the Golden Coast has blessed you.";
			}
		}

		internal override string description
		{
			get
			{
				return "Gain <style=cShrine>10%</style> <style=cStack>(+10% per stack)</style> <style=cShrine>more gold</style>.";
			}
		}

		internal override string lore
		{
			get
			{
				return "";
			}
		}

		internal override string token
		{
			get
			{
				return "HiddenGoldBuffItem";
			}
		}

		internal override GameObject modelPrefab
		{
			get
			{
				return null;
			}
		}

		internal override Sprite iconSprite
		{
			get
			{
				return GCPAssets.hiddenGoldBuffIcon;
			}
		}

		internal override ItemTier Tier
		{
			get
			{
				return ItemTier.NoTier;
			}
		}

		internal override ItemTag[] ItemTags
		{
			get
			{
				return new ItemTag[] { ItemTag.BrotherBlacklist, ItemTag.CannotDuplicate };
			}
		}

		internal override bool hidden
		{
			get
			{
				return true;
			}
		}
	}
}
