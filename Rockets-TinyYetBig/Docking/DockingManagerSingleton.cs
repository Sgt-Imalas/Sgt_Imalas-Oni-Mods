using KSerialization;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static CellChangeMonitor.CellChangedEntry;
using static Rockets_TinyYetBig.ModAssets;
using static STRINGS.UI.UISIDESCREENS.AUTOPLUMBERSIDESCREEN.BUTTONS;

namespace Rockets_TinyYetBig.Docking
{
    internal class DockingManagerSingleton : KMonoBehaviour, ISim1000ms
    {
        public static DockingManagerSingleton Instance { get; set; }

        [Serialize][SerializeField] private Dictionary<string, string> DockingConnections = new Dictionary<string, string>(); //active connections by door id;

        [Serialize][SerializeField] public Dictionary<string, string> PendingUndocks = new Dictionary<string, string>(); //pending undock door ids;
        [Serialize][SerializeField] public Dictionary<string, string> PendingDocks = new Dictionary<string, string>(); //pending dock door ids;
        [Serialize][SerializeField] public HashSet<string> PendingDockBlockers = new HashSet<string>(); //doors currently in a pending dock process
        [Serialize][SerializeField] public HashSet<string> PendingUndockBlockers = new HashSet<string>(); //doors currently in a pending undock process
        [Serialize][SerializeField] public Dictionary<Ref<MinionAssignablesProxy>, int> MinionDockingTransferAssignments = new Dictionary<Ref<MinionAssignablesProxy>, int>(); //current minion crew assignments

        [Serialize][SerializeField] public Dictionary<int, int> PendingToStationDocks = new Dictionary<int, int>(); //rocket worlds that want to connect to a station, rocketworld = key, stationworld = value


        //private Dictionary<string, AssignmentGroupController> MinionAssignmentGroupControllers = new Dictionary<string, AssignmentGroupController>();
        public Dictionary<string, IDockable> IDockables = new Dictionary<string, IDockable>(); 
        public HashSet<DockingSpacecraftHandler> DockingSpacecraftHandlers = new HashSet<DockingSpacecraftHandler>();
        public Dictionary<int,DockingSpacecraftHandler> WorldToDockingSpacecraftHandlers = new Dictionary<int, DockingSpacecraftHandler> ();

        //public void OnLoading()
        //{
        //    //foreach (var connection in DockingConnectionData)
        //    //{
        //    //    DockingConnections[connection.first] = connection.second;
        //    //    SgtLogger.l(connection.first + " <-> " + connection.second, "Loading");
        //    //}
        //}
        //public void OnSaving()
        //{
        //    //DockingConnectionData.Clear();
        //    foreach (var connection in DockingConnections)
        //    {
        //        //DockingConnectionData.Add(new Tuple<string, string>(connection.Key, connection.Value));
        //        SgtLogger.l(connection.Key + " <-> " + connection.Value, "Saving");
        //    }
        //}

        public override void OnSpawn()
        {
            //OnLoading();
            base.OnSpawn();
            SgtLogger.l("DockingManager OnSpawn");
        }


        public void AddPendingToStationDock(int craftWorldId, int StationWorldId)
        {
            if(!WorldToDockingSpacecraftHandlers.ContainsKey(craftWorldId))
            {
                SgtLogger.error("craft handler for rocket with worldId " + craftWorldId + " was null!");
                return;
            }
            if (!WorldToDockingSpacecraftHandlers.ContainsKey(StationWorldId))
            {
                SgtLogger.error("craft handler for station with worldId " + StationWorldId + " was null!");
                return;
            }
            if (PendingToStationDocks.ContainsKey(craftWorldId))
            {
                SgtLogger.l("already a pending docking process in place for " + craftWorldId);
                return;
            }

            PendingToStationDocks.Add(craftWorldId, StationWorldId);
        }

        public void RemoveToStationDock(int craftWorldId)
        {

            if (PendingToStationDocks.ContainsKey(craftWorldId))
            {
                PendingToStationDocks.Remove(craftWorldId);
            }
        }
        public void UpdatePendingStationDockings()
        {
            List<int> ToRemove = new List<int>();
            foreach(var kvp in PendingToStationDocks)
            {
                if(WorldToDockingSpacecraftHandlers.ContainsKey((int)kvp.Key) || WorldToDockingSpacecraftHandlers.ContainsKey((int)kvp.Value))
                {
                    var RocketHandler = WorldToDockingSpacecraftHandlers[kvp.Key];
                    var StationHandler = WorldToDockingSpacecraftHandlers[(int)kvp.Value];
                    if (TryInitializingDockingBetweenHandlers(RocketHandler, StationHandler))
                    {
                        ToRemove.Add(kvp.Key);
                    }
                }
                else
                    ToRemove.Add(kvp.Key);
            }

            foreach (var rocket in ToRemove)
                RemoveToStationDock(rocket);
        }


