using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Docking
{
    public class DockingSpacecraftHandler : KMonoBehaviour
    {
        [MyCmpGet] public WorldContainer world;
        [MyCmpGet] public Clustercraft clustercraft;
        public Dictionary<string, IDockable> WorldDockables = new Dictionary<string, IDockable>();
        public PassengerRocketModule PassengerModule;


        bool isLoading = false;

        public System.Action OnFinishedLoading = null;
        public bool IsLoading => isLoading;
        public int worldId;

        [MyCmpGet]
        public CraftModuleInterface Interface;


        DockableType Type = DockableType.Rocket;

        public DockableType GetCraftType => Type;

        public int WorldId => world.id;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            DockingManagerSingleton.Instance.RegisterSpacecraftHandler(this);

        }
        public override void OnCleanUp()
        {
            DockingManagerSingleton.Instance.UnregisterSpacecraftHander(this);
            base.OnCleanUp();
        }
        public void SetCurrentlyLoadingStuff(bool IsLoading)
        {
            isLoading = IsLoading;

            if (!IsLoading && OnFinishedLoading != null)
                OnFinishedLoading.Invoke();
        }

        internal bool CanDock()
        {
            bool cando =
                HasDoors()
                && AvailableConnections() > 0
                && (clustercraft.status == Clustercraft.CraftStatus.InFlight);
            return cando;
        }

        internal void RegisterDockable(IDockable dockable)
        {
            WorldDockables.Add(dockable.GUID, dockable);
            SgtLogger.l(gameObject.GetProperName() + " total dockable count: " + WorldDockables.Count);
        }

        internal void UnregisterDockable(IDockable dockable)
        {
            WorldDockables.Remove(dockable.GUID);
        }

        internal void UndockAll()
        {
            foreach (var dockable in WorldDockables.Keys)
            {
                if (DockingManagerSingleton.Instance.IsDocked(dockable, out var dockedTo))
                {
                    DockingManagerSingleton.Instance.AddPendingUndock(dockable, dockedTo);
                }
            }
        }
        public Sprite GetDockingIcon()
        {
            Sprite returnVal = null;
            switch (Type)
            {
                case DockableType.Rocket:
                case DockableType.SpaceStation:
                    returnVal = clustercraft.GetUISprite();
                    break;
                case DockableType.Derelict:
                // break;

                default:
                    returnVal = Assets.GetSprite("unknown");
                    break;
            }
            return returnVal;

        }

        public List<int> GetConnectedWorlds()
        {
            var list = new List<int>();
            foreach (var door in WorldDockables)
            {
                if (DockingManagerSingleton.Instance.TryGetDockableIfDocked(door.Value.GUID, out var dockedTo))
                    list.Add(dockedTo.WorldId);

            }
            return list;
        }
        public List<int> GetConnectedRockets()
        {
            var list = new List<int>();
            foreach (var door in WorldDockables)
            {
                if (DockingManagerSingleton.Instance.TryGetDockableIfDocked(door.Value.GUID, out var dockedTo) && SpaceStationManager.WorldIsRocketInterior(dockedTo.WorldId))
                    list.Add(dockedTo.WorldId);

            }
            return list;
        }

        internal List<IDockable> GetCurrentDocks()
        {
            var list = new List<IDockable>();
            foreach (var door in WorldDockables)
            {
                if (DockingManagerSingleton.Instance.TryGetDockableIfDocked(door.Value.GUID, out var dockedTo) && SpaceStationManager.WorldIsRocketInterior(dockedTo.WorldId))
                    list.Add(dockedTo);

            }
            return list;
        }
        public int AvailableConnections()
        {
            int count = WorldDockables.Values.ToList().FindAll(k => !DockingManagerSingleton.Instance.IsDocked(k.GUID, out _)).Count();
            return count;
        }
        public int TotalConnections()
        {
            int count = WorldDockables.Count();
            return count;
        }


        internal bool HasDoors()
        {
            return WorldDockables.Count > 0;
        }

        public enum DockableType
        {
            undefined = 0,
            Rocket = 1,
            SpaceStation = 2,
            Derelict = 3
        }

    }
}
