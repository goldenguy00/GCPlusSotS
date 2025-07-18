﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using GoldenCoastPlusRevived.Buffs;
using GoldenCoastPlusRevived.Items;
using GoldenCoastPlusRevived.Modules;
using HarmonyLib;
using MiscFixes.Modules;

namespace GoldenCoastPlusRevived
{
    public static class PluginConfig
    {
        private static ConfigFile _backupConfig;
        private static bool _versionChanged;
        public static ConfigEntry<bool> AutoConfig { get; set; }
        public static ConfigEntry<string> LatestVersion { get; set; }

        internal static void Init(ConfigFile cfg)
        {
            BindConfig(cfg, "1. Main");
            BindFightChanges(cfg, "2. Aurelionite Fight Changes");
            BindTitanicGreatsword(cfg, "3. Titanic Greatsword");
            BindGoldenKnurl(cfg, "4. Golden Knurl");
            BindGuardiansEye(cfg, "5. Guardians Eye");
            ValidateConfig(cfg);
        }

        private static void BindConfig(ConfigFile cfg, string section)
        {
            PluginConfig.AutoConfig = cfg.BindOption(section,
                "Enable Auto Config Sync",
                "Disabling this would stop GCP from syncing config whenever a new version is found.",
                true,
                Extensions.ConfigFlags.RestartRequired);

            bool _preVersioning = !((Dictionary<ConfigDefinition, string>)AccessTools.DeclaredPropertyGetter(typeof(ConfigFile), "OrphanedEntries").Invoke(cfg, null)).Keys.Any(x => x.Key == "Latest Version");

            PluginConfig.LatestVersion = cfg.BindOption(section,
                "Latest Version", 
                "DO NOT CHANGE THIS",
                GoldenCoastPlusPlugin.MOD_VERSON);

            if (AutoConfig.Value && (_preVersioning || (LatestVersion.Value != GoldenCoastPlusPlugin.MOD_VERSON)))
            {
                LatestVersion.Value = GoldenCoastPlusPlugin.MOD_VERSON;
                _versionChanged = true;
                Log.Info("Config Autosync Enabled.");
            }
        }

        private static void BindFightChanges(ConfigFile cfg, string section)
        {
            FightChanges.EnableFightChanges = cfg.BindOption(section,
                "Enable Fight Changes",
                "Should the changes to Aurelionite's fight be enabled?",
                true, 
                Extensions.ConfigFlags.RestartRequired);

            FightChanges.BossHealthMult = cfg.BindOptionSlider(section,
                "Boss Aurelionites health per level multiplier",
                "Adjust the multiplier on the bosses health per level.",
                1f,
                0.1f, 5f,
                Extensions.ConfigFlags.RestartRequired);

            FightChanges.UseAdaptiveArmor = cfg.BindOption(section,
                "Boss Adaptive Armor",
                "Should the boss Aurelionite have adaptive armor?",
                true,
                Extensions.ConfigFlags.RestartRequired);

            TitanGoldArmorBroken.BossArmorBrokenMult = cfg.BindOptionSlider(section,
                "Initial Damage taken multiplier debuff",
                "Adjust the boss's damage taken multiplier, for a short window after each invulnerability phase ends",
                1.5f,
                0.1f, 3f,
                Extensions.ConfigFlags.ServerSided);

            HiddenGoldItem.AurelioniteBlessingGoldGain = cfg.BindOptionSlider(section,
                "Aurelionites Blessing gold gain multiplier",
                "Adjust the percent additional gold gain.",
                25f,
                0f, 100f,
                Extensions.ConfigFlags.ServerSided);
        }

        private static void BindTitanicGreatsword(ConfigFile cfg, string section)
        {
            BigSword.EnableSword = cfg.BindOption(section, 
                "Enable Titanic Greatsword", 
                "Should Titanic Greatsword be enabled?", 
                true, 
                Extensions.ConfigFlags.RestartRequired);

            BigSword.SwordDamage = cfg.BindOptionSlider(section, 
                "Titanic Greatsword Damage", 
                "Adjust the TOTAL damage coefficient of Titanic Greatsword",
                8f, 
                1f, 25f,
                Extensions.ConfigFlags.ServerSided);

            BigSword.SwordChance = cfg.BindOptionSlider(section, 
                "Titanic Greatsword Chance", 
                "Adjust Titanic Greatsword's chance to proc, as a percentage.",
                5f,
                1f, 100f, 
                Extensions.ConfigFlags.ServerSided);

            BigSword.SwordProcCoeff = cfg.BindOptionSlider(section, 
                "Titanic Greatsword Proc Coeff", 
                "Adjust Titanic Greatsword's proc coefficient.", 
                1f,
                0f, 3f, 
                Extensions.ConfigFlags.ServerSided);
        }
        
