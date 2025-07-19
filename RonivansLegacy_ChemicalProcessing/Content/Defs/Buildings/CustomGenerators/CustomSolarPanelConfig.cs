using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomGenerators
{
    class CustomSolarPanelConfig : IBuildingConfig
	{
		public static string ID = "CustomSolarPanel";

		public const float MAX_WATTS = 80f; // 380 / 14 * 3 <- vanilla panel has 14 solar cells, this panel has 3, rounded

		public override BuildingDef CreateBuildingDef()
		{
			float[] cost = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
			string[] glasses = TUNING.MATERIALS.GLASSES;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 5, "custom_solar_panel_kanim", 20, 120f, cost, glasses, 2400f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NONE);
			buildingDef.GeneratorWattageRating = MAX_WATTS;
			buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
			buildingDef.ExhaustKilowattsWhenActive = 0.0f;
			buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
			buildingDef.BuildLocationRule = BuildLocationRule.Anywhere;
			buildingDef.HitPoints = 10;
			buildingDef.RequiresPowerOutput = true;
			buildingDef.PowerOutputOffset = new CellOffset(0, 0);
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.AudioSize = "large";
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.POWER);
			SoundUtils.CopySoundsToAnim("custom_solar_panel_kanim", "solar_panel_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.PowerBuilding);
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.GeneratorType);
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.HeavyDutyGeneratorType);
			go.AddOrGet<LoopingSounds>();
			Prioritizable.AddRef(go);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<Repairable>().expectedRepairTime = 10f;
			var panel = go.AddOrGet<RotatableSmallSolarPanel>();
			panel.powerDistributionOrder = 9;
			panel.MaxWattage = MAX_WATTS;
			go.AddOrGetDef<PoweredActiveController.Def>();
			MakeBaseSolid.Def def = go.AddOrGetDef<MakeBaseSolid.Def>();
			def.occupyFoundationLayer = false;
			def.solidOffsets = [new(0, 0)];

			go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		}
	}
}
