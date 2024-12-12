using AkisSnowThings.Content.Scripts.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace AkisSnowThings.Content.Defs.Buildings
{
	internal class SapTapConfig : IBuildingConfig
	{
		public static string ID = "SnowSculptures_SapTap";

		public override BuildingDef CreateBuildingDef()
		{
			var def = BuildingTemplates.CreateBuildingDef(
			   ID,
			   1,
			   1,
			   "ventliquid_kanim",
			   BUILDINGS.HITPOINTS.TIER1,
			   BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
			   BUILDINGS.CONSTRUCTION_MASS_KG.TIER2,
			   MATERIALS.REFINED_METALS,
			   1600f,
			   BuildLocationRule.OnFloor,
			   BUILDINGS.DECOR.PENALTY.TIER1,
			   NOISE_POLLUTION.NONE
		   );

			def.Floodable = false;
			def.Overheatable = false;
			def.AudioCategory = AUDIO.CATEGORY.HOLLOW_METAL;
			def.ViewMode = OverlayModes.LiquidConduits.ID;
			def.DefaultAnimState = "off";
			def.PermittedRotations = PermittedRotations.Unrotatable;

			def.ObjectLayer = ObjectLayer.AttachableBuilding;
			def.SceneLayer = Grid.SceneLayer.BuildingFront;
			def.ForegroundLayer = Grid.SceneLayer.Background;
			def.AttachmentSlotTag = ModAssets.GlassCaseAttachmentTag;
			def.attachablePosition = new CellOffset(0, 0);
			//def.attachablePosition = new CellOffset(0, 0);
			//def.AttachmentSlotTag = ModAssets.AttachmentTag;
			return def;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<BuildingAttachPoint>().points = [new BuildingAttachPoint.HardPoint(new CellOffset(0, 0), ModAssets.TreeAttachmentTag, null)];
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
		}
	}
}
