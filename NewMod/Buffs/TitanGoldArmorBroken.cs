using System;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
	internal class TitanGoldArmorBroken : BuffBase
	{
		internal override string name
		{
			get
			{
				return "TitanGoldArmorBroken";
			}
		}

		internal override Sprite icon
		{
			get
			{
				return GCPAssets.TitanGoldArmorBrokenIcon;
			}
		}

		internal override Color color
		{
			get
			{
				return Color.yellow;
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
				return true;
			}
		}

		internal override EliteDef eliteDef
		{
			get
			{
				return null;
			}
		}
	}
}
