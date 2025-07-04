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
        public const string MOD_VERSON = "1.3.0";

        public void Awake()
		{
            Log.Init(Logger);
            PluginConfig.Init(Config);

            GCPAssets.RegisterAssets();

            new BigSword();
            new GoldenKnurl();
            new LaserEye();
            new LaserEyeCharge();
            new LaserEyeReady();
            new TitanGoldArmorBroken();
            new HiddenGoldItem();
            new HiddenGoldBuff();

            FightChanges.Init();
		}
    }
}
