using AkisSnowThings.Content.Scripts.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace AkisSnowThings.Content.Defs.Buildings
{
	internal class ChristmasTreeAttachmentConfig : IBuildingConfig
	{
		public static string ID = "SnowSculptures_ChristmasTree";
		public const int LUX = 4000;
		public const int LIGHT_RANGE = 16;
		public static EffectorValues TREE_DECOR = new(30, 8);

		public static CellOffset MAIN_LIGHT_OFFSET = new CellOffset(0, 3);

		public override BuildingDef CreateBuildingDef()
		{
			var def = BuildingTemplates.CreateBuildingDef(
			   ID,
			   3,
			   4,
			   "sm_christmastree_kanim",
			   BUILDINGS.HITPOINTS.TIER1,
			   BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
			   BUILDINGS.CONSTRUCTION_MASS_KG.TIER4,
			   MATERIALS.REFINED_METALS,
			   1600f,
			   BuildLocationRule.BuildingAttachPoint,
				TREE_DECOR,
			   NOISE_POLLUTION.NONE
		   );

			def.Floodable = false;
			def.RequiresPowerInput = true;
			def.EnergyConsumptionWhenActive = 100f;
			def.SelfHeatKilowattsWhenActive = 1f;

			def.AudioCategory = AUDIO.CATEGORY.METAL;
			def.ViewMode = OverlayModes.Power.ID;
			def.DefaultAnimState = "off";
			def.PermittedRotations = PermittedRotations.Unrotatable;

			def.ObjectLayer = ObjectLayer.AttachableBuilding;
			def.SceneLayer = Grid.SceneLayer.BuildingFront;
			def.ForegroundLayer = Grid.SceneLayer.Background;
			def.AttachmentSlotTag = ModAssets.TreeAttachmentTag;
			def.attachablePosition = new CellOffset(0, 0);
			//def.attachablePosition = new CellOffset(0, 0);
			//def.AttachmentSlotTag = ModAssets.AttachmentTag;
			return def;
		}
		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			LightShapePreview lightShapePreview = go.AddComponent<LightShapePreview>();
			lightShapePreview.lux = LUX;
			lightShapePreview.radius = LIGHT_RANGE;
			lightShapePreview.shape = LightShape.Circle;
			lightShapePreview.offset = MAIN_LIGHT_OFFSET;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<BuildingAttachPoint>().points = [new BuildingAttachPoint.HardPoint(new CellOffset(0, 0), ModAssets.TreeAttachmentTag, null)];
			go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<EnergyConsumer>();
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<ChristmasPresentSpawner>();
			Light2D light2D = go.AddOrGet<Light2D>();
			light2D.Lux = LUX;
			light2D.overlayColour = LIGHT2D.SUNLAMP_OVERLAYCOLOR;
			light2D.Color = LIGHT2D.SUNLAMP_COLOR;
			light2D.Range = LIGHT_RANGE;
			light2D.Offset = new(MAIN_LIGHT_OFFSET.x,MAIN_LIGHT_OFFSET.y+0.5f);
			light2D.shape = LightShape.Circle;
			light2D.drawOverlay = true;
			go.AddOrGetDef<LightController.Def>();
			go.AddOrGet<TreeAttachableBuilding>();
		}
	}
}
