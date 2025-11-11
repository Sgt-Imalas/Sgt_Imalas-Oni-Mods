using HarmonyLib;
using KSerialization;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations;
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
		static Chemical_Wooden_BoilerConfig()
		{
			GeneratorList.AddCombustionGenerator(ID);
		}
		public static string ID = "Chemical_Wooden_Boiler";
		

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayOutput co2Port = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-1, 2), null, UIUtils.rgb(6, 62, 42));
		private static readonly PortDisplayOutput steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 3), null, new Color32(167, 180, 201, 255));
		
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
			SoundUtils.CopySoundsToAnim("wooden_boiler_kanim", "generatorwood_kanim");
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
			go.AddOrGet<Reservoir>();
			go.AddOrGet<ElementConversionBuilding>();
			Prioritizable.AddRef(go);

			ManualDeliveryKG woodDelivery = go.AddComponent<ManualDeliveryKG>();
			woodDelivery.SetStorage(storage);
			woodDelivery.RequestedItemTag = SimHashes.WoodLog.CreateTag();
			woodDelivery.capacity = 2160;
			woodDelivery.refillMass = 720;
			woodDelivery.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

			ConduitConsumer waterInput = go.AddOrGet<ConduitConsumer>();
			waterInput.conduitType = ConduitType.Liquid;
			waterInput.consumptionRate = 40f;
			waterInput.capacityKG = 1000f;
			waterInput.capacityTag = SimHashes.Water.CreateTag();
			waterInput.forceAlwaysSatisfied = true;
			waterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;


			//base value wattage: 850W, 2kg pump rate
			CustomizeBuildings.TryGetSteamTurbineWattageAndPumpRate(out float wattage, out float pumpRate);
			float conversionRate = 4f;
			float totalSTOutputWattage = wattage * (conversionRate / pumpRate); //default: 850W * (4kg/2kg) = 1700W/1
			float efficiencyGain = 4.0f; //4x more efficient than vanilla generator (wood needs that extra boost, its very bad in base game and competes with wood->syngas recipe)
			float targetWattage = totalSTOutputWattage / efficiencyGain; 

			float vanillaGeneratorConsumption = WoodGasGeneratorConfig.CONSUMPTION_RATE;
			float vanillaGeneratorWattage = 300f;

			float fuelConsumptionRate = vanillaGeneratorConsumption * (targetWattage / vanillaGeneratorWattage);

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [
				new ElementConverter.ConsumedElement(GameTags.BuildingWood, fuelConsumptionRate),
				new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), conversionRate) ];
			converter.outputElements = [
				new(conversionRate, SimHashes.Steam, UtilMethods.GetKelvinFromC(200), false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new(0.5f,SimHashes.CarbonDioxide,UtilMethods.GetKelvinFromC(110),false, true,-1,2)
				];
			//-------------------------------------------------------------------
			var o = go.AddOrGet<SolidDeliverySelection>();
			o.Options = [.. RefinementRecipeHelper.GetWoods().Select(sh => sh.CreateTag())];
			o.AnyTag = GameTags.BuildingWood;

			PipedConduitDispenser dispenser = go.AddComponent<PipedConduitDispenser>();
			dispenser.storage = storage;
			dispenser.elementFilter = [SimHashes.Steam];
			dispenser.AssignPort(steamOutputPort);
			dispenser.alwaysDispense = true;
			dispenser.SkipSetOperational = true;

			PipedConduitDispenser co2Dispenser = go.AddComponent<PipedConduitDispenser>();
			co2Dispenser.storage = storage;
			co2Dispenser.elementFilter = [SimHashes.CarbonDioxide];
			co2Dispenser.AssignPort(co2Port);
			co2Dispenser.alwaysDispense = true;
			co2Dispenser.SkipSetOperational = true;

			PipedOptionalExhaust co2Exhaust = go.AddComponent<PipedOptionalExhaust>();
			co2Exhaust.dispenser = co2Dispenser;
			co2Exhaust.elementTag = SimHashes.CarbonDioxide.CreateTag();
			co2Exhaust.capacity = 5f;

			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);

			controller.AssignPort(go, steamOutputPort);
			controller.AssignPort(go, co2Port);
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
