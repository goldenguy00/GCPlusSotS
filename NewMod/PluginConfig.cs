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
                true, 
                Extensions.ConfigFlags.RestartRequired);

            AurelioniteBossVulnerabilityTime = cfg.BindOptionSlider(section,
                "Vulnerability Time",
                "Time in seconds that Aurelionite becomes vulnerable per phase",
                20,
                10, 120,
                Extensions.ConfigFlags.ServerSided);

            AurelioniteArmorBrokenMult = cfg.BindOptionSlider(section,
                "Damage taken multiplier debuff",
                "Adjust the boss's damage taken multiplier as decimal.",
                1.5f,
                0f, 5f,
                Extensions.ConfigFlags.ServerSided);

            AurelioniteBlessingGoldGain = cfg.BindOptionSlider(section,
                "Aurelionite's Blessing buff additional gold gain multiplier",
                "Adjust the buffs gold gain multiplier (0.25 = 25%)",
                0.25f,
                0f, 1f,
                Extensions.ConfigFlags.ServerSided);
        }

        private static void BindTitanicGreatsword(ConfigFile cfg, string section)
        {
            BigSword.EnableSword = cfg.BindOption(section, "Enable Titanic Greatsword", "Should Titanic Greatsword be enabled?", true, Extensions.ConfigFlags.RestartRequired);
            BigSword.SwordDamage = cfg.BindOptionSlider(section, "Titanic Greatsword Damage", "Adjust Titanic Greatsword's damage coefficient, as a decimal.", 12.5f, 1f, 25f, Extensions.ConfigFlags.ServerSided);
            BigSword.SwordChance = cfg.BindOptionSlider(section, "Titanic Greatsword Chance", "Adjust Titanic Greatsword's chance to proc, as a percentage.", 5f, 0f, 100f, Extensions.ConfigFlags.ServerSided);
            BigSword.SwordProcCoeff = cfg.BindOptionSlider(section, "Titanic Greatsword Proc Coeff", "Adjust Titanic Greatsword's proc coeff.", 1f, 0f, 5f, Extensions.ConfigFlags.ServerSided);
        }
        
        private static void BindGoldenKnurl(ConfigFile cfg, string section)
        {

            GoldenKnurl.EnableKnurl = cfg.BindOption(section, "Enable Golden Knurl", "Should Golden Knurl be enabled?", true, Extensions.ConfigFlags.RestartRequired);
            GoldenKnurl.KnurlHealth = cfg.BindOptionSlider(section, "Golden Knurl Health", "Adjust how much max health Golden Knurl grants, as a decimal.", 0.1f, 0f, 1f);
            GoldenKnurl.KnurlRegen = cfg.BindOptionSlider(section, "Golden Knurl Regen", "Adjust how much regen Golden Knurl grants.", 2.4f, 0f, 10f);
            GoldenKnurl.KnurlArmor = cfg.BindOptionSlider(section, "Golden Knurl Armor", "Adjust how much armor Golden Knurl grants.", 20f, 0f, 100f);

        }
        
        private static void BindGuardiansEye(ConfigFile cfg, string section)
        {
            LaserEye.EnableEye = cfg.BindOption(section, "Enable Guardians Eye", "Should Guardian's Eye be enabled?", true, Extensions.ConfigFlags.RestartRequired);
            LaserEye.EyeDamage = cfg.BindOptionSlider(section, "Guardians Eye Damage", "Adjust how much damage Guardian's Eye does, as a decimal.", 2.5f, 0f, 10f, Extensions.ConfigFlags.ServerSided);
            LaserEye.EyeDamage = cfg.BindOptionSlider(section, "Guardians Eye Blast Proc Coeff", "Adjust the proc coefficient of Guardian's Eye blast, as a decimal.", 1f, 0f, 5f, Extensions.ConfigFlags.ServerSided);
            LaserEye.EyeStacksRequired = cfg.BindOptionSlider(section, "Guardians Eye Stacks Required", "Adjust how many stacks are required for Guardian's Eye to trigger.", 10, 1, 20, Extensions.ConfigFlags.ServerSided);
        }
    }
}
