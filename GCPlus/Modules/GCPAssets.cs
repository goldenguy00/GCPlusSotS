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

        public static GameObject tracerGolem;
        public static GameObject tracerGolem2;

        public static void RegisterAssets()
        {
            BigSwordIcon = RegisterIcon("Titanic_Greatsword");
            GoldenKnurlIcon = RegisterIcon("Golden_Knurl");
            LaserEyeIcon = RegisterIcon("Guardian_s_Eye");
            HiddenGoldBuffIcon = RegisterIcon("Aurelionite_s_Blessing");

            LaserEyeReadyIcon = Object.Instantiate(Addressables.LoadAssetAsync<BuffDef>(RoR2_Base_Merc.bdMercExpose_asset).WaitForCompletion().iconSprite);
            TitanGoldArmorBrokenIcon = Object.Instantiate(Addressables.LoadAssetAsync<BuffDef>(RoR2_Base_ArmorReductionOnHit.bdPulverized_asset).WaitForCompletion().iconSprite);
            Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Golem.TracerGolem_prefab).Completed += (task) => tracerGolem = task.Result;
            Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Golem.ExplosionGolem_prefab).Completed += (task) => tracerGolem2 = task.Result;
        }

        private static Sprite RegisterIcon(string name)
        {
            Sprite ret = null;
            try
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"GoldenCoastPlusRevived.Assets.{name}.png");

                var buffer = new byte[stream.Length];
                stream.Read(buffer);

                var tex2D = new Texture2D(2, 2);
                tex2D.LoadImage(buffer);
                ret = Sprite.Create(tex2D, new Rect(Vector2.zero, new Vector2(128, 128)), new Vector2(0.5f, 0.5f));

                if (ret == null)
                    throw new System.NullReferenceException();
            }
            catch (System.Exception e)
            {
                Log.Error("Unable to load " + name);
                Log.Error(e);
            }
            return ret;
        }
    }
}
