using System;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
	internal class AffixGold : BuffBase
	{
		internal override string name
		{
			get
			{
				return "AffixGold";
			}
		}

		internal override Sprite icon
		{
			get
			{
				return GCPAssets.AffixGoldIcon;
			}
		}

		internal override Color color
		{
			get
			{
				return Color.white;
			}
		}

		internal override bool canStack
		{
			get
			{
				return false;
			}
		}

		internal override bool isDebuff
		{
			get
			{
				return false;
			}
		}

		internal override EliteDef eliteDef
		{
			get
			{
				return Resources.Load<EliteDef>("elitedefs/Gold");
			}
		}
	}
}
