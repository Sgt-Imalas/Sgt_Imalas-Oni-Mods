using NaturalConstruction.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;

namespace NaturalConstruction.Content.Defs
{
	internal class NaturalTilePlanterWideConfig : IBuildingConfig
	{
		public static string ID = "NC_NaturalPlanterWide";
		public override string[] GetRequiredDlcIds() => DlcManager.DLC5;

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = [Config.Instance.DefaultMass_Tile * 3];
			string[] materials = [GameTags.Solid.ToString()];
			EffectorValues none1 = NOISE_POLLUTION.NONE;
			EffectorValues none2 = TUNING.BUILDINGS.DECOR.NONE;
			EffectorValues noise = none1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 1, "nc_natural_planter_wide_kanim", 100, 30f, tieR2, materials, 1600f, BuildLocationRule.Tile, none2, noise);
			BuildingTemplates.CreateFoundationTileDef(buildingDef);
			buildingDef.Floodable = false;
			buildingDef.Entombable = false;
			buildingDef.Overheatable = false;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
			buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
			buildingDef.DragBuild = false;
			buildingDef.PermittedRotations = PermittedRotations.Unrotatable;
			//buildingDef.PermittedRotations = PermittedRotations.FlipV;
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			Prioritizable.AddRef(go);

			SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
			simCellOccupier.doReplaceElement = true;
			simCellOccupier.notifyOnMelt = true;
			go.AddOrGet<TileTemperature>();

			go.GetComponent<KPrefabID>().prefabInitFn += new KPrefabID.PrefabFn(this.OnPrefabInit);
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			UnityEngine.Object.DestroyImmediate(go.GetComponent<Constructable>());
			var spawner = go.AddComponent<ConstructableNaturalSpawner>();
			spawner.ReplaceOnFinish = false;
			go.AddOrGet<CopyBuildingSettings>();
			go.AddOrGet<TrellisAnimator>().DefaultHeight = 2;
		}
		private void OnPrefabInit(GameObject instance)
		{
			instance.AddOrGet<PlantablePlot>().AddAdditionalCriteria(new Func<GameObject, bool>(WideFarmTileConfig.MatchesTags));
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<KBatchedAnimController>().initialBlendParameters = 4;
			BuildingTemplates.CreateDefaultStorage(go).SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			PlantablePlot plot = go.AddOrGet<PlantablePlot>();
			plot.SetFertilizationFlags(false, false);
			plot.AddAdditionalCriteria(new Func<GameObject, bool>(WideFarmTileConfig.MatchesTags));
			plot.AddDepositTag(GameTags.LargeSeed);

			go.AddOrGet<MakePlantedPlantWild>();
			var spawner = go.AddComponent<CompleteBuildingNaturalSpawner>();
			spawner.ReplaceOnSpawn = false;
			go.AddOrGet<TrellisAnimator>().DefaultHeight = 2;
			go.AddComponent<ZoneTile>();

		}
	}
}
