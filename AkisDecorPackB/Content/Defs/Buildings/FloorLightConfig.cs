using AkisDecorPackB.Content.ModDb;
using AkisDecorPackB.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static ResearchTypes;
using static STRINGS.BUILDINGS.PREFABS;

namespace AkisDecorPackB.Content.Defs.Buildings
{
	public class FloorLightConfig : IBuildingConfig
	{
		public const string ID = $"DecorPackB_FloorLight";
		public static readonly int BlockTileConnectorID = Hash.SDBMLower("decorpackb_lamp_tops");

		public override BuildingDef CreateBuildingDef()
		{

			var def = BuildingUtil.CreateTileDef(
				ID,
				"dpii_floor_lamp_kanim",
				[150f, 50f],
				[MATERIALS.METAL, ModAssets.Tags.FloorLampPaneMaterial.ToString()],
				TUNING.BUILDINGS.DECOR.BONUS.TIER1,
				true);

			BuildingTemplates.CreateFoundationTileDef(def);

			def.ShowInBuildMenu = true;
			def.BlockTileIsTransparent = true;
			def.SceneLayer = Grid.SceneLayer.TileMain;

			//AssetUtils.AddCustomTileAtlas(def, $"metal_frame");
			//AssetUtils.AddCustomTileTops(def, $"metal_frame",true, "tiles_metal_tops_decor_info", "tiles_glass_tops_decor_place_info");

			////def.BlockTileMaterial = Assets.GetMaterial("metal_frame");
			def.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_metal_tops_decor_info");
			def.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_metal_tops_decor_info");
			//def.DragBuild = true;

			AssetUtils.AddCustomTileAtlas(def, "metal_frame_tiles");
			AssetUtils.AddCustomTileTops(def, "metal_frame_tiles_tops", false, "tiles_metal_tops_decor_info");


			def.DecorBlockTileInfo.atlasSpec = null;
			def.RequiresPowerInput = true;
			def.EnergyConsumptionWhenActive = 8f;
			def.SelfHeatKilowattsWhenActive = 0.5f;
			def.ViewMode = OverlayModes.Light.ID;
			def.AudioCategory = AUDIO.CATEGORY.HOLLOW_METAL;

			return def;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), tag);

			var simCellOccupier = go.AddOrGet<SimCellOccupier>();
			simCellOccupier.notifyOnMelt = true;
			simCellOccupier.setTransparent = true;
			simCellOccupier.movementSpeedMultiplier = 1.0f;

			go.AddOrGet<TileTemperature>();
			go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = BlockTileConnectorID;
			go.AddOrGet<BuildingHP>().destroyOnDamaged = true;

			go.GetComponent<KPrefabID>().AddTag(GameTags.LightSource);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			var lightShapePreview = go.AddComponent<LightShapePreview>();
			lightShapePreview.lux = 300;
			lightShapePreview.radius = 2f;
			lightShapePreview.shape = LightShape.Circle;
			lightShapePreview.offset = CellOffset.none;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			GeneratedBuildings.RemoveLoopingSounds(go);

			if (go.TryGetComponent(out KPrefabID prefabID))
			{
				prefabID.AddTag(GameTags.FloorTiles);
				prefabID.AddTag(GameTags.Window);
				prefabID.AddTag(ModAssets.Tags.FloorLamp);
				prefabID.AddTag(ModAssets.Tags.NoPaint);
			}

			ConfigureKbac(go, "off");


			go.AddOrGet<EnergyConsumer>();
			go.AddOrGet<LoopingSounds>();

			var light2D = go.AddOrGet<Light2D>();
			light2D.Color = new Color(1.5f, 1.2f, 0.4f);
			light2D.Range = 2f;
			light2D.Direction = LIGHT2D.FLOORLAMP_DIRECTION;
			light2D.Offset = Vector2.zero;
			light2D.shape = LightShape.Circle;
			light2D.drawOverlay = false;

			go.AddComponent<FloorLamp>();

			go.AddOrGetDef<LightController.Def>();
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			go.AddOrGet<KAnimGridTileVisualizer>();
			ConfigureKbac(go, "place");
			go.AddOrGet<FloorLampUnderConstruction>(); 
		}

		private static void ConfigureKbac(GameObject go, string initialAnim)
		{
			// manually adding this because being a tile it is skipped from being added by the game
			var kbac = go.AddOrGet<KBatchedAnimController>();

			//var selected = go.GetComponent<Constructable>()?.selectedElementsTags.Last() ?? null;
			string anim = "dpii_floorlamppane_glass_kanim";
			//if (selected != null)
			//{
			//	var paneId = FloorLampPane.GetIdFromElement(selected.ToString());
			//	var pane = Mod_Db.FloorLampPanes.TryGet(paneId);
			//	if (pane != null)
			//	{
			//		//anim = pane.animFile;
			//	}
			//}

			kbac.AnimFiles = [Assets.GetAnim(anim)];
			kbac.initialAnim = initialAnim;
			kbac.Offset = new Vector3(0, 0.5f);
			kbac.SetSceneLayer(Grid.SceneLayer.TileMain);
		}
	}
}
