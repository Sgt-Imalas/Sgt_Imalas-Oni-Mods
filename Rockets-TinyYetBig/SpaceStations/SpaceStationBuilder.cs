using Database;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Operational;
using static ProcGen.Mob;
using static Rockets_TinyYetBig.ModAssets;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.UI.STARMAP.LAUNCHCHECKLIST;

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
            if (this.Constructing())
            {
                return Math.Min((ConstructionProgress / ModAssets.SpaceStationTypes[CurrentSpaceStationTypeInt].constructionTime) * 100f, 100f);
            }
            else if (this.Demolishing())
            {
                return Math.Min((DemolishingProgress / CurrentLocationDemolishTime) * 100f, 100f);
            }
            return -1;
        }
        public float RemainingTime()
        {
            if (this.Constructing())
            {
                return Math.Max(ModAssets.SpaceStationTypes[CurrentSpaceStationTypeInt].constructionTime - ConstructionProgress,0);
            }
            else if (this.Demolishing())
            {
                return Math.Max(CurrentLocationDemolishTime - DemolishingProgress, 0);
            }
            return 0;
        }
        public bool HasResources(bool consumeMaterial = false)
        {
            if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
            {
                return true;
            }

            if (this.TryGetComponent<RocketModuleCluster>(out var rocketModuleCluster))
            {
                var cmi = rocketModuleCluster.CraftInterface;
                if(!cmi.HasCargoModule)
                    return false;

                var CargoBays = ListPool<CargoBayCluster, SpaceStationBuilder>.Allocate();
                foreach(var clusterModule in cmi.ClusterModules) 
                { 
                    if(clusterModule.Get().TryGetComponent<CargoBayCluster>(out var cargoBay)) 
                    {
                        CargoBays.Add(cargoBay);
                    }
                }
                var NeededMats = ModAssets.SpaceStationTypes[CurrentSpaceStationTypeInt].materials;
                foreach(CargoBayCluster cargoBayCluster in CargoBays)
                {
                    if (cargoBayCluster.storage.Count != 0)
                    {
                        for (int index = cargoBayCluster.storage.items.Count - 1; index >= 0; --index)
                        {
                            GameObject go = cargoBayCluster.storage.items[index];

                            foreach (var material in NeededMats.ToArray())
                            {
                                string cargoItemPrefabID = go.PrefabID().ToString();
                                if (NeededMats.Keys.Contains(cargoItemPrefabID))
                                {
                                    if ((double)NeededMats[cargoItemPrefabID] > 0.0)
                                    {
                                        go.TryGetComponent<Pickupable>(out var cargoItem);

                                        float itemMass = cargoItem.PrimaryElement.Mass > NeededMats[cargoItemPrefabID] ? NeededMats[cargoItemPrefabID] : cargoItem.PrimaryElement.Mass;

                                        NeededMats[cargoItemPrefabID] -= cargoItem.PrimaryElement.Mass;

                                        if (consumeMaterial)
                                        {
                                            go.GetComponent<Pickupable>().Take(itemMass);
                                        }
                                    }
                                    else
                                        break;
                                }
                            }

                        }
                        if (NeededMats.Values.Any(value => value > 0.0))
                            continue;
                        else
                            return true;
                    }
                }
                CargoBays.Dispose();
                return false;
            }
            Debug.LogError("Space station builder has no rocket module attached");
            return false;
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
                    HasResources(true);
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
                    if (worldId > -1)
                    {
                        SpaceStationManager.Instance.GetSpaceStationFromWorldId(worldId).DestroySpaceStation();
                    }
                    else if (worldId == -2)
                    {

                    }
                    FinishedProgress();
                }
            }
        }

        private void SpawnStation(AxialI location)
        {
            if (ModAssets.SpaceStationTypes[CurrentSpaceStationTypeInt].HasInterior)
            {
                Vector3 position = new Vector3(-1f, -1f, 0.0f);
                GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)SpaceStationConfig.ID), position);
                sat.SetActive(true);
                var spaceStation = sat.GetComponent<SpaceStation>();
                spaceStation.Location = location;
                spaceStation._currentSpaceStationType = CurrentSpaceStationTypeInt;
            }
        }
        public override void OnSpawn()
        {
            base.OnSpawn();
            this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().Subscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.ResetStation));
        }
        public override void OnCleanUp()
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
        public bool IsOccupyingPoiAtCurrentLocation()
        {
            Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
            var locationToCheck = component.Location;

            if(ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(locationToCheck, EntityLayer.Asteroid )!= null || ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(locationToCheck, EntityLayer.POI) != null)
                return true;

            return false;
        }

        public bool SpaceCellOccupied()
        {
            if( IsOccupyingPoiAtCurrentLocation() || IsStationAtCurrentLocation())
                return true;
            return false;
        }

        public bool IsStationAtCurrentLocation()
        {
            Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
            var locationToCheck = component.Location;

            int worldId = SpaceStationManager.GetSpaceStationWorldIdAtLocation(locationToCheck);
            return worldId != -1;
        }
        public bool CanDeconstructAtCurrentLocation()
        {
            Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
            var locationToCheck = component.Location;
            int worldId = SpaceStationManager.GetSpaceStationWorldIdAtLocation(locationToCheck);
            if (worldId == -1)
                return false;
            if (worldId == -2)
                return true;
            ///GetDupesInside==false
            foreach(MinionIdentity dupe in Components.MinionIdentities)
            {
                if (dupe.GetMyWorldId() == worldId)
                    return false;
            }
            return true;
        }

        internal bool CanConstructCurrentSpaceStation(out string reason)
        {
            reason = string.Empty;
            if(this.SpaceCellOccupied())
            {
                reason = "Hex Occupied.";
                return false;
            }
            else if (!SpaceStationManager.Instance.CanConstructSpaceStation())
            {
                reason = "station limit reached";
                return false;
            }
            else if (!this.HasResources())
            {
                reason = "missing resources";
                return false;
            }
            return true;
        }

        public Recipe GetCurrentCraftRecipe()
        {
            var station = ModAssets.SpaceStationTypes[CurrentSpaceStationTypeInt];
            var recipe = new Recipe(station.ID, nameOverride: station.Name);
            foreach(var ing in station.materials)
            {
                recipe.Ingredients.Add(new Recipe.Ingredient(ing.Key, ing.Value));
            }
            return recipe;// ModAssets.SpaceStationTypes[CurrentSpaceStationTypeInt]
        }
    }
}
