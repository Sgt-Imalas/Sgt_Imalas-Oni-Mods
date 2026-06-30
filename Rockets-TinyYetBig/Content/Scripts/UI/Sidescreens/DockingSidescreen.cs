using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Content.Scripts.UI.UIComponents;
using Rockets_TinyYetBig.Docking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static ProcGen.Mob;
using static Rockets_TinyYetBig.Docking.DockingSpacecraftHandler;
using static Rockets_TinyYetBig.STRINGS.UI;
using static UtilLibs.UIUtils;

namespace Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens
{
	class DockingSidescreen : SideScreenContent, IRender200ms
	{

		private CrewAssignmentSidescreen crewScreen;
		public override void OnSpawn()
		{
			base.OnSpawn();

			ConnectReference();
			ClusterManager.Instance.Subscribe(ModAssets.Hashes.DockableAdded, new Action<object>(ManagerAddedHandler));
			ClusterManager.Instance.Subscribe(ModAssets.Hashes.DockableRemoved , new System.Action<object>(this.ManagerRemovedHandler));

		}

		private void ManagerAddedHandler(object obj)
		{
			if (obj is IDockable mng)
			{
				SgtLogger.l("OnDockableAdded: " + obj); 
				if (!DockingTargets.ContainsKey(mng.spacecraftHandler))
				{
					AddOrGetRowEntry(mng.spacecraftHandler);
					DelayedRefresh();
				}
				else
				{
					Debug.LogWarning("already had DockingManager");
				}
			}
		}
		private void ManagerRemovedHandler(object obj)
		{
			if (obj is IDockable mng)
			{
				SgtLogger.l("OnDockableRemoved: " + obj);
				if (DockingTargets.ContainsKey(mng.spacecraftHandler))
				{
					DelayedRefresh();
				}
			}
		}
		//private void ManagerRemovedHandler(object obj)
		//{
		//    if (obj is DockingManager mng)
		//    {
		//        if (!DockingTargets.ContainsKey(mng))
		//        {
		//            AddRowEntry(mng);
		//            Refresh();
		//        }
		//        else
		//        {
		//            Debug.LogWarning("already had DockingManager");
		//        }

		//    }

		//}



		protected DockingSpacecraftHandler targetSpacecraftHandler;
		protected Clustercraft targetCraft;
		protected DockingDoor targetDoor;

		protected GameObject rowPrefab;
		protected GameObject listContainer;
		protected LocText headerLabel;
		//[SerializeField]
		//private GameObject noChannelRow;
		private Dictionary<DockingSpacecraftHandler, DockingHandlerEntry> DockingTargets = new();
		private Dictionary<DockingSpacecraftHandler, RectTransform> Rotatings = new Dictionary<DockingSpacecraftHandler, RectTransform>();


		private List<int> refreshHandles = new List<int>();


		public override float GetSortKey() => 21f;
		public override bool IsValidForTarget(GameObject target)
		{
			if (!target.TryGetComponent<DockingSpacecraftHandler>(out var manager)
				&& target.TryGetComponent(out DockingDoor door))
			{
				manager = door.spacecraftHandler;
			}
			if (manager == null)
			{
				return false;
			}

			var spaceship = manager.clustercraft;

			bool flying = spaceship != null ? spaceship.Status == Clustercraft.CraftStatus.InFlight : false;

			return manager != null && manager.HasDoors()
				//&& manager.CraftType != DockableType.Derelict 
				&& flying
				&& !RocketryUtils.IsRocketTraveling(spaceship);
		}
		public override void ClearTarget()
		{
			//SgtLogger.l("clearing Target");
			foreach (int id in refreshHandles)
				targetCraft.Unsubscribe(id);
			refreshHandles.Clear();
			targetSpacecraftHandler = null;
			targetDoor = null;
			targetCraft = null;
			base.ClearTarget();
		}

		public override void SetTarget(GameObject target)
		{

			if (target != null)
			{
				foreach (int id in refreshHandles)
					target.Unsubscribe(id);
				refreshHandles.Clear();
			}

			SgtLogger.l("setting Target");
			base.SetTarget(target);

			ConnectReference();
			target.TryGetComponent(out targetSpacecraftHandler); ///??? revisit
			target.TryGetComponent(out targetCraft);
			target.TryGetComponent(out targetDoor);
			if (targetDoor != null)
			{
				targetSpacecraftHandler = targetDoor.spacecraftHandler;
			}
			targetCraft = targetSpacecraftHandler.clustercraft;

			Build();
			refreshHandles.Add(targetCraft.gameObject.Subscribe((int)GameHashes.ClusterDestinationChanged, new Action<object>(RefreshAll)));
			refreshHandles.Add(targetCraft.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new Action<object>(RefreshAll)));
			refreshHandles.Add(targetSpacecraftHandler.gameObject.Subscribe((int)ModAssets.Hashes.DockingConnectionChanged, new Action<object>(RefreshAll)));

