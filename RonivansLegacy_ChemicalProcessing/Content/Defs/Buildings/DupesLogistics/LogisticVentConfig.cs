using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesLogistics
{
	public class LogisticVentConfig : IBuildingConfig
	{
		public static string ID = "LogisticVent";

		private const ConduitType CONDUIT_TYPE = ConduitType.Solid;
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<LogicOperationalController>();
		}

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues nONE = NOISE_POLLUTION.NONE;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "logistic_vent_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, nONE, 0.2f);
			def1.InputConduitType = ConduitType.Solid;
			def1.Floodable = false;
			def1.Overheatable = false;
			def1.ViewMode = OverlayModes.SolidConveyor.ID;
			def1.AudioCategory = "Metal";
			def1.UtilityInputOffset = new CellOffset(0, 0);
			def1.UtilityOutputOffset = new CellOffset(0, 0);
			def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			SoundUtils.CopySoundsToAnim("logistic_vent_kanim", "conveyer_dropper_kanim");
			return def1;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<SimpleVent>();
			go.AddOrGet<SolidConduitConsumer>();
			go.AddOrGet<SolidConduitDropper>();
			Storage storage1 = BuildingTemplates.CreateDefaultStorage(go, false);
			storage1.capacityKg = 100f;
			storage1.showInUI = true;
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
		}
	}
}
