using AkisDecorPackB.Content.ModDb;
using AkisDecorPackB.Content.Scripts;
using AkisDecorPackB.Content.Scripts.BigFossil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace AkisDecorPackB.Content.Defs.Buildings
{
	internal class GiantFossilDisplayConfig : IBuildingConfig
	{
		public static string ID = "DecorPackB_GiantFossilDisplay";

		public override BuildingDef CreateBuildingDef()
		{
			var def = BuildingTemplates.CreateBuildingDef(
				ID,
				7,
				6,
				"decorpackb_giantfossil_default_kanim",
				BUILDINGS.HITPOINTS.TIER2,
				BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER4,
				[
					800f,
					50f,
					1f
				],
				[
					ModAssets.Tags.FossilMaterial.ToString(),
					SimHashes.Steel.ToString(),
					ModAssets.Tags.BuildingFossilNodule.ToString(),
				],
				BUILDINGS.MELTING_POINT_KELVIN.TIER1,
				Mod_Db.BuildLocationRules.GiantFossilRule,
				DECOR.BONUS.TIER5,
				NOISE_POLLUTION.NONE
			);

			def.Floodable = false;
			def.Overheatable = false;
			def.AudioCategory = "plastic";
			def.BaseTimeUntilRepair = -1f;
			def.ViewMode = OverlayModes.Decor.ID;
			def.DefaultAnimState = "base";
			def.PermittedRotations = PermittedRotations.FlipH;
			def.ContinuouslyCheckFoundation = false; // handled manually

			return def;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddTag(GameTags.Decoration);
			go.AddTag(ModAssets.Tags.FossilBuilding);
			go.AddOrGet<BuildingComplete>().isArtable = true;
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddComponent<InspireAll>().effectId = ModEffects.INSPIRED_GIANT;
			ConfigureCables(go, Color.black, false);

			var def = go.GetComponent<Building>().Def;
			ConfigureCables(def.BuildingPreview, Color.white, true);
			ConfigureCables(def.BuildingUnderConstruction, Color.white, false);
		}

		private static void ConfigureCables(GameObject go, Color color, bool alwaysUpdate)
		{
			go.AddComponent<BigFossil>().alwaysUpdate = alwaysUpdate;
			go.AddComponent<AnchorMonitor>();
			go.AddComponent<BigFossilCablesRenderer>().baseColor = color;
		}
	}
}
