using KSerialization;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.SpaceStations;
using Rockets_TinyYetBig.SpaceStations.Construction;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
            if (TryGetComponent<KSelectable>(out var overlay))
            {
                NameDisplayScreen.Instance.UpdateName(overlay.gameObject);
            }

            if (!RTB_SavegameStoredSettings.Instance.DerelictInteriorWorlds.Contains(SpaceStationInteriorId))
                RTB_SavegameStoredSettings.Instance.DerelictInteriorWorlds.Add(SpaceStationInteriorId);

        }
        public static void SpawnNewDerelictStation(ArtifactPOIClusterGridEntity source)
        {

            source.TryGetComponent<KPrefabID>(out var id);
            var targetStationId = id.PrefabID() + DerelictStationConfigs.DerelictTemplateName;
            SgtLogger.l(targetStationId, "targetStation");
            if (Assets.GetPrefab(targetStationId) == null)
                return;
            var originalDef = source.gameObject.GetSMI<ArtifactPOIStates.Instance>();
            if (originalDef == null || originalDef.configuration.DestroyOnHarvest())
                return;

            Vector3 position = new Vector3(-1f, -1f, 0.0f);
            GameObject sat = Util.KInstantiate(Assets.GetPrefab(targetStationId), position);
            sat.SetActive(true);
            var spaceStation = sat.GetComponent<DerelictStation>();
            spaceStation.Location = source.Location;
            var site = sat.AddOrGet<SpaceConstructable>();
            site.buildPartStorage = sat.AddComponent<Storage>();
            site.SetDerelict( true);
            site.ForceFinishProject(ConstructionProjects.DerelictStation);
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
