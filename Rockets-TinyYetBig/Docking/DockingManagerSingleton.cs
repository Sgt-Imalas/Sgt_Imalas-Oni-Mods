using KSerialization;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static Rockets_TinyYetBig.ModAssets;

namespace Rockets_TinyYetBig.Docking
{
    internal class DockingManagerSingleton : KMonoBehaviour, ISim1000ms
    {
        public static DockingManagerSingleton Instance { get; set; }

        [Serialize]
        private Dictionary<string, string> DockingConnections = new Dictionary<string, string>();

        [Serialize] public Dictionary<string, System.Action> PendingUndockUIRefreshActions = new Dictionary<string, System.Action>();

        [Serialize] public Dictionary<string, string> PendingUndocks = new Dictionary<string, string>();
        [Serialize] public Dictionary<string, string> PendingDocks = new Dictionary<string, string>();
        [Serialize] public HashSet<string> PendingDockBlockers = new HashSet<string>();
        [Serialize] public HashSet<string> PendingUndockBlockers = new HashSet<string>();

        private Dictionary<string, AssignmentGroupController> MinionAssignmentGroupControllers = new Dictionary<string, AssignmentGroupController>();
        public Dictionary<string, IDockable> IDockables = new Dictionary<string, IDockable>();
        public HashSet<DockingSpacecraftHandler> DockingSpacecraftHandlers = new HashSet<DockingSpacecraftHandler>();


        public void RegisterSpacecraftHandler(DockingSpacecraftHandler handler)
        {
            if (!DockingSpacecraftHandlers.Contains(handler))
                DockingSpacecraftHandlers.Add(handler);
        }
        public void UnregisterSpacecraftHander(DockingSpacecraftHandler handler)
        {
            if (DockingSpacecraftHandlers.Contains(handler))
                DockingSpacecraftHandlers.Remove(handler);
        }
        public void RegisterDockable(IDockable dockable)
        {
            IDockables[dockable.GUID] = dockable;
            if (!DockingConnections.ContainsKey(dockable.GUID))
                DockingConnections.Add(dockable.GUID, null);

        }
        public void UnregisterDockable(IDockable dockable)
        {
            if (IsDocked(dockable.GUID, out var dockedTo))
            {
                SgtLogger.l("was docked");
                FinalizeUndocking(dockable.GUID, dockedTo, true);
                if(TryGetDockable(dockedTo, out var dockedToDockable))
                {
                    dockedToDockable.gameObject.Trigger(ModAssets.Hashes.DockingConnectionChanged);
                }

            }
            else
                SgtLogger.l("not docked");

            IDockables.Remove(dockable.GUID);
        }

        public bool AddPendingDock(string first, string second)
        {
            if (PendingUndocks.ContainsKey(first) || PendingUndockBlockers.Contains(first))
            {
                SgtLogger.warning(first + " is already on the list of pending undocks");
                return false;
            }
            if (PendingUndocks.ContainsKey(second) || PendingUndockBlockers.Contains(second))
            {
                SgtLogger.warning(second + " is already on the list of pending undocks");
                return false;
            }
            if (PendingDocks.ContainsKey(first) || PendingDockBlockers.Contains(first))
            {
                SgtLogger.warning(first + " is already on the list of pending docks");
                return false;
            }
            if (PendingDocks.ContainsKey(second) || PendingDockBlockers.Contains(second))
            {
                SgtLogger.warning(second + " is already on the list of pending docks");
                return false;
            }
            SgtLogger.l("adding pending dock: " + first + "<->" + second);
            PendingDocks.Add(first, second);
            PendingDockBlockers.Add(first);
            PendingDockBlockers.Add(second);
            return true;
        }
        public bool AddPendingUndock(string first, string second, System.Action UiRefreshActionOnFinished = null)
        {
            if (PendingDocks.ContainsKey(first) || PendingDockBlockers.Contains(first))
            {
                SgtLogger.warning(first + " is already on the list of pending docks");
                return false;
            }
            if (PendingDocks.ContainsKey(second) || PendingDockBlockers.Contains(second))
            {
                SgtLogger.warning(second + " is already on the list of pending docks");
                return false;
            }
            if (PendingUndocks.ContainsKey(first) || PendingUndockBlockers.Contains(first))
            {
                SgtLogger.warning(first + " is already on the list of pending undocks");
                return false;
            }
            if (PendingUndocks.ContainsKey(second) || PendingUndockBlockers.Contains(second))
            {
                SgtLogger.warning(second + " is already on the list of pending undocks");
                return false;
            }
            if (DockingConnections[first] != second || DockingConnections[second] != first)
            {
                SgtLogger.l(first + " and " + second + " were not docked");
                return false;
            }
            SgtLogger.l("adding pending undock between" + first + " and " + second);

            PendingUndocks.Add(first, second);
            PendingUndockBlockers.Add(first);
            PendingUndockBlockers.Add(second);
            if (UiRefreshActionOnFinished != null)
            {
                PendingUndockUIRefreshActions[first] = UiRefreshActionOnFinished;
                PendingUndockUIRefreshActions[second] = UiRefreshActionOnFinished;
            }
            return true;
        }


        

