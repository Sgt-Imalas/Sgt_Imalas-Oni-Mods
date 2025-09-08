using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.BuildingConfigInterfaces;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications.HighCapacityLogisticRails
{
	class HPATransferArmConfig : IBuildingConfig, IHasConfigurableWattage, IHasConfigurableRange
	{
		public static float Wattage = HighPressureConduitRegistration.GetConduitMultiplier(ConduitType.Solid) * 0.8f * 120f;//default to 960;
		
		public float GetWattage() => Wattage;
		public void SetWattage(float mass) => Wattage = mass;

		public static float TurnRatePerSecond = 3 * 360;		

		public static float StorageCapacity = HighPressureConduitRegistration.GetConduitMultiplier(ConduitType.Solid) * 1000f;

		public static int SweeperArmRange = 12;
		public int GetTileRange() => SweeperArmRange;

		public void SetTileRange(int tiles) => SweeperArmRange = tiles;

		public string GetDescriptorText() => Strings.Get("STRINGS.UI.BUILDINGEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT.RANGESETTINGS_SWEEPER");

		public Tuple<int, int> GetTileValueRange() => new(6,24);


		public static string ID = "HPA_TransferArm";

		public override BuildingDef CreateBuildingDef()
		{
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 1, "hpa_transferarm_kanim", 10, 10f, [400,100], [GameTags.Steel.ToString(), GameTags.Plastic.ToString()], 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
			buildingDef.Floodable = false;
			buildingDef.AudioCategory = "Metal";
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = Wattage;
			buildingDef.ExhaustKilowattsWhenActive = 0f;
			buildingDef.SelfHeatKilowattsWhenActive = 0f;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			buildingDef.PermittedRotations = PermittedRotations.R360;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			SoundUtils.CopySoundsToAnim("hpa_transferarm_kanim", "conveyor_transferarm_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<Operational>();
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			AddVisualizer(go, true);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			AddVisualizer(go, false);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			var capacity = go.AddOrGet<VariableCapacityForTransferArm>();
			capacity.TargetCarryCapacity = StorageCapacity;
			capacity.TargetTurnRate = TurnRatePerSecond;
			var arm = go.AddOrGet<SolidTransferArm>();
			arm.pickupRange = GetTileRange();
			AddVisualizer(go, false);
		}

		private static void AddVisualizer(GameObject prefab, bool movable)
		{
			int range = SweeperArmRange;
			RangeVisualizer rangeVisualizer = prefab.AddOrGet<RangeVisualizer>();
			rangeVisualizer.OriginOffset = new Vector2I(0, 0);
			rangeVisualizer.RangeMin.x = -range;
			rangeVisualizer.RangeMin.y = -range;
			rangeVisualizer.RangeMax.x = range;
			rangeVisualizer.RangeMax.y = range;
			rangeVisualizer.BlockingTileVisible = true;
		}

	}
}
