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
	//==== [ CHEMICAL: CARBON RECYCLING UNIT CONFIG ] ===============================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_Co2RecyclerConfig : IBuildingConfig
	{
		//--[ Base Information ]---------------------------------------------------------------------------
		public static string ID = "Chemical_Co2Recycler";

		//--[ Identification and DLC stuff ]--------------------------------------------------------------

		public static readonly List<Storage.StoredItemModifier> RecyclerStoredItemModifiers;

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayInput liquidco2InputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-3, 1));
		private static readonly PortDisplayInput gasco2InputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(-3, 0));
		private static readonly PortDisplayOutput steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(3, 2));
		private static readonly PortDisplayOutput methaneOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(3, 1));

		static Chemical_Co2RecyclerConfig()
		{
			Color? liquidCo2PortColor = new Color32(143, 143, 143, 255);
			liquidco2InputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-3, 1), null, liquidCo2PortColor);
			Color? gasCo2PortColor = new Color32(143, 143, 143, 255);
			gasco2InputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(-3, 0), null, gasCo2PortColor);
			Color? steamPortColor = new Color32(167, 180, 201, 255);
			steamOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(3, 2), null, steamPortColor);
			Color? methanePortColor = new Color32(255, 114, 33, 255);
			methaneOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(3, 1), null, methanePortColor);

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Seal);
			list1.Add(Storage.StoredItemModifier.Insulate);
			RecyclerStoredItemModifiers = list1;
		}

		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [800f, 400f];
			string[] ingredient_types = [SimHashes.Ceramic.ToString(), SimHashes.Steel.ToString()];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 4, "co2_recycler_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 1000f;
			buildingDef.ExhaustKilowattsWhenActive = 14f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(-3, 2);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(3, 0);
			SoundUtils.CopySoundsToAnim("co2_recycler_kanim", "supermaterial_refinery_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			Tag catalyst = SimHashes.Iron.CreateTag();

			Storage solidStorage = go.AddOrGet<Storage>();
			solidStorage.SetDefaultStoredItemModifiers(RecyclerStoredItemModifiers);
			solidStorage.showInUI = true;

			ManualDeliveryKG catalystDelivery = go.AddComponent<ManualDeliveryKG>();
			catalystDelivery.SetStorage(solidStorage);
			catalystDelivery.RequestedItemTag = catalyst;
			catalystDelivery.capacity = 300f;
			catalystDelivery.refillMass = 50f;
			catalystDelivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ConduitConsumer hydrogenInput = go.AddOrGet<ConduitConsumer>();
			hydrogenInput.conduitType = ConduitType.Gas;
			hydrogenInput.consumptionRate = 10f;
			hydrogenInput.capacityKG = 50f;
			hydrogenInput.capacityTag = SimHashes.Hydrogen.CreateTag();
			hydrogenInput.forceAlwaysSatisfied = true;
			hydrogenInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer liquidCo2Input = go.AddComponent<PortConduitConsumer>();
			liquidCo2Input.conduitType = ConduitType.Liquid;
			liquidCo2Input.consumptionRate = 10f;
			liquidCo2Input.capacityKG = 50f;
			liquidCo2Input.capacityTag = SimHashes.LiquidCarbonDioxide.CreateTag();
			liquidCo2Input.forceAlwaysSatisfied = true;
			liquidCo2Input.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			liquidCo2Input.AssignPort(liquidco2InputPort);

			PortConduitConsumer gasCo2Input = go.AddComponent<PortConduitConsumer>();
			gasCo2Input.conduitType = ConduitType.Gas;
			gasCo2Input.consumptionRate = 10f;
			gasCo2Input.capacityKG = 50f;
			gasCo2Input.capacityTag = SimHashes.CarbonDioxide.CreateTag();
			gasCo2Input.forceAlwaysSatisfied = true;
			gasCo2Input.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			gasCo2Input.AssignPort(gasco2InputPort);

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter sabatier = go.AddComponent<ElementConverter>();
			sabatier.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.LiquidCarbonDioxide.CreateTag(), 0.2f),
				new ElementConverter.ConsumedElement(SimHashes.Hydrogen.CreateTag(), 0.6f),
				new ElementConverter.ConsumedElement(catalyst, 0.025f) ];
			sabatier.outputElements = [
				new ElementConverter.OutputElement(0.5f, SimHashes.Water, 337.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.3f, SimHashes.Methane, 367.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.025f, SimHashes.Rust, 319.15f, false, true, 0f, 0.5f, 0.25f, 0xff, 0) ];

			ElementConverter bosch = go.AddComponent<ElementConverter>();
			bosch.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.CarbonDioxide.CreateTag(), 0.4f),
				new ElementConverter.ConsumedElement(SimHashes.Hydrogen.CreateTag(), 0.4f),
				new ElementConverter.ConsumedElement(catalyst, 0.025f) ];
			bosch.outputElements = [
				new ElementConverter.OutputElement(0.4f, SimHashes.Steam, 382.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.3f, DlcManager.IsExpansion1Active() ? SimHashes.Graphite : SimHashes.Fullerene, 319.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.025f, SimHashes.Rust, 319.15f, false, true, 0f, 0.5f, 0.25f, 0xff, 0) ];
			//--------------------------------------------------------------------

			ElementDropper rustDropper = go.AddComponent<ElementDropper>();
			rustDropper.emitMass = 10f;
			rustDropper.emitTag = SimHashes.Rust.CreateTag();
			rustDropper.emitOffset = new Vector3(0f, 1f, 0f);

			//Fullerene in base game,
			ElementDropper RefinedCarbonDropper = go.AddComponent<ElementDropper>();
			RefinedCarbonDropper.emitMass = 10f;
			RefinedCarbonDropper.emitTag = SimHashes.Fullerene.CreateTag();
			RefinedCarbonDropper.emitOffset = new Vector3(0f, 1f, 0f);
			//graphite in spaced out
			ElementDropper graphiteDropper = go.AddComponent<ElementDropper>();
			graphiteDropper.emitMass = 10f;
			graphiteDropper.emitTag = SimHashes.Graphite.CreateTag();
			graphiteDropper.emitOffset = new Vector3(0f, 1f, 0f);

			Storage outputStorage = go.AddOrGet<Storage>();
			outputStorage.SetDefaultStoredItemModifiers(RecyclerStoredItemModifiers);
			outputStorage.showInUI = true;

			ConduitDispenser waterOutput = go.AddOrGet<ConduitDispenser>();
			waterOutput.conduitType = ConduitType.Liquid;
			waterOutput.storage = outputStorage;
			waterOutput.elementFilter = [SimHashes.Water];

			PipedConduitDispenser steamOutput = go.AddComponent<PipedConduitDispenser>();
			steamOutput.conduitType = ConduitType.Gas;
			steamOutput.alwaysDispense = true;
			steamOutput.elementFilter = [SimHashes.Steam];
			steamOutput.AssignPort(steamOutputPort);

			PipedConduitDispenser methaneOutput = go.AddComponent<PipedConduitDispenser>();
			methaneOutput.conduitType = ConduitType.Gas;
			methaneOutput.alwaysDispense = true;
			methaneOutput.elementFilter = [SimHashes.Methane];
			methaneOutput.AssignPort(methaneOutputPort);

			go.AddOrGet<ElementConversionBuilding>(); //Handles element converter
			Prioritizable.AddRef(go);
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, liquidco2InputPort);
			controller.AssignPort(go, gasco2InputPort);
			controller.AssignPort(go, steamOutputPort);
			controller.AssignPort(go, methaneOutputPort);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			this.AttachPort(go);
			go.AddOrGet<PortPreviewVisualizer>();
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			this.AttachPort(go);
		}
	}
}
