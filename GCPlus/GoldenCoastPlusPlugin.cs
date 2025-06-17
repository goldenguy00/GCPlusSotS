using System.Security.Permissions;
using System.Security;
using BepInEx;
using GoldenCoastPlusRevived.Buffs;
using GoldenCoastPlusRevived.Items;
using GoldenCoastPlusRevived.Modules;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete


namespace GoldenCoastPlusRevived
{
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSON)]
    public class GoldenCoastPlusPlugin : BaseUnityPlugin
	{
        public const string MOD_AUTHOR = "TechDebtCollector";
        public const string MOD_NAME = "GoldenCoastPlus";
        public const string MOD_GUID = $"com.{MOD_AUTHOR}.{MOD_NAME}";
        public const string MOD_VERSON = "1.2.2";

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
