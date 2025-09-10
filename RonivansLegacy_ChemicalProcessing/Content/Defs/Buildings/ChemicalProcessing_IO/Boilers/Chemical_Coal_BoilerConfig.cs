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



	//===[ CHEMICAL: COAL BOILER CONFIG ]=====================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_Coal_BoilerConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_Coal_Boiler";
		

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayOutput steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 3));
		static Chemical_Coal_BoilerConfig()
		{
			Color? steamPortColor = new Color32(167, 180, 201, 255);
			steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 3), null, steamPortColor);
		}

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] singleArray1 = [1200f, 1000f];
			string[] textArray1 = ["RefinedMetal", SimHashes.Ceramic.ToString()];

			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER3;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 4, "coal_boiler_kanim", 100, 30f, singleArray1, textArray1, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, noise, 0.2f);
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
			SoundUtils.CopySoundsToAnim("coal_boiler_kanim", "generatorphos_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage.capacityKg = 10000f;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			storage.showDescriptor = true;
			go.AddOrGet<SmartReservoir>();
			go.AddOrGet<ElementConversionBuilding>();
			Prioritizable.AddRef(go);

			ManualDeliveryKG coalDelivery = go.AddComponent<ManualDeliveryKG>();
			coalDelivery.SetStorage(storage);
			coalDelivery.RequestedItemTag = SimHashes.Carbon.CreateTag();
			coalDelivery.capacity = 1500f;
			coalDelivery.refillMass = 600f;
			coalDelivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

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
				new (GameTags.CombustibleSolid, 1.5f),
				new (SimHashes.Water.CreateTag(), 4f)];
			converter.outputElements = [
				new(4f, SimHashes.Steam, UtilMethods.GetKelvinFromC(200), false, true, 0f, 0.5f, 0.75f, 0xff, 0), 
				new(0.2f ,SimHashes.CarbonDioxide,UtilMethods.GetKelvinFromC(110),false, false,-1,2)
				];
			//-------------------------------------------------------------------

			go.AddOrGet<SolidDeliverySelection>().Options = [SimHashes.Carbon.CreateTag(),SimHashes.Peat.CreateTag()];

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
