using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.DerelictPropBuildings
{
	internal class DerelictAntennaConfig : IBuildingConfig
	{
		public const string ID = "RTB_DerelictAntenna";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] refinedMetals = MATERIALS.REFINED_METALS;
			EffectorValues tieR2_2 = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
			EffectorValues noise = tieR2_2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 3, "landing_beacon_kanim", 1000, 30f, tieR2_1, refinedMetals, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.DefaultAnimState = "off";
			buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
			buildingDef.OverheatTemperature = 398.15f;
			buildingDef.Floodable = false;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.RequiresPowerInput = false;
			buildingDef.CanMove = false;
			buildingDef.Invincible = true;
			buildingDef.ShowInBuildMenu = false;
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			KPrefabID component = go.GetComponent<KPrefabID>();
			component.AddTag(GameTags.Bunker);
			component.AddTag(GameTags.Bunker);
			component.AddTag(GameTags.NoRocketRefund);
			go.GetComponent<Deconstructable>().allowDeconstruction = false;
		}
	}
}
