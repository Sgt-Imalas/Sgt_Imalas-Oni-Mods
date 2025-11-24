using AkisDecorPackB.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace AkisDecorPackB.Content.Defs.Buildings
{
	internal class FossilDisplayConfig : IBuildingConfig
	{
		public const string ID = "DecorPackB_FossilDisplay";

		public override BuildingDef CreateBuildingDef()
		{
			var functionalFossils = Config.Instance.FunctionalFossils;

			var def = BuildingTemplates.CreateBuildingDef(
				ID,
				3,
				2,
				"decorpackb_fossildisplay_base_kanim",
				BUILDINGS.HITPOINTS.TIER2,
				BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER4,
				functionalFossils ? BUILDINGS.CONSTRUCTION_MASS_KG.TIER4 : BUILDINGS.CONSTRUCTION_MASS_KG.TIER2,
				TUNING.MATERIALS.FOSSILS,
				BUILDINGS.MELTING_POINT_KELVIN.TIER1,
				BuildLocationRule.OnFloor,
				//new EffectorValues(Mod.Settings.FossilDisplay.BaseDecor.Amount, Mod.Settings.FossilDisplay.BaseDecor.Range),
				DECOR.BONUS.TIER2,
				NOISE_POLLUTION.NONE
			);

			def.Floodable = false;
			def.Overheatable = false;
			def.AudioCategory = AUDIO.CATEGORY.PLASTIC;
			def.BaseTimeUntilRepair = -1f;
			def.ViewMode = OverlayModes.Decor.ID;
			def.DefaultAnimState = "base";
			def.PermittedRotations = PermittedRotations.FlipH;
			def.RequiredSkillPerkID = Db.Get().SkillPerks.IncreaseLearningSmall.Id;

			return def;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddTag(GameTags.Decoration);
			go.AddTag(ModAssets.Tags.FossilBuilding);
			//go.AddOrGet<BuildingComplete>().isArtable = true;
			go.AddOrGet<RequiresResearcheReconstruction>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddComponent<Exhibition>().defaultAnimName = "base";
			go.AddComponent<Inspiring>();
		}
	}
}
