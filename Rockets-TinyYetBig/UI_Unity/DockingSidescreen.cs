using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Docking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static Rockets_TinyYetBig.Docking.DockingSpacecraftHandler;
using static Rockets_TinyYetBig.STRINGS.UI;
using static UtilLibs.UIUtils;

namespace Rockets_TinyYetBig.UI_Unity
{
	class DockingSidescreen : SideScreenContent, IRender200ms
	{

		private CrewAssignmentSidescreen crewScreen;
		public override void OnSpawn()
		{
			base.OnSpawn();

			ConnectReference();
			ClusterManager.Instance.Subscribe(ModAssets.Hashes.DockableAdded, new System.Action<object>(this.ManagerAddedHandler));
			//ClusterManager.Instance.Subscribe(ModAssets.Hashes.DockingManagerRemoved , new System.Action<object>(this.ManagerRemovedHandler));

		}

		private void ManagerAddedHandler(object obj)
		{
			GameScheduler.Instance.ScheduleNextFrame("refreshUI", (oj =>
			{
				if (obj is IDockable mng)
				{
					if (!DockingTargets.ContainsKey(mng.spacecraftHandler))
					{
						AddRowEntry(mng.spacecraftHandler);
						Refresh();
					}
					else
					{
						Debug.LogWarning("already had DockingManager");
					}

				}
			})
			);

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
		private Dictionary<DockingSpacecraftHandler, GameObject> DockingTargets = new Dictionary<DockingSpacecraftHandler, GameObject>();
		private Dictionary<DockingSpacecraftHandler, RectTransform> Rotatings = new Dictionary<DockingSpacecraftHandler, RectTransform>();


		private List<int> refreshHandle = new List<int>();


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

			return manager != null && manager.HasDoors() && manager.CraftType != DockableType.Derelict && flying && !RocketryUtils.IsRocketTraveling(spaceship);
		}
		public override void ClearTarget()
		{
			//SgtLogger.l("clearing Target");
			foreach (int id in refreshHandle)
				targetCraft.Unsubscribe(id);
			refreshHandle.Clear();
			targetSpacecraftHandler = null;
			targetDoor = null;
			targetCraft = null;
			base.ClearTarget();
		}

		public override void SetTarget(GameObject target)
		{

			if (target != null)
			{
				foreach (int id in refreshHandle)
					target.Unsubscribe(id);
				refreshHandle.Clear();
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
			refreshHandle.Add(targetCraft.gameObject.Subscribe((int)GameHashes.ClusterDestinationChanged, new Action<object>(RefreshAll)));
			refreshHandle.Add(targetCraft.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new Action<object>(RefreshAll)));
			refreshHandle.Add(targetSpacecraftHandler.gameObject.Subscribe((int)ModAssets.Hashes.DockingConnectionChanged, new Action<object>(RefreshAll)));

			Refresh();
		}

		private void RefreshAll(object data = null) => DelayedRefresh();
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			//titleKey = "STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.TITLE";
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
				uirow.Value.SetActive(false);
			}

			if (targetSpacecraftHandler == null || headerLabel == null)
				return;
			var location = targetSpacecraftHandler.clustercraft.Location;
			//headerLabel.SetText("Docking Ports: " + targetSpacecraftHandler.GetUiDoorInfo());
			var AllDockers = DockingManagerSingleton.Instance.GetAllAvailableDockingHandlersAtPosition(location);
			SgtLogger.l(location.Q + "," + location.R + " <- count here: " + AllDockers.Count);
			if (targetSpacecraftHandler.CraftType == DockableType.SpaceStation)
			{
				AllDockers.RemoveAll(craft => craft.CraftType == DockableType.SpaceStation);
			}

			foreach (var targetSpacecraftHandler in AllDockers)
			{
				if (!DockingTargets.ContainsKey(targetSpacecraftHandler))
				{
					AddRowEntry(targetSpacecraftHandler);
				}

				DockingTargets[targetSpacecraftHandler].SetActive(true);
			}


			//noChannelRow.SetActive(AllDockers.Count == 0);

