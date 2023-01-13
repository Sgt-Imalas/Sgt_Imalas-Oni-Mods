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

        [Serialize]
        float CurrentLocationDemolishTime = -1;

        public float GetProgressPercentage()
        {
            if (this.Constructing () )
            {
                return Math.Min((ConstructionProgress/ ModAssets.SpaceStationTypes[CurrentSpaceStationTypeInt].constructionTime ) *100f,100f);
            }
            else if (this.Demolishing())
            {
                return Math.Min((DemolishingProgress / CurrentLocationDemolishTime) * 100f,100f);
            }
            return -1;
        }
        public float RemainingTime()
        {
            if (this.Constructing())
            {
                return ModAssets.SpaceStationTypes[CurrentSpaceStationTypeInt].constructionTime- ConstructionProgress;
            }
            else if (this.Demolishing())
            {
                return CurrentLocationDemolishTime - DemolishingProgress;
            }
            return 0;
        }

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
                    FinishedProgress();
                }
            }
            else if (DemolishingProgress > -1)
            {
                if (DemolishingProgress < CurrentLocationDemolishTime)
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
                    FinishedProgress();
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
            spaceStation._currentSpaceStationType = CurrentSpaceStationTypeInt;
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
        public void SetStationType(int type)
        {
            CurrentSpaceStationTypeInt = type;
            ResetStationProgress();
        }

        private void ResetStation(object data = null) => this.ResetStationProgress();

        void FinishedProgress()
        {
            ResetStationProgress(); 
            GameScheduler.Instance.ScheduleNextFrame("SpaceStationConstructor.UpdateScreen", (System.Action<object>)(obj => TriggerScreen(obj)));
        }
        void TriggerScreen(object obj = null)
        {
            this.gameObject.Trigger((int)GameHashes.JettisonedLander);
        }

        public void ResetStationProgress()
        {
            ConstructionProgress = -1;
            DemolishingProgress = -1;
            this.gameObject.Trigger((int)GameHashes.JettisonedLander);
            //this.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().gameObject.Trigger((int)GameHashes.JettisonedLander);
            //DetailsScreen.Instance.Refresh(gameObject);
        }
        public void StartStationBuildProgress()
        {
            if (ConstructionProgress == -1)
                ConstructionProgress = 0;
        }
        public void StartStationDemolishProgress()
        {
            if (DemolishingProgress == -1)
            {
                Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
                var locationToCheck = component.Location;
                CurrentLocationDemolishTime = ((SpaceStation)SpaceStationManager.GetSpaceStationAtLocation(locationToCheck)).CurrentSpaceStationType.demolishingTime;
                DemolishingProgress = 0;
            }
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
