using GoldenCoastPlusRevived.Items;
using GoldenCoastPlusRevived.Modules;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
    internal class HiddenGoldBuff : BuffBase<HiddenGoldBuff>
	{
        public HiddenGoldBuff() : base(FightChanges.EnableFightChanges.Value && HiddenGoldItem.EnableBlessing.Value) { }

        internal override string name => "bdGCPAurelioniteBlessing";
        internal override Sprite icon => GCPAssets.HiddenGoldBuffIcon;
        internal override Color color => Color.white;
        internal override bool canStack => true;
        internal override bool isDebuff => false;
        internal override bool isCooldown => false;
        internal override EliteDef eliteDef => null;
    }
}
