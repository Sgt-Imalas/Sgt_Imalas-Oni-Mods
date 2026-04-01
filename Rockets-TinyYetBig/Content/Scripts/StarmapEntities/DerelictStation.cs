using KSerialization;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Content.Scripts.StarmapEntities;
using Rockets_TinyYetBig.Elements;
using Rockets_TinyYetBig.SpaceStations;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Derelicts
{
	internal class DerelictStation : SpaceStation
	{
		[Serialize]
		public string artifactInReference;
		public override bool SpaceOutInSameHex() => false;

		public string poiID;

		public string m_Anim;
		public override string Name => l_name;

		public override EntityLayer Layer => EntityLayer.POI;

		public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
		{
			new AnimConfig
			{
				animFile = Assets.GetAnim("gravitas_space_poi_kanim"),
				initialAnim = (m_Anim.IsNullOrWhiteSpace() ? "station_1" : m_Anim)
			}
		};

		public override Sprite GetUISprite()
		{
			List<ClusterGridEntity.AnimConfig> animConfigs = this.AnimConfigs;
			if (animConfigs.Count > 0)
				return Def.GetUISpriteFromMultiObjectAnim(animConfigs[0].animFile, animConfigs[0].initialAnim, true);
			return null;
		}
		public override bool IsVisible => true;

		public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Peeked;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();

		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (TryGetComponent<KSelectable>(out var overlay) && overlay.IsSelected)
			{
				NameDisplayScreen.Instance.UpdateName(overlay.gameObject);
			}

			if (!RTB_SavegameStoredSettings.Instance.DerelictInteriorWorlds.Contains(SpaceStationInteriorId))
				RTB_SavegameStoredSettings.Instance.DerelictInteriorWorlds.Add(SpaceStationInteriorId);

		}
		public static bool SpawnNewDerelictStation(ArtifactPOIClusterGridEntity source, out DerelictStation spaceStation)
		{
			spaceStation = null;
			source.TryGetComponent<KPrefabID>(out var id);
			var targetStationId = id.PrefabID() + DerelictStationConfigs.DerelictTemplateName;
			SgtLogger.l(targetStationId, "targetStation");
			if (Assets.TryGetPrefab(targetStationId) == null)
				return false;

			if(source.TryGetComponent<ArtifactPOIConfigurator>(out var cfg) && cfg.MakeConfiguration().DestroyOnHarvest())
			{
				SgtLogger.l("artifactPOIstates destroys itself on harvest on: "+source);
				return false;
			}

			Vector3 position = new Vector3(-1f, -1f, 0.0f);
			GameObject sat = Util.KInstantiate(Assets.GetPrefab(targetStationId), position);
			sat.SetActive(true);
			spaceStation = sat.GetComponent<DerelictStation>();
			spaceStation.Location = source.Location;
			//var site = sat.GetComponent<SpaceConstructable>();
			//site.SetDerelict(true);
			//site.ForceFinishProject(ConstructionProjects.DerelictStation);

			sat.AddOrGet<StationDeconstructable>().Resources = [new (ModElements.UnobtaniumAlloy.Tag, 125),new ("Steel", 500)];
			return true;
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			if (RTB_SavegameStoredSettings.Instance.DerelictInteriorWorlds.Contains(SpaceStationInteriorId))
				RTB_SavegameStoredSettings.Instance.DerelictInteriorWorlds.Remove(SpaceStationInteriorId);
		}

		public void Init(AxialI location)
		{
			base.Location = location;
		}
	}
}
