using TUNING;
using UnityEngine;

namespace RoboRockets
{
	class AINoseconeConfig : IBuildingConfig
	{
		public const string ID = "RR_AINosecone";

		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;
		public override BuildingDef CreateBuildingDef()
		{
			float[] mass = new float[] {
				350f,
				250f,
				200f
			}; ;
			string[] construction_materials_ = new string[]
			{
				"Steel",
				"Glass",
				"Insulator"
			};
			EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
			EffectorValues none = BUILDINGS.DECOR.NONE;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 5,
				height: 2,
				anim: "rocket_command_module_remote_kanim",
				hitpoints: 1000,
				construction_time: 70f,
				construction_mass: mass,
				construction_materials: construction_materials_,
				melting_point: 9999f,
				BuildLocationRule.Anywhere,
				decor: none,
				noise: noiseLevel);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.AttachmentSlotTag = GameTags.Rocket;
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
			buildingDef.RequiresPowerInput = false;
			buildingDef.attachablePosition = new CellOffset(0, 0);
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
			go.GetComponent<KPrefabID>().AddTag(GameTags.LaunchButtonRocketModule);
			go.GetComponent<KPrefabID>().AddTag(GameTags.NoseRocketModule);

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
			Ownable ownable = go.AddOrGet<Ownable>();
			ownable.slotID = Db.Get().AssignableSlots.HabitatModule.Id;
			ownable.canBePublic = false;

			go.AddOrGet<BuildingCellVisualizer>();

			go.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition)new TopOnly());
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

