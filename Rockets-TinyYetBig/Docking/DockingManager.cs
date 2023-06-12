using Klei;
using KSerialization;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.CODEX;
using static STRINGS.UI.NEWBUILDCATEGORIES;
using static UnityEngine.GraphicsBuffer;

namespace Rockets_TinyYetBig.Behaviours
{
    [SerializationConfig(MemberSerialization.Invalid)]
    public class DockingManager : KMonoBehaviour, IListableOption, ISim1000ms
    {
        [MyCmpGet]
        public Clustercraft clustercraft;

        public void StartupID(int world)
        {
            MyWorldId = world;
        }

        /// <summary>
        /// My door + connectedDoor
        /// </summary>
        int MyWorldId=-1;

        public Dictionary<DockingDoor, int> DockingDoors = new Dictionary<DockingDoor, int>();
        public int GetWorldId() => MyWorldId;

        DockableType Type = DockableType.Rocket;

        public DockableType GetCraftType => Type;

        public List<int> GetConnectedWorlds()
        {
            var list = new List<int>();
            foreach(var door in DockingDoors)
            {
                if (door.Value != -1)
                    list.Add(door.Value);

            }
            return list;
        }
        public List<int> GetConnectedRockets()
        {
            var list = new List<int>();
            foreach (var door in DockingDoors)
            {
                if (door.Value != -1&& SpaceStationManager.WorldIsRocketInterior(door.Value))
                    list.Add(door.Value);

            }
            return list;
        }



        public void SetManagerType(int overrideType = -1)
        {
            if (MyWorldId != -1)
            {
                if (SpaceStationManager.WorldIsSpaceStationInterior(MyWorldId))
                {
                    Type = DockableType.SpaceStation;
                }
                else if(SpaceStationManager.WorldIsRocketInterior(MyWorldId))
                {
                    Type = DockableType.Rocket;
                }
            }
#if DEBUG
            SgtLogger.debuglog("CraftType set to: "+ Type);
#endif
        }


        public void SetCurrentlyLoadingStuff(bool IsLoading)
        {
           SgtLogger.l("IsNowLoading? " + IsLoading);
            isLoading = IsLoading;

            if (!IsLoading&&OnFinishedLoading!=null)
                OnFinishedLoading.Invoke();
        }
        bool isLoading=false;

        public System.Action OnFinishedLoading = null;

        public bool IsLoading => isLoading;

        List<int> PendingDocks = new List<int>();

