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
using UtilLibs.BuildingPortUtils;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//====[ CHEMICAL: NITRIC ACID SYNTHESIZER CONFIG ]===================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_SynthesizerNitricConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_SynthesizerNitric";


		//--[ Identification and DLC stuff ]-----------------------------------
		public static readonly List<Storage.StoredItemModifier> StoredItemModifiers;

		//--[ Special Settings ]-----------------------------------------------

		private static readonly PortDisplayInput sulfuricAcidInputPort = new(ConduitType.Liquid, new CellOffset(0, 3));
		private static readonly PortDisplayOutput SteamGasOutputPort = new(ConduitType.Gas, new CellOffset(0, 0));


		static Chemical_SynthesizerNitricConfig()
		{
			Color? sulfuricPortColor = new Color32(252, 252, 3, 255);
			sulfuricAcidInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(0, 3), null, sulfuricPortColor);

			Color? SteamOutputPortColor = new Color32(167, 180, 201, 255);
			SteamGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 0), null, SteamOutputPortColor);

			List<Storage.StoredItemModifier> list1 =
			[
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Seal,
				Storage.StoredItemModifier.Insulate,
			];
			StoredItemModifiers = list1;
		}

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

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [
				new(ModElements.Ammonia_Gas.Tag, 0.6f),
				new( ModElements.SulphuricAcid_Liquid.Tag, 0.5f) ];
			converter.outputElements = [
				new(0.5f, ModElements.NitricAcid_Liquid, 345.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new(0.3f, SimHashes.Sulfur, 320.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new(0.2f, SimHashes.Steam, 392.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0) ];
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

			ElementDropper sulfurDropper = go.AddComponent<ElementDropper>();
			sulfurDropper.emitMass = 30f;
			sulfurDropper.emitTag = SimHashes.Sulfur.CreateTag();
			sulfurDropper.emitOffset = new Vector3(0f, 1f, 0f);

			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, sulfuricAcidInputPort);
			controller.AssignPort(go, SteamGasOutputPort);
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
