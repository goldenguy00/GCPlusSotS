﻿using System;
using GoldenCoastPlusRevived.Properties;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace GoldenCoastPlusRevived
{
	public class GCPAssets
	{
		public static void RegisterAssets()
		{
			ItemDisplayRuleSet itemDisplayRuleSet = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion().GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;
			Material material = Object.Instantiate<Material>(Addressables.LoadAssetAsync<Material>("RoR2/Base/Titan/matTitanGold.mat").WaitForCompletion());
			Material material2 = Object.Instantiate<Material>(Addressables.LoadAssetAsync<Material>("RoR2/Base/NearbyDamageBonus/matDiamond.mat").WaitForCompletion());
			material2.color = Color.red;
			Material material3 = Object.Instantiate<Material>(Addressables.LoadAssetAsync<Material>("RoR2/Base/Titan/matTitanGold.mat").WaitForCompletion());
			Material[] materials = new Material[]
			{
				material2,
				material3
			};
			GoldenKnurlPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Knurl/PickupKnurl.prefab").WaitForCompletion().InstantiateClone("PickupGoldenKnurl", false);
			Renderer[] componentsInChildren = GoldenKnurlPrefab.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.material = material;
			}
			GoldenKnurlFollowerPrefab = itemDisplayRuleSet.FindDisplayRuleGroup(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Knurl/Knurl.asset").WaitForCompletion()).rules[0].followerPrefab.InstantiateClone("GoldenKnurlFollowerPrefab", false);
			GoldenKnurlFollowerPrefab.transform.Find("mdlKnurl").gameObject.SetActive(false);
			GoldenKnurlFollowerPrefab.transform.Find("KnurlPebbleParticles").gameObject.SetActive(false);
			GameObject gameObject = itemDisplayRuleSet.FindDisplayRuleGroup(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Knurl/Knurl.asset").WaitForCompletion()).rules[0].followerPrefab.transform.Find("mdlKnurl").gameObject.InstantiateClone("GoldenKnurlFollowerModel", false);
			Renderer component = gameObject.GetComponent<Renderer>();
			component.material = material;
			gameObject.transform.parent = GoldenKnurlFollowerPrefab.transform;
			GoldenKnurlFollowerPrefab.transform.Find("GoldenKnurlFollowerModel").localPosition = GoldenKnurlFollowerPrefab.transform.Find("mdlKnurl").localPosition;
			GoldenKnurlFollowerPrefab.transform.Find("GoldenKnurlFollowerModel").localEulerAngles = GoldenKnurlFollowerPrefab.transform.Find("mdlKnurl").localEulerAngles;
			GameObject gameObject2 = itemDisplayRuleSet.FindDisplayRuleGroup(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Knurl/Knurl.asset").WaitForCompletion()).rules[0].followerPrefab.transform.Find("KnurlPebbleParticles").gameObject.InstantiateClone("GoldenKnurlFollowerPebbles", false);
			Renderer component2 = gameObject2.GetComponent<Renderer>();
			component2.material = material;
			gameObject2.transform.parent = GoldenKnurlFollowerPrefab.transform;
			GoldenKnurlFollowerPrefab.transform.Find("GoldenKnurlFollowerPebbles").localPosition = GoldenKnurlFollowerPrefab.transform.Find("KnurlPebbleParticles").localPosition;
			GoldenKnurlFollowerPrefab.transform.Find("GoldenKnurlFollowerPebbles").localEulerAngles = GoldenKnurlFollowerPrefab.transform.Find("KnurlPebbleParticles").localEulerAngles;
			GoldenKnurlIcon = RegisterIcons(GCPResources.Golden_Knurl);
			BigSwordPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanGoldBody.prefab").WaitForCompletion().GetComponent<ModelLocator>().modelTransform.Find("TitanArmature").Find("ROOT").Find("base").Find("stomach").Find("chest").Find("upper_arm.r").Find("lower_arm.r").Find("hand.r").Find("RightFist").gameObject.InstantiateClone("PickupBigSword", false);
			Transform transform = BigSwordPrefab.transform.Find("Sword");
			transform.transform.localPosition = new Vector3(4f, -5.5f, 0f);
			transform.transform.localEulerAngles = new Vector3(135f, 270f, 0f);
			BigSwordIcon = RegisterIcons(GCPResources.Titanic_Greatsword);
			LaserEyePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Meteor/PickupMeteor.prefab").WaitForCompletion().InstantiateClone("PickupLaserEye", false);
			Renderer componentInChildren = LaserEyePrefab.GetComponentInChildren<Renderer>();
			componentInChildren.materials = materials;
			LaserEyeIcon = RegisterIcons(GCPResources.Guardian_s_Eye);
			LaserEyeReadyIcon = Object.Instantiate<Sprite>(LegacyResourcesAPI.Load<BuffDef>("BuffDefs/MercExpose").iconSprite);
			TitanGoldArmorBrokenIcon = Object.Instantiate<Sprite>(LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Pulverized").iconSprite);
			AffixGoldIcon = RegisterIcons(GCPResources.Gold_Elite_Icon);
			hiddenGoldBuffIcon = RegisterIcons(GCPResources.Aurelionite_s_Blessing);
		}

		public static Sprite RegisterIcons(byte[] resourceBytes)
		{
			Texture2D texture2D = new Texture2D(128, 128, TextureFormat.RGBA32, false);
			ImageConversion.LoadImage(texture2D, resourceBytes, false);
			return Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(1f, 1f));
		}

		public static GameObject GoldenKnurlPrefab;
		public static GameObject GoldenKnurlFollowerPrefab;
		public static GameObject BigSwordPrefab;
		public static GameObject LaserEyePrefab;
		public static Sprite GoldenKnurlIcon;
		public static Sprite BigSwordIcon;
		public static Sprite LaserEyeIcon;
		public static Sprite LaserEyeReadyIcon;
		public static Sprite TitanGoldArmorBrokenIcon;
		public static Sprite AffixGoldIcon;
		public static Sprite hiddenGoldBuffIcon;
	}
}
