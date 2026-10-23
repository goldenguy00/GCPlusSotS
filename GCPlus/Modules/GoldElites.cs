using BepInEx.Configuration;
using GoldenCoastPlusRevived.Buffs;
using R2API;
using RoR2;
using RoR2BepInExPack.GameAssetPaths.Version_1_35_0;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace GoldenCoastPlusRevived.Modules
{
    /// <summary>
    /// Brings back the unused Gold elites (Coven of Gold) from the game's junk content, spawning only in Golden Coast.
    /// Their attacks steal gold on hit, which they drop on death along with a bigger gold reward.
    /// A player holding Coven of Gold gains gold on hit instead.
    /// </summary>
    internal static class GoldElites
    {
        public static ConfigEntry<bool> EnableGoldElites { get; set; }
        public static ConfigEntry<float> EliteCostMultiplier { get; set; }
        public static ConfigEntry<float> HealthBoostCoefficient { get; set; }
        public static ConfigEntry<float> DamageBoostCoefficient { get; set; }
        public static ConfigEntry<float> GoldStealAmount { get; set; }
        public static ConfigEntry<float> GoldRewardMultiplier { get; set; }

        public static EliteDef GoldEliteDef;
        public static EquipmentDef GoldEliteEquipment;

        private static GameObject coinImpactEffect;

        /// <summary>
        /// Called before the AffixGold buff is created, since the buff needs the elite def.
        /// </summary>
        internal static void LoadAssets()
        {
            if (!EnableGoldElites.Value)
                return;

            GoldEliteDef = Addressables.LoadAssetAsync<EliteDef>(RoR2_Junk_EliteGold.edGold_asset).WaitForCompletion();
            GoldEliteEquipment = Addressables.LoadAssetAsync<EquipmentDef>(RoR2_Junk_EliteGold.EliteGoldEquipment_asset).WaitForCompletion();
            coinImpactEffect = Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Common_VFX.CoinImpact_prefab).WaitForCompletion();

            var gildedElite = Addressables.LoadAssetAsync<EliteDef>(RoR2_DLC2_Elites_EliteAurelionite.edAurelionite_asset).WaitForCompletion();

            LanguageAPI.Add("GCP_ELITE_MODIFIER_GOLD", "Gold {0}");
            LanguageAPI.Add("GCP_EQUIPMENT_AFFIXGOLD_NAME", "Coven of Gold");
            LanguageAPI.Add("GCP_EQUIPMENT_AFFIXGOLD_PICKUP", "Become an aspect of wealth.");
            LanguageAPI.Add("GCP_EQUIPMENT_AFFIXGOLD_DESC", "Become an aspect of wealth. Your attacks <style=cShrine>grant gold on hit</style>, scaling with time.");

            GoldEliteDef.modifierToken = "GCP_ELITE_MODIFIER_GOLD";
            GoldEliteDef.eliteEquipmentDef = GoldEliteEquipment;
            GoldEliteDef.color = new Color32(255, 214, 89, 255);
            GoldEliteDef.healthBoostCoefficient = HealthBoostCoefficient.Value;
            GoldEliteDef.damageBoostCoefficient = DamageBoostCoefficient.Value;

            // the gold ramp from the old elite was never finished and renders black, borrow the gilded one instead
            if (gildedElite)
                GoldEliteDef.shaderEliteRampIndex = gildedElite.shaderEliteRampIndex;

            GoldEliteEquipment.nameToken = "GCP_EQUIPMENT_AFFIXGOLD_NAME";
            GoldEliteEquipment.pickupToken = "GCP_EQUIPMENT_AFFIXGOLD_PICKUP";
            GoldEliteEquipment.descriptionToken = "GCP_EQUIPMENT_AFFIXGOLD_DESC";
            GoldEliteEquipment.pickupIconSprite = GCPAssets.AffixGoldIcon;
            if (!GoldEliteEquipment.pickupModelPrefab)
                GoldEliteEquipment.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2_DLC2_Elites_EliteAurelionite.PickupAffixAurelionite_prefab).WaitForCompletion();
        }

        internal static void Init()
        {
            if (!EnableGoldElites.Value || !GoldEliteDef || !AffixGold.instance?.buffDef)
                return;

            GoldEliteEquipment.passiveBuffDef = AffixGold.instance.buffDef;

            EliteAPI.AddCustomEliteTier(new CombatDirector.EliteTierDef
            {
                costMultiplier = CombatDirector.baseEliteCostMultiplier * EliteCostMultiplier.Value,
                eliteTypes = [GoldEliteDef],
                isAvailable = rules => rules == SpawnCard.EliteRules.Default && IsGoldenCoast(),
                canSelectWithoutAvailableEliteDef = false
            });

            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            On.RoR2.DeathRewards.OnKilledServer += DeathRewards_OnKilledServer;
        }

        private static bool IsGoldenCoast()
        {
            var scene = SceneCatalog.mostRecentSceneDef;
            return scene && scene.cachedName == "goldshores";
        }

        private static void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            var attackerBody = report.attackerBody;
            if (!attackerBody || !attackerBody.master || report.damageInfo.procCoefficient <= 0f || !attackerBody.HasBuff(AffixGold.instance.buffDef))
                return;

            var amount = (uint)Mathf.Max(1f, GoldStealAmount.Value * Run.instance.difficultyCoefficient * report.damageInfo.procCoefficient);

            // players holding the aspect simply earn gold on hit
            if (attackerBody.isPlayerControlled)
            {
                attackerBody.master.GiveMoney(amount);
            }
            else
            {
                // monsters steal it from players, and hold on to it until they die
                var victimMaster = report.victimMaster;
                if (!victimMaster || victimMaster.money == 0)
                    return;

                amount = System.Math.Min(amount, victimMaster.money);
                victimMaster.money -= amount;

                var stash = attackerBody.gameObject.GetComponent<StolenGold>() ?? attackerBody.gameObject.AddComponent<StolenGold>();
                stash.amount += amount;
            }

            EffectManager.SimpleImpactEffect(coinImpactEffect, report.damageInfo.position, Vector3.up, true);
        }

        private static void DeathRewards_OnKilledServer(On.RoR2.DeathRewards.orig_OnKilledServer orig, DeathRewards self, DamageReport damageReport)
        {
            if (NetworkServer.active && self.characterBody && self.characterBody.HasBuff(AffixGold.instance.buffDef))
            {
                self.goldReward = (uint)(self.goldReward * GoldRewardMultiplier.Value);

                if (self.TryGetComponent<StolenGold>(out var stash))
                    self.goldReward += stash.amount;
            }

            orig(self, damageReport);
        }

        private class StolenGold : MonoBehaviour
        {
            public uint amount;
        }
    }
}
