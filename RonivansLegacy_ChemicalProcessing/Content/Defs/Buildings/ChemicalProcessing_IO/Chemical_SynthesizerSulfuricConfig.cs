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


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//====[ CHEMICAL: SULFURIC ACID SYNTHESIZER CONFIG ]===================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_SynthesizerSulfuricConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_SynthesizerSulfuric";

		//--[ Identification and DLC stuff ]-----------------------------------
		public static readonly List<Storage.StoredItemModifier> StoredItemModifiers;

		//--[ Special Settings ]-----------------------------------------------
		static Chemical_SynthesizerSulfuricConfig()
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
			float[] ingredient_mass = [100f, 200f];
			string[] ingredient_types = [SimHashes.Ceramic.ToString(), "RefinedMetal"];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 4, "mixer_sulfuric_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 120f;
			buildingDef.ExhaustKilowattsWhenActive = 8f;
			buildingDef.SelfHeatKilowattsWhenActive = 4f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(0, 3);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(StoredItemModifiers);
			storage.capacityKg = 300f;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			storage.showDescriptor = true;
			go.AddOrGet<WaterPurifier>();
			Prioritizable.AddRef(go);

			ConduitConsumer steamInput = go.AddOrGet<ConduitConsumer>();
			steamInput.conduitType = ConduitType.Gas;
			steamInput.consumptionRate = 10f;
			steamInput.capacityTag = SimHashes.Steam.CreateTag();
			steamInput.forceAlwaysSatisfied = true;
			steamInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ManualDeliveryKG sulfur_delivery = go.AddOrGet<ManualDeliveryKG>();
			sulfur_delivery.SetStorage(storage);
			sulfur_delivery.RequestedItemTag = SimHashes.Sulfur.CreateTag();
			sulfur_delivery.capacity = 200f;
			sulfur_delivery.refillMass = 50f;
			sulfur_delivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [new ElementConverter.ConsumedElement(SimHashes.Steam.CreateTag(), 0.4f), new ElementConverter.ConsumedElement(SimHashes.Sulfur.CreateTag(), 0.6f)];
			converter.outputElements = [new ElementConverter.OutputElement(1f, ModElements.SulphuricAcid_Liquid, 345.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0)];
			//-------------------------------------------------------------------

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.alwaysDispense = true;
			dispenser.storage = storage;
			dispenser.elementFilter = [ModElements.SulphuricAcid_Liquid];
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
