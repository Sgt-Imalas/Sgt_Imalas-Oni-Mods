using HarmonyLib;
using KSerialization;
using PeterHan.PLib.UI;
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
	//====[ CHEMICAL: NITRIC ACID SYNTHESIZER CONFIG ]===================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_SynthesizerNitricConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_SynthesizerNitric";


		//--[ Special Settings ]-----------------------------------------------

		private static readonly PortDisplayInput sulfuricAcidInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(0, 3), null, new Color32(252, 252, 3, 255));

		private static readonly PortDisplayInput oxygenInputPort = new(ConduitType.Gas, new CellOffset(0, 2),null,UIUtils.rgb(183, 255, 255));

		private static readonly PortDisplayOutput SteamGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 0), null, new Color32(167, 180, 201, 255));


		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [100f, 200f];
			string[] ingredient_types = [SimHashes.Ceramic.ToString(), TUNING.MATERIALS.REFINED_METAL];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 4, "mixer_nitric_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 240f;
			buildingDef.ExhaustKilowattsWhenActive = 8f;
			buildingDef.SelfHeatKilowattsWhenActive = 4f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(0, 3);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			SoundUtils.CopySoundsToAnim("mixer_nitric_kanim", "waterpurifier_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			//storage.capacityKg = 300f;
			//storage.showCapacityStatusItem = true;
			//storage.showCapacityAsMainStatus = true;
			//storage.showDescriptor = true;
			go.AddOrGet<ElementConversionBuilding>();
			Prioritizable.AddRef(go);

			ConduitConsumer ammoniaInput = go.AddOrGet<ConduitConsumer>();
			ammoniaInput.conduitType = ConduitType.Gas;
			ammoniaInput.consumptionRate = 10f;
			ammoniaInput.capacityKG = 50f;
			ammoniaInput.capacityTag = ModElements.Ammonia_Gas.Tag;
			ammoniaInput.forceAlwaysSatisfied = true;
			ammoniaInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer sulfuricInput = go.AddComponent<PortConduitConsumer>();
			sulfuricInput.conduitType = ConduitType.Liquid;
			sulfuricInput.consumptionRate = 10f;
			sulfuricInput.capacityKG = 50f;
			sulfuricInput.capacityTag = ModElements.SulphuricAcid_Liquid.Tag;
			sulfuricInput.forceAlwaysSatisfied = true;
			sulfuricInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			sulfuricInput.AssignPort(sulfuricAcidInputPort);

			PortConduitConsumer oxygenInput = go.AddComponent<PortConduitConsumer>();
			oxygenInput.conduitType = ConduitType.Gas;
			oxygenInput.consumptionRate = 10f;
			oxygenInput.capacityKG = 50f;
			oxygenInput.capacityTag = SimHashes.Oxygen.CreateTag();
			oxygenInput.forceAlwaysSatisfied = true;
			oxygenInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			oxygenInput.AssignPort(oxygenInputPort);

			ManualDeliveryKG salt_delivery = go.AddOrGet<ManualDeliveryKG>();
			salt_delivery.SetStorage(storage);
			salt_delivery.RequestedItemTag = SimHashes.Salt.CreateTag();
			salt_delivery.capacity = 800f;
			salt_delivery.refillMass = 200f;
			salt_delivery.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
			salt_delivery.operationalRequirement = Operational.State.None;

			//-----[ Element Converter Section ]---------------------------------
			///old converter
			//ElementConverter converter = go.AddOrGet<ElementConverter>();
			//converter.consumedElements = [
			//	new(ModElements.Ammonia_Gas.Tag, 0.6f),
			//	new( ModElements.SulphuricAcid_Liquid.Tag, 0.5f) ];
			//converter.outputElements = [
			//	new(0.5f, ModElements.NitricAcid_Liquid, 345.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
			//	new(0.3f, SimHashes.Sulfur, 320.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
			//	new(0.2f, SimHashes.Steam, 392.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0) ];


			/// New converters based on different chemical reactions to produce nitric acid:

			///NH3 + 2O2 → H2O + HNO3
			///17g ammonia + 64g oxygen = 18g water + 63g nitric acid
			
			ElementConverter ammoniaConverter = go.AddComponent<ElementConverter>();
			ammoniaConverter.consumedElements = [
				new(SimHashes.Oxygen.CreateTag(), 0.640f),
				new( ModElements.Ammonia_Gas.Tag, 0.170f)];
			ammoniaConverter.outputElements = [
				new(0.630f, ModElements.NitricAcid_Liquid, UtilMethods.GetKelvinFromC(68), false, true),
				new(0.180f, SimHashes.Steam, UtilMethods.GetKelvinFromC(130), false, true)];

			/// 2NaNO3 + H2SO4 → Na2SO4 + 2HNO3
			/// 170g salt + 98g sulfuric acid → 142g sand + 126g nitric acid
			/// rates multiplied by 5 to match ammonia reaction output amount; 630 / 126 = 5

			ElementConverter sulphuricConverter = go.AddComponent<ElementConverter>();
			sulphuricConverter.consumedElements = [
				new(SimHashes.Salt.CreateTag(), 0.170f * 5),
				new(ModElements.SulphuricAcid_Liquid.Tag, 0.098f * 5)];
			sulphuricConverter.outputElements = [
				new(0.126f * 5, ModElements.NitricAcid_Liquid, UtilMethods.GetKelvinFromC(68), false, true),
				new(0.142f * 5, SimHashes.Sand, UtilMethods.GetKelvinFromC(45), false, true)];

			var selector = go.AddOrGet<NitricAcidRecipeSelector>();
			selector.acidConverter = sulphuricConverter;
			selector.ammoniaConverter = ammoniaConverter;
			selector.saltDelivery = salt_delivery;

			//-------------------------------------------------------------------

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.alwaysDispense = true;
			dispenser.storage = storage;
			dispenser.elementFilter = [ModElements.NitricAcid_Liquid];

			PipedConduitDispenser steamOutput = go.AddComponent<PipedConduitDispenser>();
			steamOutput.storage = storage;
			steamOutput.conduitType = ConduitType.Gas;
			steamOutput.alwaysDispense = true;
			steamOutput.elementFilter = [SimHashes.Steam];
			steamOutput.AssignPort(SteamGasOutputPort);
						
			ElementDropper sandDropper = go.AddComponent<ElementDropper>();
			sandDropper.emitMass = 30f;
			sandDropper.emitTag = SimHashes.Sand.CreateTag();
			sandDropper.emitOffset = new Vector3(0f, 1f, 0f);

			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, sulfuricAcidInputPort);
			controller.AssignPort(go, SteamGasOutputPort); 
			controller.AssignPort(go, oxygenInputPort); 
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
