using Rockets_TinyYetBig.ClustercraftRouting;
using Rockets_TinyYetBig.Derelicts;
using Rockets_TinyYetBig.SpaceStations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Docking
{
	public class DockingSpacecraftHandler : KMonoBehaviour
	{
		[MyCmpGet] public WorldContainer world;
		[MyCmpGet] public Clustercraft clustercraft;
		[MyCmpGet] public RocketClusterDestinationSelector destinationSelector;
		public Dictionary<string, IDockable> WorldDockables = new Dictionary<string, IDockable>();
		//public PassengerRocketModule PassengerModule;

		public bool IsSpaceStation => Type == DockableType.SpaceStation;

		bool isLoading = false;

		//public System.Action OnFinishedLoading = null;
		public bool IsLoading => isLoading;

		[MyCmpGet]
		public CraftModuleInterface Interface;

		DockableType Type = DockableType.Rocket;

		public bool IsRocket => Type == DockableType.Rocket;
		public DockableType CraftType => Type;

		public int WorldId => world?.id ?? -1;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();

		}
		public override void OnCleanUp()
		{
			DockingManagerSingleton.Instance.UnregisterSpacecraftHander(this);

			foreach (var handler in _handlers)
				Unsubscribe(handler);
			base.OnCleanUp();
		}
		List<int> _handlers = [];
		public override void OnSpawn()
		{
			base.OnSpawn();
			if(WorldId != -1)
				DockingManagerSingleton.Instance.RegisterSpacecraftHandler(this);
			if (clustercraft is SpaceStation)
				Type = DockableType.SpaceStation;
			if (clustercraft is DerelictStation)
				Type = DockableType.Derelict;
			//else
			//{
			//    GameScheduler.Instance.ScheduleNextFrame("CheckIfWaiting", (_) => UpdateWaitingStatus());
			//}
			SgtLogger.l("DockingHandler " + gameObject.name + " OnSpawn. Type: " + Type);
			if(Type == DockableType.Rocket)
			{
				_handlers.Add(Subscribe((int)GameHashes.ClusterDestinationChanged, OnClusterDestinationChanged));
				_handlers.Add(Subscribe((int)GameHashes.RocketModuleChanged, OnRocketModulesChanged));
			}
		}
		void OnRocketModulesChanged(object _)
		{
			StartCoroutine(RefreshWorldStateDelayed());
		}

		IEnumerator RefreshWorldStateDelayed()
		{
			///wait 2 frames for world creation one frame after module addition
			yield return null;
			yield return null;

			world = GetComponent<WorldContainer>();
			if (world == null)
				DockingManagerSingleton.Instance.UnregisterSpacecraftHander(this);
			else
				DockingManagerSingleton.Instance.RegisterSpacecraftHandler(this);
		}

		void OnClusterDestinationChanged(object boxed)
		{
			SgtLogger.l("DockingSpaceCraftHandler.OnClusterDestinationChanged");
			var myDestination = destinationSelector.GetDestination();
			foreach (var docked in WorldDockables)
			{
				if (!DockingManagerSingleton.Instance.TryGetDockableIfDocked(docked.Value.GUID, out var dockedDockable))
					continue;

				if (SpaceStationManager.WorldIsSpaceStationInterior(dockedDockable.WorldId))
				{
					DockingManagerSingleton.Instance.AddPendingUndock(docked.Value.GUID, dockedDockable.GUID);
				}
				else
				{
					var selector = dockedDockable.spacecraftHandler.clustercraft.ModuleInterface.GetClusterDestinationSelector();
					if (selector.GetDestination() != myDestination)
					{
						selector.SetDestination(myDestination);
					}
				}
			}

		}

		public void SetCurrentlyLoadingStuff(bool IsLoading)
		{
			isLoading = IsLoading;
			if (destinationSelector == null)
				return;

			SgtLogger.l("setting loading: " + IsLoading);
			if (!IsLoading && destinationSelector.Repeat)
			{
				UndockAll();
				if (destinationSelector is ExtendedRocketClusterDestinationSelector Extended)
				{
					Extended.ProceedToNextTarget();
				}
				else
				{
					destinationSelector.SetUpReturnTrip();
				}
			}
		}

		public bool InSpace => world != null ? world.ParentWorldId == world.id : clustercraft.status == Clustercraft.CraftStatus.InFlight;
		internal bool CanDock()
		{
			bool cando =
				HasDoors()
				&& AvailableConnections() > 0
				&& InSpace;
			if (Type == DockableType.Rocket)
				cando = cando && !RocketryUtils.IsRocketTraveling(clustercraft);

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
			DockingManagerSingleton.Instance.RemoveToStationDock(WorldId);
		}
		public Sprite GetDockingIcon()
		{
			Sprite returnVal = null;
			switch (Type)
			{
				case DockableType.SpaceStation:
					returnVal = clustercraft.GetUISprite();
					break;
				case DockableType.Rocket:
				case DockableType.Derelict:
					returnVal = clustercraft.GetUISprite();
					break;
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