        public bool IsDocked(string dockableGUID, out string dockedTo)
        {
            dockedTo = null;
            if (dockableGUID != null && dockableGUID.Length > 0 && DockingConnections.ContainsKey(dockableGUID))
            {
                dockedTo = DockingConnections[dockableGUID];
                bool iscurrentlyDocked = dockedTo != null && dockedTo.Length > 0;

                //SgtLogger.l(dockableGUID + " is currently docked? " + iscurrentlyDocked);

                return iscurrentlyDocked;
            }
            return false;
        }


        internal void UpdateAssignmentController(IDockable dockable, AssignmentGroupController assignmentController)
        {
            MinionAssignmentGroupControllers[dockable.GUID] = assignmentController;
        }
        public bool TryGetDockableIfDocked(string ID, out IDockable dockable)
        {
            dockable = null;

            if (ID == null || ID.Length == 0)
                return false;

            if (IDockables.ContainsKey(ID) && IsDocked(ID, out var DOckedID) && IDockables.ContainsKey(DOckedID))
            {
                dockable = IDockables[DOckedID];
                return true;
            }

            return false;
        }



        public bool TryGetDockable(string ID, out IDockable dockable)
        {
            dockable = null;

            if (ID == null || ID.Length == 0)
                return false;

            if (IDockables.ContainsKey(ID))
            {
                dockable = IDockables[ID];
                return true;
            }

            return false;
        }
        public bool TryGetAssignmentController(string ID, out AssignmentGroupController ctrl)
        {
            ctrl = null;

            if (ID == null || ID.Length == 0)
                return false;

            if (MinionAssignmentGroupControllers.ContainsKey(ID))
            {
                ctrl = MinionAssignmentGroupControllers[ID];
                return true;
            }

            return false;
        }
        public bool HandlersConnected(DockingSpacecraftHandler first, DockingSpacecraftHandler second, out IDockable firstDock, out IDockable secondDock)
        {
            firstDock = null;
            secondDock = null;

            if (first == null || second == null)
                return false;

            foreach (var hander in first.WorldDockables.Keys)
            {
                if (TryGetDockableIfDocked(hander, out secondDock) && secondDock.spacecraftHandler == second)
                {
                    return TryGetDockable(hander, out firstDock);
                }
            }
            return false;
        }

        bool CurrentlyInDockingProcess(string ID)
        {
            bool currentlyInProcess = PendingDockBlockers.Contains(ID) && PendingUndockBlockers.Contains(ID);
            SgtLogger.l(ID + " currently in a docking process? " + currentlyInProcess);

            return currentlyInProcess;
        }

