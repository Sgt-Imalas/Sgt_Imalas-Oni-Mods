using ForceFieldWallTile.Content.Scripts;
using ForceFieldWallTile.Content.Scripts.MeshGen;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using TUNING;
using UnityEngine;

namespace ForceFieldWallTile.Content.Defs.Buildings
{
	internal class ForceFieldTileConfig : IBuildingConfig
	{
		public static string ID = "FFT_ForceFieldProjector";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] rawMineralsOrWood = TUNING.MATERIALS.REFINED_METALS;
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues decor = new()
			{
				amount = -5,
				radius = 0
			};
			EffectorValues noise = none;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "fft_shield_generator_kanim", 30, 3f, tieR2, rawMineralsOrWood, 1600f, BuildLocationRule.NotInTiles, decor, noise);
			buildingDef.Entombable = false;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.DefaultAnimState = "off";
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
			buildingDef.AddLogicPowerPort = true;
			buildingDef.ExhaustKilowattsWhenActive = 0;
			buildingDef.SelfHeatKilowattsWhenActive = 0;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = Config.Instance.NormalWattage;

			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Building;
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);

			var saverMode = go.AddOrGet<FridgeSaverDescriptor>();
			saverMode.CachedMaxWattage = Config.Instance.NormalWattage;
			saverMode.CachedSteadyWattage = Config.Instance.SteadyWattage();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			GeneratedBuildings.RemoveLoopingSounds(go);
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGet<ForceFieldTile>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.Bunker);
			go.AddOrGet<ConnectorTileable>();
		}
	}
}