        private static void BindGoldenKnurl(ConfigFile cfg, string section)
        {
            GoldenKnurl.EnableKnurl = cfg.BindOption(section, 
                "Enable Golden Knurl",
                "Should Golden Knurl be enabled?", 
                true,
                Extensions.ConfigFlags.RestartRequired);

            GoldenKnurl.KnurlLevelHealth = cfg.BindOptionSlider(section, 
                "Golden Knurl Health", 
                "Adjust how much health per level Golden Knurl grants.",
                33f,
                0f, 100f);

            GoldenKnurl.KnurlLevelRegen = cfg.BindOptionSlider(section,
                "Golden Knurl Regen", 
                "Adjust how much regen per level Golden Knurl grants.", 
                3f, 
                0f, 10f);

            GoldenKnurl.KnurlLevelArmor = cfg.BindOptionSlider(section,
                "Golden Knurl Armor", 
                "Adjust how much armor per level Golden Knurl grants.",
                3f,
                0f, 10f);
        }
        
        private static void BindGuardiansEye(ConfigFile cfg, string section)
        {
            LaserEye.EnableEye = cfg.BindOption(section, 
                "Enable Guardians Eye",
                "Should Guardian's Eye be enabled?",
                true,
                Extensions.ConfigFlags.RestartRequired);

            LaserEye.EyeDamage = cfg.BindOptionSlider(section,
                "Guardians Eye Damage",
                "Adjust the BASE damage coefficient of Guardian's Eye blast, per target.",
                6f,
                1f, 20f,
                Extensions.ConfigFlags.ServerSided);

            LaserEye.EyeTargetsPerStack = cfg.BindOptionSlider(section,
                "Guardians Eye Max Targets",
                "Adjust the max targets per stack the Guardian's Eye will hit.",
                2,
                1, 10,
                Extensions.ConfigFlags.ServerSided);

            LaserEye.EyeBlastProcCoeff = cfg.BindOptionSlider(section,
                "Guardians Eye Blast Proc Coeff",
                "Adjust the proc coefficient of Guardian's Eye blast.",
                1f,
                0f, 3f,
                Extensions.ConfigFlags.ServerSided);

            LaserEye.EyeMaxRange = cfg.BindOptionSlider(section,
                "Guardians Eye Range",
                "Adjust the targeting range of Guardian's Eye.",
                30f,
                1f, 100f,
                Extensions.ConfigFlags.ServerSided);

            LaserEye.EyeStacksMultiplier = cfg.BindOptionSlider(section,
                "Guardians Eye Stacks Required",
                "Adjust how much gold income is required for Guardian's Eye to trigger, as a multiple of the cost of a small chest.",
                1f, 
                0.1f, 5f,
                Extensions.ConfigFlags.ServerSided);
        }
        private static void ValidateConfig(ConfigFile cfg)
        {
            _backupConfig = new ConfigFile(System.IO.Path.Combine(BepInEx.Paths.ConfigPath, $"{GoldenCoastPlusPlugin.MOD_GUID}.Backup.cfg"), true);
            _backupConfig.Bind(": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :");

            foreach (var key in cfg.Keys)
            {
                var entry = cfg[key];
                var backupEntry = BindBackup(cfg, entry);

                if (!ConfigEqual(backupEntry.DefaultValue, backupEntry.BoxedValue))
                {
                    Log.Debug("Config Updated: " + entry.Definition.Section + " : " + entry.Definition.Key + " from " + entry.BoxedValue + " to " + entry.DefaultValue);
                    if (_versionChanged)
                    {
                        Log.Debug("Autosyncing...");
                        entry.BoxedValue = entry.DefaultValue;
                        backupEntry.BoxedValue = backupEntry.DefaultValue;
                    }
                }
            }

            cfg.WipeConfig();
            _backupConfig.WipeConfig();
        }

        private static ConfigEntryBase BindBackup(ConfigFile cfg, ConfigEntryBase entry)
        {
            var newDef = new ConfigDefinition(Regex.Replace(cfg.ConfigFilePath, "\\W", "") + " : " + entry.Definition.Section, entry.Definition.Key);
            var newDesc = new ConfigDescription(entry.Description.Description, entry.Description.AcceptableValues);

            if (entry.SettingType == typeof(string))
                return _backupConfig.Bind(newDef, (string)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(bool))
                return _backupConfig.Bind(newDef, (bool)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(sbyte))
                return _backupConfig.Bind(newDef, (sbyte)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(byte))
                return _backupConfig.Bind(newDef, (byte)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(short))
                return _backupConfig.Bind(newDef, (short)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(ushort))
                return _backupConfig.Bind(newDef, (ushort)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(int))
                return _backupConfig.Bind(newDef, (int)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(uint))
                return _backupConfig.Bind(newDef, (uint)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(long))
                return _backupConfig.Bind(newDef, (long)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(ulong))
                return _backupConfig.Bind(newDef, (ulong)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(float))
                return _backupConfig.Bind(newDef, (float)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(double))
                return _backupConfig.Bind(newDef, (double)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(decimal))
                return _backupConfig.Bind(newDef, (decimal)entry.DefaultValue, newDesc);

            if (entry.SettingType == typeof(Enum))
                return _backupConfig.Bind(newDef, entry.DefaultValue as Enum, newDesc);

            throw new InvalidOperationException("Cannot convert to type " + entry.SettingType.Name);
        }

        private static bool ConfigEqual(object a, object b)
        {
            if (a?.Equals(b) == true)
                return true;

            if (float.TryParse(a?.ToString(), out var fa) && float.TryParse(b?.ToString(), out var fb))
                return UnityEngine.Mathf.Abs(fa - fb) < 0.0001;

            return false;
        }
    }
}
