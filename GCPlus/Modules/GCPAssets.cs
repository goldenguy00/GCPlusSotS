using System.Reflection;
using System.Resources;
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
            foreach (var n in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                Log.Warning(n);
                if (n.Contains("Titanic_Greatsword"))
                    BigSwordIcon = RegisterIcon(n, new Vector2(512, 512));
                if (n.Contains("Golden_Knurl"))
                    GoldenKnurlIcon = RegisterIcon(n, new Vector2(256, 256));
                if (n.Contains("Guardian_s_Eye"))
                    LaserEyeIcon = RegisterIcon(n, new Vector2(512, 512));
                if (n.Contains("Aurelionite_s_Blessing"))
                    HiddenGoldBuffIcon = RegisterIcon(n, new Vector2(128, 128));
            }

            LaserEyeReadyIcon = Object.Instantiate(Addressables.LoadAssetAsync<BuffDef>(RoR2_Base_Merc.bdMercExpose_asset).WaitForCompletion().iconSprite);
            TitanGoldArmorBrokenIcon = Object.Instantiate(Addressables.LoadAssetAsync<BuffDef>(RoR2_Base_ArmorReductionOnHit.bdPulverized_asset).WaitForCompletion().iconSprite);
            Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Golem.TracerGolem_prefab).Completed += (task) => tracerGolem = task.Result;
            Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Golem.ExplosionGolem_prefab).Completed += (task) => tracerGolem2 = task.Result;
        }

        private static Sprite RegisterIcon(string name, Vector2 size)
        {
            Sprite ret = null;
            try
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
                using var reader = new ResourceReader(stream);
                reader.GetResourceData(name, out _, out var buffer);

                var tex2D = new Texture2D((int)size.x,  (int)size.y);
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
