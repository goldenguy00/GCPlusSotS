using System.Reflection;
using RoR2;
using RoR2BepInExPack.GameAssetPaths;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GoldenCoastPlusRevived.Modules
{
    public static class GCPAssets
    {
        public static GameObject GoldenKnurlPrefab;
        public static GameObject GoldenKnurlFollowerPrefab;
        public static GameObject LaserEyePrefab;
        public static GameObject SwordProjectile;
        public static GameObject BigSwordPrefab;

        public static Sprite BigSwordIcon;
        public static Sprite GoldenKnurlIcon;
        public static Sprite LaserEyeIcon;
        public static Sprite LaserEyeReadyIcon;
        public static Sprite TitanGoldArmorBrokenIcon;
        public static Sprite HiddenGoldBuffIcon;

        public static void RegisterAssets()
        {
            BigSwordIcon = RegisterIcon("Titanic_Greatsword");
            GoldenKnurlIcon = RegisterIcon("Golden_Knurl");
            LaserEyeIcon = RegisterIcon("Guardian_s_Eye");
            HiddenGoldBuffIcon = RegisterIcon("Aurelionite_s_Blessing");

            LaserEyeReadyIcon = Object.Instantiate(Addressables.LoadAssetAsync<BuffDef>(RoR2_Base_Merc.bdMercExpose_asset).WaitForCompletion().iconSprite);
            TitanGoldArmorBrokenIcon = Object.Instantiate(Addressables.LoadAssetAsync<BuffDef>(RoR2_Base_ArmorReductionOnHit.bdPulverized_asset).WaitForCompletion().iconSprite);
        }

        private static Sprite RegisterIcon(string name)
        {
            try
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"GoldenCoastPlusRevived.Assets.{name}.png");

                var buffer = new byte[stream.Length];
                stream.Read(buffer);

                var tex2D = new Texture2D(2, 2);
                if (tex2D.LoadImage(buffer))
                    return Sprite.Create(tex2D, new Rect(Vector2.zero, new Vector2(128, 128)), new Vector2(0.5f, 0.5f));
            }
            catch (System.Exception e)
            {
                Log.Error("Unable to load " + name);
                Log.Error(e);
            }
            return null;
        }
    }
}