        public void RegisterSpacecraftHandler(DockingSpacecraftHandler handler)
        {
            if (!DockingSpacecraftHandlers.Contains(handler))
            {
                SgtLogger.l("registering spacecraft docking handler for " + handler.GetProperName());
                DockingSpacecraftHandlers.Add(handler);
                WorldToDockingSpacecraftHandlers.Add(handler.WorldId, handler);

                if (handler.CraftType == DockingSpacecraftHandler.DockableType.Rocket)
                {
                    SgtLogger.l("Spacecraft detected: " + handler.GetProperName());
                    foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(handler.WorldId))
                    {
                        var proxy = minion.assignableProxy;
                        if (!MinionDockingTransferAssignments.ContainsKey(proxy))
                        {
                            SgtLogger.l("newly assigning " + minion.GetProperName() + " to " + handler.GetProperName());
                            MinionDockingTransferAssignments[proxy] = handler.WorldId;
                        }
                    }
                }
            }
        }
        public void UnregisterSpacecraftHander(DockingSpacecraftHandler handler)
        {
            if (DockingSpacecraftHandlers.Contains(handler))
            {
                SgtLogger.l("unregistering spacecraft docking handler for " + handler.GetProperName());
                DockingSpacecraftHandlers.Remove(handler);
                WorldToDockingSpacecraftHandlers.Remove(handler.WorldId);
                RemoveToStationDock(handler.WorldId);

                if (handler.CraftType == DockingSpacecraftHandler.DockableType.Rocket)
                {
                    SgtLogger.l("Spacecraft detected: " + handler.GetProperName());
                    foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(handler.WorldId))
                    {
                        var proxy = minion.assignableProxy;
                        if (MinionDockingTransferAssignments.ContainsKey(proxy))
                        {
                            SgtLogger.l("removing assignment " + minion.GetProperName() + " from world: " + MinionDockingTransferAssignments[proxy]);
                            MinionDockingTransferAssignments.Remove(proxy);
                        }
                    }
                }
            }
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
                if (TryGetDockable(dockedTo, out var dockedToDockable))
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
            SgtLogger.l("adding pending undock between " + first + " and " + second);

            PendingUndocks.Add(first, second);
            PendingUndockBlockers.Add(first);
            PendingUndockBlockers.Add(second);
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


        //internal void UpdateAssignmentController(IDockable dockable, AssignmentGroupController assignmentController)
        //{
        //    MinionAssignmentGroupControllers[dockable.GUID] = assignmentController;
        //}
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
        //public bool TryGetAssignmentController(string ID, out AssignmentGroupController ctrl)
        //{
        //    ctrl = null;

        //    if (ID == null || ID.Length == 0)
        //        return false;

        //    if (MinionAssignmentGroupControllers.ContainsKey(ID))
        //    {
        //        ctrl = MinionAssignmentGroupControllers[ID];
        //        return true;
        //    }

        //    return false;
        //}
        public bool HandlersConnected(DockingSpacecraftHandler first, DockingSpacecraftHandler second, out IDockable firstDock, out IDockable secondDock)
        {
            SgtLogger.l("Handlers connected? "+first.GetProperName()+" & "+second.GetProperName());
            firstDock = null;
            secondDock = null;

            if (first == null || second == null)
                return false;

            if (first == second)
                SgtLogger.error("first handler was also the second");


            foreach (var handlerFirst in first.WorldDockables.Values)
            {
                foreach (var handlerSecond in second.WorldDockables.Values)
                {
                    if(IsDocked(handlerFirst.GUID, out var compare) && compare == handlerSecond.GUID)
                    {
                        firstDock = handlerFirst;
                        secondDock = handlerSecond;
                        SgtLogger.l("Handlers connected! " + handlerFirst.spacecraftHandler.GetProperName() +" "+ handlerFirst.WorldId+" & " + handlerSecond.spacecraftHandler.GetProperName() + " " + handlerSecond.WorldId );
                        return true;
                    }
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

            if(RocketryUtils.IsRocketInFlight(handler.clustercraft))
                return false;


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


            ///Preassigning dupes for transfer
            PrepareWorldAssignments(new Tuple<string, string>(firstId, secondId));
            ///Updating internal data in connectors
            first.UpdateDockingConnection(cleanup);
            second.UpdateDockingConnection(cleanup);
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

            ///setting assigned crew rocket for each dupe to the rocket they are in
            //CleanupWorldAssignments(new Tuple<string, string>(firstId, secondId));
            first.UpdateDockingConnection();
            second.UpdateDockingConnection();

        }
        public void PrepareWorldAssignments(Tuple<string, string> connection)
        {
            var firstDockable = IDockables[connection.first];
            var secondDockable = IDockables[connection.second];

            var worldFirst = ClusterManager.Instance.GetWorld(firstDockable.WorldId);
            var worldSecond = ClusterManager.Instance.GetWorld(secondDockable.WorldId);


            foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(worldFirst.id))
            {
                if (!TryGetMinionAssignment(minion, out _))
                {
                    SgtLogger.l("assigning " + minion.GetProperName() + " to " + worldFirst.GetProperName());
                    SetMinionAssignment(minion, worldFirst);
                }


            }
            foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(worldSecond.id))
            {
                if (!TryGetMinionAssignment(minion, out _))
                {
                    SgtLogger.l("assigning " + minion.GetProperName() + " to " + worldSecond.GetProperName());
                    SetMinionAssignment(minion, worldSecond);
                }
            }
        }




