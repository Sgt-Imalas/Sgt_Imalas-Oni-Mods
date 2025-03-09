using TUNING;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.BeeGeyser
{
	internal class BeeGeyserConfig : IEntityConfig
	{
		public static string ID = "ITCE_BeeGeyser";
		public GameObject CreatePrefab()
		{
			GameObject placedEntity =
				EntityTemplates.CreatePlacedEntity(ID,
				STRINGS.ENTITIES.GEYSERS.ITCE_BEEGEYSER.NAME,
				STRINGS.ENTITIES.GEYSERS.ITCE_BEEGEYSER.DESC,
				2000f, Assets.GetAnim("geyser_bees_kanim"),
				"inactive",
				Grid.SceneLayer.BuildingBack,
				2,
				4,
				BUILDINGS.DECOR.BONUS.TIER1,
				NOISE_POLLUTION.NOISY.TIER6
			);



			placedEntity.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
			{
				ObjectLayer.Building
			};
			PrimaryElement component = placedEntity.GetComponent<PrimaryElement>();
			component.SetElement(SimHashes.Katairite);
			component.Temperature = UtilMethods.GetKelvinFromC(666);
			placedEntity.AddOrGet<Prioritizable>();

			//Studyable studyable = placedEntity.AddOrGet<Studyable>();
			//studyable.meterTrackerSymbol = "geotracker_target";
			//studyable.meterAnim = "tracker";
			placedEntity.AddOrGet<LoopingSounds>();
			placedEntity.AddOrGet<BeeGeyser>();
			placedEntity.AddOrGet<BeeGeyserDemolishable>();
			SoundEventVolumeCache.instance.AddVolume("geyser_side_steam_kanim", "Geyser_shake_LP", NOISE_POLLUTION.NOISY.TIER5);
			SoundEventVolumeCache.instance.AddVolume("geyser_side_steam_kanim", "Geyser_erupt_LP", NOISE_POLLUTION.NOISY.TIER6);
			return placedEntity;
		}

		public string[] GetDlcIds() => null;

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
