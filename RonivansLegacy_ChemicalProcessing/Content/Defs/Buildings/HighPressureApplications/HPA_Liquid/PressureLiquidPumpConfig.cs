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
	public class PressureLiquidPumpConfig : IBuildingConfig, IHasConfigurableRange
	{
		public static string ID = "PressureLiquidPump";
		public static int PumpRange = 8;
		public int GetTileRange() => PumpRange;

		public void SetTileRange(int tiles) => PumpRange = tiles;

		public string GetDescriptorText() => STRINGS.UI.BUILDINGEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT.RANGESETTINGS_PUMP;
		public Tuple<int, int> GetTileValueRange() => new(4, 24);

		public override BuildingDef CreateBuildingDef()
		{
			float[] quantity1 = [400f, 200f];
			string[] materials1 = [GameTags.Steel.ToString(),GameTags.Plastic.ToString()];
			EffectorValues nONE = NOISE_POLLUTION.NONE;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "pressure_liquid_pump_kanim", 240, 120f, quantity1, materials1, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, nONE, 0.2f);
			def1.RequiresPowerInput = true;
			def1.Overheatable = false;
			def1.EnergyConsumptionWhenActive = Config.Instance.HPA_Pump_Base_Mult_Liquid * HighPressureConduitRegistration.GetConduitMultiplier(ConduitType.Liquid);
			def1.ExhaustKilowattsWhenActive = 0f;
			def1.SelfHeatKilowattsWhenActive = 2f;
			def1.OutputConduitType = ConduitType.Liquid;
			def1.Floodable = false;
			def1.ViewMode = OverlayModes.LiquidConduits.ID;
			def1.AudioCategory = "Metal";
			def1.PowerInputOffset = new CellOffset(0, 1);
			def1.UtilityOutputOffset = new CellOffset(1, 2);
			def1.PermittedRotations = PermittedRotations.R90;
			def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
			SoundUtils.CopySoundsToAnim("pressure_liquid_pump_kanim", "pumpliquid_kanim");
			return def1;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<EnergyConsumer>();
			go.AddOrGet<Pump>();
			go.AddOrGet<Storage>().capacityKg = Config.Instance.HPA_Capacity_Liquid*2;
			ElementConsumer pumpConsumer = go.AddOrGet<ElementConsumer>();
			pumpConsumer.configuration = ElementConsumer.Configuration.AllLiquid;
			pumpConsumer.consumptionRate = Config.Instance.HPA_Capacity_Liquid;
			pumpConsumer.storeOnConsume = true;
			pumpConsumer.showInStatusPanel = false;
			pumpConsumer.consumptionRadius = (byte)GetTileRange();
			go.AddOrGet<RotatablePump>().PumpOffset = new CellOffset(1, 0);

			ConduitDispenser local2 = go.AddOrGet<ConduitDispenser>();
			local2.conduitType = ConduitType.Liquid;
			local2.alwaysDispense = true;
			local2.elementFilter = null;

			go.AddOrGetDef<OperationalController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
			go.AddOrGet<HPA_ConduitRequirement>().RequiresHighPressureOutput = true;
			UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireOutputs>());//handled by HighPressureOutput
			go.AddOrGet<ContentTintable>();
		}
	}
}