        /// <summary>
        /// Clean up world assignments if there was an unexpected decoupling (by blow up or deconstruction)
        /// </summary>
        /// <param name="targetWorldId"></param>
        public void CleanupWorldAssignments(Tuple<string, string> connection)
        {
            return;
            var firstDockable = IDockables[connection.first];
            var secondDockable = IDockables[connection.second];

            var worldFirst = ClusterManager.Instance.GetWorld(firstDockable.WorldId);
            var worldSecond = ClusterManager.Instance.GetWorld(secondDockable.WorldId);


            foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(worldFirst.id))
            {
                if (firstDockable.spacecraftHandler.IsRocket)
                {
                    SgtLogger.l("assigning " + minion.GetProperName() + " to " + worldFirst.GetProperName());
                    SetMinionAssignment(minion, worldFirst);
                }


            }
            foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(worldSecond.id))
            {
                SgtLogger.l("assigning " + minion.GetProperName() + " to " + worldSecond.GetProperName());
                SetMinionAssignment(minion, worldSecond);
            }
        }
        public void SetMinionAssignment(MinionIdentity minion, WorldContainer world) => SetMinionAssignment(minion.assignableProxy, world.id);

        public void SetMinionAssignment(Ref<MinionAssignablesProxy> proxy, int worldId)
        {
            MinionDockingTransferAssignments[proxy] = worldId;
        }
        public bool TryGetMinionAssignment(MinionIdentity minion, out int worldID)
        {
            worldID = -1;
            if (!MinionDockingTransferAssignments.ContainsKey(minion.assignableProxy))
                return false;

            worldID = MinionDockingTransferAssignments[minion.assignableProxy];
            return true;

        }



        HashSet<Tuple<string, string>> ToRemove = new HashSet<Tuple<string, string>>();
        public void Sim1000ms(float dt)
        {
            UpdatePendingStationDockings();


            foreach (var undockingProcess in PendingUndocks)
            {
                List<MinionIdentity> MoveToSecondDupes = new List<MinionIdentity>();
                List<MinionIdentity> MoveToFirstDupes = new List<MinionIdentity>();
                if (TryGetDockable(undockingProcess.Key, out var first) && TryGetDockable(undockingProcess.Value, out var second)
                    && first is DockingDoor && second is DockingDoor
                    )
                {


                    foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(first.WorldId))
                    {
                        if (TryGetMinionAssignment(minion, out var minionWorldId))
                        {
                            if (minionWorldId != first.WorldId && minionWorldId == second.WorldId)
                            {
                                MoveToSecondDupes.Add(minion);
                            }
                        }
                    }
                    foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(second.WorldId))
                    {
                        if (TryGetMinionAssignment(minion, out var minionWorldId))
                        {
                            if (minionWorldId != second.WorldId && minionWorldId == first.WorldId)
                            {
                                MoveToFirstDupes.Add(minion);
                            }
                        }
                    }

                    DockingDoor firstDoor = first as DockingDoor;
                    DockingDoor secondDoor = second as DockingDoor;

                    foreach (var minion in MoveToSecondDupes)
                    {
                        var smi = minion.GetSMI<RocketPassengerMonitor.Instance>();
                        smi.SetMoveTarget(secondDoor.GetPorterCell());
                        firstDoor.RefreshAccessStatus(minion, false);
                        secondDoor.RefreshAccessStatus(minion, true);

                    }
                    foreach (var minion in MoveToFirstDupes)
                    {
                        var smi = minion.GetSMI<RocketPassengerMonitor.Instance>();
                        smi.SetMoveTarget(firstDoor.GetPorterCell());

                        firstDoor.RefreshAccessStatus(minion, true);
                        secondDoor.RefreshAccessStatus(minion, false);
                    }
                    SgtLogger.l("wrong dupes first: " + MoveToSecondDupes.Count + ", wrong dupes second: " + MoveToFirstDupes.Count);
                    if (MoveToSecondDupes.Count == 0 && MoveToFirstDupes.Count == 0)
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