			Refresh();
			DelayedRefresh();
		}

		void DelayedRefresh()
		{
			GameScheduler.Instance.ScheduleNextFrame("dockingUiRefresh", (d) => Refresh());
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
			this.crewScreen = null;
		}

		private void AddRowEntry(DockingSpacecraftHandler referencedManager, bool startActive = true)
		{
			GameObject RowEntry = Util.KInstantiateUI(rowPrefab, listContainer);
			///ListAllChildren(RowEntry.transform);
			RowEntry.name = referencedManager.GetProperName();
			RowEntry.transform.Find("Row1/TitleText").gameObject.GetComponent<LocText>().SetText(referencedManager.GetProperName());
			RowEntry.transform.Find("Row1/SpaceCraftIcon/Image").GetComponent<Image>().sprite = referencedManager.GetDockingIcon();


			Debug.Assert(!DockingTargets.ContainsKey(referencedManager), "Adding two of the same DockingManager to DockingSideScreen UI: " + referencedManager.gameObject.GetProperName());

			var DockButton = RowEntry.transform.Find("Row1/Dock").gameObject.AddComponent<FButton>();
			var UndockButton = RowEntry.transform.Find("Row1/Undock").gameObject.AddComponent<FButton>();
			var TransferButton = RowEntry.transform.Find("Row2/TransferButton").gameObject.AddComponent<FButton>();
			var ViewDockedButton = RowEntry.transform.Find("Row2/ViewDockedButton").gameObject.AddComponent<FButton>();

			var SpaceShipIconRotatabe = RowEntry.transform.Find("Row1/WaitContainer/loading").rectTransform();
			SpaceShipIconRotatabe.gameObject.SetActive(false);

			Rotatings[referencedManager] = SpaceShipIconRotatabe;

			DockButton.OnClick += () =>
			{
				ClearSecondarySideScreen();
				ToggleDockingProcess(referencedManager);
				Refresh();
			};

			UndockButton.OnClick += () =>
			{
				ClearSecondarySideScreen();
				ToggleUndockingProcess(referencedManager);
				Refresh();
			};
			TransferButton.OnClick += () =>
			{
				ToggleCrewScreen(referencedManager);
			};

			ViewDockedButton.OnClick += () =>
			{
				if (referencedManager != null && DockingManagerSingleton.Instance.HandlersConnected(referencedManager, targetSpacecraftHandler, out var firstDock, out var secondDock))
				{
					ViewDockedButton.SetInteractable(secondDock.HasDupeTeleporter);
					ClusterManager.Instance.SetActiveWorld(firstDock.WorldId);
					SelectTool.Instance.Activate();
				}
			};

			DockingTargets.Add(referencedManager, RowEntry);
			RowEntry.SetActive(startActive);
		}

		private void Refresh()
		{
			if (targetSpacecraftHandler == null)
			{
				SgtLogger.l("Skipping refresh");
				return;
			}
			// SgtLogger.l("refreshing docking screen, target go count: "+DockingTargets.Count() );
			headerLabel.SetText(string.Format(STRINGS.UI.DOCKINGSCREEN.DOCKINGBRIDGES.TITLETEXT, targetSpacecraftHandler.AvailableConnections(), targetSpacecraftHandler.TotalConnections()));

			foreach (var kvp in DockingTargets)
			{
				//if (!kvp.Value.activeInHierarchy)
				//    continue;

				// SgtLogger.l("trying refreshing " + kvp.Key.GetProperName());

				var manager = kvp.Key;

				if (manager == targetSpacecraftHandler)
				{
					SgtLogger.l(kvp.Key.GetProperName() + " was target, skipping");
					kvp.Value.SetActive(false);
					// SgtLogger.l("1");
					continue;
				}
				if (manager.CraftType == DockableType.Rocket && RocketryUtils.IsRocketTraveling(manager.clustercraft))
				{
					kvp.Value.SetActive(false);
					// SgtLogger.l("2");

					continue;
				}
				if (manager.clustercraft.Location != targetSpacecraftHandler.clustercraft.Location)
				{
					kvp.Value.SetActive(false);
					//SgtLogger.l("3");
					continue;
				}

				kvp.Value.SetActive(true);
				// SgtLogger.l("refreshing " + kvp.Key.GetProperName());


				var DockButton = kvp.Value.transform.Find("Row1/Dock").gameObject.GetComponent<FButton>();
				var UndockButton = kvp.Value.transform.Find("Row1/Undock").gameObject.GetComponent<FButton>();
				var TransferButton = kvp.Value.transform.Find("Row2/TransferButton").gameObject.GetComponent<FButton>();
				var ViewDockedButton = kvp.Value.transform.Find("Row2/ViewDockedButton").gameObject.GetComponent<FButton>();


				bool currentlyConnected = DockingManagerSingleton.Instance.HandlersConnected(targetSpacecraftHandler, manager, out var firstDock, out var secondDock);

				bool CanDock = targetSpacecraftHandler.AvailableConnections() > 0 && manager.AvailableConnections() > 0 && !currentlyConnected;
				DockButton.SetInteractable(CanDock);

				bool CanUnDock = currentlyConnected && !DockingManagerSingleton.Instance.HasPendingUndocks(firstDock.GUID) && !DockingManagerSingleton.Instance.HasPendingUndocks(secondDock.GUID);

				UndockButton.SetInteractable(CanUnDock);

				bool canViewInterior =
					 CanUnDock
					&& secondDock.HasDupeTeleporter
					;

				ViewDockedButton.SetInteractable(canViewInterior);
				if (Rotatings.ContainsKey(manager))
					Rotatings[manager].gameObject.SetActive(currentlyConnected && (DockingManagerSingleton.Instance.HasPendingUndocks(firstDock.GUID) || DockingManagerSingleton.Instance.HasPendingUndocks(secondDock.GUID)));
				TransferButton.SetInteractable(canViewInterior);
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
