using TUNING;
using UnityEngine;
namespace RoboRockets
{
	class AIControlModuleConfig : IBuildingConfig
	{
		public const string ID = "RR_AIControlModule";



		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;
		public override BuildingDef CreateBuildingDef()
		{
			float[] matCosts = { 300f, 1 };

			string[] construction_materials = new string[]
				{
					"RefinedMetal"
					,GeneShufflerRechargeConfig.tag.ToString()
				};
			EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues none = BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR2;

			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "rocket_habitat_ai_module_kanim", 1000, 400f, matCosts, construction_materials, 9999f, BuildLocationRule.Anywhere, none, noise);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.AttachmentSlotTag = GameTags.Rocket;
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.CanMove = true;
			buildingDef.Cancellable = false;

			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.GetComponent<KPrefabID>().AddTag(GameTags.LaunchButtonRocketModule);

			go.AddOrGet<AssignmentGroupController>().generateGroupOnStart = true;
			var aiConfig = go.AddOrGet<AIPassengerModule>();
			aiConfig.interiorReverbSnapshot = AudioMixerSnapshots.Get().SmallRocketInteriorReverbSnapshot;
			go.AddOrGet<ClustercraftExteriorDoor>().interiorTemplateName = "interiors/AIRocketV2";
			go.AddOrGet<NavTeleporter>();
			go.AddOrGet<LaunchableRocketCluster>();
			go.AddOrGet<RocketAiConditions>();

			go.AddOrGet<RocketProcessConditionDisplayTarget>();
			go.AddOrGet<RocketLaunchConditionVisualizer>();
			go.AddOrGet<CharacterOverlay>().shouldShowName = true;


			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1] //top module attaches here
            {
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 3), GameTags.Rocket, (AttachableBuilding) null)
			};
		}
		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			go.AddOrGet<BuildingCellVisualizer>();
		}
		public override void DoPostConfigureComplete(GameObject go)
		{

			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MINOR_PLUS);

			ModAssets.RemoveCountCondition(go);
			go.AddOrGet<BuildingCellVisualizer>();
			go.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition)new LimitOneCommandModule());
		}


		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.AddOrGet<BuildingCellVisualizer>();
			go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
		}
	}

}
