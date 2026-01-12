using RoboRockets.Scripts;
using TUNING;
using UnityEngine;
namespace RoboRockets.LearningBrain
{
	class AIControlModuleLearningV2Config : IBuildingConfig
	{
		public const string ID = "RR_AILearningControlModuleV2";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;
		public override BuildingDef CreateBuildingDef()
		{
			float[] matCosts = { 300f };

			string[] construction_materials = {
				"Steel"
			};
			EffectorValues tieR2 = NOISE_POLLUTION.NONE;
			EffectorValues none = BUILDINGS.DECOR.BONUS.TIER1;
			EffectorValues noise = tieR2;

			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 2, "rocket_habitat_ai_module_v2_kanim", 1000, 400f, matCosts, construction_materials, 9999f, BuildLocationRule.Anywhere, none, noise);
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
			aiConfig.variableSpeed = true;
			go.AddOrGet<ClustercraftExteriorDoor>().interiorTemplateName = "interiors/AIRocketV2";
			go.AddOrGet<NavTeleporter>();
			go.AddOrGet<LaunchableRocketCluster>();
			go.AddOrGet<RocketAiConditions>();

			go.AddOrGet<RocketProcessConditionDisplayTarget>();
			go.AddOrGet<RocketLaunchConditionVisualizer>();
			go.AddOrGet<CharacterOverlay>().shouldShowName = true;


			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1] //top module attaches here
            {
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.Rocket,  null)
			};


		}
		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			go.AddOrGet<BuildingCellVisualizer>();
		}
		public override void DoPostConfigureComplete(GameObject go)
		{

			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MINOR_PLUS);

			ModAssets.RemoveCountCondition(go);

			Storage storage = go.AddOrGet<Storage>();
			storage.capacityKg = 1f;
			storage.showInUI = false;
			storage.useWideOffsets = true;
			ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
			manualDeliveryKg.SetStorage(storage);
			manualDeliveryKg.RequestedItemTag = ModAssets.Tags.SpaceBrain;
			manualDeliveryKg.capacity = storage.capacityKg;
			manualDeliveryKg.refillMass = storage.capacityKg;
			manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

			go.AddOrGet<BrainTeacher>().BrainStorage = storage;

			Ownable ownable = go.AddOrGet<Ownable>();
			ownable.slotID = Db.Get().AssignableSlots.HabitatModule.Id;
			ownable.canBePublic = false;
			go.AddOrGet<BuildingCellVisualizer>(); 
			if (go.TryGetComponent<ReorderableBuilding>(out var rb))
			{
				rb.buildConditions.Add(new LimitOneCommandModule());
				rb.buildConditions.Add(new LimitOneAiCommandModule());
				rb.buildConditions.Add(new LimitOneRoboPilotModule());
			}
		}


		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.AddOrGet<BuildingCellVisualizer>();
			go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
		}
	}

}
