using GoldenCoastPlusRevived.Modules;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
    internal class HiddenGoldBuff : BuffBase<HiddenGoldBuff>
	{
        public HiddenGoldBuff() : base(PluginConfig.FightChanges.Value) { }

        internal override string name => "<style=cShrine>Aurelionite's Blessing</style>";
        internal override Sprite icon => GCPAssets.HiddenGoldBuffIcon;
        internal override Color color => Color.white;
        internal override bool canStack => true;
        internal override bool isDebuff => false;
        internal override EliteDef eliteDef => null;
    }
}