        public bool TryGetAvailableDockable(DockingSpacecraftHandler handler, out IDockable dockable, DockingDoor overrideDoor = null)
        {
            dockable = null;
            if (handler == null)
                SgtLogger.error("Handler was null" + handler);

            SgtLogger.l("trying to get dockable for " + handler.GetProperName());
            if (!DockingSpacecraftHandlers.Contains(handler))
            {
                SgtLogger.error("Handlers didnt contain " + handler);
                return false;
            }

            if (overrideDoor != null)
            {
                var entry = handler.WorldDockables.Values.ToList().FirstOrDefault(data => !IsDocked(data.GUID, out _) && data == overrideDoor && !CurrentlyInDockingProcess(data.GUID));

                if (entry != null)
                    dockable = entry;
            }
            if (dockable == null)
                dockable = handler.WorldDockables.Values.ToList().FirstOrDefault(entry => !IsDocked(entry.GUID, out _) && !CurrentlyInDockingProcess(entry.GUID));

            SgtLogger.l(handler.GetProperName() + " can currently dock? " + (dockable != null));

            return dockable != null;
        }
        public bool TryInitializingDockingBetweenHandlers(DockingSpacecraftHandler first, DockingSpacecraftHandler second, DockingDoor overrideDoor = null)
        {
            SgtLogger.l("trying to dock " + first.GetProperName() + " and " + second.GetProperName());

            if (TryGetAvailableDockable(first, out var firstDock, overrideDoor) && TryGetAvailableDockable(second, out var secondDock, overrideDoor))
            {
                return AddPendingDock(firstDock.GUID, secondDock.GUID);
            }
            return false;
        }
        public bool TryInitializingUndockingBetweenHandlers(DockingSpacecraftHandler first, DockingSpacecraftHandler second, DockingDoor overrideDoor = null, System.Action UiRefreshActionOnFinished = null)
        {
            SgtLogger.l("trying to undock " + first.GetProperName() + " and " + second.GetProperName());
            foreach (var door in first.WorldDockables)
            {
                if (TryGetDockableIfDocked(door.Value.GUID, out var dockedTo) && dockedTo.spacecraftHandler == second)
                {
                    return AddPendingUndock(door.Value.GUID, dockedTo.GUID, UiRefreshActionOnFinished);
                }
            }
            return false;
        }



        public void FinalizeUndocking(string firstId, string secondId, bool cleanup = false)
        {
            PendingUndocks.Remove(firstId);
            PendingUndocks.Remove(secondId);
            PendingUndockBlockers.Remove(firstId);
            PendingUndockBlockers.Remove(secondId);

            if (!TryGetDockable(firstId, out IDockable first))
            {
                SgtLogger.error(firstId + " not found in finalizeUndocking");
            }
            if (!TryGetDockable(secondId, out IDockable second))
            {
                SgtLogger.error(secondId + " not found in finalizeUndocking");
            }
            ///Removing docking entry in list
            DockingConnections[firstId] = null;
            DockingConnections[secondId] = null;


            ///Parenting the two worlds to themselves again
            if (ClusterManager.Instance.GetWorld(first.WorldId) != null)
                ClusterManager.Instance.GetWorld(first.WorldId).SetParentIdx(first.WorldId);

            if (ClusterManager.Instance.GetWorld(second.WorldId) != null)
                ClusterManager.Instance.GetWorld(second.WorldId).SetParentIdx(second.WorldId);


            ///Updating internal data in connectors
            first.UpdateDockingConnection(cleanup);
            second.UpdateDockingConnection(cleanup);

            if (PendingUndockUIRefreshActions.ContainsKey(firstId) && PendingUndockUIRefreshActions[firstId] != null)
                PendingUndockUIRefreshActions[firstId]();
            PendingUndockUIRefreshActions[firstId] = null;

            if (PendingUndockUIRefreshActions.ContainsKey(secondId) && PendingUndockUIRefreshActions[secondId] != null)
                PendingUndockUIRefreshActions[secondId]();
            PendingUndockUIRefreshActions[secondId] = null;


            ///Preassigning dupes for transfer
            CleanupWorldAssignments(new Tuple<string, string>(firstId, secondId));
        }
        public void FinalizeDocking(string firstId, string secondId)
        {
            if (!TryGetDockable(firstId, out IDockable first))
            {
                SgtLogger.error(firstId + " not found in finalizeUndocking");
            }
            if (!TryGetDockable(secondId, out IDockable second))
            {
                SgtLogger.error(secondId + " not found in finalizeUndocking");
            }
            DockingConnections[firstId] = secondId;
            DockingConnections[secondId] = firstId;

            PendingDocks.Remove(firstId);
            PendingDocks.Remove(secondId);
            PendingDockBlockers.Remove(firstId);
            PendingDockBlockers.Remove(secondId);



            ///Space stations are always the parent, otherwise the first becomes the child
            if (SpaceStationManager.WorldIsSpaceStationInterior(first.WorldId))
            {
                ClusterManager.Instance.GetWorld(second.WorldId).SetParentIdx(first.WorldId);
            }
            else if (SpaceStationManager.WorldIsSpaceStationInterior(second.WorldId))
            {
                ClusterManager.Instance.GetWorld(first.WorldId).SetParentIdx(second.WorldId);
            }
            else
                ClusterManager.Instance.GetWorld(first.WorldId).SetParentIdx(second.WorldId);

            first.UpdateDockingConnection();
            second.UpdateDockingConnection();

            ///setting assigned crew rocket for each dupe to the rocket they are in
            CleanupWorldAssignments(new Tuple<string, string>(firstId, secondId));
        }



