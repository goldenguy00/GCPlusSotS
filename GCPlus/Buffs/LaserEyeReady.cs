using GoldenCoastPlusRevived.Items;
using RoR2;
using RoR2BepInExPack.GameAssetPaths;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GoldenCoastPlusRevived.Buffs
{
    public class LaserEyeReady : BuffBase<LaserEyeReady>
	{
        public LaserEyeReady() : base(LaserEye.EnableEye.Value) { }

        internal override string name => "LaserEyeReady";
        internal override Sprite icon => Object.Instantiate(Addressables.LoadAssetAsync<BuffDef>(RoR2_Base_Merc.bdMercExpose_asset).WaitForCompletion().iconSprite);
        internal override Color color => Color.red;
        internal override bool canStack => false;
        internal override bool isDebuff => false;
        internal override bool isCooldown => false;
        internal override EliteDef eliteDef => null;
    }
}
