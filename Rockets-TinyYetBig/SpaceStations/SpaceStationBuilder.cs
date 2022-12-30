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
    class SpaceStationBuilder : KMonoBehaviour, ISim1000ms//, ISidescreenButtonControl
    {

        [Serialize]
        public int CurrentSpaceStationTypeInt = 0;

        [Serialize]
        public float ConstructionProgress = -1;

        [Serialize]
        public float DemolishingProgress = -1;


        public void Sim1000ms(float dt)
        {
            if (ConstructionProgress > -1)
            {
                if (ConstructionProgress < ModAssets.SpaceStationTypes[CurrentSpaceStationTypeInt].constructionTime)
                {
                    ConstructionProgress += dt;
                }
                else
                {
                    Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
                    var locationToCheck = component.Location;
                    SpawnStation(locationToCheck);
                    ResetStationProgress();
                }
            }
            else if (DemolishingProgress > -1)
            {
                if (DemolishingProgress < ModAssets.SpaceStationTypes[CurrentSpaceStationTypeInt].constructionTime / 4)
                {
                    DemolishingProgress += dt;
                }
                else
                {
                    Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
                    var locationToCheck = component.Location;
                    int worldId = SpaceStationManager.GetSpaceStationWorldIdAtLocation(locationToCheck);
                    if (worldId != -1)
                    {
                        SpaceStationManager.Instance.GetSpaceStationFromWorldId(worldId).DestroySpaceStation();
                    }
                }
            }
        }

        private void SpawnStation(AxialI location)
        {
            Vector3 position = new Vector3(-1f, -1f, 0.0f);
            GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)SpaceStationConfig.ID), position);
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
            ResetStationProgress();
        }

        private void ResetStation(object data = null) => this.ResetStationProgress();
        public void ResetStationProgress()
        {
            ConstructionProgress = -1;
            DemolishingProgress = -1;
        }
        public void StartStationBuildProgress()
        {
            if (ConstructionProgress == -1)
                ConstructionProgress = 0;
        }
        public void StartStationDemolishProgress()
        {
            if (DemolishingProgress == -1)
                DemolishingProgress = 0;
        }

        public bool Demolishing()
        {
            return DemolishingProgress > -1f;
        }
        public bool Constructing()
        {
            return ConstructionProgress > -1;
        }

        public void ConstructButtonPressed()
        {
            if (!Demolishing())
            {
                if (!Constructing())
                {
                    StartStationBuildProgress();
                }
                else
                {
                    ResetStationProgress();
                }
            }
        }
        public void DemolishButtonPressed()
        {
            if (!Constructing())
            {
                if (!Demolishing())
                {
                    StartStationDemolishProgress();
                }
                else
                {
                    ResetStationProgress();
                }
            }
        }

        public bool IsStationAtCurrentLocation()
        {
            Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
            var locationToCheck = component.Location;
            //location.q += 2;

            int worldId = SpaceStationManager.GetSpaceStationWorldIdAtLocation(locationToCheck);
            return worldId != -1;
        }


    }
}
