﻿using TUNING;
using UnityEngine;

namespace Radiator_Mod
{
	class HabitatMediumRadiator : IBuildingConfig
	{
		public static string ID = "HabitatModuleMediumRadiator";
		public static string NAME = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULEMEDIUM.NAME + " with 2 Space Radiators.";
		public static string DESC = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULEMEDIUM.DESC + "\n\nComes preinstalled with 2 Radiators.";
		public static string EFFECT = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULEMEDIUM.EFFECT + "\n\nHas 2 Space Radiators build in the walls.";

		private ConduitPortInfo gasInputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(-2, 0));
		private ConduitPortInfo gasOutputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(2, 0));
		private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(-2, 3));
		private ConduitPortInfo liquidOutputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(2, 3));


		public override string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];

		public override string[] GetForbiddenDlcIds() => null;
		public override BuildingDef CreateBuildingDef()
		{
			float[] constructionMass = {
				BUILDINGS.ROCKETRY_MASS_KG.DENSE_TIER1[0],
				1600f
			};
			string[] materials = {
				"Metal",
				"Steel"
			};
			EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues none = BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 4, "rocket_habitat_medium_module_kanim", 10, 60f, constructionMass, materials, 9999f, BuildLocationRule.Anywhere, none, noise);
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
			go.AddOrGet<AssignmentGroupController>().generateGroupOnStart = true;
			go.AddOrGet<PassengerRocketModule>().interiorReverbSnapshot = AudioMixerSnapshots.Get().MediumRocketInteriorReverbSnapshot;
			go.AddOrGet<ClustercraftExteriorDoor>().interiorTemplateName = "interiors/habitat_medium_radiator";
			go.AddOrGetDef<SimpleDoorController.Def>();
			go.AddOrGet<NavTeleporter>();
			go.AddOrGet<AccessControl>();
			go.AddOrGet<LaunchableRocketCluster>();
			go.AddOrGet<RocketCommandConditions>();
			go.AddOrGet<RocketProcessConditionDisplayTarget>();
			go.AddOrGet<CharacterOverlay>().shouldShowName = true;
			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
			{
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 4), GameTags.Rocket, (AttachableBuilding) null)
			};
			Storage storage1 = go.AddComponent<Storage>();
			storage1.showInUI = false;
			storage1.capacityKg = 10f;
			RocketConduitSender rocketConduitSender1 = go.AddComponent<RocketConduitSender>();
			rocketConduitSender1.conduitStorage = storage1;
			rocketConduitSender1.conduitPortInfo = this.liquidInputPort;
			go.AddComponent<RocketConduitReceiver>().conduitPortInfo = this.liquidOutputPort;
			Storage storage2 = go.AddComponent<Storage>();
			storage2.showInUI = false;
			storage2.capacityKg = 1f;
			RocketConduitSender rocketConduitSender2 = go.AddComponent<RocketConduitSender>();
			rocketConduitSender2.conduitStorage = storage2;
			rocketConduitSender2.conduitPortInfo = this.gasInputPort;
			go.AddComponent<RocketConduitReceiver>().conduitPortInfo = this.gasOutputPort;
		}

		private void AttachPorts(GameObject go)
		{
			go.AddComponent<ConduitSecondaryInput>().portInfo = this.liquidInputPort;
			go.AddComponent<ConduitSecondaryOutput>().portInfo = this.liquidOutputPort;
			go.AddComponent<ConduitSecondaryInput>().portInfo = this.gasInputPort;
			go.AddComponent<ConduitSecondaryOutput>().portInfo = this.gasOutputPort;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MAJOR);
			Ownable ownable = go.AddOrGet<Ownable>();
			ownable.slotID = Db.Get().AssignableSlots.HabitatModule.Id;
			ownable.canBePublic = false;
			FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
			fakeFloorAdder.floorOffsets = new CellOffset[5]
			{
	  new CellOffset(-2, -1),
	  new CellOffset(-1, -1),
	  new CellOffset(0, -1),
	  new CellOffset(1, -1),
	  new CellOffset(2, -1)
			};
			fakeFloorAdder.initiallyActive = false;
			go.AddOrGet<BuildingCellVisualizer>();
			go.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition)new LimitOneCommandModule());
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			go.AddOrGet<BuildingCellVisualizer>();
			this.AttachPorts(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.AddOrGet<BuildingCellVisualizer>();
			this.AttachPorts(go);
		}
	}
}