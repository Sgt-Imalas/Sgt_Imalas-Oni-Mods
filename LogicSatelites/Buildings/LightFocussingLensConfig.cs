using TUNING;
using UnityEngine;

namespace LogicSatellites.Buildings
{
	class LightFocussingLensConfig : IBuildingConfig
	{
		public const string ID = "LS_FocussingLens";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;
		public override BuildingDef CreateBuildingDef()
		{
			float[] materialMass = new float[2]
			{
				25f,
				75f
			};
			string[] materialType = new string[2]
			{
				"Metal",
				"Transparent"
			};
			EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
			EffectorValues decorValue = BUILDINGS.DECOR.BONUS.TIER0;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 1,
				height: 1,
				anim: "lense_convex_kanim",
				hitpoints: 100,
				construction_time: 40f,
				construction_mass: materialMass,
				construction_materials: materialType,
				melting_point: 800f,
				BuildLocationRule.Tile,
				decor: decorValue,
				noise: noiseLevel);

			BuildingTemplates.CreateFoundationTileDef(buildingDef);
			buildingDef.Floodable = false;
			buildingDef.Entombable = false;
			buildingDef.Overheatable = false;
			buildingDef.UseStructureTemperature = false;
			buildingDef.AudioCategory = "Glass";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.SceneLayer = Grid.SceneLayer.GlassTile;

			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
			simCellOccupier.setTransparent = false;
			simCellOccupier.notifyOnMelt = true;
			go.AddOrGet<TileTemperature>();
			go.AddOrGet<BuildingHP>().destroyOnDamaged = true;

			go.AddOrGet<UserNameable>().savedName = "Unnamed Lens Tile";
		}


		public override void DoPostConfigureComplete(GameObject go)
		{
			GeneratedBuildings.RemoveLoopingSounds(go);
			go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles);
			go.AddOrGet<SolarReciever>();

			go.AddOrGet<LaserLens>();
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{

		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{

		}
	}
}
