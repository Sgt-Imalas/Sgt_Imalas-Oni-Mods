using Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals;
using HarmonyLib;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//===[ CHEMICAL: RAYON LOOM CONFIG ]=============================================================================
	public class Chemical_RayonLoomConfig : IBuildingConfig
	{
		public static string ID = "Chemical_RayonLoom";

		private Tag FUEL_TAG = SimHashes.Syngas.CreateTag();

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			var fuelConsumer = go.AddOrGet<Chemical_FueledFabricatorAddon>();
			fuelConsumer.fuelTag = this.FUEL_TAG;

			ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
			fabricator.heatedTemperature = 368.15f;
			fabricator.duplicantOperated = false;
			fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			go.AddOrGet<ComplexFabricatorWorkable>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
			fabricator.outStorage.capacityKg = 10f;
			fabricator.inStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			fabricator.buildStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			fabricator.outStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			ConduitConsumer local1 = go.AddOrGet<ConduitConsumer>();
			local1.capacityTag = this.FUEL_TAG;
			local1.capacityKG = 10f;
			local1.alwaysConsume = true;
			local1.storage = fabricator.inStorage;
			local1.forceAlwaysSatisfied = true;
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [new ElementConverter.ConsumedElement(this.FUEL_TAG, 0.8f)];
			converter.outputElements = [new ElementConverter.OutputElement(0.025f, SimHashes.Steam, 373.15f, false, false, 0f, 3f, 1f, 0xff, 0)];
			Prioritizable.AddRef(go);
		}


		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER3;
			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 6, 4, "viscose_frame_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.NONE, noise);
			BuildingTemplates.CreateElectricalBuildingDef(def);
			def.AudioCategory = "Metal";
			def.AudioSize = "large";
			def.Overheatable = true;
			def.OverheatTemperature = 348.15f;
			def.EnergyConsumptionWhenActive = 480f;
			def.ExhaustKilowattsWhenActive = 0.5f;
			def.SelfHeatKilowattsWhenActive = 32f;
			def.PowerInputOffset = new CellOffset(0, 0);
			def.InputConduitType = ConduitType.Gas;
			def.UtilityInputOffset = new CellOffset(-1, 0);
			def.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
			SoundUtils.CopySoundsToAnim("viscose_frame_kanim", "clothingfactory_kanim");
			return def;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
		}
	}
}
