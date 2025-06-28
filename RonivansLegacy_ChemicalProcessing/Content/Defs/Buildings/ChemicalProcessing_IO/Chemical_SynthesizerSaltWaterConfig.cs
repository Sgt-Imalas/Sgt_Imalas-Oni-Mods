using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//====[ CHEMICAL: SALT WATER SYNTHESIZER CONFIG ]===================================================================
	public class Chemical_SynthesizerSaltWaterConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_SynthesizerSaltWater";


		//--[ Identification and DLC stuff ]-----------------------------------
		public static readonly List<Storage.StoredItemModifier> StoredItemModifiers;

		//--[ Special Settings ]-----------------------------------------------
		static Chemical_SynthesizerSaltWaterConfig()
		{
			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Seal);
			list1.Add(Storage.StoredItemModifier.Insulate);
			StoredItemModifiers = list1;
		}

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 4, "mixer_saltwater_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 90f;
			buildingDef.ExhaustKilowattsWhenActive = 4f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(0, 3);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(StoredItemModifiers);
			storage.capacityKg = 300f;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			storage.showDescriptor = true;
			go.AddOrGet<WaterPurifier>();
			Prioritizable.AddRef(go);

			ConduitConsumer steamInput = go.AddOrGet<ConduitConsumer>();
			steamInput.conduitType = ConduitType.Liquid;
			steamInput.consumptionRate = 10f;
			steamInput.capacityTag = SimHashes.Water.CreateTag();
			steamInput.forceAlwaysSatisfied = true;
			steamInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ManualDeliveryKG salt_delivery = go.AddOrGet<ManualDeliveryKG>();
			salt_delivery.SetStorage(storage);
			salt_delivery.RequestedItemTag = SimHashes.Salt.CreateTag();
			salt_delivery.capacity = 200f;
			salt_delivery.refillMass = 50f;
			salt_delivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), 4.65f), new ElementConverter.ConsumedElement(SimHashes.Salt.CreateTag(), 0.35f)];
			converter.outputElements = [new ElementConverter.OutputElement(5f, SimHashes.SaltWater, 300.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0)];
			//-------------------------------------------------------------------

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.alwaysDispense = true;
			dispenser.storage = storage;
			dispenser.elementFilter = [SimHashes.SaltWater];
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
		}
	}
}
