using BepInEx;
using GoldenCoastPlusRevived.Buffs;
using GoldenCoastPlusRevived.Items;
using GoldenCoastPlusRevived.Modules;

namespace GoldenCoastPlusRevived
{
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSON)]
    public class GoldenCoastPlusPlugin : BaseUnityPlugin
	{
        public const string MOD_AUTHOR = "TechDebtCollector";
        public const string MOD_NAME = "GoldenCoastPlus";
        public const string MOD_GUID = $"com.{MOD_AUTHOR}.{MOD_NAME}";
        public const string MOD_VERSON = "1.2.0";

        public static GoldenCoastPlusPlugin instance { get; private set; }

        public void Awake()
		{
            instance = this;
            Log.Init(Logger);
            PluginConfig.Init(Config);

            GCPAssets.RegisterAssets();

            new BigSword();
            new GoldenKnurl();
            new LaserEye();
            new LaserEyeCharge();
            new TitanGoldArmorBroken();
            new HiddenGoldItem();
            new HiddenGoldBuff();

            GCPHooks.Init();
		}
    }
}
