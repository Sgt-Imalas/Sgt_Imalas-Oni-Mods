using HarmonyLib;
using OniRetroEdition.Behaviors;
using OniRetroEdition.BuildingDefModification;
using OniRetroEdition.Buildings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition
{
	internal class Patches
	{
		/// <summary>
		/// add buildings to plan screen
		/// </summary>
		[HarmonyPatch(typeof(GeneratedBuildings))]
		[HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{

			public static void Prefix()
			{
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, WallLampConfig.ID, CeilingLightConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Food, GammaRayOvenConfig.ID, MicrobeMusherConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Power, BatteryLargeConfig.ID, BatteryMediumConfig.ID);



				foreach (var config in BuildingModifications.Instance.LoadedBuildingOverrides)
				{


					if (config.Value.buildMenuCategory != null && config.Value.buildMenuCategory.Length > 0)
					{
						string buildingId = config.Key;
						string category = config.Value.buildMenuCategory;

						if (config.Value.placedBehindBuildingId != null && config.Value.placedBehindBuildingId.Length > 0)
						{
							string relativeBuildingId = config.Value.placedBehindBuildingId;
							if (relativeBuildingId == null || relativeBuildingId.Length == 0)
								continue;

							if (config.Value.placeBefore.HasValue)
							{
								bool before = config.Value.placeBefore.Value;
								InjectionMethods.MoveExistingBuildingToNewCategory(category, buildingId, relativeBuildingId, string.Empty, before ? ModUtil.BuildingOrdering.Before : ModUtil.BuildingOrdering.After);
								continue;
							}

							InjectionMethods.MoveExistingBuildingToNewCategory(category, buildingId, relativeBuildingId);
							continue;
						}
						InjectionMethods.MoveExistingBuildingToNewCategory(category, buildingId);
					}
				}
			}

		}
		/// <summary>
		/// Register Buildings to existing Technologies (newly added techs are in "ResearchTreePatches" class
		/// </summary>
		[HarmonyPatch(typeof(Db))]
		[HarmonyPatch("Initialize")]
		public class Db_Initialize_Patch
		{
			public static void Postfix(Db __instance)
			{
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.Employment, RoleStationConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.GlassBlowing, WallLampConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.AdvancedPowerRegulation, BatteryLargeConfig.ID);


				foreach (var config in BuildingModifications.Instance.LoadedBuildingOverrides)
				{
					if (config.Value.techOverride != null && config.Value.techOverride.Length > 0)
					{
						var previousTech = __instance.Techs.TryGetTechForTechItem(config.Key);
						if (previousTech != null)
						{
							previousTech.RemoveUnlockedItemIDs(config.Key);
						}

						InjectionMethods.AddBuildingToTechnology(config.Value.techOverride, config.Key);
					}
				}
			}
		}

		[HarmonyPatch(typeof(RoleStationConfig))]
		[HarmonyPatch(nameof(RoleStationConfig.ConfigureBuildingTemplate))]
		public static class ReviveOldJobStation
		{
			public static void Postfix(GameObject go)
			{
				go.GetComponent<KPrefabID>().AddTag(GameTags.Experimental);
				RoleStation roleStation = go.AddOrGet<RoleStation>();
				roleStation.overrideAnims = new KAnimFile[1]
				{
				Assets.GetAnim((HashedString) "anim_interacts_job_station_kanim")
				};
				go.AddOrGet<JobBoardSkillbutton>();
			}
		}

		[HarmonyPatch(typeof(ManagementMenu))]
		[HarmonyPatch(nameof(ManagementMenu.AddToggleTooltip))]
		public static class SwapTooltipForSkillStation
		{

			public static void Prefix(ManagementMenu __instance, ManagementMenu.ManagementMenuToggleInfo toggleInfo, ref string disabledTooltip)
			{
				if (toggleInfo == __instance.skillsInfo)
				{
					disabledTooltip = STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_SKILL_STATION_RETRO;
				}
			}
		}
		[HarmonyPatch(typeof(PressureDoorConfig), nameof(PressureDoorConfig.DoPostConfigureComplete))]
		public static class MechanizedAirlockTileable
		{

			public static void Postfix(GameObject go)
			{
				go.AddOrGet<AnimTileable>();
			}
		}

		[HarmonyPatch]
		///Connects mesh+airflow and normal tiles
		public static class ConnectingTiles
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.TileTopsMerge;

			[HarmonyPostfix]
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = TileConfig.BlockTileConnectorID;
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.ConfigureBuildingTemplate);
				yield return typeof(MeshTileConfig).GetMethod(name);
				yield return typeof(GasPermeableMembraneConfig).GetMethod(name);
			}
		}


		/// <summary>
		/// fixing crash on end
		/// </summary>
		[HarmonyPatch(typeof(RoleStation))]
		[HarmonyPatch(nameof(RoleStation.OnStopWork))]
		public static class FixCrash
		{

			public static bool Prefix(RoleStation __instance)
			{
				Telepad.StatesInstance sMI = __instance.GetSMI<Telepad.StatesInstance>();
				return sMI != null;
			}
		}

		/// <summary>
		/// Adjusting work time
		/// </summary>
		[HarmonyPatch(typeof(RoleStation))]
		[HarmonyPatch(nameof(RoleStation.OnSpawn))]
		public static class RoleStationFix
		{
			public static void Postfix(RoleStation __instance)
			{
				__instance.SetWorkTime(3f);
			}
		}

		#region reenableDeprecateds
		[HarmonyPatch(typeof(LureSideScreen))]
		[HarmonyPatch(nameof(LureSideScreen.SetTarget))]
		public static class Revive_AirborneCreatureLureScreenExtension
		{
			public static void Prefix(LureSideScreen __instance)
			{

				__instance.baitAttractionStrings = new Dictionary<Tag, string>
				{
					{ SimHashes.SlimeMold.CreateTag(), global::STRINGS.CREATURES.SPECIES.PUFT.NAME},
					{ SimHashes.Phosphorus.CreateTag(),global::STRINGS.CREATURES.SPECIES.LIGHTBUG.NAME},
					{ SimHashes.Phosphorite.CreateTag(), global::STRINGS.CREATURES.SPECIES.LIGHTBUG.NAME},
					{ SimHashes.BleachStone.CreateTag(),global::STRINGS.CREATURES.SPECIES.PUFT.NAME},
					{ SimHashes.Diamond.CreateTag(),global::STRINGS.CREATURES.SPECIES.LIGHTBUG.NAME},
					{ SimHashes.OxyRock.CreateTag(),global::STRINGS.CREATURES.SPECIES.PUFT.NAME},
				};
			}
		}

		[HarmonyPatch(typeof(AirborneCreatureLureConfig))]
		[HarmonyPatch(nameof(AirborneCreatureLureConfig.ConfigureBuildingTemplate))]
		public static class Revive_AirborneCreatureLureConfig2
		{
			public static void Postfix(GameObject prefab)
			{

				CreatureLure creatureLure = prefab.AddOrGet<CreatureLure>();
				creatureLure.baitTypes = new List<Tag>
				{
					SimHashes.SlimeMold.CreateTag(),
					SimHashes.Phosphorus.CreateTag(),
					SimHashes.Phosphorite.CreateTag(),
					SimHashes.BleachStone.CreateTag(),
					SimHashes.Diamond.CreateTag(),
					SimHashes.OxyRock.CreateTag(),
				};
				creatureLure.baitStorage.storageFilters = creatureLure.baitTypes;
			}
		}
		[HarmonyPatch(typeof(MouldingTileConfig))]
		[HarmonyPatch(nameof(MouldingTileConfig.DoPostConfigureComplete))]
		public static class Revive_MouldingTileConfig2
		{
			public static void Postfix(GameObject go)
			{
				KPrefabID component = go.GetComponent<KPrefabID>();
				component.AddTag(GameTags.FloorTiles);
			}
		}
		[HarmonyPatch(typeof(GeyserGenericConfig))]
		[HarmonyPatch(nameof(GeyserGenericConfig.CreateGeyser))]
		public static class GeyserResize
		{
			public static void Prefix(string id, ref int width, ref int height)
			{
				if (id.Contains("steam") || id.Contains("hot_steam") || id.Contains("methane"))
				{
					width = 3;
					height = 3;
				}

			}
		}

		[HarmonyPatch(typeof(RoleStationConfig))]
		[HarmonyPatch(nameof(RoleStationConfig.CreateBuildingDef))]
		public static class ReviveOldJobStation2
		{
			public static void Postfix(ref BuildingDef __result)
			{
				__result.ShowInBuildMenu = true;
				__result.Deprecated = false;
				__result.Overheatable = false;

			}
		}

		#endregion

		[HarmonyPatch(typeof(LogicElementSensorGasConfig))]
		[HarmonyPatch(nameof(LogicElementSensorGasConfig.CreateBuildingDef))]
		public static class LogicElementSensorGasNeedsPower
		{
			public static bool Prepare() => Config.Instance.gassensorpower;

			public static void Postfix(ref BuildingDef __result)
			{
				__result.RequiresPowerInput = true;
				__result.AddLogicPowerPort = false;
				__result.EnergyConsumptionWhenActive = 25f;

			}
		}
		[HarmonyPatch(typeof(LogicElementSensorLiquidConfig))]
		[HarmonyPatch(nameof(LogicElementSensorLiquidConfig.CreateBuildingDef))]
		public static class LogicElementSensorliquidNeedsPower
		{
			public static bool Prepare() => Config.Instance.liquidsensorpower;

			public static void Postfix(ref BuildingDef __result)
			{
				__result.RequiresPowerInput = true;
				__result.AddLogicPowerPort = false;
				__result.EnergyConsumptionWhenActive = 25f;

			}
		}

		[HarmonyPatch(typeof(ExobaseHeadquartersConfig))]
		[HarmonyPatch(nameof(ExobaseHeadquartersConfig.ConfigureBuildingTemplate))]
		public static class RemoveSkillUpFromPrinterMini
		{
			public static void Postfix(GameObject go)
			{
				if (go.TryGetComponent<RoleStation>(out var station))
				{
					UnityEngine.Object.Destroy(station);
				}
			}
		}

		[HarmonyPatch(typeof(HeadquartersConfig))]
		[HarmonyPatch(nameof(HeadquartersConfig.ConfigureBuildingTemplate))]
		public static class RemoveSkillUpFromPrinterMain
		{
			public static void Postfix(GameObject go)
			{
				if (go.TryGetComponent<RoleStation>(out var station))
				{
					UnityEngine.Object.Destroy(station);
				}
			}
		}

		[HarmonyPatch(typeof(TelepadSideScreen))]
		[HarmonyPatch(nameof(TelepadSideScreen.OnSpawn))]
		public static class RemoveSkillsButtonFromPrinterScreen
		{
			public static void Postfix(TelepadSideScreen __instance)
			{
				__instance.openRolesScreenButton.ClearOnClick();
				__instance.openRolesScreenButton.gameObject.SetActive(false);

			}
		}
		[HarmonyPatch(typeof(TelepadSideScreen))]
		[HarmonyPatch(nameof(TelepadSideScreen.UpdateSkills))]
		public static class RemoveSkillsButtonFromPrinterScreen2
		{
			public static bool Prefix(TelepadSideScreen __instance)
			{
				__instance.skillPointsAvailable.gameObject.SetActive(false);
				return false;
			}
		}


		//[HarmonyPatch(typeof(CO2ScrubberConfig))]
		//[HarmonyPatch(nameof(CO2ScrubberConfig.CreateBuildingDef))]
		//public static class CO2Scrubber_CreateBuildingDef_Postfix
		//{
		//    public static void Postfix(ref BuildingDef __result)
		//    {
		//        BuildLocationRule newBuildingRule = BuildLocationRule.OnCeiling;

		//        __result.BuildLocationRule = newBuildingRule;
		//        __result.ContinuouslyCheckFoundation = !(
		//               newBuildingRule == BuildLocationRule.Anywhere
		//            || newBuildingRule == BuildLocationRule.Tile
		//            || newBuildingRule == BuildLocationRule.Conduit
		//            || newBuildingRule == BuildLocationRule.LogicBridge
		//            || newBuildingRule == BuildLocationRule.WireBridge
		//            );
		//    }
		//}
		[HarmonyPatch(typeof(MinionConfig))]
		[HarmonyPatch(nameof(MinionConfig.CreatePrefab))]
		public static class MinionConfig_AddModNoiseListener
		{
			public static void Postfix(ref GameObject __result)
			{
				__result.AddOrGetDef<NoiseRecieverSMI.Def>();
			}
		}


		//[HarmonyPatch(typeof(MopTool))]
		//[HarmonyPatch(nameof(MopTool.OnPrefabInit))]
		//public static class Moppable_AlwaysMop
		//{
		//    [HarmonyPrepare]
		//    public static bool Prepare() => Config.Instance.succmop;
		//    public static void Postfix(MopTool __instance)
		//    {
		//        MopTool.maxMopAmt = float.PositiveInfinity;
		//    }
		//}


		//[HarmonyPatch(typeof(MopTool), "OnDragTool")]
		//public class MopTool_OnDragTool_Patch
		//{
		//    [HarmonyPrepare]
		//    public static bool Prepare() => Config.Instance.succmop;
		//    public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
		//    {
		//        var codes = orig.ToList();

		//        // find injection point
		//        var index = codes.FindIndex(ci => ci.opcode == OpCodes.Stloc_1);

		//        if (index == -1)
		//        {
		//            SgtLogger.error("mop transpiler found no target!");
		//            return codes;
		//        }

		//        var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(MopTool_OnDragTool_Patch), "InjectedMethod");

		//        // inject right after the found index
		//        codes.InsertRange(index, new[]
		//        {
		//                    new CodeInstruction(OpCodes.Call, m_InjectedMethod)
		//                });

		//        //TranspilerHelper.PrintInstructions(codes);
		//        return codes;
		//    }

		//    private static bool InjectedMethod(bool old)
		//    {
		//        return true;
		//    }
		//}
		//[HarmonyPatch(typeof(Moppable))]
		//[HarmonyPatch(nameof(Moppable.OnSpawn))]
		//public static class Moppable_Watergun
		//{
		//    [HarmonyPrepare]
		//    public static bool Prepare() => Config.Instance.succmop;
		//    public static void Postfix(Moppable __instance)
		//    {
		//        __instance.overrideAnims = null;
		//        __instance.faceTargetWhenWorking = true;
		//        __instance.multitoolContext = "fetchliquid";
		//        __instance.multitoolHitEffectTag = WaterSuckEffect.ID;
		//        __instance.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		//    }
		//}
		/// <summary>
		/// Teleports "mopped" liquids to the dupe
		/// </summary>
		//[HarmonyPatch(typeof(Moppable), "OnCellMopped")]
		//public class Moppable_OnCellMopped_Patch
		//{
		//    [HarmonyPrepare]
		//    public static bool Prepare() => Config.Instance.succmop;


		//    public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
		//    {
		//        var codes = orig.ToList();

		//        // find injection point
		//        var index = codes.FindIndex(ci => ci.Calls(AccessTools.Method(typeof(TransformExtensions), nameof(TransformExtensions.SetPosition))));

		//        if (index == -1)
		//        {
		//            SgtLogger.error("mop transpiler found no target!");
		//            TranspilerHelper.PrintInstructions(codes);
		//            return codes;
		//        }

		//        var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(Moppable_OnCellMopped_Patch), "InjectedMethod");

		//        // inject right after the found index
		//        codes.InsertRange(index, new[]
		//        {
		//                    new CodeInstruction(OpCodes.Ldarg_0),
		//                    new CodeInstruction(OpCodes.Call, m_InjectedMethod)
		//                });


		//        return codes;
		//    }

		//    private static Vector3 InjectedMethod(Vector3 toConsume, Moppable instance)
		//    {
		//        if (instance == null || instance.worker == null)
		//            return toConsume;

		//        return instance.worker.transform.GetPosition();

		//    }
		//}



		//[HarmonyPatch(typeof(FlyingCreatureBaitConfig))]
		//[HarmonyPatch(nameof(FlyingCreatureBaitConfig.CreateBuildingDef))]
		//public static class AirborneCritterBait_CeilingOnly
		//{
		//    public static void Postfix(ref BuildingDef __result)
		//    {
		//        __result.ShowInBuildMenu = true;
		//        __result.Deprecated = false;
		//        __result.BuildLocationRule = BuildLocationRule.OnCeiling;
		//    }
		//}

		[HarmonyPatch(typeof(Assets))]
		[HarmonyPatch(nameof(Assets.GetAnim))]
		public static class TryGetRetroAnim_GetAnim
		{

			public static void Prefix(Assets __instance, ref HashedString name)
			{
				string retroStringVariant = name.ToString().Replace("_kanim", "_retro_kanim");
				if (name.IsValid && Assets.AnimTable.ContainsKey(retroStringVariant))
				{
					name = retroStringVariant;
				}
			}
		}


		[HarmonyPatch(typeof(ComplexFabricatorSM.States))]
		[HarmonyPatch(nameof(ComplexFabricatorSM.States.InitializeStates))]
		public static class Add_Working_pst_complete_anim_ComplexFabricator
		{

			public static void Postfix(ComplexFabricatorSM.States __instance)
			{
				__instance.operating.working_pst.transitions.Clear();
				__instance.operating.working_pst.OnAnimQueueComplete(__instance.operating.working_pst_complete);
			}
		}

		//[HarmonyPatch(typeof(PoweredActiveController))]
		//[HarmonyPatch(nameof(PoweredActiveController.InitializeStates))]
		//public static class ExtendedPoweredActiveController
		//{
		//    static GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State poweredAnimComplete;
		//    public static void Postfix(PoweredActiveController __instance)
		//    {
		//        //for(int i = __instance.states.Count- 1; i >= 0;i--)
		//        //{
		//        //    var state = __instance.states[i];
		//        //    if (state.name.Contains("stressed"))
		//        //    {
		//        //        __instance.states.RemoveAt(i);
		//        //    }
		//        //}

		//        poweredAnimComplete = new GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State();


		//        poweredAnimComplete
		//            //.PlayAnim("stop")
		//            .OnAnimQueueComplete(__instance.on);

		//        __instance.working.pst.transitions.Clear();
		//        __instance.working.pst.OnAnimQueueComplete(poweredAnimComplete);
		//    }
		//}

		[HarmonyPatch(typeof(Assets))]
		[HarmonyPatch(nameof(Assets.TryGetAnim))]
		public static class TryGetRetroAnim_TryGetAnim
		{

			public static void Prefix(ref HashedString name)
			{
				string retroStringVariant = name.ToString().Replace("_kanim", "_retro_kanim");
				if (name.IsValid && Assets.AnimTable.ContainsKey(retroStringVariant))
				{
					name = retroStringVariant;
				}
			}
		}
		//[HarmonyPatch(typeof(RailGunPayloadOpenerConfig))]
		//[HarmonyPatch(nameof(RailGunPayloadOpenerConfig.ConfigureBuildingTemplate))]
		//public static class ManualRailgunOpener
		//{

		//    public static bool Prepare() => Config.Instance.manualRailgunPayloadOpener;
		//    public static void Postfix(GameObject go)
		//    {
		//        var manualOperatable = go.AddComponent<GenericWorkableComponent>();
		//        manualOperatable.overrideAnims = new KAnimFile[1]
		//        {
		//            Assets.GetAnim((HashedString) "retro_anim_interact_railgun_opener_kanim")
		//        };
		//        RailGunPayloadOpener railGunPayloadOpener = go.GetComponent<RailGunPayloadOpener>();

		//        manualOperatable.workOffset = new CellOffset(0, 0);
		//        manualOperatable.WorkTime = (11f);
		//        manualOperatable.workLayer = Grid.SceneLayer.BuildingUse;
		//        manualOperatable.IsWorkable = () =>
		//        {
		//            return railGunPayloadOpener.payloadStorage.Count > 0;
		//        };
		//    }
		//}



		[HarmonyPatch(typeof(AlgaeDistilleryConfig))]
		[HarmonyPatch(nameof(AlgaeDistilleryConfig.ConfigureBuildingTemplate))]
		public static class ManualAlgaeDestillery
		{
			public class AlgaeDestilleryWorkable : ComplexFabricatorWorkable
			{
				//public override Vector3 GetWorkOffset()
				//{
				//    return new(-1, 0);
				//}
				public override void OnSpawn()
				{
					base.OnSpawn();

					this.SetOffsets(new[] { new CellOffset(-1, 0) });
				}
			}


			public static bool Prepare() => Config.Instance.manualSlimemachine;
			public static bool Prefix(GameObject go)
			{
				go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
				go.AddOrGet<DropAllWorkable>();
				go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
				ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
				fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
				fabricator.duplicantOperated = true;
				fabricator.outputOffset = new(1, 0);
				go.AddOrGet<FabricatorIngredientStatusManager>();
				go.AddOrGet<CopyBuildingSettings>();
				AlgaeDestilleryWorkable fabricatorWorkable = go.AddOrGet<AlgaeDestilleryWorkable>();
				fabricatorWorkable.SetOffsets(new[] { new CellOffset(-1, 0) });
				fabricatorWorkable.workLayer = Grid.SceneLayer.BuildingUse;
				BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);

				foreach (var fab in go.GetComponents<Storage>())
				{
					fab.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
				}

				fabricatorWorkable.overrideAnims = new KAnimFile[1]
				{
					Assets.GetAnim((HashedString) "anim_interacts_algae_distillery_kanim")
				};
				//fabricatorWorkable.workingPstComplete = new HashedString[1]
				//{
				//    (HashedString) "working_pst_complete"
				//};
				ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
				conduitDispenser.conduitType = ConduitType.Liquid;
				conduitDispenser.alwaysDispense = true;
				conduitDispenser.elementFilter = (SimHashes[])null;
				conduitDispenser.storage = go.GetComponent<ComplexFabricator>().outStorage;
				;
				Prioritizable.AddRef(go);
				ConfigureAlgaeRecipes();

				return false;
			}

			private static void ConfigureAlgaeRecipes()
			{
				float ratio = 1f / 3f;
				ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[]
				{
					new ComplexRecipe.RecipeElement(SimHashes.SlimeMold.CreateTag(), 100f)
				};
				ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[]
				{
					new ComplexRecipe.RecipeElement(SimHashes.Algae.CreateTag(), 100f*ratio, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
					new ComplexRecipe.RecipeElement(SimHashes.DirtyWater.CreateTag(), 100f*(1f-ratio), ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature,true)
				};
				MushBarConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(AlgaeDistilleryConfig.ID, (IList<ComplexRecipe.RecipeElement>)recipeElementArray1, (IList<ComplexRecipe.RecipeElement>)recipeElementArray2), recipeElementArray1, recipeElementArray2)
				{
					time = 30f,
					description = global::STRINGS.ELEMENTS.ALGAE.DESC,
					nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
					fabricators = new List<Tag>()
					{
						(Tag) AlgaeDistilleryConfig.ID
					},
					sortOrder = 1
				};
			}
		}

		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}




		//[HarmonyPatch(typeof(MainCamera), nameof(MainCamera.Awake))]
		//public static class LightExperiments
		//{
		//    public static void Postfix(MainCamera __instance)
		//    {
		//        Camera.main.adj
		//    }
		//}

		[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OnSpawn))]
		public static class UpdateInfo
		{
			public static void Postfix(MainMenu __instance)
			{
				if (__instance.nextUpdateTimer != null)
				{
					__instance.nextUpdateTimer.gameObject.SetActive(true);
					__instance.nextUpdateTimer.transform.parent.gameObject.SetActive(true);
					__instance.nextUpdateTimer.transform.parent.parent.gameObject.SetActive(true);
				}
				else
				{
					SgtLogger.error("nextupdatetimer is null");
				}
			}
		}

		/// <summary>
		/// old conveyor box
		/// </summary>
		[HarmonyPatch(typeof(SolidConduitFlowVisualizer))]
		[HarmonyPatch(typeof(SolidConduitFlowVisualizer), MethodType.Constructor)]
		[HarmonyPatch(new Type[] { typeof(SolidConduitFlow), typeof(Game.ConduitVisInfo), typeof(FMODUnity.EventReference), typeof(SolidConduitFlowVisualizer.Tuning) })]
		public class AddRetroConveyorBox
		{
			public static void Prefix(
				SolidConduitFlowVisualizer.Tuning tuning)
			{
				SgtLogger.l("GamePrefabInit");

				var path = Path.Combine(UtilMethods.ModPath, "assets");
				var texture = AssetUtils.LoadTexture("conveyor_box_retro", path);


				SgtLogger.Assert("TextureNotNull", texture);
				if (texture != null)
				{
					tuning.foregroundTexture = texture;
				}

			}
		}
		[HarmonyPatch(typeof(Assets), "LoadAnims")]
		public class Assets_OnPrefabInit_Patch
		{
			public static void Prefix(Assets __instance)
			{
				var path = Path.Combine(Path.Combine(UtilMethods.ModPath, "assets"), "ReplacementSprites");

				SgtLogger.l(path, "PATH for imports");
				var files = new DirectoryInfo(path).GetFiles();

				SgtLogger.l(files.Count().ToString(), "Files to import and override");

				for (int i = 0; i < files.Count(); i++)
				{
					var File = files[i];
					try
					{
						AssetUtils.OverrideSpriteTextures(__instance, File);
					}
					catch (Exception e)
					{
						SgtLogger.logError("Failed at importing sprite: " + File.FullName + ",\nError: " + e);
					}
				}
			}
		}

		/// <summary>
		/// Make liquids more transparent
		/// </summary>
		[HarmonyPatch(typeof(WaterCubes), nameof(WaterCubes.Init))]
		public class WaterCubes_Init_Patch
		{
			public static void Postfix(WaterCubes __instance)
			{
				// make the liquids a little more see-through
				__instance.material.SetFloat("_BlendScreen", 0.4f); //courtesy of aki; beached
			}
		}

	}
}
