using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2BepInExPack.GameAssetPaths.Version_1_35_0;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GoldenCoastPlusRevived.Modules
{
    /// <summary>
    /// Allied Aurelionite (Halcyon Seed) is stronger, and its damage scales linearly with seed count like its health does.
    /// </summary>
    internal static class SeedChanges
    {
        public static ConfigEntry<bool> EnableSeedChanges { get; set; }
        public static ConfigEntry<int> AllyStrengthMultiplier { get; set; }
        public static ConfigEntry<float> SeedDropWeight { get; set; }

        /// <summary>
        /// The false son fight spawns a hostile Aurelionite through the same code path, that one is left vanilla.
        /// </summary>
        private static bool spawningHostileTitan;

        internal static void Init()
        {
            SetSeedDropWeight();

            if (!EnableSeedChanges.Value)
                return;

            On.RoR2.GoldTitanManager.CalcTitanPowerAndBestTeam += GoldTitanManager_CalcTitanPowerAndBestTeam;
            IL.RoR2.GoldTitanManager.TryStartChannelingTitansServer += GoldTitanManager_TryStartChannelingTitansServer;

            AddSeedDescription();
        }

        private static void SetSeedDropWeight()
        {
            var dt = Addressables.LoadAssetAsync<ExplicitPickupDropTable>(RoR2_Base_Titan.dtBossTitanGold_asset).WaitForCompletion();
            var seed = Addressables.LoadAssetAsync<ItemDef>(RoR2_Base_TitanGoldDuringTP.TitanGoldDuringTP_asset).WaitForCompletion();
            for (int i = 0; i < dt.pickupEntries.Length; i++)
            {
                if (dt.pickupEntries[i].pickupDef == seed)
                    dt.pickupEntries[i].pickupWeight = SeedDropWeight.Value;
            }
        }

        private static void AddSeedDescription()
        {
            var mult = AllyStrengthMultiplier.Value * 100;
            R2API.LanguageAPI.Add("ITEM_TITANGOLDDURINGTP_DESC",
                "Summon <style=cIsDamage>Aurelionite</style> during the teleporter event. " +
                $"It has <style=cIsDamage>{mult}%</style> <style=cStack>(+{mult}% per stack)</style> <style=cIsDamage>damage</style> " +
                $"and <style=cIsHealing>{mult}%</style> <style=cStack>(+{mult}% per stack)</style> <style=cIsHealing>health</style>.");
        }

        private static void GoldTitanManager_CalcTitanPowerAndBestTeam(On.RoR2.GoldTitanManager.orig_CalcTitanPowerAndBestTeam orig, out int totalItemCount, out TeamIndex teamIndex)
        {
            orig(out totalItemCount, out teamIndex);

            // read before TryStartChannelingTitansServer resets it
            spawningHostileTitan = GoldTitanManager.isFalseSonBossLunarShardBrokenMaster;
            if (!spawningHostileTitan)
                totalItemCount *= AllyStrengthMultiplier.Value;
        }

        private static void GoldTitanManager_TryStartChannelingTitansServer(ILContext il)
        {
            var c = new ILCursor(il);

            // currentBoostDamageCoefficient *= Mathf.Pow(totalItemCount, 0.5f);
            if (c.TryGotoNext(MoveType.After,
                    x => x.MatchLdcR4(0.5f),
                    x => x.MatchCallOrCallvirt<Mathf>(nameof(Mathf.Pow))))
            {
                c.Index--;
                c.EmitDelegate<System.Func<float, float>>(exponent => spawningHostileTitan ? exponent : 1f);
            }
            else Log.Error("Patch failed for GoldTitanManager_TryStartChannelingTitansServer");
        }
    }
}
