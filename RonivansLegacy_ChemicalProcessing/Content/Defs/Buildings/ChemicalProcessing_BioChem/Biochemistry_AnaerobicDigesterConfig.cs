using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using YamlDotNet.Helpers;

namespace Biochemistry.Buildings
{
	//===[ ANAEROBIC DIGESTER CONFIG ]===================================================================================
	public class Biochemistry_AnaerobicDigesterConfig : IBuildingConfig
	{
		public static string ID = "Biochemistry_AnaerobicDigester";
		
		private static readonly List<Storage.StoredItemModifier> DigesterStoredItemModifiers;

		static Biochemistry_AnaerobicDigesterConfig()
		{
			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Preserve);
			list1.Add(Storage.StoredItemModifier.Insulate);
			list1.Add(Storage.StoredItemModifier.Seal);
			DigesterStoredItemModifiers = list1;
		}

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 3, "anaerobic_digester_kanim", 100, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 90f;
			buildingDef.ExhaustKilowattsWhenActive = 0.2f;
			buildingDef.SelfHeatKilowattsWhenActive = 3f;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.OutputConduitType = ConduitType.Gas;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
			SoundUtils.CopySoundsToAnim("anaerobic_digester_kanim", "algae_distillery_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;

			Storage inputStorage = go.AddOrGet<Storage>();
			inputStorage.SetDefaultStoredItemModifiers(DigesterStoredItemModifiers);
			inputStorage.showCapacityStatusItem = false;
			inputStorage.showCapacityAsMainStatus = false;
			inputStorage.showDescriptor = false;

			Storage outputStorage = go.AddOrGet<Storage>();
			outputStorage.SetDefaultStoredItemModifiers(DigesterStoredItemModifiers);
			outputStorage.showCapacityStatusItem = false;
			outputStorage.showCapacityAsMainStatus = false;
			outputStorage.showDescriptor = false;

			//----------------------------- Fabricator Section
			ComplexFabricator digester = go.AddOrGet<ComplexFabricator>();
			digester.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			Workable workable = go.AddOrGet<ComplexFabricatorWorkable>();
			digester.duplicantOperated = false;
			digester.heatedTemperature = 298.15f;
			BuildingTemplates.CreateComplexFabricatorStorage(go, digester);
			digester.inStorage.capacityKg = 1000f;
			digester.buildStorage.capacityKg = 1000f;
			digester.outStorage.capacityKg = 1000f;
			digester.storeProduced = true;
			digester.keepAdditionalTag = SimHashes.Methane.CreateTag();
			digester.inStorage.SetDefaultStoredItemModifiers(DigesterStoredItemModifiers);
			digester.buildStorage.SetDefaultStoredItemModifiers(DigesterStoredItemModifiers);
			digester.outStorage.SetDefaultStoredItemModifiers(DigesterStoredItemModifiers);
			digester.inStorage = inputStorage;
			digester.outStorage = outputStorage;
			digester.outputOffset = new Vector3(1f, 0.5f);
			//-----------------------------

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Gas;
			dispenser.storage = outputStorage;
			dispenser.elementFilter = [SimHashes.Methane];

			ConfigureRecipes();
			Prioritizable.AddRef(go);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
		}

		//===[ ANAEROBIC DIGESTER RECIPES ]===============================================================================
		public static void ConfigureRecipes()
		{
			///These recipes are moved to ModDb.AdditionalRecipes because they are generated from plant products dynamically and need to be initialized later
			SgtLogger.l("anaerobic_digester: recipe config method runs");
		}
	}
}

