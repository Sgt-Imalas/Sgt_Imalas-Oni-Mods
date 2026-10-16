using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Entities.DerelictProps
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
			GameObject gameObject = EntityTemplates.CreatePlacedEntity(ID, 
				STRINGS.BUILDINGS.PREFABS.RTB_TECHUNLOCK.NAME,
				STRINGS.BUILDINGS.PREFABS.RTB_TECHUNLOCK.DESC, 100f, decor: BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, 
				anim: Assets.GetAnim("gravitas_desk_podium_kanim"), 
				initialAnim: "off", sceneLayer: Grid.SceneLayer.Building,
				width: 1, 
				height: 2, 
				element: SimHashes.Creature, 
				additionalTags: new List<Tag>
				{
					GameTags.Gravitas,
					GameTags.RoomProberBuilding
				});
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.SetElement(SimHashes.Unobtanium);
			component.Temperature = 294.15f;
			OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
			occupyArea.objectLayers = new ObjectLayer[1] { ObjectLayer.Building };
			gameObject.AddOrGet<Demolishable>();
			POITechItemUnlockWorkable pOITechItemUnlockWorkable = gameObject.AddOrGet<POITechItemUnlockWorkable>();
			pOITechItemUnlockWorkable.synchronizeAnims = false;
			pOITechItemUnlockWorkable.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_metalrefinery_kanim") };
			pOITechItemUnlockWorkable.workTime = 5f;
			POITechItemUnlocks.Def def = gameObject.AddOrGetDef<POITechItemUnlocks.Def>();
			def.POITechUnlockIDs = [ModAssets.DeepSpaceScienceID];
			def.PopUpName = STRINGS.BUILDINGS.PREFABS.RTB_TECHUNLOCK.NAME;
			def.animName = "space_station_research_unlocked_kanim";
			
			gameObject.AddOrGet<Prioritizable>();
			return gameObject;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
			inst.GetComponent<KBatchedAnimController>().FlipX = true;
		}
	}
}
