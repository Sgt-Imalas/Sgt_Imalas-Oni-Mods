using NaturalConstruction.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;

namespace NaturalConstruction.Content.Defs
{
	internal class NaturalTilePlanterBackwallConfig : IBuildingConfig
	{
		public static string ID = "NC_NaturalPlanterBackwall";
		public override string[] GetRequiredDlcIds() => DlcManager.DLC5;

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = [Config.Instance.DefaultMass_Tile * 4];
			string[] materials = [GameTags.Solid.ToString()];
			EffectorValues none1 = NOISE_POLLUTION.NONE;
			EffectorValues none2 = TUNING.BUILDINGS.DECOR.NONE;
			EffectorValues noise = none1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "nc_natural_backwall_planter_kanim", 100, 30f, tieR2, materials, 1600f, BuildLocationRule.NotInTiles, none2, noise);
			buildingDef.ObjectLayer = ObjectLayer.Backwall;
			buildingDef.SceneLayer = Grid.SceneLayer.Backwall;
			buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
			buildingDef.PermittedRotations = PermittedRotations.Unrotatable;
			buildingDef.DragBuild = true;
			buildingDef.Replaceable = false;
			buildingDef.Overheatable = false;
			buildingDef.Floodable = false;
			buildingDef.Repairable = false;
			buildingDef.ReplacementTags = new List<Tag>()
			{
				GameTags.Backwall
			};
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			Prioritizable.AddRef(go);
			go.GetComponent<KPrefabID>().prefabInitFn += new KPrefabID.PrefabFn(this.OnPrefabInit);
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			UnityEngine.Object.DestroyImmediate(go.GetComponent<Constructable>());
			var spawner = go.AddComponent<ConstructableNaturalSpawner>();
			spawner.ReplaceOnFinish = false;
			go.AddOrGet<CopyBuildingSettings>();
		}
		private void OnPrefabInit(GameObject instance)
		{
			instance.AddOrGet<PlantablePlot>().AddAdditionalCriteria(new Func<GameObject, bool>(LargeBackwallFarmConfig.ForbiddenTags));
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<KBatchedAnimController>().initialBlendParameters = 4;
			BuildingTemplates.CreateDefaultStorage(go).SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			PlantablePlot plot = go.AddOrGet<PlantablePlot>();
			plot.occupyingObjectRelativePosition = new Vector3(0.0f, 0.0f, -0.5f);
			plot.occupyingObjectVisualOffset = new Vector3(0.5f, 0.0f, 0);
			plot.SetFertilizationFlags(false, false);
			plot.AddAdditionalCriteria(new Func<GameObject, bool>(LargeBackwallFarmConfig.ForbiddenTags));
			plot.AddDepositTag(GameTags.BackwallSeed);
			plot.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Top);

			go.AddOrGet<MakePlantedPlantWild>();
			var spawner = go.AddComponent<CompleteBuildingNaturalSpawner>();
			spawner.ReplaceOnSpawn = false;
			go.AddComponent<ZoneTile>();
		}
	}
}
