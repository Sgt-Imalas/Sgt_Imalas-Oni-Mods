using HarmonyLib;
using KSerialization;
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


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//====[ CHEMICAL: SULFURIC ACID SYNTHESIZER CONFIG ]===================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_SynthesizerSulfuricConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_SynthesizerSulfuric";

		private static readonly PortDisplayInput oxygenInputPort = new(ConduitType.Gas, new CellOffset(0, 2), null, UIUtils.rgb(105, 219, 249));

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
			SoundUtils.CopySoundsToAnim("mixer_sulfuric_kanim", "waterpurifier_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			//storage.capacityKg = 300f;
			go.AddOrGet<ElementConversionBuilding>();
			Prioritizable.AddRef(go);

			ConduitConsumer steamInput = go.AddOrGet<ConduitConsumer>();
			steamInput.conduitType = ConduitType.Gas;
			steamInput.consumptionRate = 10f;
			steamInput.capacityKG = 20f;
			steamInput.capacityTag = SimHashes.Steam.CreateTag();
			steamInput.forceAlwaysSatisfied = true;
			steamInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;


			PortConduitConsumer oxygenInput = go.AddComponent<PortConduitConsumer>();
			oxygenInput.conduitType = ConduitType.Gas;
			oxygenInput.consumptionRate = 10f;
			oxygenInput.capacityKG = 20f;
			oxygenInput.capacityTag = SimHashes.Oxygen.CreateTag();
			oxygenInput.forceAlwaysSatisfied = true;
			oxygenInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			oxygenInput.AssignPort(oxygenInputPort);

			ManualDeliveryKG sulfur_delivery = go.AddComponent<ManualDeliveryKG>();
			sulfur_delivery.SetStorage(storage);
			sulfur_delivery.RequestedItemTag = SimHashes.Sulfur.CreateTag();
			sulfur_delivery.capacity = 200f;
			sulfur_delivery.refillMass = 50f;
			sulfur_delivery.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
			sulfur_delivery.operationalRequirement = Operational.State.None;

			ManualDeliveryKG pyrite_delivery = go.AddComponent<ManualDeliveryKG>();
			pyrite_delivery.SetStorage(storage);
			pyrite_delivery.RequestedItemTag = SimHashes.FoolsGold.CreateTag();
			pyrite_delivery.capacity = 400f;
			pyrite_delivery.refillMass = 100f;
			pyrite_delivery.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
			pyrite_delivery.operationalRequirement = Operational.State.None;

			//-----[ Element Converter Section ]---------------------------------

			///320g/s sulfur + 180g/s water + 480g/s oxygen = 980g/s sulfuric acid
			ElementConverter sulphur_converter = go.AddComponent<ElementConverter>();
			sulphur_converter.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.Steam.CreateTag(), 0.180f),
				new ElementConverter.ConsumedElement(SimHashes.Sulfur.CreateTag(), 0.320f),
				new ElementConverter.ConsumedElement(SimHashes.Oxygen.CreateTag(), 0.480f)];
			sulphur_converter.outputElements = [
				new ElementConverter.OutputElement(0.980f, ModElements.SulphuricAcid_Liquid, 345.15f, false, true)
				];

			///600g pyrite + 600g oxygen + 180g water = 980g acid + 400g rust (roughly)
			ElementConverter pyrite_converter = go.AddComponent<ElementConverter>();
			pyrite_converter.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.Steam.CreateTag(), 0.180f),
				new ElementConverter.ConsumedElement(SimHashes.FoolsGold.CreateTag(), 0.6f),
				new ElementConverter.ConsumedElement(SimHashes.Oxygen.CreateTag(), 0.600f)];
			pyrite_converter.outputElements = [
				new ElementConverter.OutputElement(0.980f, ModElements.SulphuricAcid_Liquid, 345.15f, false, true),
				new ElementConverter.OutputElement(0.400f, SimHashes.Rust, 345.15f, true, true),
				];


			var selector = go.AddOrGet<SulphuricAcidRecipeSelector>();
			selector.sulphurConverter = pyrite_converter;
			selector.pyriteConverter = pyrite_converter;

			selector.sulphurDelivery = sulfur_delivery;
			selector.pyriteDelivery = pyrite_delivery;
			//-------------------------------------------------------------------

			ElementDropper toxicDropper = go.AddComponent<ElementDropper>();
			toxicDropper.emitMass = 20f;
			toxicDropper.emitTag = SimHashes.Rust.CreateTag();
			toxicDropper.emitOffset = new Vector3(0f, 1f, 0f);

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.alwaysDispense = true;
			dispenser.storage = storage;
			dispenser.elementFilter = [ModElements.SulphuricAcid_Liquid];

			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, oxygenInputPort);
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
			this.AttachPort(go);
		}
	}
}
