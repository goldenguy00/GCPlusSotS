using BepInEx.Configuration;
using GoldenCoastPlusRevived.Items;
using MiscFixes.Modules;

namespace GoldenCoastPlusRevived
{
    public static class PluginConfig
    {
        public static ConfigEntry<bool> FightChanges { get; set; }
        public static ConfigEntry<int> AurelioniteBossVulnerabilityTime { get; set; }
        public static ConfigEntry<float> AurelioniteArmorBrokenMult { get; set; }
        public static ConfigEntry<float> AurelioniteBlessingGoldGain { get; set; }

        internal static void Init(ConfigFile cfg)
        {
            BindFightChanges(cfg, "Aurelionite Fight Changes");
            BindTitanicGreatsword(cfg, "Titanic Greatsword");
            BindGoldenKnurl(cfg, "Golden Knurl");
            BindGuardiansEye(cfg, "Guardians Eye");
        }

        private static void BindFightChanges(ConfigFile cfg, string section)
        {
            FightChanges = cfg.BindOption(section,
                "Enable Fight Changes",
                "Should the changes to Aurelionite's fight be enabled?",
                true, Extensions.ConfigFlags.RestartRequired);

            AurelioniteBossVulnerabilityTime = cfg.BindOptionSlider(section,
                "Vulnerability Time",
                "Time in seconds that Aurelionite becomes vulnerable per phase",
                20,
                10, 120,
                Extensions.ConfigFlags.ServerSided | Extensions.ConfigFlags.RestartRequired);

            AurelioniteArmorBrokenMult = cfg.BindOptionSlider(section,
                "Damage taken multiplier debuff",
                "Adjust the boss's damage taken multiplier as decimal.",
                1.5f,
                0f, 5f,
                Extensions.ConfigFlags.ServerSided);

            AurelioniteBlessingGoldGain = cfg.BindOptionSlider(section,
                "Aurelionite's Blessing additional gold gain multiplier (0.25 = 25%)",
                "Adjust the buffs gold gain multiplier",
                0.25f,
                0f, 1f,
                Extensions.ConfigFlags.ServerSided);
        }

        private static void BindTitanicGreatsword(ConfigFile cfg, string section)
        {
            BigSword.EnableSword = cfg.Bind(section, "Enable Titanic Greatsword", true, "Should Titanic Greatsword be enabled?");
            BigSword.SwordDamage = cfg.Bind(section, "Titanic Greatsword Damage", 12.5f, "Adjust Titanic Greatsword's damage coefficient, as a decimal.");
            BigSword.SwordChance = cfg.Bind(section, "Titanic Greatsword Chance", 5f, "Adjust Titanic Greatsword's chance to proc, as a percentage.");
            BigSword.SwordProcCoeff = cfg.Bind(section, "Titanic Greatsword Proc Coeff", 1f, "Adjust Titanic Greatsword's proc coeff.");
        }
        
        private static void BindGoldenKnurl(ConfigFile cfg, string section)
        {

            GoldenKnurl.EnableKnurl = cfg.Bind(section, "Enable Golden Knurl", true, "Should Golden Knurl be enabled?");
            GoldenKnurl.KnurlHealth = cfg.Bind(section, "Golden Knurl Health", 0.1f, "Adjust how much max health Golden Knurl grants, as a decimal.");
            GoldenKnurl.KnurlRegen = cfg.Bind(section, "Golden Knurl Regen", 2.4f, "Adjust how much regen Golden Knurl grants.");
            GoldenKnurl.KnurlArmor = cfg.Bind(section, "Golden Knurl Armor", 20f, "Adjust how much armor Golden Knurl grants.");

        }
        
        private static void BindGuardiansEye(ConfigFile cfg, string section)
        {
            LaserEye.EnableEye = cfg.Bind(section, "Enable Guardians Eye", true, "Should Guardian's Eye be enabled?");
            LaserEye.EyeDamage = cfg.Bind(section, "Guardians Eye Damage", 25f, "Adjust how much damage Guardian's Eye does, as a decimal.");
            LaserEye.EyeDamage = cfg.Bind(section, "Guardians Eye Blast Proc Coeff", 1f, "Adjust the proc coefficient of Guardian's Eye blast, as a decimal.");
            LaserEye.EyeStacksRequired = cfg.Bind(section, "Guardians Eye Stacks Required", 10, "Adjust how many stacks are required for Guardian's Eye to trigger.");
        }
    }
}
