using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.DerelictPropBuildings
{
	internal class DerelictLadderConfig : IBuildingConfig
	{
		public static string ID = "RTB_PropLadder";
		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
			string[] metals = [MATERIALS.REFINED_METAL];
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues tieR0 = BUILDINGS.DECOR.PENALTY.TIER0;
			EffectorValues noise = none;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "ladder_poi_kanim", 10, 10f, tieR1, metals, 1600f, BuildLocationRule.Anywhere, tieR0, noise);
			BuildingTemplates.CreateLadderDef(buildingDef);
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.Entombable = false;
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.DefaultAnimState = "off";
			buildingDef.DragBuild = true;
			buildingDef.ShowInBuildMenu = false;
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			Ladder ladder = go.AddOrGet<Ladder>();
			ladder.upwardsMovementSpeedMultiplier = 1.2f;
			ladder.downwardsMovementSpeedMultiplier = 1.2f;
			go.AddOrGet<AnimTileable>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			KPrefabID component = go.GetComponent<KPrefabID>();
			component.AddTag(GameTags.Bunker);
			component.AddTag(GameTags.FloorTiles);
			component.AddTag(GameTags.RocketEnvelopeTile);
			component.AddTag(GameTags.NoRocketRefund);
			go.GetComponent<Deconstructable>().allowDeconstruction = false;
		}
	}
}
