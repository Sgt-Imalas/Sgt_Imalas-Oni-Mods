using HarmonyLib;
using KSerialization;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
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
	//==== [ CHEMICAL: FLOCCULATION SIEVE CONFIG ] =================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_FlocculationSieveConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_FlocculationSieve";
		
		//--[ Identification and DLC stuff ]-----------------------------------
		public static readonly List<Storage.StoredItemModifier> SieveStoredItemModifiers;

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayInput pollutedWaterInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(2, 1));
		private static readonly PortDisplayInput toxicSlurryInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(2, 0));

		static Chemical_FlocculationSieveConfig()
		{
			Color? pollutedPortColor = new Color32(181, 155, 7, 255);
			pollutedWaterInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(2, 1), null, pollutedPortColor);
			Color? toxicPortColor = new Color32(130, 51, 5, 255);
			toxicSlurryInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(2, 0), null, toxicPortColor);

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Seal);
			list1.Add(Storage.StoredItemModifier.Insulate);
			SieveStoredItemModifiers = list1;
		}

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [200f, 100f];
			string[] ingredient_types = [SimHashes.Steel.ToString(), "Plastic"];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 3, "flocculation_tank_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier, 0.2f);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 240f;
			buildingDef.ExhaustKilowattsWhenActive = 4f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(-1, 1);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(-1, 0);
			return buildingDef;
		}

		//--[ Building Operation Definitions ]---------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			go.AddOrGet<Desalinator>();
			Prioritizable.AddRef(go);

			Storage standardStorage = go.AddOrGet<Storage>();
			standardStorage.SetDefaultStoredItemModifiers(SieveStoredItemModifiers);
			standardStorage.showCapacityStatusItem = true;
			standardStorage.showCapacityAsMainStatus = true;
			standardStorage.showDescriptor = true;

			ConduitConsumer chlorineInput = go.AddOrGet<ConduitConsumer>();
			chlorineInput.conduitType = ConduitType.Gas;
			chlorineInput.consumptionRate = 10f;
			chlorineInput.capacityKG = 1f;
			chlorineInput.capacityTag = SimHashes.ChlorineGas.CreateTag();
			chlorineInput.forceAlwaysSatisfied = true;
			chlorineInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer pollutedWaterInput = go.AddComponent<PortConduitConsumer>();
			pollutedWaterInput.conduitType = ConduitType.Liquid;
			pollutedWaterInput.consumptionRate = 10f;
			pollutedWaterInput.capacityKG = 50f;
			pollutedWaterInput.capacityTag = SimHashes.DirtyWater.CreateTag();
			pollutedWaterInput.forceAlwaysSatisfied = true;
			pollutedWaterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			pollutedWaterInput.AssignPort(pollutedWaterInputPort);

			PortConduitConsumer toxicSlurryInput = go.AddComponent<PortConduitConsumer>();
			toxicSlurryInput.conduitType = ConduitType.Liquid;
			toxicSlurryInput.consumptionRate = 10f;
			toxicSlurryInput.capacityKG = 50f;
			toxicSlurryInput.capacityTag = ModElements.ToxicMix_Liquid.Tag;
			toxicSlurryInput.forceAlwaysSatisfied = true;
			toxicSlurryInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			toxicSlurryInput.AssignPort(toxicSlurryInputPort);

			ManualDeliveryKG CrushedRockmanualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
			CrushedRockmanualDeliveryKG.SetStorage(standardStorage);
			CrushedRockmanualDeliveryKG.RequestedItemTag = SimHashes.CrushedRock.CreateTag();
			CrushedRockmanualDeliveryKG.capacity = 200f;
			CrushedRockmanualDeliveryKG.refillMass = 50f;
			CrushedRockmanualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ManualDeliveryKG CoalmanualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
			CoalmanualDeliveryKG.SetStorage(standardStorage);
			CoalmanualDeliveryKG.RequestedItemTag = SimHashes.RefinedCarbon.CreateTag();
			CoalmanualDeliveryKG.capacity = 200f;
			CoalmanualDeliveryKG.refillMass = 50f;
			CoalmanualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ManualDeliveryKG SandmanualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
			SandmanualDeliveryKG.SetStorage(standardStorage);
			SandmanualDeliveryKG.RequestedItemTag = SimHashes.Sand.CreateTag();
			SandmanualDeliveryKG.capacity = 200f;
			SandmanualDeliveryKG.refillMass = 50f;
			SandmanualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter pollutedWaterTreatment = go.AddComponent<ElementConverter>();
			pollutedWaterTreatment.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.DirtyWater.CreateTag(), 5f),
				new ElementConverter.ConsumedElement(SimHashes.ChlorineGas.CreateTag(), 0.0025f),
				new ElementConverter.ConsumedElement(SimHashes.CrushedRock.CreateTag(), 0.024f),
				new ElementConverter.ConsumedElement(SimHashes.RefinedCarbon.CreateTag(), 0.034f),
				new ElementConverter.ConsumedElement(SimHashes.Sand.CreateTag(), 0.042f) ];
			pollutedWaterTreatment.outputElements = [
				new ElementConverter.OutputElement(4.9f, SimHashes.Water, 0f, false, true, 0f, 0.5f, 0f, 0xff, 0),
				new ElementConverter.OutputElement(0.11f, SimHashes.Clay, 0f, false, true, 0f, 0.5f, 0f, 0xff, 0) ];

			ElementConverter toxicSlurryTreatment = go.AddComponent<ElementConverter>();
			toxicSlurryTreatment.consumedElements = [
				new ElementConverter.ConsumedElement(ModElements.ToxicMix_Liquid.Tag, 5f),
				new ElementConverter.ConsumedElement(SimHashes.ChlorineGas.CreateTag(), 0.0025f),
				new ElementConverter.ConsumedElement(SimHashes.CrushedRock.CreateTag(), 0.024f),
				new ElementConverter.ConsumedElement(SimHashes.RefinedCarbon.CreateTag(), 0.034f),
				new ElementConverter.ConsumedElement(SimHashes.Sand.CreateTag(), 0.042f) ];
			toxicSlurryTreatment.outputElements = [
				new ElementConverter.OutputElement(2f, SimHashes.Water, 0f, false, true, 0f, 0.5f, 0f, 0xff, 0),
				new ElementConverter.OutputElement(3.1f, ModElements.Slag_Solid, 0f, false, true, 0f, 0.5f, 0f, 0xff, 0) ];
			//--------------------------------------------------------------------

			ElementDropper clayDropper = go.AddComponent<ElementDropper>();
			clayDropper.emitMass = 10f;
			clayDropper.emitTag = SimHashes.Clay.CreateTag();
			clayDropper.emitOffset = new Vector3(0f, 1f, 0f);

			ElementDropper slagDropper = go.AddComponent<ElementDropper>();
			slagDropper.emitMass = 25f;
			slagDropper.emitTag = ModElements.Slag_Solid.Tag;
			slagDropper.emitOffset = new Vector3(0f, 1f, 0f);

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.elementFilter = [SimHashes.Water];
			dispenser.alwaysDispense = true;

			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, pollutedWaterInputPort);
			controller.AssignPort(go, toxicSlurryInputPort);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			this.AttachPort(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			this.AttachPort(go);
		}
	}
	
}
