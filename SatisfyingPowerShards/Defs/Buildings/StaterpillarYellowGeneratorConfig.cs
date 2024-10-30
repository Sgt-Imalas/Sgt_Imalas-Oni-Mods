using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace SatisfyingPowerShards.Defs.Buildings
{
	internal class StaterpillarYellowGeneratorConfig : IBuildingConfig
	{
		public static readonly string ID = "StaterpillarYellowGenerator";

		public const int WIDTH = 1;

		public const int HEIGHT = 2;

		public override string[] GetDlcIds()
		{
			return DlcManager.AVAILABLE_EXPANSION1_ONLY;
		}

		public override BuildingDef CreateBuildingDef()
		{
			string iD = ID;
			string[] aLL_METALS = MATERIALS.ALL_METALS;
			BuildingDef obj = BuildingTemplates.CreateBuildingDef(construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, construction_materials: aLL_METALS, melting_point: 9999f, build_location_rule: BuildLocationRule.OnFoundationRotatable, noise: NOISE_POLLUTION.NOISY.TIER0, id: iD, width: 1, height: 2, anim: "egg_caterpillar_yellow_kanim", hitpoints: 1000, construction_time: 10f, decor: BUILDINGS.DECOR.NONE);
			obj.GeneratorWattageRating = 3200f;
			obj.GeneratorBaseCapacity = 10000f;
			obj.ExhaustKilowattsWhenActive = 4f;
			obj.SelfHeatKilowattsWhenActive = 8f;
			obj.Overheatable = false;
			obj.Floodable = false;
			obj.OverheatTemperature = 423.15f;
			obj.PermittedRotations = PermittedRotations.FlipV;
			obj.ViewMode = OverlayModes.Power.ID;
			obj.AudioCategory = "Plastic";
			obj.RequiresPowerOutput = true;
			obj.PowerOutputOffset = new CellOffset(0, 1);
			obj.PlayConstructionSounds = false;
			obj.ShowInBuildMenu = false;
			return obj;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<StaterpillarGenerator>().powerDistributionOrder = 9;
			go.GetComponent<Deconstructable>().SetAllowDeconstruction(allow: false);
			go.AddOrGet<Modifiers>();
			go.AddOrGet<Effects>();
			go.GetComponent<KSelectable>().IsSelectable = false;
		}
	}
}