        public void Sim1000ms(float dt)
        {
            List<DockingDoor> ToRemove = new List<DockingDoor>();
            foreach(var undockingProcess in PendingUndocks)
            {
                List<MinionIdentity> WrongWorldDupesHERE = new List<MinionIdentity>();
                List<MinionIdentity> WrongWorldDupesTHERE = new List<MinionIdentity>();
                if (undockingProcess.Key.IsConnected && DockingDoors.ContainsKey(undockingProcess.Key))
                {
                    PassengerRocketModule passengerOwn = undockingProcess.Key.GetCraftModuleInterface().GetPassengerModule();
                    PassengerRocketModule passengerDock = undockingProcess.Key.GetDockedCraftModuleInterface().GetPassengerModule();
                    AssignmentGroupController assignmentGroupControllerOWN = null;
                    AssignmentGroupController assignmentGroupControllerDOCKED = null;

                    if(passengerOwn != null)
                        passengerOwn.TryGetComponent<AssignmentGroupController>(out assignmentGroupControllerOWN);
                    if(passengerDock!= null)
                        passengerDock.TryGetComponent<AssignmentGroupController>(out assignmentGroupControllerDOCKED);

                    if (assignmentGroupControllerOWN != null)
                    {
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.Key.dManager.MyWorldId))
                        {
                            //SgtLogger.l(minion.name, "minion");

                            if (!Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerOWN.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                //SgtLogger.l(minion.name, "wrong here");
                                WrongWorldDupesHERE.Add(minion);
                            }
                        }
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.Key.GetConnec().dManager.MyWorldId))
                        {
                            //SgtLogger.l(minion.name, "minion there");
                            if (Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerOWN.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                //SgtLogger.l(minion.name, "wrong there;");
                                WrongWorldDupesTHERE.Add(minion);
                            }
                        }
                    }
                    else if (assignmentGroupControllerDOCKED != null)
                    {
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.Key.dManager.MyWorldId))
                        {
                            //SgtLogger.l(minion.name, "minion 2");

                            if (Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerDOCKED.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                //SgtLogger.l(minion.name, "wrong here 2");
                                WrongWorldDupesHERE.Add(minion);
                            }
                        }
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.Key.GetConnec().dManager.MyWorldId))
                        {
                            //SgtLogger.l(minion.name, "minion there 2");
                            if (!Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerDOCKED.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                //SgtLogger.l(minion.name, "wrong there 2");
                                WrongWorldDupesTHERE.Add(minion);
                            }
                        }
                    }
                    

                    foreach(var minion in WrongWorldDupesHERE)
                    {
                        minion.GetSMI<MoveToLocationMonitor.Instance>().MoveToLocation(undockingProcess.Key.GetConnec().GetPorterCell());                        
                    }
                    foreach (var minion in WrongWorldDupesTHERE)
                    {
                        minion.GetSMI<MoveToLocationMonitor.Instance>().MoveToLocation(undockingProcess.Key.GetPorterCell());
                    }

                    if (WrongWorldDupesHERE.Count == 0 && WrongWorldDupesTHERE.Count == 0)
                    {
                        SgtLogger.log(string.Format("Undocking world {0} from {1}, now that all dupes are moved", undockingProcess.Key.GetMyWorldId(), undockingProcess.Key.GetConnec().GetMyWorldId()));
                        UndockDoor(undockingProcess.Key, false, undockingProcess.Value);
                        ToRemove.Add(undockingProcess.Key);
                    }
                }
            }
            foreach(var done in ToRemove)
            {
                PendingUndocks.Remove(done);
            }

            List<int> ToDock = new List<int>();
            foreach(var worldToDockTo in PendingDocks)
            {
                var target = ModAssets.Dockables.Items.Find(mng => mng.MyWorldId == worldToDockTo);
                if (target != null && target.TryGetComponent<ClusterGridEntity>(out var targetEntity))
                {
                    if(targetEntity.Location == clustercraft.Location && CanDock() && target.CanDock())
                    {
                        ToDock.Add(worldToDockTo);
                    }
                }
            }
            foreach(var doDock in ToDock)
            {
                DockToTargetWorld(doDock);
            }
        }

        public void AddPendingDock(int worldID)
        {
            if (PendingDocks.Contains(worldID))
                return;
            PendingDocks.Add(worldID);
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
                    break;

                default:
                    returnVal = Assets.GetSprite("unknown");
                    break;
            }
            return returnVal;
            
        }


        public override void OnSpawn()
        {
            base.OnSpawn();
            ModAssets.Dockables.Add(this);
#if DEBUG
            SgtLogger.debuglog("AddedDockable");
#endif
        }
        public override void OnCleanUp()
        {
            ModAssets.Dockables.Remove(this);
            base.OnCleanUp();
        }

        public string GetUiDoorInfo()
        {
            if (PendingUndocks.Count > 0)
                return STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.UNDOCKINGPENDING;
            else if (PendingDocks.Count > 0)
                return STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.DOCKINGPENDING;

            int count = AvailableConnections();
            if (count == 1)
                return STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.ONECONNECTION;
            else
                return string.Format(STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.MORECONNECTIONS, count);
        }
        public int AvailableConnections()
        {
            int count = DockingDoors.Keys.ToList().FindAll(k => k.GetConnec() == null).Count();
            return count;
        }

        public void AddDoor(DockingDoor door)
        {
            if(MyWorldId==-1)
                MyWorldId = ClusterUtil.GetMyWorldId(door);
            if(!DockingDoors.ContainsKey(door))
            {
                int target = -1;
                if (door.GetConnec() != null)
                    target = door.GetConnec().GetMyWorldId();
                DockingDoors.Add(door, target);
            }
            SgtLogger.debuglog("Added new Docking Door!, ID: " + MyWorldId+", Doorcount: "+DockingDoors.Count());

            //UpdateDeconstructionStates();

        }

        public void RemoveDoor(DockingDoor door)
        {
            if (DockingDoors.ContainsKey(door))
            {
                SgtLogger.debuglog(door + "-dockingDoor removed from  " + door.GetMyWorldId());
                UnDockFromTargetWorld(door.GetConnectedTargetWorldId(), true);
                ///Disconecc;
                //door.DisconnecDoor();
                DockingDoors.Remove(door);
                //UpdateDeconstructionStates();
            }
        }

        public bool CanDock()
        {
            bool cando = DockingDoors.Any(k => k.Key.GetConnec() == null)
                && HasDoors()
                && (clustercraft.status == Clustercraft.CraftStatus.InFlight);
            return cando;
        }
        public bool IsDockedToAny()
        {
            return DockingDoors.Any(k => k.Key.GetConnec() != null);
        }

        public bool HasDoors()
        {
            //SgtLogger.debuglog("HAs Doors: " + DockingDoors.Count);
            return DockingDoors.Count>0;
        }

        public bool IsDockedTo(int WorldID)
        {
            return DockingDoors.ContainsValue(WorldID);
        }

        public bool GetActiveUIState(int worldId) => IsDockedTo(worldId) || HasPendingUndocks(worldId);

        public bool HasPendingUndocks(int WorldID)
        {
            return PendingUndocks.Keys.Any(door => door.GetMyWorldId() == WorldID);
        }

        public void HandleUiDocking(int prevDockingState, int targetWorld, DockingDoor door = null, System.Action onFinished = null)
        {
            //SgtLogger.debuglog(prevDockingState == 0 ? "Trying to dock " + MyWorldId + " with dedicated door to " + targetWorld : "Trying To Undock " + MyWorldId + " from " + targetWorld);

            if (prevDockingState == 0)
                DockToTargetWorld(targetWorld, door);
            else
                UnDockFromTargetWorld(targetWorld,OnFinishedUndock: onFinished);
        }

        public void DockToTargetWorld(int targetWorldId, DockingDoor OwnDoor = null)
        {
            SgtLogger.l("Can Dock? " + this.CanDock());
            if (!this.CanDock())
                return;

            var target = ModAssets.Dockables.Items.Find(mng => mng.MyWorldId == targetWorldId);


            if (target == null || target.DockingDoors.Count == 0 || this.DockingDoors.Count==0 || !target.CanDock())
            {
                SgtLogger.warning("No doors found");
                return;
            }
            if(IsDockedTo(targetWorldId))
            {
                SgtLogger.warning("Already Docked");
                return;
            }

            if(HasPendingUndocks(targetWorldId))
            {
                var worldDoor = PendingUndocks.FirstOrDefault(door => door.Key.GetMyWorldId() == targetWorldId).Key;
                SgtLogger.l("canceled pending undocking");
                if (worldDoor != default)
                    PendingUndocks.Remove(worldDoor);
                return;
            }



            ConnectTwo(this, target, OwnDoor);

            if (SpaceStationManager.WorldIsSpaceStationInterior(MyWorldId))
            {
                ClusterManager.Instance.GetWorld(targetWorldId).SetParentIdx(MyWorldId);
            }
            else if (SpaceStationManager.WorldIsSpaceStationInterior(targetWorldId))
            {
                ClusterManager.Instance.GetWorld(MyWorldId).SetParentIdx(targetWorldId);
            }
            
            else
                ClusterManager.Instance.GetWorld(MyWorldId).SetParentIdx(targetWorldId);

            if(PendingDocks.Contains(targetWorldId))
                PendingDocks.Remove(targetWorldId);
        }
        public void UndockAll(bool force = false)
        {

            SgtLogger.debuglog("Undocking all");

            foreach(int id in GetConnectedWorlds())
            {
                SgtLogger.debuglog("from World: " + id);
                UnDockFromTargetWorld(id, force);
            }
        }

        public static void ConnectTwo(DockingManager door1mng, DockingManager door2mng, DockingDoor OverwriteOwn = null)
        {
            if (OverwriteOwn != null && OverwriteOwn.GetConnec() != null)
                OverwriteOwn = null;

            var door1 = OverwriteOwn != null? OverwriteOwn : door1mng.DockingDoors.First(k => k.Value == -1).Key;
            var door2 = door2mng.DockingDoors.First(k => k.Value == -1).Key;
            if (door1 == null || door2 == null)
                return;


            door1mng.DockingDoors[door1] = door2mng.MyWorldId;
            door2mng.DockingDoors[door2] = door1mng.MyWorldId;

            door1.ConnecDoor(door2);
            door2.ConnecDoor(door1);
            door1.Teleporter.EnableTwoWayTarget(true);
            //DetailsScreen.Instance.Refresh(door1.gameObject);
        }


        //public void StartupConnect(DockingDoor door1, DockingDoor door2)
        //{
        //    door1.ConnecDoor(door2);
        //    door2.ConnecDoor(door1);
        //    door1.Teleporter.EnableTwoWayTarget(true);
        //}

        void InitPendingUndock(DockingDoor door, bool cleanup = false, System.Action OnFinishedUndock = null)
        {
            SgtLogger.l("Pending undock: " + door.GetMyWorldId()+" force: "+cleanup);

            if (cleanup)
                UndockDoor(door, true, OnFinishedUndock);
            else
            {
                if(!PendingUndocks.Keys.Contains(door) && !PendingUndocks.Keys.Contains(door.GetConnec())) 
                    PendingUndocks.Add(door, OnFinishedUndock);
            }
        }

        public static Dictionary<DockingDoor, System.Action> PendingUndocks = new Dictionary<DockingDoor, System.Action>();


        void UndockDoor(DockingDoor door, bool cleanup = false,System.Action OnFinishedUndock = null)
        {
            door.Teleporter.EnableTwoWayTarget(false);
            var door2 = door.GetConnec();

            door2.dManager.DockingDoors[door2] = -1;
            door.dManager.DockingDoors[door] = -1;

            door.DisconnecDoor(cleanup);
            door2.DisconnecDoor(cleanup);

            int targetWorldId = door2.GetMyWorldId();

            if (OnFinishedUndock != null)
                   OnFinishedUndock.Invoke();

            if(ClusterManager.Instance.GetWorld(targetWorldId)!=null)
                ClusterManager.Instance.GetWorld(targetWorldId).SetParentIdx(targetWorldId);

            if(ClusterManager.Instance.GetWorld(MyWorldId)!=null)
                ClusterManager.Instance.GetWorld(MyWorldId).SetParentIdx(MyWorldId);

            door.gameObject.Trigger((int)GameHashes.RocketLaunched);
            door2.gameObject.Trigger((int)GameHashes.RocketLaunched);
            

            //DetailsScreen.Instance.Refresh(door.gameObject);
        }

        public void UnDockFromTargetWorld(int targetWorldId,bool cleanup=false, System.Action OnFinishedUndock = null)
        {
            SgtLogger.debuglog("TargetWorldToUndock: " + targetWorldId);

            if (targetWorldId == -1)
                return;

            var door = DockingDoors.FirstOrDefault(d => d.Value == targetWorldId).Key;
            if(door == default(DockingDoor))
            {
                Debug.LogWarning("No connection to undock from found");
                return;
            }
            InitPendingUndock(door, cleanup, OnFinishedUndock);
        }

        public string GetProperName() => this.GetComponent<Clustercraft>().name;

    }
    public enum DockableType
    {
        Rocket = 0,
        SpaceStation = 1,
        Derelict = 2
    }
}