        /// <summary>
        /// Clean up world assignments if there was an unexpected decoupling (by blow up or deconstruction)
        /// </summary>
        /// <param name="targetWorldId"></param>
        public void CleanupWorldAssignments(Tuple<string, string> connection)
        {
            var OwnWorld = ClusterManager.Instance.GetWorld(IDockables[connection.first].WorldId);
            var DockedWorld = ClusterManager.Instance.GetWorld(IDockables[connection.second].WorldId);

            if (TryGetAssignmentController(connection.first, out var firstController))
            {
                foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(OwnWorld.id))
                {
                    if (!Game.Instance.assignmentManager.assignment_groups[firstController.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                    {
                        Game.Instance.assignmentManager.assignment_groups[firstController.AssignmentGroupID].AddMember(minion.assignableProxy.Get());
                    }
                }
                foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(DockedWorld.id))
                {
                    if (Game.Instance.assignmentManager.assignment_groups[firstController.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                    {
                        Game.Instance.assignmentManager.assignment_groups[firstController.AssignmentGroupID].RemoveMember(minion.assignableProxy.Get());
                    }
                }

            }
            if (TryGetAssignmentController(connection.first, out var secondController))
            {
                foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(OwnWorld.id))
                {
                    if (!Game.Instance.assignmentManager.assignment_groups[secondController.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                    {
                        Game.Instance.assignmentManager.assignment_groups[secondController.AssignmentGroupID].RemoveMember(minion.assignableProxy.Get());
                    }
                }
                foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(DockedWorld.id))
                {
                    if (Game.Instance.assignmentManager.assignment_groups[secondController.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                    {
                        Game.Instance.assignmentManager.assignment_groups[secondController.AssignmentGroupID].AddMember(minion.assignableProxy.Get());
                    }
                }
            }
        }




        HashSet<Tuple<string, string>> ToRemove = new HashSet<Tuple<string, string>>();
        public void Sim1000ms(float dt)
        {
            foreach (var undockingProcess in PendingUndocks)
            {
                List<MinionIdentity> WrongWorldDupesHERE = new List<MinionIdentity>();
                List<MinionIdentity> WrongWorldDupesTHERE = new List<MinionIdentity>();
                if (TryGetDockable(undockingProcess.Key, out var first) && TryGetDockable(undockingProcess.Value, out var second)
                    && first is DockingDoor && second is DockingDoor
                    )
                {
                    if (TryGetAssignmentController(undockingProcess.Key, out var assignmentGroupControllerOWN))
                    {
                        Debug.Assert(Game.Instance.assignmentManager.assignment_groups.ContainsKey(assignmentGroupControllerOWN.AssignmentGroupID), "ASSIGNMENTGROUPCTRL first was null");
                        SgtLogger.l("checking first minions");
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(first.WorldId))
                        {

                            if (!Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerOWN.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                WrongWorldDupesHERE.Add(minion);
                            }
                        }
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(second.WorldId))
                        {
                            if (Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerOWN.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                WrongWorldDupesTHERE.Add(minion);
                            }
                        }

                    }
                    if (TryGetAssignmentController(undockingProcess.Value, out var assignmentGroupControllerDOCKED))
                    {
                        Debug.Assert(Game.Instance.assignmentManager.assignment_groups.ContainsKey(assignmentGroupControllerOWN.AssignmentGroupID), "ASSIGNMENTGROUPCTRL second was null");
                        SgtLogger.l("checking seconds minions");
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(first.WorldId))
                        {
                            if (Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerDOCKED.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                WrongWorldDupesHERE.Add(minion);
                            }
                        }
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(second.WorldId))
                        {
                            if (!Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerDOCKED.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                WrongWorldDupesTHERE.Add(minion);
                            }
                        }
                    }

                    DockingDoor firstDoor = first as DockingDoor;
                    DockingDoor secondDoor = second as DockingDoor;

                    foreach (var minion in WrongWorldDupesHERE)
                    {
                        var smi = minion.GetSMI<RocketPassengerMonitor.Instance>();
                        smi.SetMoveTarget(secondDoor.GetPorterCell());
                        firstDoor.RefreshAccessStatus(minion, false);
                        secondDoor.RefreshAccessStatus(minion, true);

                    }
                    foreach (var minion in WrongWorldDupesTHERE)
                    {
                        var smi = minion.GetSMI<RocketPassengerMonitor.Instance>();
                        smi.SetMoveTarget(firstDoor.GetPorterCell());

                        firstDoor.RefreshAccessStatus(minion, true);
                        secondDoor.RefreshAccessStatus(minion, false);
                    }
                    SgtLogger.l("wrong dupes first: " + WrongWorldDupesHERE.Count + ", wrong dupes second: " + WrongWorldDupesTHERE.Count);
                    if (WrongWorldDupesHERE.Count == 0 && WrongWorldDupesTHERE.Count == 0)
                    {
                        SgtLogger.log(string.Format("Undocking world {0} from {1}, now that all dupes are moved", first.WorldId, second.WorldId));
                        ToRemove.Add(new Tuple<string, string>(first.GUID, second.GUID));
                    }
                }
            }

            foreach (var finalized in ToRemove)
            {
                FinalizeUndocking(finalized.first, finalized.second);
            }
            ToRemove.Clear();

            foreach (var dockingProcess in PendingDocks)
            {
                if (TryGetDockable(dockingProcess.Key, out var first) && TryGetDockable(dockingProcess.Value, out var second))
                {
                    WorldContainer firstWorld = ClusterManager.Instance.GetWorld(first.WorldId), secondWorld = ClusterManager.Instance.GetWorld(second.WorldId);
                    if (firstWorld != null && firstWorld.TryGetComponent<DockingSpacecraftHandler>(out var firstEntity)
                        && secondWorld != null && secondWorld.TryGetComponent<DockingSpacecraftHandler>(out var secondEntity))
                    {
                        if (firstEntity.Interface.m_clustercraft.Location == secondEntity.Interface.m_clustercraft.Location && firstEntity.CanDock() && secondEntity.CanDock())
                            ToRemove.Add(new Tuple<string, string>(first.GUID, second.GUID));
                    }
                    else
                        SgtLogger.warning("couldnt dock: " + first.WorldId + " and " + second.WorldId + " - no clustergridentity");

                }
                else
                    SgtLogger.warning("couldnt dock: " + dockingProcess.Key + " and " + dockingProcess.Value);
            }
            foreach (var doDock in ToRemove)
            {
                FinalizeDocking(doDock.first, doDock.second);
            }
            ToRemove.Clear();
        }

        internal List<DockingSpacecraftHandler> GetAllAvailableDockingHandlersAtPosition(AxialI location)
        {
            var values = new List<DockingSpacecraftHandler>();
            foreach (var handler in DockingSpacecraftHandlers)
            {
                if (handler.clustercraft.Location.Equals(location) && handler.HasDoors())
                    values.Add(handler);
            }
            return values;
        }

        internal bool HasPendingUndocks(string dockerId)
        {
            return PendingUndockBlockers.Contains(dockerId);
        }
    }
}
