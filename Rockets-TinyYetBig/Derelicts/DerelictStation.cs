using KSerialization;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.SpaceStations;
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


        }
        public static void SpawnNewDerelictStation(ArtifactPOIClusterGridEntity source)
        {
            source.TryGetComponent<KPrefabID>(out var id);
            var targetStationId = id.PrefabID() + DerelictStationConfigs.DerelictTemplateName;
            SgtLogger.l(targetStationId, "targetStation");
            if (Assets.GetPrefab(targetStationId) == null)
                return;


            Vector3 position = new Vector3(-1f, -1f, 0.0f);
            GameObject sat = Util.KInstantiate(Assets.GetPrefab(targetStationId), position);
            sat.SetActive(true);
            var spaceStation = sat.GetComponent<DerelictStation>();
            spaceStation.Location = source.Location;
        }

        public void Init(AxialI location)
        {
            base.Location = location;
        }
    }
}
