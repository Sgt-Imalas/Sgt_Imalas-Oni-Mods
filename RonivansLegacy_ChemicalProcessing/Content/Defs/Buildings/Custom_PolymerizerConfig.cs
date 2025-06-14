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
	//===[ Custom: Polymer Press ]========================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Custom_PolymerizerConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Custom_Polymerizer";

		//--[ Identification and DLC stuff ]-----------------------------------

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayInput chlorineGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(1, 0));

		static Custom_PolymerizerConfig()
		{
			Color? chlorinePortColor = new Color32(202, 247, 0, 255);
			chlorineGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(1, 0), null, chlorinePortColor);
		}

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "plasticrefinery_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier, 0.2f);
			BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "large";
			buildingDef.EnergyConsumptionWhenActive = 240f;
			buildingDef.ExhaustKilowattsWhenActive = 0.5f;
			buildingDef.SelfHeatKilowattsWhenActive = 32f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.OutputConduitType = ConduitType.Gas;
			buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
			buildingDef.PermittedRotations = PermittedRotations.FlipH;
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
			EthanolPolymerizer polymerizer = go.AddOrGet<EthanolPolymerizer>();
			polymerizer.emitMass = 60f;
			polymerizer.emitTag = GameTagExtensions.Create(SimHashes.Polypropylene);
			polymerizer.emitOffset = new Vector3(-1.45f, 1f, 0f);
			//polymerizer.exhaustElement = SimHashes.Steam;

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.consumptionRate = 10f;
			conduitConsumer.capacityKG = 10f;
			conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Ethanol);
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer chlorineGasInput = go.AddComponent<PortConduitConsumer>();
			chlorineGasInput.conduitType = ConduitType.Gas;
			chlorineGasInput.consumptionRate = 10f;
			chlorineGasInput.capacityKG = 10f;
			chlorineGasInput.capacityTag = SimHashes.ChlorineGas.CreateTag();
			chlorineGasInput.forceAlwaysSatisfied = true;
			chlorineGasInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			chlorineGasInput.AssignPort(chlorineGasInputPort);

			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements =
			[
			new ElementConverter.ConsumedElement(GameTagExtensions.Create(SimHashes.Ethanol), 2.5f, true),
			new ElementConverter.ConsumedElement(GameTagExtensions.Create(SimHashes.ChlorineGas), 0.1f, true)
			];
			elementConverter.outputElements =
			[
			new ElementConverter.OutputElement(0.5f, SimHashes.Polypropylene, 348.15f, false, true, 0f, 0.5f, 1f, byte.MaxValue, 0, true),
			new ElementConverter.OutputElement(0.250f, SimHashes.Steam, 473.15f, false, true, 0f, 0.5f, 1f, byte.MaxValue, 0, true) ];

			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Gas;
			conduitDispenser.invertElementFilter = false;
			conduitDispenser.alwaysDispense = true;
			conduitDispenser.elementFilter = [SimHashes.Steam];

			go.AddOrGet<DropAllWorkable>();
			//reusing the cmp to tint the building, not actually deprecated
			go.AddOrGet<DeprecationTint>().Tint = UIUtils.Lighten( ElementLoader.GetElement(SimHashes.Ethanol.CreateTag()).substance.colour,50);

			Prioritizable.AddRef(go);
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, chlorineGasInputPort);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			this.AttachPort(go);
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
