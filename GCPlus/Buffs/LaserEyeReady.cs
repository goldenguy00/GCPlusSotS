using GoldenCoastPlusRevived.Items;
using GoldenCoastPlusRevived.Modules;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
    public class LaserEyeReady : BuffBase<LaserEyeReady>
	{
        public LaserEyeReady() : base(LaserEye.EnableEye.Value) { }

        internal override string name => "bdGCPLaserEyeReady";
        internal override Sprite icon => GCPAssets.LaserEyeIcon;
        internal override Color color => Color.yellow;
        internal override bool canStack => false;
        internal override bool isDebuff => false;
        internal override bool isCooldown => false;
        internal override EliteDef eliteDef => null;
    }
}
