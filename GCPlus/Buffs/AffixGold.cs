using GoldenCoastPlusRevived.Modules;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
    internal class AffixGold : BuffBase<AffixGold>
	{
        public AffixGold() : base(GoldElites.EnableGoldElites.Value) { }

        internal override string name => "bdGCPEliteGold";
        internal override Sprite icon => GCPAssets.AffixGoldIcon;
        internal override Color color => new Color32(255, 214, 89, 255);
        internal override bool canStack => false;
        internal override bool isDebuff => false;
        internal override bool isCooldown => false;
        internal override EliteDef eliteDef => GoldElites.GoldEliteDef;
    }
}
