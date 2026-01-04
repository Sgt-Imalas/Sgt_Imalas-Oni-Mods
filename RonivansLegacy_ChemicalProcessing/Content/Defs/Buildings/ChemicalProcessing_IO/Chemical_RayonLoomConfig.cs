using Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals;
using HarmonyLib;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators;
using RonivansLegacy_ChemicalProcessing.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using UtilLibs.BuildingPortUtils;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//===[ CHEMICAL: RAYON LOOM CONFIG ]=============================================================================
	public class Chemical_RayonLoomConfig : IBuildingConfig
	{
		public static string ID = "Chemical_RayonLoom";

		private static readonly PortDisplayOutput steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(1, 0), null, new Color32(167, 180, 201, 255));

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			Tag FUEL_TAG = (!ElementLoader.GetElement(SimHashes.Syngas.CreateTag())?.disabled ?? false) ? SimHashes.Syngas.CreateTag() : SimHashes.Methane.CreateTag();
			SgtLogger.l("RayonLoom configured to use element: " + FUEL_TAG.Name);
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			var fuelConsumer = go.AddOrGet<Chemical_FueledFabricatorAddon>();
			fuelConsumer.fuelTag = FUEL_TAG;

			CustomComplexFabricatorBase fabricator = go.AddOrGet<CustomComplexFabricatorBase>();
			fabricator.heatedTemperature = 368.15f;
			fabricator.duplicantOperated = false;
			fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			fabricator.KeepAdditionalTags = [FUEL_TAG, SimHashes.Steam.CreateTag()];

			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			go.AddOrGet<ComplexFabricatorWorkable>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
			fabricator.outStorage.capacityKg = 10f;
			fabricator.inStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			fabricator.buildStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			fabricator.outStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);

			ConduitConsumer syngasConsumer = go.AddOrGet<ConduitConsumer>();
			syngasConsumer.capacityTag = FUEL_TAG;
			syngasConsumer.capacityKG = 10f;
			syngasConsumer.alwaysConsume = true;
			syngasConsumer.storage = fabricator.inStorage;
			syngasConsumer.forceAlwaysSatisfied = true;

			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [new ElementConverter.ConsumedElement(FUEL_TAG, 0.1f)];
			converter.outputElements = [new ElementConverter.OutputElement(0.025f, SimHashes.Steam, 373.15f, false, true, 1f, 0f)];
			Prioritizable.AddRef(go);
			converter.SetStorage(fabricator.inStorage);


			PipedConduitDispenser steamDispenser = go.AddComponent<PipedConduitDispenser>();
			steamDispenser.storage = fabricator.inStorage;
			steamDispenser.elementFilter = [SimHashes.Steam];
			steamDispenser.AssignPort(steamOutputPort);
			steamDispenser.alwaysDispense = true;
			steamDispenser.SkipSetOperational = true;

			PipedOptionalExhaust steamExhaust = go.AddComponent<PipedOptionalExhaust>();
			steamExhaust.dispenser = steamDispenser;
			steamExhaust.elementTag = SimHashes.Steam.CreateTag();
			steamExhaust.capacity = 2f;
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
			this.AttachPort(go);
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
		}
		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);

			controller.AssignPort(go, steamOutputPort);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			this.AttachPort(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			this.AttachPort(go);
		}

	}
}
