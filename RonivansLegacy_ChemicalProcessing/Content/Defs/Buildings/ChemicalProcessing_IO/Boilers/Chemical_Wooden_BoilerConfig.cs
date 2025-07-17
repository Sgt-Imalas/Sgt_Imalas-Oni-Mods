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
using UtilLibs;
using UtilLibs.BuildingPortUtils;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	/// <summary>
	/// all combustion boilers now produce 1350W in steam power, while burning the equivalent of 900W of their respective generators in fuel
	/// The efficiency is a bonus from "higher efficiency" that is granted due to the higher level infrastructure required to maintain such a generator
	/// </summary>



	//===[ CHEMICAL: WOODEN BOILER CONFIG ]=====================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_Wooden_BoilerConfig : IBuildingConfig
	{
		public static string ID = "Chemical_Wooden_Boiler";
		

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayOutput steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 3));
		public static readonly List<Storage.StoredItemModifier> ChemWoodBoilerStorageModifiers;
		static Chemical_Wooden_BoilerConfig()
		{
			Color? steamPortColor = new Color32(167, 180, 201, 255);
			steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 3), null, steamPortColor);

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Seal);
			list1.Add(Storage.StoredItemModifier.Insulate);
			ChemWoodBoilerStorageModifiers = list1;
		}

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] singleArray1 = [1200f, 1000f];
			string[] textArray1 = ["RefinedMetal", SimHashes.Ceramic.ToString()];

			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER3;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 4, "wooden_boiler_kanim", 100, 30f, singleArray1, textArray1, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, noise);
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
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(ChemWoodBoilerStorageModifiers);
			storage.capacityKg = 10000f;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			storage.showDescriptor = true;
			go.AddOrGet<SmartReservoir>();
			go.AddOrGet<WaterPurifier>();
			Prioritizable.AddRef(go);

			ManualDeliveryKG woodDelivery = go.AddComponent<ManualDeliveryKG>();
			woodDelivery.SetStorage(storage);
			woodDelivery.RequestedItemTag = SimHashes.WoodLog.CreateTag();
			woodDelivery.capacity = 2160;
			woodDelivery.refillMass = 720;
			woodDelivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ConduitConsumer waterInput = go.AddOrGet<ConduitConsumer>();
			waterInput.conduitType = ConduitType.Liquid;
			waterInput.consumptionRate = 40f;
			waterInput.capacityKG = 1000f;
			waterInput.capacityTag = SimHashes.Water.CreateTag();
			waterInput.forceAlwaysSatisfied = true;
			waterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.WoodLog.CreateTag(), 3.6f),
				new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), 4f) ];
			converter.outputElements = [
				new(4f, SimHashes.Steam, UtilMethods.GetKelvinFromC(200), false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new(0.5f,SimHashes.CarbonDioxide,UtilMethods.GetKelvinFromC(110),false, false,-1,2)
				];
			//-------------------------------------------------------------------

			PipedConduitDispenser dispenser = go.AddComponent<PipedConduitDispenser>();
			dispenser.storage = storage;
			dispenser.elementFilter = [SimHashes.Steam];
			dispenser.AssignPort(steamOutputPort);
			dispenser.alwaysDispense = true;
			dispenser.SkipSetOperational = true;

			this.AttachPort(go);
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

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
		}
	}
}
