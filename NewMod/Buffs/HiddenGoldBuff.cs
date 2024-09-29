using System;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
	internal class HiddenGoldBuff : BuffBase
	{
		internal override string name
		{
			get
			{
				return "<style=cShrine>Aurelionite's Blessing</style>";
			}
		}

		internal override Sprite icon
		{
			get
			{
				return GCPAssets.hiddenGoldBuffIcon;
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
				return true;
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
				return null;
			}
		}
	}
}
