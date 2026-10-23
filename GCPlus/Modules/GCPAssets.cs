using System.Reflection;
using System.Resources;
using RoR2;
using UnityEngine;

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
        public static Sprite HiddenGoldBuffIcon;
        public static Sprite AffixGoldIcon;

        public static GameObject LaserEyeBeam;
        public static GameObject LaserEyeExplosion;
        public static GameObject LaserEyeReady;

        public static void RegisterAssets()
        {
            try
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GoldenCoastPlusRevived.Properties.Resources.resources");
                using var rr = new ResourceReader(stream);
                var enumerator = rr.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string n = enumerator.Key as string;
                    byte[] v = enumerator.Value as byte[];

                    if (n.Contains("Titanic_Greatsword"))
                        BigSwordIcon = RegisterIcon(n, v);

                    if (n.Contains("Golden_Knurl"))
                        GoldenKnurlIcon = RegisterIcon(n, v);

                    if (n.Contains("Guardian_s_Eye"))
                        LaserEyeIcon = RegisterIcon(n, v);

                    if (n.Contains("Aurelionite_s_Blessing"))
                        HiddenGoldBuffIcon = RegisterIcon(n, v);

                    if (n.Contains("Gold_Elite_Icon"))
                        AffixGoldIcon = RegisterIcon(n, v);
                }
            }
            catch (System.Exception e) 
            {
                Log.Error(e); 
            }
        }

        private static Sprite RegisterIcon(string name, byte[] buffer)
        {
            Sprite ret = null;
            try
            {
                var tex2D = new Texture2D(2, 2);
                tex2D.LoadImage(buffer);

                // world space sprites (item orbs, scrapper, mithrix item steal) use pixelsPerUnit directly,
                // so normalize every icon to the size of a vanilla 128px icon at 100ppu
                var pixelsPerUnit = tex2D.width / 1.28f;
                ret = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                ret.name = name;
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
