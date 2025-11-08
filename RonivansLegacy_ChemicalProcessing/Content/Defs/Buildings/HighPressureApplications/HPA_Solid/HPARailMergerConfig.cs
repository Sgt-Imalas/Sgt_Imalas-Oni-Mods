using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications.HighCapacityLogisticRails
{
	class HPARailMergerConfig : IBuildingConfig
	{
		public static string ID = "HPA_RailMerger";

		private static readonly PortDisplayInput leftPortInfo = new PortDisplayInput(ConduitType.Solid, new CellOffset(-1, 0));
		private static readonly PortDisplayInput rightPortInfo = new PortDisplayInput(ConduitType.Solid, new CellOffset(1, 0));
		public override BuildingDef CreateBuildingDef()
		{
			float[] quantity1 = [250f, 150f];
			string[] materials1 = [ModElements.SteelAndTungstenMaterial, GameTags.Plastic.ToString()];
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 2, "hpa_rail_merger_kanim", 100, 60f, quantity1, materials1, 800f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Solid;
			buildingDef.OutputConduitType = ConduitType.Solid;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.EnergyConsumptionWhenActive = 60f;
			buildingDef.PowerInputOffset = new(0, 1);
			buildingDef.RequiresPowerInput = true;

			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));

			buildingDef.SceneLayer = Grid.SceneLayer.GasFront;
			buildingDef.ObjectLayer = ObjectLayer.SolidConduitConnection;

			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			SoundUtils.CopySoundsToAnim("hpa_rail_merger_kanim", "filter_material_conveyor_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
			float consumerCapacity = HighPressureConduitRegistration.GetMaxConduitCapacity(ConduitType.Solid,true) * 10f;

			go.AddOrGet<LogicOperationalController>();

			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.showDescriptor = false;
			storage.storageFilters = null;
			storage.capacityKg = consumerCapacity;
			storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
			storage.showCapacityStatusItem = false;
			storage.showCapacityAsMainStatus = false;

			SolidConduitConsumer conduitConsumer = go.AddOrGet<SolidConduitConsumer>();
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.capacityKG = consumerCapacity;

			var leftPort = go.AddComponent<PortConduitConsumer>();
			leftPort.SkipSetOperational = true;
			leftPort.capacityKG = consumerCapacity;
			leftPort.alwaysConsume = true;	
			//leftPort.forceAlwaysSatisfied = true;
			leftPort.AssignPort(leftPortInfo);

			var rightPort = go.AddComponent<PortConduitConsumer>();
			rightPort.SkipSetOperational = true;
			rightPort.capacityKG = consumerCapacity;
			rightPort.alwaysConsume = true;
			//rightPort.forceAlwaysSatisfied = true;
			rightPort.AssignPort(rightPortInfo);

			var solidDispenser = go.AddOrGet<ConfigurableThresholdSolidConduitDispenser>();
			solidDispenser.massDispensed = HighPressureConduitRegistration.SolidCap_HP;
			//solidDispenser.alwaysDispense = true;
			solidDispenser.elementFilter = null; 
			this.AttachPort(go);

		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, leftPortInfo);
			controller.AssignPort(go, rightPortInfo);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			var requirement = go.AddOrGet<HPA_SolidConduitRequirement>();
			requirement.RequiresHighPressureOutput = true;
			go.AddOrGet<AIO_DecompressionValve>();
			AttachPort(go);
		}
		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			AttachPort(go);
		}
		override public void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			AttachPort(go);
		}
	}
}
