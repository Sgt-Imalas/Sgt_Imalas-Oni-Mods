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
	//==== [ CHEMICAL: ELECTRIC BOILER CONFIG ] ====================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_ElectricBoilerConfig : IBuildingConfig
	{
		//--[ Base Information ]---------------------------------------------------------------------------
		public static string ID = "Chemical_ElectricBoiler";

		//--[ Identification and DLC stuff ]--------------------------------------------------------------
		private static readonly PortDisplayOutput steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(1, 0));

		//--[ Special Settings ]-----------------------------------------------
		static Chemical_ElectricBoilerConfig()
		{
			Color? steamPortColor = new Color32(167, 180, 201, 255);
			steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(1, 0), null, steamPortColor);
		}

		public override BuildingDef CreateBuildingDef()
		{
			float[] singleArray1 = [200f, 100f];
			string[] textArray1 = [SimHashes.Ceramic.ToString(), "RefinedMetal"];

			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "electric_boiler_kanim", 100, 30f, singleArray1, textArray1, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, noise, 0.2f);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 720f;
			buildingDef.ExhaustKilowattsWhenActive = 0.12f;
			buildingDef.SelfHeatKilowattsWhenActive = 0.8f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(1, 2);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
			SoundUtils.CopySoundsToAnim("electric_boiler_kanim", "spaceheater_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage.capacityKg = 100f;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			storage.showDescriptor = true;
			go.AddOrGet<Reservoir>();
			go.AddOrGet<ElementConversionBuilding>();
			Prioritizable.AddRef(go);

			ConduitConsumer waterInput = go.AddOrGet<ConduitConsumer>();
			waterInput.conduitType = ConduitType.Liquid;
			//waterInput.consumptionRate = 1f;
			waterInput.capacityTag = SimHashes.Water.CreateTag();
			waterInput.capacityKG = 10f;
			waterInput.forceAlwaysSatisfied = true;
			waterInput.alwaysConsume = true;
			waterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			float conversionRate = 2f;
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), conversionRate)];
			converter.outputElements = [new ElementConverter.OutputElement(conversionRate, SimHashes.Steam, UtilMethods.GetKelvinFromC(120), false, true)];

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
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
		}
	}

}
