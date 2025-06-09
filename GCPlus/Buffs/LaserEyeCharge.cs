using GoldenCoastPlusRevived.Items;
using GoldenCoastPlusRevived.Modules;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
    public class LaserEyeCharge : BuffBase<LaserEyeCharge>
	{
        public LaserEyeCharge() : base(LaserEye.EnableEye.Value) { }

        internal override string name => "LaserEyeCharge";
        internal override Sprite icon => GCPAssets.LaserEyeIcon;
        internal override Color color => Color.red;
        internal override bool canStack => true;
        internal override bool isDebuff => false;
        internal override EliteDef eliteDef => null;
    }
}
