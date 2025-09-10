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
	/// <summary>
	/// the existence of this building was to replace the regular oilwellcap (vanilla building is set to fully hidden and this custom building is inserted in its place)
	/// The mod now instead modifies the regular oil well to be identical to this custom building, and no replaces it as described above
	/// </summary>
	//====[ Custom: Oil Well Cap ]========================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Custom_OilWellCapConfig// : IBuildingConfig
	{
		//--[ Base Information ]--------------------------------------------------------------------------
		public static string ID = "Custom_OilWellCap";

		//--[ Special Settings ]--------------------------------------------------------------------------
		public static readonly PortDisplayOutput GasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(1, 1));
		public static readonly PortDisplayOutput LiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(2, 1));

		//public override BuildingDef CreateBuildingDef()
		//{
		//	EffectorValues noise = NOISE_POLLUTION.NOISY.TIER2;
		//	BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 4, 4, "geyser_oil_cap_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, noise);
		//	BuildingTemplates.CreateElectricalBuildingDef(def);
		//	def.SceneLayer = Grid.SceneLayer.BuildingFront;
		//	def.ViewMode = OverlayModes.LiquidConduits.ID;
		//	def.EnergyConsumptionWhenActive = 240f;
		//	def.SelfHeatKilowattsWhenActive = 2f;
		//	def.InputConduitType = ConduitType.Liquid;
		//	def.UtilityInputOffset = new CellOffset(0, 1);
		//	def.PowerInputOffset = new CellOffset(1, 1);
		//	def.OverheatTemperature = 2273.15f;
		//	def.Floodable = false;
		//	def.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		//	def.AttachmentSlotTag = GameTags.OilWell;
		//	def.BuildLocationRule = BuildLocationRule.BuildingAttachPoint;
		//	def.ObjectLayer = ObjectLayer.AttachableBuilding;
		//	def.Deprecated = true;
		//	return def;
		//}

		//public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		//{
		//	go.AddOrGet<LoopingSounds>();
		//	Storage standardStorage = go.AddOrGet<Storage>();
		//	standardStorage.SetDefaultStoredItemModifiers(OilWellStorageModifier);
		//	standardStorage.capacityKg = 5000f;

		//	BuildingTemplates.CreateDefaultStorage(go, false).showInUI = true;
		//	ConduitConsumer consumer = go.AddOrGet<ConduitConsumer>();
		//	consumer.conduitType = ConduitType.Liquid;
		//	consumer.consumptionRate = 2f;
		//	consumer.capacityKG = 10f;
		//	consumer.capacityTag = GameTags.AnyWater;
		//	consumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

		//	ElementConverter converter = go.AddOrGet<ElementConverter>();
		//	converter.consumedElements = [new ElementConverter.ConsumedElement(GameTags.AnyWater, 1f)];
		//	converter.outputElements = [new ElementConverter.OutputElement(3.4f, SimHashes.CrudeOil, 363.15f, false, true, 2f, 1.5f, 0f, 0xff, 0)];

		//	OilWellCap cap = go.AddOrGet<OilWellCap>();
		//	cap.gasElement = ModElements.RawNaturalGas_Gas;
		//	cap.gasTemperature = 393.15f;
		//	cap.addGasRate = 0.12f;
		//	cap.maxGasPressure = 80.00001f;
		//	cap.releaseGasRate = 0.4444445f;

		//	//===> Methane Output <==============================================================
		//	PipedConduitDispenser GasDispenser = go.AddComponent<PipedConduitDispenser>();
		//	GasDispenser.elementFilter = [ModElements.RawNaturalGas_Gas];
		//	GasDispenser.AssignPort(GasOutputPort);
		//	GasDispenser.alwaysDispense = true;
		//	GasDispenser.SkipSetOperational = true;

		//	PipedOptionalExhaust GasExhaust = go.AddComponent<PipedOptionalExhaust>();
		//	GasExhaust.dispenser = GasDispenser;
		//	GasExhaust.elementTag = ModElements.RawNaturalGas_Gas.Tag;
		//	GasExhaust.capacity = 10f;

		//	//===> Crude Oil Output <============================================================
		//	PipedConduitDispenser LiquidDispenser = go.AddComponent<PipedConduitDispenser>();
		//	LiquidDispenser.elementFilter = [SimHashes.CrudeOil];
		//	LiquidDispenser.AssignPort(LiquidOutputPort);
		//	LiquidDispenser.alwaysDispense = true;
		//	LiquidDispenser.SkipSetOperational = true;

		//	PipedOptionalExhaust LiquidExhaust = go.AddComponent<PipedOptionalExhaust>();
		//	LiquidExhaust.dispenser = LiquidDispenser;
		//	LiquidExhaust.elementTag = SimHashes.CrudeOil.CreateTag();
		//	LiquidExhaust.capacity = 4f;

		//	go.AddOrGet<RequireInputs>().requireConduitHasMass = false;
		//	go.AddOrGet<DeprecationTint>();
		//	AttachPorts(go);
		//}

		public static void AttachPorts(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);

			controller.AssignPort(go, GasOutputPort);
			controller.AssignPort(go, LiquidOutputPort);
		}

		//public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		//{
		//	AttachPorts(go);
		//}

		//public override void DoPostConfigureUnderConstruction(GameObject go)
		//{
		//	AttachPorts(go);
		//}

		//public override void DoPostConfigureComplete(GameObject go)
		//{
		//	go.AddOrGet<LogicOperationalController>();
		//}
	}
}
