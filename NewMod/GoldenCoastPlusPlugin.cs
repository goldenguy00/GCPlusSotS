using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using EntityStates.Missions.Goldshores;
using EntityStates.TitanMonster;
using GoldenCoastPlusRevived.Buffs;
using GoldenCoastPlusRevived.Items;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace GoldenCoastPlusRevived
{
	[BepInPlugin("com.Phreel.GoldenCoastPlusRevived", "GoldenCoastPlusRevived", "1.1.0")]

	public class GoldenCoastPlusPlugin : BaseUnityPlugin
	{

		public static ConfigEntry<bool> FightChanges { get; set; }
		public static ConfigEntry<float> AurelioniteMaxHealthMultiplier { get; set; }
		public static ConfigEntry<float> AurelioniteArmorBrokenMult { get; set; }
		public static ConfigEntry<uint> AurelioniteBlessingGoldGain { get; set; }
		public static ConfigEntry<bool> EnableSword { get; set; }
		public static ConfigEntry<float> SwordDamage { get; set; }
		public static ConfigEntry<float> SwordChance { get; set; }
		public static ConfigEntry<float> SwordProcCoeff { get; set; }
		public static ConfigEntry<bool> EnableKnurl { get; set; }
		public static ConfigEntry<float> KnurlHealth { get; set; }
		public static ConfigEntry<float> KnurlRegen { get; set; }
		public static ConfigEntry<float> KnurlArmor { get; set; }
		public static ConfigEntry<bool> EnableEye { get; set; }
		public static ConfigEntry<float> EyeDamage { get; set; }
		public static ConfigEntry<int> EyeStacksRequired { get; set; }
		public static ConfigEntry<float> EyeBlastProcCoeff { get; set; }
		public static ConfigEntry<bool> SeedChanges { get; set; }
		public static ConfigEntry<int> TitanGoldMultiplier { get; set; }
		public static ConfigEntry<bool> EnableGoldElites { get; set; }
		public static ConfigEntry<float> EliteCostMultiplier { get; set; }
		public static ConfigEntry<float> HealthBoostMultiplier { get; set; }
		public static ConfigEntry<float> DamageBoostMultiplier { get; set; }
		public static ConfigEntry<float> GoldStealAmount { get; set; }
		public static ConfigEntry<uint> GoldRewardAmount { get; set; }

		public static GameObject newProjectile;

		public void Awake()
		{
			this.ConfigInit();

			newProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanGoldPreFistProjectile.prefab").WaitForCompletion(), "name");

			newProjectile.GetComponent<ProjectileController>().procCoefficient = SwordProcCoeff.Value;

			newProjectile.GetComponent<ProjectileImpactExplosion>().blastProcCoefficient = SwordProcCoeff.Value;

			PrefabAPI.RegisterNetworkPrefab(newProjectile);
			ContentAddition.AddProjectile(newProjectile);
			
			GCPAssets.RegisterAssets();
			bool value = EnableGoldElites.Value;
			if (value)
			{
				affixGoldDef = new AffixGold().AddBuff();
				this.GoldElites();
			}
			bool value2 = EnableSword.Value;
			if (value2)
			{
				bigSwordDef = new BigSword().AddItem();
			}
			bool value3 = EnableKnurl.Value;
			if (value3)
			{
				goldenKnurlDef = new GoldenKnurl().AddItem();
			}
			bool value4 = EnableEye.Value;
			if (value4)
			{
				laserEyeDef = new LaserEye().AddItem();
				laserEyeChargeDef = new LaserEyeCharge().AddBuff();
			}
			bool value5 = SeedChanges.Value;
			if (value5)
			{
				LanguageAPI.Add("ITEM_TITANGOLDDURINGTP_DESC", "Summon <style=cIsDamage>Aurelionite</style> during the teleporter event. It has <style=cIsDamage>100%</style> <style=cStack>(+100% per stack)</style> <style=cIsDamage>damage</style> and <style=cIsHealing>100%</style> <style=cStack>(+100% per stack)</style> <style=cIsHealing>health</style>");
			}
			bool value6 = FightChanges.Value;
			if (value6)
			{
				titanGoldArmorBrokenDef = new TitanGoldArmorBroken().AddBuff();
				hiddenGoldBuffItemDef = new HiddenGoldBuffItem().AddItem();
				hiddenGoldBuffDef = new HiddenGoldBuff().AddBuff();
				Resources.Load<GameObject>("prefabs/characterbodies/TitanGoldBody").GetComponent<CharacterBody>().baseMaxHealth *= AurelioniteMaxHealthMultiplier.Value;
				Resources.Load<GameObject>("prefabs/characterbodies/TitanGoldBody").GetComponent<CharacterBody>().levelMaxHealth *= AurelioniteMaxHealthMultiplier.Value;
				
			}
			this.Hook();
		}

		private void ConfigInit()
		{
			FightChanges = base.Config.Bind<bool>("Aurelionite Fight Changes", "Enable Fight Changes", true, "Should the changes to Aurelionite's fight be enabled?");
			AurelioniteMaxHealthMultiplier = base.Config.Bind<float>("Aurelionite Fight Changes", "Max Health multiplier", 2f, "Adjust the max health multiplier as decimal.");
			AurelioniteArmorBrokenMult = base.Config.Bind<float>("Aurelionite Fight Changes", "Damage taken multiplier debuff", 1.5f, "Adjust the damage taken debuff multiplier as decimal.");
			AurelioniteBlessingGoldGain = base.Config.Bind<uint>("Aurelionite Fight Changes", "Aurelionites Blessing hidden items gold gain multiplier", 10U, "Adjust the buffs gold gain multiplier");
			EnableSword = base.Config.Bind<bool>("Titanic Greatsword", "Enable Titanic Greatsword", true, "Should Titanic Greatsword be enabled?");
			SwordDamage = base.Config.Bind<float>("Titanic Greatsword", "Titanic Greatsword Damage", 12.5f, "Adjust Titanic Greatsword's damage coefficient, as a decimal.");
			SwordChance = base.Config.Bind<float>("Titanic Greatsword", "Titanic Greatsword Chance", 5f, "Adjust Titanic Greatsword's chance to proc, as a percentage.");
			SwordProcCoeff = base.Config.Bind<float>("Titanic Greatsword", "Titanic Greatsword Proc Coeff", 1f, "Adjust Titanic Greatsword's proc coeff.");
			EnableKnurl = base.Config.Bind<bool>("Golden Knurl", "Enable Golden Knurl", true, "Should Golden Knurl be enabled?");
			KnurlHealth = base.Config.Bind<float>("Golden Knurl", "Golden Knurl Health", 0.1f, "Adjust how much max health Golden Knurl grants, as a decimal.");
			KnurlRegen = base.Config.Bind<float>("Golden Knurl", "Golden Knurl Regen", 2.4f, "Adjust how much regen Golden Knurl grants.");
			KnurlArmor = base.Config.Bind<float>("Golden Knurl", "Golden Knurl Armor", 20f, "Adjust how much armor Golden Knurl grants.");
			EnableEye = base.Config.Bind<bool>("Guardians Eye", "Enable Guardians Eye", true, "Should Guardian's Eye be enabled?");
			EyeDamage = base.Config.Bind<float>("Guardians Eye", "Guardians Eye Damage", 25f, "Adjust how much damage Guardian's Eye does, as a decimal.");
			EyeDamage = base.Config.Bind<float>("Guardians Eye", "Guardians Eye Blast Proc Coeff", 1f, "Adjust the proc coefficient of Guardian's Eye blast, as a decimal.");
			EyeStacksRequired = base.Config.Bind<int>("Guardians Eye", "Guardians Eye Stacks Required", 10, "Adjust how many stacks are required for Guardian's Eye to trigger.");
			SeedChanges = base.Config.Bind<bool>("Halcyon Seed Changes", "Enable Halcyon Seed Changes", true, "Should the changes to Halcyon Seed be enabled?");
			TitanGoldMultiplier = base.Config.Bind<int>("Halcyon Seed Changes", "Teleporter Aurelionite Buff Multiplier", 3, "Adjust how much Aurelionite's damage and health are buffed.");
			EnableGoldElites = base.Config.Bind<bool>("Gold Elites", "Enable Gold Elites", true, "Should Gold Elites be enabled?");
			EliteCostMultiplier = base.Config.Bind<float>("Gold Elites", "Cost Multiplier of Gold Elites in Golden Coast", 1f, "Adjust the cost multiplier relative to T1 elites as decimal.");
			HealthBoostMultiplier = base.Config.Bind<float>("Gold Elites", "Health Multiplier of Gold Elites in Golden Coast", 11f, "Adjust the health boost multiplier as decimal.");
			DamageBoostMultiplier = base.Config.Bind<float>("Gold Elites", "Damage Multiplier of Gold Elites in Golden Coast", 4f, "Adjust the damage boost multiplier as decimal.");
			GoldStealAmount = base.Config.Bind<float>("Gold Elites", "Gold Steal multiplier", 2f, "Adjust the stolen gold multiplier as decimal.");
			GoldRewardAmount = base.Config.Bind<uint>("Gold Elites", "Gold Reward multiplier", 5U, "Adjust the gold reward when killed multiplier.");
		}

		private void GoldElites()
		{
			EquipmentDef equipmentDef = Resources.Load<EquipmentDef>("equipmentdefs/AffixGold");
			equipmentDef.passiveBuffDef = affixGoldDef;
			equipmentDef.pickupModelPrefab = Resources.Load<GameObject>("prefabs/pickupmodels/PickupAffixGreen");
			equipmentDef.pickupIconSprite = Resources.Load<Sprite>("textures/itemicons/texAffixGreenIcon");
			equipmentDef.passiveBuffDef.eliteDef.shaderEliteRampIndex = 7;
			EliteDef eliteDef = Resources.Load<EliteDef>("elitedefs/Gold");
			eliteDef.damageBoostCoefficient = DamageBoostMultiplier.Value;
			eliteDef.healthBoostCoefficient = HealthBoostMultiplier.Value;
			CombatDirector.EliteTierDef eliteTierDef = new CombatDirector.EliteTierDef();
			eliteTierDef.costMultiplier = CombatDirector.baseEliteCostMultiplier * EliteCostMultiplier.Value;
			eliteTierDef.eliteTypes = new EliteDef[]
			{
				Resources.Load<EliteDef>("elitedefs/Gold")
			};
			eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => SceneCatalog.GetSceneDefForCurrentScene() == Resources.Load<SceneDef>("scenedefs/goldshores"));
			CombatDirector.EliteTierDef eliteTierDef2 = eliteTierDef;
			EliteAPI.AddCustomEliteTier(eliteTierDef2);
			LanguageAPI.Add("ELITE_MODIFIER_GOLD", "Gold {0}");
		}

		private void Hook()
		{
			bool value = SeedChanges.Value;
			if (value)
			{
				IL.RoR2.GoldTitanManager.TryStartChannelingTitansServer += GoldTitanManager_TryStartChannelingTitansServer;
				IL.RoR2.GoldTitanManager.CalcTitanPowerAndBestTeam += GoldTitanManager_CalcTitanPowerAndBestTeam;
			}
			On.RoR2.PickupDropletController.CreatePickupDroplet_PickupIndex_Vector3_Vector3 += new On.RoR2.PickupDropletController.hook_CreatePickupDroplet_PickupIndex_Vector3_Vector3(this.PickupDropletController_CreatePickupDroplet_PickupIndex_Vector3_Vector3);
			bool value2 = EnableKnurl.Value;
			if (value2)
			{
				RecalculateStatsAPI.GetStatCoefficients += this.GoldenKnurlStatChanges;
			}
			bool value3 = EnableGoldElites.Value;
			if (value3)
			{
				On.RoR2.DeathRewards.OnKilledServer += DeathRewards_OnKilledServer;
			}
			bool value4 = FightChanges.Value;
			if (value4)
			{
				On.RoR2.GoldshoresMissionController.Awake += new On.RoR2.GoldshoresMissionController.hook_Awake(this.GoldshoresMissionController_Start);
				On.EntityStates.Missions.Goldshores.GoldshoresBossfight.OnEnter += new On.EntityStates.Missions.Goldshores.GoldshoresBossfight.hook_OnEnter(this.GoldshoresBossfight_OnEnter);
				On.EntityStates.Missions.Goldshores.GoldshoresBossfight.OnExit += new On.EntityStates.Missions.Goldshores.GoldshoresBossfight.hook_OnExit(this.GoldshoresBossfight_OnExit);
				On.EntityStates.Missions.Goldshores.GoldshoresBossfight.ServerFixedUpdate += new On.EntityStates.Missions.Goldshores.GoldshoresBossfight.hook_ServerFixedUpdate(this.GoldshoresBossfight_ServerFixedUpdate);
				On.RoR2.HealthComponent.TakeDamage += new On.RoR2.HealthComponent.hook_TakeDamage(this.HealthComponent_TakeDamage);
				On.EntityStates.TitanMonster.FireGoldFist.PlacePredictedAttack += new On.EntityStates.TitanMonster.FireGoldFist.hook_PlacePredictedAttack(this.FireGoldFist_PlacePredictedAttack);
				On.RoR2.CharacterMaster.GiveMoney += new On.RoR2.CharacterMaster.hook_GiveMoney(this.CharacterMaster_GiveMoney);
			}
			bool flag = EnableEye.Value || FightChanges.Value;
			if (flag)
			{
				On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
			}
			bool flag2 = EnableSword.Value || EnableGoldElites.Value;
			if (flag2)
			{
				On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
			}
		}

		private void CharacterMaster_GiveMoney(On.RoR2.CharacterMaster.orig_GiveMoney orig, CharacterMaster self, uint amount)
		{
			bool flag = self.bodyInstanceObject;
			if (flag)
			{
				amount += amount / AurelioniteBlessingGoldGain.Value * (uint)self.bodyInstanceObject.GetComponent<CharacterBody>().GetBuffCount(hiddenGoldBuffDef);
			}
			orig(self, amount);
		}

		private void FireGoldFist_PlacePredictedAttack(On.EntityStates.TitanMonster.FireGoldFist.orig_PlacePredictedAttack orig, FireGoldFist self)
		{
			bool flag = FireGoldFist.fistCount == 998;
			if (flag)
			{
				float num = UnityEngine.Random.Range(0f, 360f);
				for (int i = 0; i < 4; i++)
				{
					int num2 = 0;
					for (int j = 0; j < 6; j++)
					{
						Vector3 vector = Quaternion.Euler(0f, num + 90f * (float)i, 0f) * Vector3.forward;
						Vector3 vector2 = self.predictedTargetPosition + vector * FireGoldFist.distanceBetweenFists * (float)j;
						float num3 = 60f;
						RaycastHit raycastHit;
						bool flag2 = Physics.Raycast(new Ray(vector2 + Vector3.up * (num3 / 2f), Vector3.down), out raycastHit, num3, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
						if (flag2)
						{
							vector2 = raycastHit.point;
						}
						self.PlaceSingleDelayBlast(vector2, FireGoldFist.delayBetweenFists * (float)num2);
						num2++;
					}
				}
			}
			else
			{
				bool flag3 = FireGoldFist.fistCount == 999;
				if (flag3)
				{
					float num4 = UnityEngine.Random.Range(0f, 360f);
					for (int k = 0; k < 8; k++)
					{
						int num5 = 0;
						for (int l = 0; l < 6; l++)
						{
							Vector3 vector3 = Quaternion.Euler(0f, num4 + 45f * (float)k, 0f) * Vector3.forward;
							Vector3 vector4 = self.predictedTargetPosition + vector3 * FireGoldFist.distanceBetweenFists * (float)l;
							float num6 = 60f;
							RaycastHit raycastHit2;
							bool flag4 = Physics.Raycast(new Ray(vector4 + Vector3.up * (num6 / 2f), Vector3.down), out raycastHit2, num6, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
							if (flag4)
							{
								vector4 = raycastHit2.point;
							}
							self.PlaceSingleDelayBlast(vector4, FireGoldFist.delayBetweenFists * (float)num5);
							num5++;
						}
					}
				}
				else
				{
					orig.Invoke(self);
				}
			}
		}

		private void GoldshoresBossfight_OnExit(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_OnExit orig, GoldshoresBossfight self)
		{
			FireGoldFist.fistCount = 6;
			FireGoldMegaLaser.projectileFireFrequency = 8f;
			RechargeRocks.rockControllerPrefab.GetComponent<TitanRockController>().fireInterval = 1f;
			orig.Invoke(self);
		}

		private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
		{
			bool flag = self.body.HasBuff(titanGoldArmorBrokenDef);
			if (flag)
			{
				damageInfo.damage *= AurelioniteArmorBrokenMult.Value;
			}
			orig.Invoke(self, damageInfo);
		}

		private void GoldshoresBossfight_ServerFixedUpdate(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_ServerFixedUpdate orig, GoldshoresBossfight self)
		{
			bool flag = self.fixedAge >= GoldshoresBossfight.transitionDuration;
			if (flag)
			{
				self.missionController.ExitTransitionIntoBossfight();
				bool flag2 = !self.hasSpawnedBoss;
				if (flag2)
				{
					self.SpawnBoss();
				}
				else
				{
					bool flag3 = self.scriptedCombatEncounter.combatSquad.readOnlyMembersList.Count == 0;
					if (flag3)
					{
						self.outer.SetNextState(new Exit());
						foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
						{
							bool flag4 = characterMaster.teamIndex == TeamIndex.Player;
							if (flag4)
							{
								characterMaster.inventory.GiveItem(hiddenGoldBuffItemDef, 2 - self.serverCycleCount);
							}
						}
						bool flag5 = self.serverCycleCount < 2;
						if (flag5)
						{
							Chat.AddMessage("<style=cShrine>The Guardian blesses you...</style>");
						}
						return;
					}
				}
			}
			bool flag6 = self.scriptedCombatEncounter;
			if (flag6)
			{
				bool flag7 = !self.bossImmunity;
				if (flag7)
				{
					bool hasPassed = self.bossInvulnerabilityStartTime.hasPassed;
					if (hasPassed)
					{
						bool flag8 = (double)self.scriptedCombatEncounter.combatSquad.readOnlyMembersList[0].bodyInstanceObject.GetComponent<HealthComponent>().combinedHealthFraction <= 0.666666666666 && self.serverCycleCount == 0;
						bool flag9 = (double)self.scriptedCombatEncounter.combatSquad.readOnlyMembersList[0].bodyInstanceObject.GetComponent<HealthComponent>().combinedHealthFraction <= 0.333333333333 && self.serverCycleCount == 1;
						bool flag10 = flag9;
						if (flag10)
						{
							self.ExtinguishBeacons();
							self.SetBossImmunity(true);
							self.scriptedCombatEncounter.combatSquad.readOnlyMembersList[0].bodyInstanceObject.GetComponent<CharacterBody>().inventory.GiveItem(RoR2Content.Items.AlienHead.itemIndex, 1);
							RechargeRocks.rockControllerPrefab.GetComponent<TitanRockController>().fireInterval /= 2f;
							FireGoldFist.fistCount = 999;
							self.serverCycleCount++;
						}
						else
						{
							bool flag11 = flag8;
							if (flag11)
							{
								self.ExtinguishBeacons();
								self.SetBossImmunity(true);
								FireGoldFist.fistCount = 998;
								FireGoldMegaLaser.projectileFireFrequency *= 1.25f;
								RechargeRocks.rockControllerPrefab.GetComponent<TitanRockController>().fireInterval /= 2f;
								self.serverCycleCount++;
							}
						}
					}
				}
				else
				{
					bool flag12 = self.missionController.beaconsActive >= self.missionController.beaconsToSpawnOnMap;
					if (flag12)
					{
						self.SetBossImmunity(false);
						bool flag13 = self.serverCycleCount > 0;
						if (flag13)
						{
							self.scriptedCombatEncounter.combatSquad.readOnlyMembersList[0].bodyInstanceObject.GetComponent<CharacterBody>().AddTimedBuff(titanGoldArmorBrokenDef, 10f);
						}
						self.bossInvulnerabilityStartTime = Run.FixedTimeStamp.now + GoldshoresBossfight.shieldRemovalDuration;
					}
				}
			}
		}

		private void GoldshoresBossfight_OnEnter(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_OnEnter orig, GoldshoresBossfight self)
		{
			orig.Invoke(self);
			GoldshoresBossfight.shieldRemovalDuration = 20f;
			self.bossInvulnerabilityStartTime = Run.FixedTimeStamp.now + GoldshoresBossfight.shieldRemovalDuration;
		}

		private void GoldshoresMissionController_Start(On.RoR2.GoldshoresMissionController.orig_Awake orig, GoldshoresMissionController self)
		{
			self.beaconsToSpawnOnMap = 4;
			orig.Invoke(self);
		}

		private void GoldTitanManager_CalcTitanPowerAndBestTeam(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ILCursor ilcursor2 = ilcursor;
			Func<Instruction, bool>[] array = new Func<Instruction, bool>[6];
			array[0] = ((Instruction x) => ILPatternMatchingExt.MatchLdloc(x, 2));
			array[1] = ((Instruction x) => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.GoldTitanManager", "goldTitanItemIndex"));
			array[2] = ((Instruction x) => ILPatternMatchingExt.MatchLdcI4(x, 1));
			array[3] = ((Instruction x) => ILPatternMatchingExt.MatchLdcI4(x, 1));
			array[4] = ((Instruction x) => ILPatternMatchingExt.MatchCallOrCallvirt(x, "RoR2.Util", "GetItemCountForTeam"));
			array[5] = ((Instruction x) => ILPatternMatchingExt.MatchStloc(x, 3));
			ilcursor2.GotoNext(array);
			ilcursor.Index += 5;
			ilcursor.Emit(OpCodes.Ldc_I4, TitanGoldMultiplier.Value);
			ilcursor.Emit(OpCodes.Mul);
		}

		private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
		{
			bool flag = SceneManager.GetActiveScene().name == "goldshores";
			if (flag)
			{
			}
			orig.Invoke(self);
		}

		private void DeathRewards_OnKilledServer(On.RoR2.DeathRewards.orig_OnKilledServer orig, DeathRewards self, DamageReport damageReport)
		{
			bool flag = self.characterBody.HasBuff(affixGoldDef);
			if (flag)
			{
				self.goldReward *= GoldRewardAmount.Value;
			}
			orig.Invoke(self, damageReport);
		}

		private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
		{
			orig.Invoke(self);
			bool active = NetworkServer.active;
			if (active)
			{
				bool value = EnableEye.Value;
				if (value)
				{
					self.AddItemBehavior<LaserEyeBehavior>(self.inventory.GetItemCount(laserEyeDef));
				}
				bool value2 = FightChanges.Value;
				if (value2)
				{
					self.AddItemBehavior<HiddenGoldBuffBehavior>(self.inventory.GetItemCount(hiddenGoldBuffItemDef));
				}
			}
		}

		private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
		{
			bool flag = damageInfo.attacker;
			if (flag)
			{
				CharacterBody component = damageInfo.attacker.GetComponent<CharacterBody>();
				CharacterBody characterBody = victim ? victim.GetComponent<CharacterBody>() : null;
				bool flag2 = component;
				if (flag2)
				{
					CharacterMaster master = component.master;
					bool flag3 = master;
					if (flag3)
					{
						bool value = EnableGoldElites.Value;
						if (value)
						{
							bool flag4 = component.HasBuff(affixGoldDef);
							if (flag4)
							{
								if (characterBody != null && characterBody.master != null)
								{
									characterBody.master.money -= ((characterBody.master.money < (uint)damageInfo.damage) ? characterBody.master.money : (uint)damageInfo.damage);
									master.GiveMoney((uint)(GoldStealAmount.Value * Run.instance.difficultyCoefficient));
									EffectManager.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CoinImpact"), damageInfo.position, Vector3.up, true);
								}
							}
						}
						bool value2 = EnableSword.Value;
						if (value2)
						{
							int itemCount = component.inventory.GetItemCount(bigSwordDef);
							bool flag5 = itemCount > 0 && Util.CheckRoll(SwordChance.Value * damageInfo.procCoefficient, component.master) && !damageInfo.procChainMask.HasProc(ProcType.AACannon);
							if (flag5)
							{
								float damage = Util.OnHitProcDamage(damageInfo.damage, component.damage, SwordDamage.Value * (float)itemCount);
								ProcChainMask procChainMask = damageInfo.procChainMask;
								procChainMask.AddProc(ProcType.AACannon);
								RaycastHit raycastHit;
								Physics.Raycast(damageInfo.position, Vector3.down, out raycastHit, float.PositiveInfinity, LayerMask.GetMask(new string[]
								{
									"World"
								}));
								FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
								fireProjectileInfo.projectilePrefab = newProjectile;
								fireProjectileInfo.position = raycastHit.point;
								fireProjectileInfo.rotation = Quaternion.identity;
								fireProjectileInfo.procChainMask = procChainMask;
								fireProjectileInfo.target = victim;
								fireProjectileInfo.owner = component.gameObject;
								fireProjectileInfo.damage = damage;
								fireProjectileInfo.crit = damageInfo.crit;
								fireProjectileInfo.force = 10000f;
								fireProjectileInfo.damageColorIndex = (DamageColorIndex)3;
								fireProjectileInfo.fuseOverride = 0.5f;
								FireProjectileInfo fireProjectileInfo2 = fireProjectileInfo;
								ProjectileManager.instance.FireProjectile(fireProjectileInfo2);
							}
						}
					}
				}
			}
			orig.Invoke(self, damageInfo, victim);
		}

		private void GoldenKnurlStatChanges(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			bool flag = sender.master;
			if (flag)
			{
				bool flag2 = sender.master.inventory;
				if (flag2)
				{
					int itemCount = sender.inventory.GetItemCount(goldenKnurlDef);
					bool flag3 = itemCount > 0;
					if (flag3)
					{
						args.armorAdd += KnurlArmor.Value * (float)itemCount;
						args.baseRegenAdd += KnurlRegen.Value * (float)itemCount;
						args.healthMultAdd += KnurlHealth.Value * (float)itemCount;
					}
				}
			}
		}

		private void PickupDropletController_CreatePickupDroplet_PickupIndex_Vector3_Vector3(On.RoR2.PickupDropletController.orig_CreatePickupDroplet_PickupIndex_Vector3_Vector3 orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
		{
			bool flag = pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.TitanGoldDuringTP.itemIndex);
			if (flag)
			{
				System.Random random = new System.Random();
				switch (random.Next(0, newItemList.Count + 1))
				{
					case 0:
						pickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.TitanGoldDuringTP.itemIndex);
						break;
					case 1:
						pickupIndex = PickupCatalog.FindPickupIndex(newItemList[0].itemIndex);
						break;
					case 2:
						pickupIndex = PickupCatalog.FindPickupIndex(newItemList[1].itemIndex);
						break;
					case 3:
						pickupIndex = PickupCatalog.FindPickupIndex(newItemList[2].itemIndex);
						break;
				}
			}
			orig.Invoke(pickupIndex, position, velocity);
		}

		private void GoldTitanManager_TryStartChannelingTitansServer(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ILCursor ilcursor2 = ilcursor;
			Func<Instruction, bool>[] array = new Func<Instruction, bool>[5];
			array[0] = ((Instruction x) => ILPatternMatchingExt.MatchLdloc(x, 1));
			array[1] = ((Instruction x) => ILPatternMatchingExt.MatchConvR4(x));
			array[2] = ((Instruction x) => ILPatternMatchingExt.MatchLdcR4(x, 0.5f));
			array[3] = ((Instruction x) => ILPatternMatchingExt.MatchCall<Mathf>(x, "Pow"));
			array[4] = ((Instruction x) => ILPatternMatchingExt.MatchMul(x));
			ilcursor2.GotoNext(array);
			ilcursor.Index += 2;
			ilcursor.Next.Operand = 1f;
		}

		public static ItemDef goldenKnurlDef;
		public static ItemDef bigSwordDef;
		public static ItemDef laserEyeDef;
		public static ItemDef hiddenGoldBuffItemDef;
		internal static List<ItemDef> newItemList = new List<ItemDef>();
		public static BuffDef laserEyeChargeDef;
		public static BuffDef titanGoldArmorBrokenDef;
		public static BuffDef affixGoldDef;
		public static BuffDef hiddenGoldBuffDef;
	}
}
