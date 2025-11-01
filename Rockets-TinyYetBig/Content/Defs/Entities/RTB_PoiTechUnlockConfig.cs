using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Entities
{
    class RTB_PoiTechUnlockConfig : IEntityConfig
	{
		public string[] GetDlcIds()
		{
			return null;
		}
		public static string ID = "RTB_TechUnlock";
		public GameObject CreatePrefab()
		{
			GameObject gameObject = EntityTemplates.CreatePlacedEntity(ID, global::STRINGS.BUILDINGS.PREFABS.DLC4POITECHUNLOCKS.NAME, global::STRINGS.BUILDINGS.PREFABS.DLC4POITECHUNLOCKS.DESC, 100f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("gravitas_desk_podium_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 1, height: 2, element: SimHashes.Creature, additionalTags: new List<Tag>
		{
			GameTags.Gravitas,
			GameTags.LightSource,
			GameTags.RoomProberBuilding
		});
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.SetElement(SimHashes.Unobtanium);
			component.Temperature = 294.15f;
			OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
			occupyArea.objectLayers = new ObjectLayer[1] { ObjectLayer.Building };
			gameObject.AddOrGet<Demolishable>();
			POITechItemUnlockWorkable pOITechItemUnlockWorkable = gameObject.AddOrGet<POITechItemUnlockWorkable>();
			pOITechItemUnlockWorkable.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_metalrefinery_kanim") };
			POITechItemUnlocks.Def def = gameObject.AddOrGetDef<POITechItemUnlocks.Def>();
			def.POITechUnlockIDs = [ModAssets.DeepSpaceScienceID];
			def.PopUpName = "Abandoned Station Computer";
			def.animName = "space_station_research_unlocked_kanim";
			Light2D light2D = gameObject.AddComponent<Light2D>();
			light2D.Color = LIGHT2D.POI_TECH_UNLOCK_COLOR;
			light2D.Range = 5f;
			light2D.Angle = 2.6f;
			light2D.Direction = LIGHT2D.POI_TECH_DIRECTION;
			light2D.Offset = LIGHT2D.POI_TECH_UNLOCK_OFFSET;
			light2D.overlayColour = LIGHT2D.POI_TECH_UNLOCK_OVERLAYCOLOR;
			light2D.shape = LightShape.Cone;
			light2D.drawOverlay = true;
			light2D.Lux = 1800;
			gameObject.AddOrGet<Prioritizable>();
			return gameObject;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
