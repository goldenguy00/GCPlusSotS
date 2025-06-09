using BepInEx.Configuration;
using GoldenCoastPlusRevived.Modules;
using R2API;
using RoR2;
using RoR2BepInExPack.GameAssetPaths;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GoldenCoastPlusRevived.Items
{
    internal class GoldenKnurl : ItemBase<GoldenKnurl>
	{
        public GoldenKnurl() : base(EnableKnurl.Value) { }

        internal override string name => "Golden Knurl";
        internal override string token => "GoldenKnurl";
        internal override string pickup => "Boosts health, regeneration, and armor.";
        internal override string description => "<style=cIsHealing>Increase maximum health</style> by <style=cIsHealing>+10%</style> <style=cStack>(+10% per stack)</style>, " +
            "<style=cIsHealing>base health regen</style> by <style=cIsHealing>+2.4 hp/s</style> <style=cStack>(+2.4 hp/s per stack)</style>, and <style=cIsUtility>armor</style> " +
            "by <style=cIsUtility>+20</style> <style=cStack>(+20 per stack)</style>.";
        internal override string lore => "A well-tested design. One that's proven powerful, yet gentle. That which can coexist with this planet's creatures, but defend them when need be." +
            "\n\nA construction of gold. Something unique. Something distinct. Something iconic. One to stand out, and be remembered both by friend and foe.\n\nA guardian." +
            " A creature who can defend this Heaven. One day, we will no longer be here for these creatures. But this one will. One to ensure their survival, when we can not.";


        internal override GameObject modelPrefab => GCPAssets.GoldenKnurlPrefab;
        internal override Sprite iconSprite => GCPAssets.GoldenKnurlIcon;
        internal override ItemTier Tier => ItemTier.Boss;
        internal override ItemTag[] ItemTags => new ItemTag[] { ItemTag.BrotherBlacklist, ItemTag.CannotDuplicate, ItemTag.WorldUnique };
        internal override bool hidden => false;

        public static ConfigEntry<bool> EnableKnurl { get; set; }
        public static ConfigEntry<float> KnurlHealth { get; set; }
        public static ConfigEntry<float> KnurlRegen { get; set; }
        public static ConfigEntry<float> KnurlArmor { get; set; }

        internal override void AddItem()
        {
            base.AddItem();

            var material = Object.Instantiate(Addressables.LoadAssetAsync<Material>(RoR2_Base_Titan.matTitanGold_mat).WaitForCompletion());
            GCPAssets.GoldenKnurlPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Knurl.PickupKnurl_prefab).WaitForCompletion().InstantiateClone("PickupGoldenKnurl", false);
            var componentsInChildren = GCPAssets.GoldenKnurlPrefab.GetComponentsInChildren<Renderer>();
            foreach (var renderer in componentsInChildren)
            {
                renderer.material = material;
            }
            GCPAssets.GoldenKnurlFollowerPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Knurl.DisplayKnurl_prefab).WaitForCompletion().InstantiateClone("GCPAssets.GoldenKnurlFollowerPrefab", false);
            GCPAssets.GoldenKnurlFollowerPrefab.transform.Find("mdlKnurl").gameObject.SetActive(false);
            GCPAssets.GoldenKnurlFollowerPrefab.transform.Find("KnurlPebbleParticles").gameObject.SetActive(false);

            var gameObject = Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Knurl.DisplayKnurl_prefab).WaitForCompletion().transform.Find("mdlKnurl").gameObject.InstantiateClone("GoldenKnurlFollowerModel", false);
            var component = gameObject.GetComponent<Renderer>();
            component.material = material;
            gameObject.transform.parent = GCPAssets.GoldenKnurlFollowerPrefab.transform;
            GCPAssets.GoldenKnurlFollowerPrefab.transform.Find("GoldenKnurlFollowerModel").localPosition = GCPAssets.GoldenKnurlFollowerPrefab.transform.Find("mdlKnurl").localPosition;
            GCPAssets.GoldenKnurlFollowerPrefab.transform.Find("GoldenKnurlFollowerModel").localEulerAngles = GCPAssets.GoldenKnurlFollowerPrefab.transform.Find("mdlKnurl").localEulerAngles;
            var gameObject2 = Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Knurl.DisplayKnurl_prefab).WaitForCompletion().transform.Find("KnurlPebbleParticles").gameObject.InstantiateClone("GoldenKnurlFollowerPebbles", false);
            var component2 = gameObject2.GetComponent<Renderer>();
            component2.material = material;
            gameObject2.transform.parent = GCPAssets.GoldenKnurlFollowerPrefab.transform;
            GCPAssets.GoldenKnurlFollowerPrefab.transform.Find("GoldenKnurlFollowerPebbles").localPosition = GCPAssets.GoldenKnurlFollowerPrefab.transform.Find("KnurlPebbleParticles").localPosition;
            GCPAssets.GoldenKnurlFollowerPrefab.transform.Find("GoldenKnurlFollowerPebbles").localEulerAngles = GCPAssets.GoldenKnurlFollowerPrefab.transform.Find("KnurlPebbleParticles").localEulerAngles;

            var body = Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Titan.TitanGoldBody_prefab).WaitForCompletion().GetComponent<CharacterBody>();
            var dt = body.GetComponent<DeathRewards>().bossDropTable as ExplicitPickupDropTable;
            HG.ArrayUtils.ArrayAppend(ref dt.pickupEntries, new ExplicitPickupDropTable.PickupDefEntry
            {
                pickupWeight = 1,
                pickupDef = itemDef
            });
        }

        internal override void AddHooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += GoldenKnurlStatChanges;
        }

        private static void GoldenKnurlStatChanges(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var itemCount = sender.inventory?.GetItemCount(GoldenKnurl.ItemIndex) ?? 0;
            if (itemCount > 0)
            {
                args.armorAdd += KnurlArmor.Value * (float)itemCount;
                args.baseRegenAdd += KnurlRegen.Value * (float)itemCount;
                args.healthMultAdd += KnurlHealth.Value * (float)itemCount;
            }
        }

        internal override ItemDisplayRuleDict AddItemDisplays()
		{
			ItemDisplayRule[] array = new ItemDisplayRule[1];
			int num = 0;
			ItemDisplayRule itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.GoldenKnurlFollowerPrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(0.344f, 0.425f, -0.053f);
			itemDisplayRule.localAngles = new Vector3(276.5326f, 108.8338f, 239.9354f);
			itemDisplayRule.localScale = new Vector3(0.16f, 0.16f, 0.16f);
			array[num] = itemDisplayRule;
			ItemDisplayRuleDict itemDisplayRuleDict = new ItemDisplayRuleDict(array);
			ItemDisplayRuleDict itemDisplayRuleDict2 = itemDisplayRuleDict;
			string bodyPrefabName = "mdlHuntress";
			ItemDisplayRule[] array2 = new ItemDisplayRule[1];
			int num2 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.GoldenKnurlFollowerPrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(0.17f, 0.316f, 0.138f);
			itemDisplayRule.localAngles = new Vector3(280.0211f, 150.599f, 218.2666f);
			itemDisplayRule.localScale = new Vector3(0.11f, 0.11f, 0.11f);
			array2[num2] = itemDisplayRule;
			itemDisplayRuleDict2.Add(bodyPrefabName, array2);
			ItemDisplayRuleDict itemDisplayRuleDict3 = itemDisplayRuleDict;
			string bodyPrefabName2 = "mdlToolbot";
			ItemDisplayRule[] array3 = new ItemDisplayRule[1];
			int num3 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.GoldenKnurlFollowerPrefab;
			itemDisplayRule.childName = "UpperArmR";
			itemDisplayRule.localPos = new Vector3(0.32f, 0.09f, -0.03f);
			itemDisplayRule.localAngles = new Vector3(85.4049f, -0.0001f, -0.0001f);
			itemDisplayRule.localScale = new Vector3(1.01f, 1.01f, 1.01f);
			array3[num3] = itemDisplayRule;
			itemDisplayRuleDict3.Add(bodyPrefabName2, array3);
			ItemDisplayRuleDict itemDisplayRuleDict4 = itemDisplayRuleDict;
			string bodyPrefabName3 = "mdlEngi";
			ItemDisplayRule[] array4 = new ItemDisplayRule[1];
			int num4 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.GoldenKnurlFollowerPrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(0.386f, 0.425f, -0.053f);
			itemDisplayRule.localAngles = new Vector3(276.5326f, 108.8338f, 239.9354f);
			itemDisplayRule.localScale = new Vector3(0.16f, 0.16f, 0.16f);
			array4[num4] = itemDisplayRule;
			itemDisplayRuleDict4.Add(bodyPrefabName3, array4);
			ItemDisplayRuleDict itemDisplayRuleDict5 = itemDisplayRuleDict;
			string bodyPrefabName4 = "mdlMage";
			ItemDisplayRule[] array5 = new ItemDisplayRule[1];
			int num5 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.GoldenKnurlFollowerPrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(0.225f, 0.307f, 0.004f);
			itemDisplayRule.localAngles = new Vector3(279.6995f, 13.3832f, 338.7414f);
			itemDisplayRule.localScale = new Vector3(0.086f, 0.086f, 0.086f);
			array5[num5] = itemDisplayRule;
			itemDisplayRuleDict5.Add(bodyPrefabName4, array5);
			ItemDisplayRuleDict itemDisplayRuleDict6 = itemDisplayRuleDict;
			string bodyPrefabName5 = "mdlMerc";
			ItemDisplayRule[] array6 = new ItemDisplayRule[1];
			int num6 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.GoldenKnurlFollowerPrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(0.343f, 0.253f, -0.054f);
			itemDisplayRule.localAngles = new Vector3(276.8653f, 259.4112f, 89.62f);
			itemDisplayRule.localScale = new Vector3(0.11f, 0.11f, 0.11f);
			array6[num6] = itemDisplayRule;
			itemDisplayRuleDict6.Add(bodyPrefabName5, array6);
			ItemDisplayRuleDict itemDisplayRuleDict7 = itemDisplayRuleDict;
			string bodyPrefabName6 = "mdlTreebot";
			ItemDisplayRule[] array7 = new ItemDisplayRule[1];
			int num7 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.GoldenKnurlFollowerPrefab;
			itemDisplayRule.childName = "FlowerBase";
			itemDisplayRule.localPos = new Vector3(-0.5f, 0f, -0.023f);
			itemDisplayRule.localAngles = new Vector3(0f, 0f, 0f);
			itemDisplayRule.localScale = new Vector3(0.11f, 0.11f, 0.11f);
			array7[num7] = itemDisplayRule;
			itemDisplayRuleDict7.Add(bodyPrefabName6, array7);
			ItemDisplayRuleDict itemDisplayRuleDict8 = itemDisplayRuleDict;
			string bodyPrefabName7 = "mdlLoader";
			ItemDisplayRule[] array8 = new ItemDisplayRule[1];
			int num8 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.GoldenKnurlFollowerPrefab;
			itemDisplayRule.childName = "MechUpperArmR";
			itemDisplayRule.localPos = new Vector3(-0.095f, 0.066f, -0.071f);
			itemDisplayRule.localAngles = new Vector3(48.8601f, 224.1132f, 52.647f);
			itemDisplayRule.localScale = new Vector3(0.11f, 0.11f, 0.11f);
			array8[num8] = itemDisplayRule;
			itemDisplayRuleDict8.Add(bodyPrefabName7, array8);
			ItemDisplayRuleDict itemDisplayRuleDict9 = itemDisplayRuleDict;
			string bodyPrefabName8 = "mdlCroco";
			ItemDisplayRule[] array9 = new ItemDisplayRule[1];
			int num9 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.GoldenKnurlFollowerPrefab;
			itemDisplayRule.childName = "UpperArmR";
			itemDisplayRule.localPos = new Vector3(1.82f, 1.78f, 0.11f);
			itemDisplayRule.localAngles = new Vector3(284.9999f, 90.0006f, 284.9994f);
			itemDisplayRule.localScale = new Vector3(1.01f, 1.01f, 1.01f);
			array9[num9] = itemDisplayRule;
			itemDisplayRuleDict9.Add(bodyPrefabName8, array9);
			ItemDisplayRuleDict itemDisplayRuleDict10 = itemDisplayRuleDict;
			string bodyPrefabName9 = "mdlCaptain";
			ItemDisplayRule[] array10 = new ItemDisplayRule[1];
			int num10 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.GoldenKnurlFollowerPrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(0.405f, 0.404f, -0.042f);
			itemDisplayRule.localAngles = new Vector3(276.8653f, 259.4112f, 89.62f);
			itemDisplayRule.localScale = new Vector3(0.11f, 0.11f, 0.11f);
			array10[num10] = itemDisplayRule;
			itemDisplayRuleDict10.Add(bodyPrefabName9, array10);
			ItemDisplayRuleDict itemDisplayRuleDict11 = itemDisplayRuleDict;
			string bodyPrefabName10 = "mdlBandit2";
			ItemDisplayRule[] array11 = new ItemDisplayRule[1];
			int num11 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.GoldenKnurlFollowerPrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(0.228f, 0.383f, -0.109f);
			itemDisplayRule.localAngles = new Vector3(276.5326f, 108.8338f, 239.9354f);
			itemDisplayRule.localScale = new Vector3(0.086f, 0.086f, 0.086f);
			array11[num11] = itemDisplayRule;
			itemDisplayRuleDict11.Add(bodyPrefabName10, array11);
			return itemDisplayRuleDict;
		}
	}
}
