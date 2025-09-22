using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.BuildingConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications
{
	public class PressureGasPumpConfig : IBuildingConfig, IHasConfigurableRange
	{
		public static string ID = "PressureGasPump";

		public static int PumpRange = 12;
		public int GetTileRange() => PumpRange;

		public void SetTileRange(int tiles) => PumpRange = tiles;

		public string GetDescriptorText() => Strings.Get("STRINGS.UI.BUILDINGEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT.RANGESETTINGS_PUMP");

		public Tuple<int, int> GetTileValueRange() => new(4, 24);
		public override BuildingDef CreateBuildingDef()
		{
			float[] quantity1 = [400f, 200f];
			string[] materials1 = [GameTags.Steel.ToString(), GameTags.Plastic.ToString()];
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER2;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "pressure_gas_pump_kanim", 30, 30f, quantity1, materials1, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, noise, 0.2f);
			def1.RequiresPowerInput = true;
			def1.Overheatable = false;
			def1.EnergyConsumptionWhenActive = Config.Instance.HPA_Pump_Base_Mult_Gas * HighPressureConduitRegistration.GetConduitMultiplier(ConduitType.Gas);
			def1.ExhaustKilowattsWhenActive = 0f;
			def1.SelfHeatKilowattsWhenActive = 0f;
			def1.OutputConduitType = ConduitType.Gas;
			def1.Floodable = true;
			def1.ViewMode = OverlayModes.GasConduits.ID;
			def1.AudioCategory = "Metal";
			def1.PowerInputOffset = new CellOffset(0, 1);
			def1.UtilityOutputOffset = new CellOffset(0, 2);
			def1.PermittedRotations = PermittedRotations.R90;
			def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
			SoundUtils.CopySoundsToAnim("pressure_gas_pump_kanim", "pumpgas_kanim");
			return def1;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<EnergyConsumer>();
			go.AddOrGet<Storage>().capacityKg = HighPressureConduitRegistration.GasCap_HP * 2;


			var pumpOffset = new CellOffset(0, 2);
			go.AddOrGet<RotatablePump>().PumpOffset = pumpOffset;

			ElementConsumer pumpConsumer = go.AddOrGet<ElementConsumer>();
			pumpConsumer.configuration = ElementConsumer.Configuration.AllGas;
			pumpConsumer.consumptionRate = HighPressureConduitRegistration.GasCap_HP;
			pumpConsumer.storeOnConsume = true;
			pumpConsumer.showInStatusPanel = false;
			pumpConsumer.consumptionRadius = (byte)GetTileRange();
			pumpConsumer.sampleCellOffset = pumpOffset.ToVector3();

			ConduitDispenser local2 = go.AddOrGet<ConduitDispenser>();
			local2.conduitType = ConduitType.Gas;
			local2.alwaysDispense = true;
			local2.elementFilter = null;


			go.AddOrGetDef<OperationalController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
			go.AddOrGet<HPA_ConduitRequirement>().RequiresHighPressureOutput = true;
			UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireOutputs>());//handled by HighPressureOutput
		}
	}
}
