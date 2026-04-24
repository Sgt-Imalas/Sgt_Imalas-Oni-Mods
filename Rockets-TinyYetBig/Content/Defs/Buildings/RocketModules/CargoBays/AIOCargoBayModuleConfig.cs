using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.RocketModules.CargoBays
{
	internal class AIOCargoBayModuleConfig : IBuildingConfig
	{
		public const string ID = "RTB_AIOCargoBayCluster";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public override BuildingDef CreateBuildingDef()
		{
			float[] cargoMass = [250];
			string[] construction_materials = [MATERIALS.REFINED_METAL];

			EffectorValues tieR2 = NOISE_POLLUTION.NONE;
			EffectorValues none = BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 2, "coal_generator_module_kanim", 1000, 60f, cargoMass, construction_materials, 9999f, BuildLocationRule.Anywhere, none, noise);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.AttachmentSlotTag = GameTags.Rocket;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.RequiresPowerInput = false;
			buildingDef.attachablePosition = new CellOffset(0, 0);
			buildingDef.CanMove = true;
			buildingDef.Cancellable = false;
			buildingDef.ShowInBuildMenu = false;
			//CustomCargoBayDB.AddCargoBayLogicPorts(buildingDef);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
			{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.Rocket, (AttachableBuilding) null)
			};
		}


		public override void DoPostConfigureComplete(GameObject go)
		{
			var combinedStorage = go.AddComponent<Storage>();
			combinedStorage.capacityKg = Config.Instance.SolidCargoBayKgPerUnit + Config.Instance.LiquidCargoBayKgPerUnit + Config.Instance.GasCargoBayKgPerUnit;
			combinedStorage.storageFilters = STORAGEFILTERS.STORAGE_SOLID_CARGO_BAY.Concat(STORAGEFILTERS.LIQUIDS).Concat(STORAGEFILTERS.GASES).ToList();
			combinedStorage.SetDefaultStoredItemModifiers(Config.Instance.InsulatedCargoBays ? Storage.StandardInsulatedStorage : Storage.StandardSealedStorage);

			TreeFilterable treeFilterable = go.AddOrGet<TreeFilterable>();
			treeFilterable.dropIncorrectOnFilterChange = false;
			treeFilterable.autoSelectStoredOnLoad = false;
			treeFilterable.uiHeight = TreeFilterable.UISideScreenHeight.Short;

			var bay = go.AddOrGet<MultiMaterialCargoBay>();

			bay.Configure(Config.Instance.SolidCargoBayKgPerUnit, Config.Instance.LiquidCargoBayKgPerUnit, Config.Instance.GasCargoBayKgPerUnit);

			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MINOR);
		}
	}
}