			Refresh();
		}

		private void RefreshAll(object data = null) => DelayedRefresh();
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			//titleKey = "STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.TITLE";
		}
		public override string GetTitle()
		{
			return DOCKINGSCREEN.TITLE.TITLETEXT;
		}

		bool refsConnected = false;
		void ConnectReference()
		{
			if (refsConnected)
				return;
			refsConnected = true;

			rowPrefab = transform.Find("OwnDupesContainer/ScrollRectContainer/ItemPrefab").gameObject;
			listContainer = transform.Find("OwnDupesContainer/ScrollRectContainer").gameObject;
			//var layout = transform.Find("Content/ContentScrollRect/Scrollbar").GetComponent<LayoutElement>();
			//SgtLogger.debuglog(String.Format("{0}, {1}, {2}", layout.minHeight, layout.preferredHeight, layout.flexibleHeight));
			//layout.minHeight = 150f;
			rowPrefab.gameObject.SetActive(false);
			headerLabel = TryFindComponent<LocText>(transform.Find("DockingBridges/TitleText"));
			//noChannelRow = transform.Find("Content/ContentScrollRect/RowContainer/Rows/NoChannelRow").gameObject;

			//TryChangeText(noChannelRow.transform, "Labels/Label", "Nothing");
			//TryChangeText(noChannelRow.transform, "Labels/DescLabel", "Nothing to dock to.");

		}

		private void Build()
		{
			SgtLogger.l("build docking ui");
			foreach (var uirow in DockingTargets)
			{
				uirow.Value.gameObject.SetActive(false);
			}

			if (targetSpacecraftHandler == null || headerLabel == null)
				return;
			//headerLabel.SetText("Docking Ports: " + targetSpacecraftHandler.GetUiDoorInfo());
			//SgtLogger.l(location.Q + "," + location.R + " <- count here: " + AllDockers.Count);
			//noChannelRow.SetActive(AllDockers.Count == 0);

			Refresh();
			DelayedRefresh();
		}

		IEnumerator DelayedRefreshCor()
		{
			yield return null;
			Refresh();
		}
		void DelayedRefresh()
		{
			if(this.isActiveAndEnabled)
				StartCoroutine(DelayedRefreshCor());
		}
		void ToggleDockingProcess(DockingSpacecraftHandler target)
		{
			DockingManagerSingleton.Instance.TryInitializingDockingBetweenHandlers(targetSpacecraftHandler, target, targetDoor);
		}
		void ToggleUndockingProcess(DockingSpacecraftHandler target)
		{
			DockingManagerSingleton.Instance.TryInitializingUndockingBetweenHandlers(targetSpacecraftHandler, target, targetDoor);
		}


		void ToggleCrewScreen(DockingSpacecraftHandler target)
		{
			if (crewScreen == null)
			{
				if (DockingManagerSingleton.Instance.HandlersConnected(targetSpacecraftHandler, target, out IDockable first, out IDockable second))
				{
					crewScreen = (CrewAssignmentSidescreen)DetailsScreen.Instance.SetSecondarySideScreen(ModAssets.DupeTransferSecondarySideScreen, DOCKINGTRANSFERSCREEN.TITLE.TITLETEXT);

					if (first.spacecraftHandler != targetSpacecraftHandler)
						crewScreen.UpdateForConnection(second.WorldId, first.WorldId);
					else
						crewScreen.UpdateForConnection(first.WorldId, second.WorldId);
				}
				else
					SgtLogger.l("not connected: " + targetSpacecraftHandler.GetProperName() + " & " + target.GetProperName());
			}
			else
			{
				ClearSecondarySideScreen();
			}
		}
		public override void OnShow(bool show)
		{
			base.OnShow(show);

			if (!show)
			{
				ClearSecondarySideScreen();
			}
		}
		void ClearSecondarySideScreen()
		{
			DetailsScreen.Instance.ClearSecondarySideScreen();
			crewScreen = null;
		}

		private DockingHandlerEntry AddOrGetRowEntry(DockingSpacecraftHandler referencedManager, bool startActive = true)
		{
			if (!DockingTargets.TryGetValue(referencedManager, out DockingHandlerEntry target))
			{

				GameObject RowEntry = Util.KInstantiateUI(rowPrefab, listContainer);
				var cmp = RowEntry.AddOrGet<DockingHandlerEntry>();
				cmp.Init();
				cmp.Target = referencedManager;
				///ListAllChildren(RowEntry.transform);
				RowEntry.SetActive(true);
				RowEntry.name = referencedManager.GetProperName();
				Debug.Assert(!DockingTargets.ContainsKey(referencedManager), "Adding two of the same DockingManager to DockingSideScreen UI: " + referencedManager.gameObject.GetProperName());

				var SpaceShipIconRotatabe = RowEntry.transform.Find("Row1/WaitContainer/loading").rectTransform();
				SpaceShipIconRotatabe.gameObject.SetActive(false);

				Rotatings[referencedManager] = SpaceShipIconRotatabe;

				cmp.DockClicked = () =>
				{
					ClearSecondarySideScreen();
					ToggleDockingProcess(referencedManager);
					DelayedRefresh();
				};

				cmp.UndockClicked += () =>
				{
					ClearSecondarySideScreen();
					ToggleUndockingProcess(referencedManager);
					Refresh();
				};
				cmp.TransferClicked += () =>
				{
					ToggleCrewScreen(referencedManager);
				};

				cmp.ViewOtherClicked += () =>
				{
					if (referencedManager != null && DockingManagerSingleton.Instance.HandlersConnected(referencedManager, targetSpacecraftHandler, out var firstDock, out var secondDock))
					{
						cmp.ViewOther.SetInteractable(secondDock.HasDupeTeleporter);
						ClusterManager.Instance.SetActiveWorld(firstDock.WorldId);
						if (ClusterMapScreen.Instance != null && ClusterMapScreen.Instance.gameObject.activeSelf && ManagementMenu.Instance != null)
							ManagementMenu.Instance.ToggleClusterMap();
						SelectTool.Instance.Activate();
					}
				};
				DockingTargets.Add(referencedManager, cmp);
				target = cmp;
			}
			target.gameObject.SetActive(startActive);
			return target;
		}

		private void Refresh()
		{
			if (targetSpacecraftHandler == null)
			{
				SgtLogger.l("Skipping refresh");
				return;
			}
			// SgtLogger.l("refreshing docking screen, target go count: "+DockingTargets.Count() );
			headerLabel.SetText(string.Format(DOCKINGSCREEN.DOCKINGBRIDGES.TITLETEXT, targetSpacecraftHandler.AvailableConnections(), targetSpacecraftHandler.TotalConnections()));

			var location = targetSpacecraftHandler.clustercraft.Location;
			var allDockers = DockingManagerSingleton.Instance.GetAllAvailableDockingHandlersAtPosition(location);

			SgtLogger.l(location.Q + "," + location.R + " <- count here: " + allDockers.Count);
			foreach (var dock in allDockers)
				SgtLogger.l(dock + " is station? " + dock.IsSpaceStation + ", is rocketInMove: " + (dock.CraftType == DockableType.Rocket && RocketryUtils.IsRocketTraveling(dock.clustercraft)));

			if (targetSpacecraftHandler.IsSpaceStation)
				allDockers.RemoveAll(x => x.IsSpaceStation);
			allDockers.Remove(targetSpacecraftHandler);
			allDockers.RemoveAll(x => x.CraftType == DockableType.Rocket && RocketryUtils.IsRocketTraveling(x.clustercraft));


			foreach (var manager in allDockers)
			{
				var logic = AddOrGetRowEntry(manager);
				// SgtLogger.l("refreshing " + kvp.Key.GetProperName());

				bool currentlyConnected = DockingManagerSingleton.Instance.HandlersConnected(targetSpacecraftHandler, manager, out var firstDock, out var secondDock);

				bool CanDock = targetSpacecraftHandler.AvailableConnections() > 0 && manager.AvailableConnections() > 0 && !currentlyConnected;
				bool CanUnDock = currentlyConnected && !DockingManagerSingleton.Instance.HasPendingUndocks(firstDock.GUID) && !DockingManagerSingleton.Instance.HasPendingUndocks(secondDock.GUID);

				bool canViewInterior =
					 CanUnDock
					&& secondDock.HasDupeTeleporter
					;

				bool currentlyUndocking = currentlyConnected && (DockingManagerSingleton.Instance.HasPendingUndocks(firstDock.GUID, secondDock.GUID));
				bool currentlyDocking = DockingManagerSingleton.Instance.HasPendingDocks(manager);

				logic.SetIsDockable(CanDock,CanUnDock);
				logic.SetHasInterior(canViewInterior);
				if (Rotatings.ContainsKey(manager))
					Rotatings[manager].gameObject.SetActive(currentlyUndocking|| currentlyDocking);
			}
		}


		public void Render200ms(float dt)
		{
			foreach (var rotatable in Rotatings.Values)
			{
				rotatable.Rotate(new Vector3(0, 0, 25));
			}
		}
	}
}
