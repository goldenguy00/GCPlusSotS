using System;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
	internal class LaserEyeCharge : BuffBase
	{
		internal override string name
		{
			get
			{
				return "LaserEyeCharge";
			}
		}

		internal override Sprite icon
		{
			get
			{
				return GCPAssets.LaserEyeIcon;
			}
		}

		internal override Color color
		{
			get
			{
				return Color.red;
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
