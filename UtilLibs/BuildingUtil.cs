using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace UtilLibs
{
	/// <summary>
	/// Credit: Akis FUtility
	/// </summary>
	public static class BuildingUtil
	{
		public static void AddToResearch(string ID, string tech)
		{
			if (!tech.IsNullOrWhiteSpace())
			{
				Db.Get().Techs.Get(tech).unlockedItemIDs.Add(ID);
			}
		}

		public static BuildingDef CreateTileDef(string ID, string anim, float constructionMass, string material, EffectorValues decor, bool transparent)
		{
			return CreateTileDef(ID, anim, new float[] { constructionMass }, new string[] { material }, decor, transparent);
		}

		public static BuildingDef CreateTileDef(string ID, string anim, float[] constructionMass, string[] material, EffectorValues decor, bool transparent)
		{

			BuildingDef def = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 1,
				height: 1,
				anim: anim,
				hitpoints: BUILDINGS.HITPOINTS.TIER1,
				construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
				construction_mass: constructionMass,
				construction_materials: material,
				melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER3,
				build_location_rule: BuildLocationRule.Tile,
				decor: decor,
				noise: NOISE_POLLUTION.NONE
				);

			BuildingTemplates.CreateFoundationTileDef(def);
			def.Floodable = false;
			def.Overheatable = false;
			def.Entombable = false;
			def.UseStructureTemperature = false;
			def.AudioCategory = "Glass";
			def.AudioSize = "small";
			def.BaseTimeUntilRepair = -1f;
			def.SceneLayer = transparent ? Grid.SceneLayer.GlassTile : Grid.SceneLayer.TileMain;
			def.isKAnimTile = true;
			def.BlockTileIsTransparent = transparent;

			def.BlockTileMaterial = global::Assets.GetMaterial("tiles_solid");

			return def;
		}

		/// <summary>
		/// Makes the kbac render above liquids for buildings that are meant to show up in front of them, like conveyortiles
		/// </summary>
		/// <param name="go"></param>
		public static void RenderAboveLiquids(GameObject go)
		{
			//mirrored from Fake tile buildings like storage tile and farm tiles
			if (go.TryGetComponent<KBatchedAnimController>(out var kbac))
				kbac.initialBlendParameters = 4;
			else
				SgtLogger.error("KBatchedAnimController not found on " + go.name + ", cannot mark to render above liquids!");
		}
		public static void RenderAboveLiquids(KBatchedAnimController kbac)
		{
			kbac.initialBlendParameters = 4;
		}
	}
}
