using System;
using R2API;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
	internal abstract class BuffBase
	{
		internal abstract string name { get; }
		internal abstract Sprite icon { get; }
		internal abstract Color color { get; }
		internal abstract bool canStack { get; }
		internal abstract bool isDebuff { get; }
		internal abstract EliteDef eliteDef { get; }

		internal BuffDef AddBuff()
		{
			BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
			buffDef.name = this.name;
			buffDef.iconSprite = this.icon;
			buffDef.buffColor = this.color;
			buffDef.canStack = this.canStack;
			buffDef.isDebuff = this.isDebuff;
			buffDef.eliteDef = this.eliteDef;
			ContentAddition.AddBuffDef(buffDef);
			return buffDef;
		}
	}
}
