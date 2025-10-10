using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS;
using TUNING;
using UnityEngine;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.NuclearProcessing
{
	public class HepProjectorConfig : IBuildingConfig
	{
		public override string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];

		public static string ID = "HepProjector";

		private Tag RADFUEL = SimHashes.UraniumOre.CreateTag();



		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
		}

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [100f, 50f];
			string[] construction_materials = ["RefinedMetal", "Plastic"];
			EffectorValues none = NOISE_POLLUTION.NONE;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 1, "hep_projector_kanim", 60, 60f, construction_mass, construction_materials, 800f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.NONE, none);
			buildingDef.RequiresPowerInput = false;
			buildingDef.Floodable = false;
			buildingDef.Entombable = true;
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			buildingDef.ViewMode = OverlayModes.Radiation.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.PermittedRotations = PermittedRotations.R90;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			SoundUtils.CopySoundsToAnim("hep_projector_kanim", "radiation_lamp_kanim");
			return buildingDef;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.TryGetComponent<KPrefabID>(out var kPrefabID);
			kPrefabID.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			kPrefabID.AddTag(GameTags.CorrosionProof);
			UnityEngine.Object.Destroy(go.GetComponent<BuildingEnabledButton>());

			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			go.AddOrGet<HepProjector>();

			Storage storage = go.AddOrGet<Storage>();
			storage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			storage.showInUI = true;
			storage.allowItemRemoval = false;
			storage.capacityKg = 500f;

			ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
			manualDeliveryKg.SetStorage(storage);
			manualDeliveryKg.RequestedItemTag = this.RADFUEL;
			manualDeliveryKg.capacity = 300f;
			manualDeliveryKg.refillMass = 100f;
			manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements = [new(ModAssets.Tags.AIO_RadEmitterInputMaterial, 0.008f)];
			elementConverter.outputElements = [new ElementConverter.OutputElement(0.008f, SimHashes.NuclearWaste, UtilMethods.GetKelvinFromC(60), storeOutput: true, diseaseWeight: 0.75f)];

			RadiationEmitter radiationEmitter = go.AddComponent<RadiationEmitter>();
			radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
			radiationEmitter.emitRads = 0.0f;
			radiationEmitter.emitRadiusX = 6;
			radiationEmitter.emitRadiusY = 6;
			radiationEmitter.radiusProportionalToRads = false;
			radiationEmitter.emissionOffset = new Vector3(0.0f, 0.0f, 0.0f);

			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.elementFilter = [SimHashes.NuclearWaste];
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			go.AddOrGet<HepProjector>();
			go.AddOrGet<SolidDeliverySelectionHEPEmitter>();
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
		}
	}
}
