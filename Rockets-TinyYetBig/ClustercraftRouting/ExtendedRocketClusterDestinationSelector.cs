using KSerialization;
using Rockets_TinyYetBig.Docking;
using System;
using System.Collections.Generic;

namespace Rockets_TinyYetBig.ClustercraftRouting
{
    public enum RouteMode
    {
        undefined = 0,
        Circle = 1,
        Line = 2,
    }

    internal class ExtendedRocketClusterDestinationSelector : RocketClusterDestinationSelector
    {

        [MyCmpGet]
        Clustercraft clustercraft;

        [Serialize]
        public List<AxialI> RouteDestinations = new List<AxialI>();

        [Serialize]
        public int CurrentDestinationIndex = 0;

        [Serialize]
        public int CurrentRouteMode = (int)RouteMode.Circle;

        [Serialize]
        public bool GoingBackwards = false;

        public override void OnSpawn()
        {
            base.OnSpawn();
            InitRouting();

        }

        public void RemoveDestination(AxialI location)
        {
            var index = RouteDestinations.FindIndex(item => item == location);

            if (index != 1)
            {
                RouteDestinations.RemoveAt(index);
                if (index == CurrentDestinationIndex && GoingBackwards)
                {
                    ProceedToNextTarget();
                }
            }
        }

        public override void SetDestination(AxialI location)
        {
            Trigger(543433792, location);

            if (location == clustercraft.Location)
                return;


            if (!RouteDestinations.Contains(location))
            {
                RouteDestinations.Add(location);
            }
        }

        public void InitRouting()
        {
            if (!Repeat)
                return;

            if (RouteDestinations.Count > 0)
            {
                if (CurrentDestinationIndex >= RouteDestinations.Count)
                    CurrentDestinationIndex = 0;

                if (clustercraft.Location == CurrentTarget)
                {
                    ProceedToNextTarget();
                }

            }
        }
        public override void OnClusterLocationChanged(object data)
        {
            ClusterLocationChangedEvent clusterLocationChangedEvent = (ClusterLocationChangedEvent)data;
            if (clusterLocationChangedEvent.newLocation != CurrentTarget)
                return;


            GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.ClusterDestinationReached, null);
            if (m_repeat)
            {
                if (ShouldRocketWait())
                {
                    WaitForHarvestAndDocking();
                }
                else
                {
                    ProceedToNextTarget();
                }
            }
            //base.OnClusterLocationChanged(data);
        }

        public bool ShouldRocketWait()
        {
            if (CanRocketHarvest())
                return true;

            if (TryGetComponent<DockingSpacecraftHandler>(out var dockingSpacecraftHandler))
            {
                if (dockingSpacecraftHandler.IsLoading)
                {
                    return true;
                }
            }


            return false;
        }

        public void OnStorageChangeExtended(object data)
        {
            if (ShouldRocketWait())
            {
                return;
            }

            isHarvesting = false;
            foreach (Ref<RocketModuleCluster> clusterModule in clustercraft.ModuleInterface.ClusterModules)
            {
                if ((bool)clusterModule.Get().GetComponent<Storage>())
                {
                    Unsubscribe(clusterModule.Get().gameObject, -1697596308, OnStorageChangeExtended);
                    Unsubscribe(clusterModule.Get().gameObject, (int)GameHashes.OnParticleStorageChanged, OnStorageChangeExtended);
                }
            }

            ProceedToNextTarget();

        }


        public void WaitForHarvestAndDocking()
        {
            isHarvesting = true;
            foreach (Ref<RocketModuleCluster> clusterModule in clustercraft.ModuleInterface.ClusterModules)
            {
                if (clusterModule.Get().GetComponent<Storage>())
                {
                    Subscribe(clusterModule.Get().gameObject, -1697596308, OnStorageChangeExtended);
                }
                if (clusterModule.Get().GetComponent<HighEnergyParticleStorage>())
                {
                    Subscribe(clusterModule.Get().gameObject, (int)GameHashes.OnParticleStorageChanged, OnStorageChangeExtended);
                }
            }
        }


        internal void ProceedToNextTarget()
        {
            if (RouteDestinations.Count > 1)
            {

                CurrentDestinationIndex += GoingBackwards ? -1 : 1;

                if (CurrentDestinationIndex == RouteDestinations.Count || CurrentDestinationIndex < 0)
                {
                    switch (CurrentRouteMode)
                    {
                        case (int)RouteMode.Line:
                            CurrentDestinationIndex = GoingBackwards ? 0 : RouteDestinations.Count - 1;
                            GoingBackwards = !GoingBackwards;
                            break;
                        default:
                        case (int)RouteMode.Circle:
                            CurrentDestinationIndex = GoingBackwards ? RouteDestinations.Count - 1 : 0;
                            break;
                    }
                }
            }
            m_destination = CurrentTarget;
        }
        public AxialI CurrentTarget => RouteDestinations[CurrentDestinationIndex];

    }
}