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
	internal class GlassCaseConfig : IBuildingConfig
	{
		public static string ID = "SnowSculptures_GlassCase";

		public override BuildingDef CreateBuildingDef()
		{
			var def = BuildingTemplates.CreateBuildingDef(
			   ID,
			   2,
			   3,
			   "sm_glasscase_kanim",
			   BUILDINGS.HITPOINTS.TIER2,
			   BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER4,
			   BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
			   MATERIALS.RAW_MINERALS,
			   BUILDINGS.MELTING_POINT_KELVIN.TIER0,
			   BuildLocationRule.BuildingAttachPoint,
			   DECOR.NONE,
			   NOISE_POLLUTION.NONE
		   );

			def.Floodable = false;
			def.AudioCategory = AUDIO.CATEGORY.GLASS;
			def.ViewMode = OverlayModes.Power.ID;

			def.ObjectLayer = ObjectLayer.AttachableBuilding;
			def.SceneLayer = Grid.SceneLayer.BuildingFront;
			def.ForegroundLayer = Grid.SceneLayer.Background;
			def.AttachmentSlotTag = ModAssets.AttachmentTag;
			def.attachablePosition = new CellOffset(0, 0);

			return def;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddComponent<GlassCase>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
		}
	}
}
