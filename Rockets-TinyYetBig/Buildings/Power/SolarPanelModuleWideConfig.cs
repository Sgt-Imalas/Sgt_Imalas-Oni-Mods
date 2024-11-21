using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Generators
{
	internal class SolarPanelModuleWideConfig : IBuildingConfig
	{
		public const string ID = "RTB_SolarPanelModule_Wide";

		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public override BuildingDef CreateBuildingDef()
		{
			float[] hollowTieR1 = BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER1;
			string[] glasses = MATERIALS.GLASSES;
			EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues none = BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 1, "rocket_solar_panel_module_wide_kanim", 1000, 30f, hollowTieR1, glasses, 9999f, BuildLocationRule.Anywhere, none, noise);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.DefaultAnimState = "grounded";
			buildingDef.AttachmentSlotTag = GameTags.Rocket;
			buildingDef.GeneratorWattageRating = SolarPanelModuleConfig.MAX_WATTS;
			buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
			buildingDef.ExhaustKilowattsWhenActive = 0.0f;
			buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.PowerInputOffset = SolarPanelModuleConfig.PLUG_OFFSET;
			buildingDef.PowerOutputOffset = SolarPanelModuleConfig.PLUG_OFFSET;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.RequiresPowerOutput = true;
			buildingDef.UseWhitePowerOutputConnectorColour = true;
			buildingDef.CanMove = true;
			buildingDef.Cancellable = false;
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddComponent<RequireInputs>();
			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
			{
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 1), GameTags.Rocket, (AttachableBuilding) null)
			};
			go.AddComponent<PartialLightBlocking>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			Prioritizable.AddRef(go);
			go.AddOrGet<ModuleSolarPanel>().showConnectedConsumerStatusItems = false;
			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.INSIGNIFICANT);
			go.GetComponent<RocketModule>().operationalLandedRequired = false;
		}
	}
}
