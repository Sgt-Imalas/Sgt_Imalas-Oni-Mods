using PeterHan.PLib.Options;
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

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomReservoirs
{
	class MedGasReservoirConfig : IBuildingConfig
	{
		public static string ID = "MedGasReservoir";
		public string KANIM = "medium_gas_reservoir_kanim";
		public int Width = 3, Height = 3;


		public PortDisplayInput PrimaryInputPort = new (ConduitType.Gas,new CellOffset(-1,3));
		public PortDisplayInput SecondaryInputPort = new(ConduitType.Gas,new CellOffset(1,3));
		public PortDisplayOutput PrimaryOutputPort = new (ConduitType.Gas,new CellOffset(-1,0));
		public PortDisplayOutput SecondaryOutputPort = new(ConduitType.Gas,new CellOffset(1,0));

		public PermittedRotations Rotations = PermittedRotations.Unrotatable;
		public BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;

		public override BuildingDef CreateBuildingDef()
		{
			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, Width, Height, KANIM, 50, 60f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 800f, buildLocationRule, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, TUNING.NOISE_POLLUTION.NOISY.TIER0);
			def.Floodable = false;
			def.Overheatable = false;
			def.PermittedRotations = Rotations;
			def.ViewMode = OverlayModes.GasConduits.ID;
			def.AudioCategory = "HollowMetal";
			List<LogicPorts.Port> list1 = new List<LogicPorts.Port>();
			list1.Add(LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE, false, false));
			def.LogicOutputPorts = list1;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
			def.DefaultAnimState = "off";
			return def;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<UserNameable>();
			go.AddOrGet<Reservoir>();
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.showDescriptor = true;
			storage.storageFilters = STORAGEFILTERS.GASES;
			storage.capacityKg = 800;
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			go.AddOrGet<SmartReservoir>();

			PortConduitConsumer consumer1 = go.AddComponent<PortConduitConsumer>();
			consumer1.conduitType = ConduitType.Gas;
			consumer1.ignoreMinMassCheck = true;
			consumer1.forceAlwaysSatisfied = true;
			consumer1.alwaysConsume = true;
			consumer1.capacityKG = storage.capacityKg;
			consumer1.AssignPort(PrimaryInputPort);

			PortConduitConsumer consumer2 = go.AddComponent<PortConduitConsumer>();
			consumer2.conduitType = ConduitType.Gas;
			consumer2.ignoreMinMassCheck = true;
			consumer2.forceAlwaysSatisfied = true;
			consumer2.alwaysConsume = true;
			consumer2.capacityKG = storage.capacityKg;
			consumer2.AssignPort(SecondaryInputPort);
					

			go.AddComponent<PipedConduitDispenser>().AssignPort(PrimaryOutputPort);
			go.AddComponent<PipedConduitDispenser>().AssignPort(SecondaryOutputPort);

			AttachPorts(go);
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<StorageController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
			go.AddOrGet<ContentBasedReservoirTint>();
			AttachPorts(go);
		}
		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			AttachPorts(go);
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			AttachPorts(go);
		}

		void AttachPorts(GameObject go)
		{
			PortDisplayController portDisplayController = go.AddComponent<PortDisplayController>();
			portDisplayController.Init(go);
			portDisplayController.AssignPort(go, PrimaryOutputPort);
			portDisplayController.AssignPort(go, PrimaryInputPort);
			portDisplayController.AssignPort(go, SecondaryOutputPort);
			portDisplayController.AssignPort(go, SecondaryInputPort);
		}
	}
}
