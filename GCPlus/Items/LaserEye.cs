using System.Collections.Generic;
using BepInEx.Configuration;
using GoldenCoastPlusRevived.Modules;
using R2API;
using RoR2;
using RoR2BepInExPack.GameAssetPaths;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace GoldenCoastPlusRevived.Items
{
    internal class LaserEye : ItemBase<LaserEye>
	{
        public LaserEye() : base(EnableEye.Value) { }

        internal override string name => "Guardian's Eye";
        internal override string token => "LaserEye";
        internal override string pickup => "Collecting gold will charge up a powerful laser.";
        internal override string description => "<style=cShrine>Collecting gold</style> will charge up a <style=cDeath>powerful laser</style> and grant a <style=cIsUtility>stacking buff</style>. " +
            "At ten stacks of this buff, the <style=cDeath>laser</style> will fire at all enemies within 30 meters, dealing <style=cIsDamage>2500%</style> <style=cStack>(+2500% per stack)</style> BASE damage.";
        internal override string lore => "The ability to see. Designs such as this have no need to taste. They do not feel, nor smell, nor hear that which surrounds them. " +
            "They have no need.\n\n...\n\nA sad existence. Forced to follow orders. Nothing more than a servant. No feelings.\n\nPerhaps one exception. " +
            "One day, this guardian will have no one to take orders from. Will have new challenges to overcome, and new decisions to make. A proper protector for this planet.";


        internal override GameObject modelPrefab => GCPAssets.LaserEyePrefab;
        internal override Sprite iconSprite => GCPAssets.LaserEyeIcon;
        internal override ItemTier Tier => ItemTier.Boss;
        internal override ItemTag[] ItemTags => new ItemTag[] { ItemTag.BrotherBlacklist, ItemTag.CannotDuplicate, ItemTag.WorldUnique };
        internal override bool hidden => false;

        public static ConfigEntry<bool> EnableEye { get; set; }
        public static ConfigEntry<float> EyeDamage { get; set; }
        public static ConfigEntry<int> EyeStacksRequired { get; set; }
        public static ConfigEntry<float> EyeBlastProcCoeff { get; set; }

        internal override void AddItem()
        {
            GCPAssets.LaserEyePrefab = Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Meteor.PickupMeteor_prefab).WaitForCompletion().InstantiateClone("GCPPickupLaserEye", false);
            GCPAssets.LaserEyePrefab.GetComponentInChildren<Renderer>().materials =
            [   
                new Material(Addressables.LoadAssetAsync<Material>(RoR2_Base_NearbyDamageBonus.matDiamond_mat).WaitForCompletion()) { color = Color.red },
                new Material(Addressables.LoadAssetAsync<Material>(RoR2_Base_Titan.matTitanGold_mat).WaitForCompletion())
            ];

            var infos = new List<CharacterModel.RendererInfo>();
            foreach (var renderer in GCPAssets.LaserEyePrefab.GetComponentsInChildren<Renderer>())
            {
                infos.Add(new CharacterModel.RendererInfo
                {
                    renderer = renderer,
                    defaultMaterial = renderer.material
                });
            }

            GCPAssets.LaserEyePrefab.AddComponent<ItemDisplay>().rendererInfos = [.. infos];

            base.AddItem();

            var dt = Addressables.LoadAssetAsync<ExplicitPickupDropTable>(RoR2_Base_Titan.dtBossTitanGold_asset).WaitForCompletion();
            HG.ArrayUtils.ArrayAppend(ref dt.pickupEntries, new ExplicitPickupDropTable.PickupDefEntry
            {
                pickupWeight = 1,
                pickupDef = itemDef
            });
        }

        internal override void AddHooks()
        {
            base.AddHooks();

            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                body.AddItemBehavior<LaserEyeBehavior>(body.inventory.GetItemCount(itemDef));
            }
        }

        internal override ItemDisplayRuleDict AddItemDisplays()
		{
			ItemDisplayRule[] array = new ItemDisplayRule[1];
			int num = 0;
			ItemDisplayRule itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(-0.0013f, 0.177f, 0.1923f);
			itemDisplayRule.localAngles = new Vector3(357.5498f, 279.5151f, 288.6346f);
			itemDisplayRule.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			array[num] = itemDisplayRule;
			ItemDisplayRuleDict itemDisplayRuleDict = new ItemDisplayRuleDict(array);
			ItemDisplayRuleDict itemDisplayRuleDict2 = itemDisplayRuleDict;
			string bodyPrefabName = "mdlHuntress";
			ItemDisplayRule[] array2 = new ItemDisplayRule[1];
			int num2 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(0.018f, 0.1588f, 0.1245f);
			itemDisplayRule.localAngles = new Vector3(341.0291f, 278.4884f, 331.1128f);
			itemDisplayRule.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			array2[num2] = itemDisplayRule;
			itemDisplayRuleDict2.Add(bodyPrefabName, array2);
			ItemDisplayRuleDict itemDisplayRuleDict3 = itemDisplayRuleDict;
			string bodyPrefabName2 = "mdlToolbot";
			ItemDisplayRule[] array3 = new ItemDisplayRule[1];
			int num3 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "Head";
			itemDisplayRule.localPos = new Vector3(0.4261f, 2.8735f, -0.9273f);
			itemDisplayRule.localAngles = new Vector3(351.3385f, 95.4094f, 354.9869f);
			itemDisplayRule.localScale = new Vector3(1.2285f, 1.2285f, 1.2285f);
			array3[num3] = itemDisplayRule;
			itemDisplayRuleDict3.Add(bodyPrefabName2, array3);
			ItemDisplayRuleDict itemDisplayRuleDict4 = itemDisplayRuleDict;
			string bodyPrefabName3 = "mdlEngi";
			ItemDisplayRule[] array4 = new ItemDisplayRule[1];
			int num4 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(-0.0016f, 0.2146f, 0.243f);
			itemDisplayRule.localAngles = new Vector3(0f, 271.3374f, 288.438f);
			itemDisplayRule.localScale = new Vector3(0.1369f, 0.1369f, 0.1369f);
			array4[num4] = itemDisplayRule;
			itemDisplayRuleDict4.Add(bodyPrefabName3, array4);
			ItemDisplayRuleDict itemDisplayRuleDict5 = itemDisplayRuleDict;
			string bodyPrefabName4 = "mdlEngiTurret";
			ItemDisplayRule[] array5 = new ItemDisplayRule[1];
			int num5 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "Head";
			itemDisplayRule.localPos = new Vector3(0f, 0.76f, 0.698f);
			itemDisplayRule.localAngles = new Vector3(0f, 269.6484f, 293.6638f);
			itemDisplayRule.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			array5[num5] = itemDisplayRule;
			itemDisplayRuleDict5.Add(bodyPrefabName4, array5);
			ItemDisplayRuleDict itemDisplayRuleDict6 = itemDisplayRuleDict;
			string bodyPrefabName5 = "mdlMage";
			ItemDisplayRule[] array6 = new ItemDisplayRule[1];
			int num6 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(-0.0249f, 0.1892f, 0.0943f);
			itemDisplayRule.localAngles = new Vector3(359.8906f, 278.0999f, 317.4402f);
			itemDisplayRule.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			array6[num6] = itemDisplayRule;
			itemDisplayRuleDict6.Add(bodyPrefabName5, array6);
			ItemDisplayRuleDict itemDisplayRuleDict7 = itemDisplayRuleDict;
			string bodyPrefabName6 = "mdlMerc";
			ItemDisplayRule[] array7 = new ItemDisplayRule[1];
			int num7 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(-0.0033f, 0.1505f, 0.1603f);
			itemDisplayRule.localAngles = new Vector3(355.2179f, 277.7787f, 314.9767f);
			itemDisplayRule.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			array7[num7] = itemDisplayRule;
			itemDisplayRuleDict7.Add(bodyPrefabName6, array7);
			ItemDisplayRuleDict itemDisplayRuleDict8 = itemDisplayRuleDict;
			string bodyPrefabName7 = "mdlTreebot";
			ItemDisplayRule[] array8 = new ItemDisplayRule[1];
			int num8 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "PlatformBase";
			itemDisplayRule.localPos = new Vector3(-0.0304f, -0.3919f, 0.5383f);
			itemDisplayRule.localAngles = new Vector3(1.1289f, 279.7583f, 267.7908f);
			itemDisplayRule.localScale = new Vector3(0.415f, 0.415f, 0.415f);
			array8[num8] = itemDisplayRule;
			itemDisplayRuleDict8.Add(bodyPrefabName7, array8);
			ItemDisplayRuleDict itemDisplayRuleDict9 = itemDisplayRuleDict;
			string bodyPrefabName8 = "mdlLoader";
			ItemDisplayRule[] array9 = new ItemDisplayRule[1];
			int num9 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(-0.001f, 0.1805f, 0.1468f);
			itemDisplayRule.localAngles = new Vector3(354.7388f, 278.3596f, 305.6868f);
			itemDisplayRule.localScale = new Vector3(0.135f, 0.135f, 0.135f);
			array9[num9] = itemDisplayRule;
			itemDisplayRuleDict9.Add(bodyPrefabName8, array9);
			ItemDisplayRuleDict itemDisplayRuleDict10 = itemDisplayRuleDict;
			string bodyPrefabName9 = "mdlCroco";
			ItemDisplayRule[] array10 = new ItemDisplayRule[1];
			int num10 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "Head";
			itemDisplayRule.localPos = new Vector3(0.034f, 2.327f, 0.9617f);
			itemDisplayRule.localAngles = new Vector3(358.9204f, 280.5208f, 318.6186f);
			itemDisplayRule.localScale = new Vector3(0.766f, 0.766f, 0.766f);
			array10[num10] = itemDisplayRule;
			itemDisplayRuleDict10.Add(bodyPrefabName9, array10);
			ItemDisplayRuleDict itemDisplayRuleDict11 = itemDisplayRuleDict;
			string bodyPrefabName10 = "mdlCaptain";
			ItemDisplayRule[] array11 = new ItemDisplayRule[1];
			int num11 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(0f, 0.1768f, 0.191f);
			itemDisplayRule.localAngles = new Vector3(350.8254f, 277.3032f, 304.3293f);
			itemDisplayRule.localScale = new Vector3(0.1213f, 0.1213f, 0.1213f);
			array11[num11] = itemDisplayRule;
			itemDisplayRuleDict11.Add(bodyPrefabName10, array11);
			ItemDisplayRuleDict itemDisplayRuleDict12 = itemDisplayRuleDict;
			string bodyPrefabName11 = "mdlBandit2";
			ItemDisplayRule[] array12 = new ItemDisplayRule[1];
			int num12 = 0;
			itemDisplayRule = default(ItemDisplayRule);
			itemDisplayRule.ruleType = 0;
			itemDisplayRule.followerPrefab = GCPAssets.LaserEyePrefab;
			itemDisplayRule.childName = "Chest";
			itemDisplayRule.localPos = new Vector3(-0.0021f, 0.2298f, 0.1222f);
			itemDisplayRule.localAngles = new Vector3(358.215f, 265.8196f, 314.7138f);
			itemDisplayRule.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			array12[num12] = itemDisplayRule;
			itemDisplayRuleDict12.Add(bodyPrefabName11, array12);
			return itemDisplayRuleDict;
		}
	}
}
