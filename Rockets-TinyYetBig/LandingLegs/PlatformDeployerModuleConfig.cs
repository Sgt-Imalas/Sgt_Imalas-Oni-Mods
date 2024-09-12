using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.LandingLegs
{
	public class PlatformDeployerModuleConfig : IBuildingConfig
	{
		public const string ID = "RTB_LandingPlatformDeployerModule";

		public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

		public override BuildingDef CreateBuildingDef()
		{

			float[] constructionMass = new float[] { 300f };
			string[] constructioMaterials = MATERIALS.REFINED_METALS;
			EffectorValues noiseval = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues decorval = BUILDINGS.DECOR.NONE;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 5,
				height: 5,
				anim: "rocket_storage_live_kanim",
				hitpoints: 1000,
				construction_time: 60f,
				construction_mass: constructionMass,
				construction_materials: constructioMaterials,
				melting_point: 9999f,
				BuildLocationRule.Anywhere,
				decor: decorval,
				noise: noiseval);

			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.AttachmentSlotTag = GameTags.Rocket;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.attachablePosition = new CellOffset(0, 0);
			buildingDef.RequiresPowerInput = false;
			buildingDef.RequiresPowerOutput = false;
			buildingDef.CanMove = true;
			buildingDef.Cancellable = false;
			buildingDef.ShowInBuildMenu = false;


			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage storage = go.AddComponent<Storage>();
			storage.showInUI = true;
			storage.useWideOffsets = true;
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			BuildingInternalConstructor.Def def1 = go.AddOrGetDef<BuildingInternalConstructor.Def>();
			def1.constructionMass = 750f;
			def1.outputIDs = new List<string>() { PlatformLanderConfig.ID };
			def1.spawnIntoStorage = true;
			def1.storage = (DefComponent<Storage>)storage;
			def1.constructionSymbol = "under_construction";
			go.AddOrGet<BuildingInternalConstructorWorkable>().SetWorkTime(120f);
			JettisonableCargoModule.Def def2 = go.AddOrGetDef<JettisonableCargoModule.Def>();
			def2.landerPrefabID = PlatformLanderConfig.ID.ToTag();
			def2.landerContainer = (DefComponent<Storage>)storage;
			def2.clusterMapFXPrefabID = "DeployingPioneerLanderFX";

			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
			{
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket,  null)
			};
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MODERATE_PLUS);
			go.GetComponent<KPrefabID>().prefabInitFn += inst => { };
		}
	}
}

