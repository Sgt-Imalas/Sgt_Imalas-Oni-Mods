using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rockets_TinyYetBig.ModAssets;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SpaceStationBuilder : KMonoBehaviour//, ISidescreenButtonControl
    {

        [Serialize]
        public int CurrentSpaceStationTypeInt = 0;

        [Serialize]
        public float ConstructionProgress = -1;

        private void SpawnStation(AxialI location, string prefab)
        {
            Vector3 position = new Vector3(-1f, -1f, 0.0f);
            GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)prefab), position);
            sat.SetActive(true);
            var spaceStation = sat.GetComponent<SpaceStation>();
            spaceStation.Location = location;
            spaceStation.CurrentSpaceStationType = ModAssets.SpaceStationTypes[CurrentSpaceStationTypeInt];
        }
        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().Subscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.ResetStation));
        }
        protected override void OnCleanUp()
        {
            this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().Unsubscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.ResetStation));
            base.OnCleanUp();
        }
        public void SetStationType(SpaceStationWithStats type)
        {
            CurrentSpaceStationTypeInt = ModAssets.GetStationIndex(type);
            ResetStationBuildProgress();
        }

        private void ResetStation(object data = null) => this.ResetStationBuildProgress();
        public void ResetStationBuildProgress()
        {
            ConstructionProgress = -1;
        }

        public void OnSidescreenButtonPressed()
        {

            Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
            var locationToCheck = component.Location;
            //location.q += 2;

            int worldId = SpaceStationManager.GetSpaceStationWorldIdAtLocation(locationToCheck);
            if (worldId!=-1)
            {
                ///other transfering items ?...
                SpaceStationManager.Instance.GetSpaceStationFromWorldId(worldId).DestroySpaceStation();
            }
            else
            {
                ///Add Ressource Check
                SpawnStation(locationToCheck, SmallOrbitalSpaceStationConfig.ID);
            }

        }

    }
}
