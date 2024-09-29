using System;
using RoR2;

namespace GoldenCoastPlusRevived.Items
{
	public class HiddenGoldBuffBehavior : CharacterBody.ItemBehavior
	{
		private void FixedUpdate()
		{
			this.body.SetBuffCount(GoldenCoastPlusPlugin.hiddenGoldBuffDef.buffIndex, this.stack);
		}
	}
}
