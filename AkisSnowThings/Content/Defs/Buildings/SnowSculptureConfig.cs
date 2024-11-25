using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.BUILDINGS.PREFABS;
using TUNING;
using UnityEngine;
using AkisSnowThings.Content.Scripts.Buildings;
using UtilLibs;

namespace AkisSnowThings.Content.Defs.Buildings
{
	public class SnowSculptureConfig : IBuildingConfig
	{
		public static string ID = "SnowSculptures_SnowSculpture";

		public override BuildingDef CreateBuildingDef()
		{
			var def = BuildingTemplates.CreateBuildingDef(
			   ID,
			   2,
			   3,
			   "sm_sculpture_snow_kanim",
			   BUILDINGS.HITPOINTS.TIER2,
			   BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER4,
			   BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
			   new[] { SimHashes.Snow.ToString() },
			   UtilMethods.GetKelvinFromC(0),
			   BuildLocationRule.OnFloor,
			   DECOR.BONUS.TIER3,
			   NOISE_POLLUTION.NONE
		   );

			def.Floodable = false;
			def.Overheatable = false;
			def.AudioCategory = AUDIO.CATEGORY.PLASTIC;
			def.BaseTimeUntilRepair = -1f;
			def.ViewMode = OverlayModes.Decor.ID;
			def.DefaultAnimState = "pile";
			def.PermittedRotations = PermittedRotations.FlipH;
			//def.attachablePosition = new CellOffset(0, 0);
			//def.AttachmentSlotTag = ModAssets.AttachmentTag;
			return def;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<BuildingComplete>().isArtable = true;
			go.AddTag(GameTags.Decoration);
			go.AddOrGet<BuildingAttachPoint>().points = [new BuildingAttachPoint.HardPoint(new CellOffset(0, 0), ModAssets.AttachmentTag, null)];
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddComponent<SnowPile>().defaultAnimName = "pile";
		}
	}
}
