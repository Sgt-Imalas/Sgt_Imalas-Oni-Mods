using HarmonyLib;
using KSerialization;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs.BuildingPortUtils;

namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//===[ CHEMICAL: GAS BOILER CONFIG ]=====================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_Gas_BoilerConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_Gas_Boiler";
		
		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayInput combustibleGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(0, 0));
		private static readonly PortDisplayOutput steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 3));
		public static readonly List<Storage.StoredItemModifier> ChemGasBoilerStorageModifiers;
		static Chemical_Gas_BoilerConfig()
		{
			Color? steamPortColor = new Color32(167, 180, 201, 255);
			steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 3), null, steamPortColor);
			Color? combustibleGasPortColor = new Color32(255, 114, 33, 255);
			combustibleGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(0, 0), null, combustibleGasPortColor);

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Seal);
			list1.Add(Storage.StoredItemModifier.Insulate);
			ChemGasBoilerStorageModifiers = list1;
		}

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] singleArray1 = new float[] { 1200f, 1000f };
			string[] textArray1 = new string[] { "RefinedMetal", SimHashes.Ceramic.ToString() };

			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER3;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 4, "gas_boiler_b_kanim", 100, 30f, singleArray1, textArray1, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, noise, 0.2f);
			buildingDef.ExhaustKilowattsWhenActive = 8f;
			buildingDef.SelfHeatKilowattsWhenActive = 1f;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(-1, 0));
			List<LogicPorts.Port> list1 = new List<LogicPorts.Port>();
			list1.Add(LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(1, 0), (string)STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT, (string)STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE, (string)STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE, false, false));
			buildingDef.LogicOutputPorts = list1;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "WaterPurifier");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(ChemGasBoilerStorageModifiers);
			storage.capacityKg = 10000f;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			storage.showDescriptor = true;
			go.AddOrGet<Reservoir>();
			go.AddOrGet<SmartReservoir>();
			go.AddOrGet<WaterPurifier>();
			Prioritizable.AddRef(go);

			ConduitConsumer waterInput = go.AddOrGet<ConduitConsumer>();
			waterInput.conduitType = ConduitType.Liquid;
			waterInput.consumptionRate = 40f;
			waterInput.capacityKG = 1000f;
			waterInput.capacityTag = SimHashes.Water.CreateTag();
			waterInput.forceAlwaysSatisfied = true;
			waterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer combustibleGasInput = go.AddComponent<PortConduitConsumer>();
			combustibleGasInput.conduitType = ConduitType.Gas;
			combustibleGasInput.consumptionRate = 1f;
			combustibleGasInput.capacityKG = 30f;
			combustibleGasInput.capacityTag = GameTags.CombustibleGas;
			combustibleGasInput.forceAlwaysSatisfied = true;
			combustibleGasInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			combustibleGasInput.AssignPort(combustibleGasInputPort);

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = new ElementConverter.ConsumedElement[] {
				new ElementConverter.ConsumedElement(GameTags.CombustibleGas, 0.09f),
				new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), 3f) };
			converter.outputElements = new ElementConverter.OutputElement[] {
				new ElementConverter.OutputElement(3f, SimHashes.Steam, 474.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0) };

			//-------------------------------------------------------------------

			BuildingElementEmitter co2emitter = go.AddOrGet<BuildingElementEmitter>();
			co2emitter.emitRate = 0.00833f;
			co2emitter.temperature = 383.15f;
			co2emitter.element = SimHashes.CarbonDioxide;
			co2emitter.modifierOffset = new Vector2(-1f, 2f);

			PipedConduitDispenser dispenser = go.AddComponent<PipedConduitDispenser>();
			dispenser.storage = storage;
			dispenser.elementFilter = new SimHashes[] { SimHashes.Steam };
			dispenser.AssignPort(steamOutputPort);
			dispenser.alwaysDispense = true;
			dispenser.SkipSetOperational = true;

			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);

			controller.AssignPort(go, combustibleGasInputPort);
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

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
		}

	}	
}
