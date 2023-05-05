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

namespace Rockets_TinyYetBig.Behaviours
{
    [SerializationConfig(MemberSerialization.Invalid)]
    public class DockingManager : KMonoBehaviour, IListableOption, ISim1000ms
    {
        [MyCmpGet]
        Clustercraft clustercraft;

        public void StartupID(int world)
        {
            OwnWorldId = world;
        }

        /// <summary>
        /// My door + connectedDoor
        /// </summary>
        int OwnWorldId=-1;

        public Dictionary<DockingDoor, int> DockingDoors = new Dictionary<DockingDoor, int>();
        public int GetWorldId() => OwnWorldId;

        DockableType Type = DockableType.Rocket;

        public DockableType GetCraftType => Type;

        public void SetManagerType(int overrideType = -1)
        {
            if (OwnWorldId != -1)
            { 
            
            }

            
            if (OwnWorldId != -1)
            {
                if (SpaceStationManager.WorldIsSpaceStationInterior(OwnWorldId))
                {
                    Type = DockableType.SpaceStation;
                }
                else if(SpaceStationManager.WorldIsRocketInterior(OwnWorldId))
                {
                    Type = DockableType.Rocket;
                }
            }
#if DEBUG
            SgtLogger.debuglog("CraftType set to: "+ Type);
#endif
        }

        public bool AllUndocked()
        {
            foreach(DockingDoor door in DockingDoors.Keys)
            {
                if (door.IsConnected)
                    return true;
            }
            return false;
        }

        //bool IsUndockingFrom(int worldID)
        //{

        //}

        public void Sim1000ms(float dt)
        {
            List<DockingDoor> ToRemove = new List<DockingDoor>();
            foreach(var undockingProcess in PendingUndocks)
            {
                List<MinionIdentity> WrongWorldDupesHERE = new List<MinionIdentity>();
                List<MinionIdentity> WrongWorldDupesTHERE = new List<MinionIdentity>();
                if (undockingProcess.IsConnected && DockingDoors.ContainsKey(undockingProcess))
                {
                    PassengerRocketModule passengerOwn = undockingProcess.GetCraftModuleInterface().GetPassengerModule();
                    PassengerRocketModule passengerDock = undockingProcess.GetDockedCraftModuleInterface().GetPassengerModule();


                    passengerOwn.TryGetComponent<AssignmentGroupController>(out var assignmentGroupControllerOWN);
                    passengerDock.TryGetComponent<AssignmentGroupController>(out var assignmentGroupControllerDOCKED);


                    if (assignmentGroupControllerOWN != null)
                    {
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.dManager.OwnWorldId))
                        {
                            SgtLogger.l(minion.name, "minion");

                            if (!Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerOWN.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                SgtLogger.l(minion.name, "wrong here;");
                                WrongWorldDupesHERE.Add(minion);
                            }
                        }
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.GetConnec().dManager.OwnWorldId))
                        {
                            SgtLogger.l(minion.name, "minion there");
                            if (Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerOWN.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                SgtLogger.l(minion.name, "wrong there;");
                                WrongWorldDupesTHERE.Add(minion);
                            }
                        }
                    }
                    else if (assignmentGroupControllerDOCKED != null)
                    {
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.dManager.OwnWorldId))
                        {
                            SgtLogger.l(minion.name, "minion");

                            if (!Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerDOCKED.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                SgtLogger.l(minion.name, "wrong here;");
                                WrongWorldDupesHERE.Add(minion);
                            }
                        }
                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.GetConnec().dManager.OwnWorldId))
                        {
                            SgtLogger.l(minion.name, "minion there");
                            if (Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerDOCKED.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
                            {
                                SgtLogger.l(minion.name, "wrong there;");
                                WrongWorldDupesTHERE.Add(minion);
                            }
                        }
                    }
                    

                    foreach(var minion in WrongWorldDupesHERE)
                    {
                        minion.GetSMI<MoveToLocationMonitor.Instance>().MoveToLocation(undockingProcess.GetConnec().GetPorterCell());                        
                    }
                    foreach (var minion in WrongWorldDupesTHERE)
                    {
                        minion.GetSMI<MoveToLocationMonitor.Instance>().MoveToLocation(undockingProcess.GetPorterCell());
                    }

                    if (WrongWorldDupesHERE.Count == 0 && WrongWorldDupesTHERE.Count == 0)
                    {
                        SgtLogger.log(string.Format("Undocking world {0} from {1}, now that all dupes are moved", undockingProcess.GetMyWorldId(), undockingProcess.GetConnec().GetMyWorldId()));
                        UndockDoor(undockingProcess, false);
                        ToRemove.Add(undockingProcess);
                    }
                }
            }
            foreach(var done in ToRemove)
            {
                PendingUndocks.Remove(done);
            }
        }


        public Sprite GetDockingIcon()
        {
            Sprite returnVal = null;
            switch (Type)
            {
                case DockableType.Rocket:
                    returnVal = Assets.GetSprite("rocket_landing"); ///change to habitat icon TODO
                    break;
                case DockableType.SpaceStation:
                    returnVal = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim((HashedString)"gravitas_space_poi_kanim"), "station_1", true);
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
            StringBuilder sb = new StringBuilder();
            int count = AvailableConnections();
            sb.Append(count);
            sb.Append(count!=1?" available connections":" available connection"); //TODO
            return sb.ToString();
        }
        public int AvailableConnections()
        {
            int count = DockingDoors.Keys.ToList().FindAll(k => k.GetConnec() == null).Count();
            return count;
        }

        public void AddDoor(DockingDoor door)
        {
            if(OwnWorldId==-1)
                OwnWorldId = ClusterUtil.GetMyWorldId(door);
            if(!DockingDoors.ContainsKey(door))
            {
                int target = -1;
                if (door.GetConnec() != null)
                    target = door.GetConnec().GetMyWorldId();
                DockingDoors.Add(door, target);
            }
            SgtLogger.debuglog("Added new Docking Door!, ID: " + OwnWorldId+", Doorcount: "+DockingDoors.Count());

            //UpdateDeconstructionStates();

        }

        void UpdateDeconstructionStates()
        {
            if (Type == DockableType.SpaceStation)
            {
                if (DockingDoors.Count == 1)
                    DockingDoors.First().Key.gameObject.GetComponent<Deconstructable>().SetAllowDeconstruction(false);
                else if (DockingDoors.Count > 1)
                {
                    foreach (var unlockDoor in DockingDoors.Keys)
                    {
                        unlockDoor.gameObject.GetComponent<Deconstructable>().SetAllowDeconstruction(true);
                    }
                }
            }
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
                && clustercraft.status == Clustercraft.CraftStatus.InFlight;
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
        public void HandleUiDocking(int prevDockingState,int targetWorld)
        {
            SgtLogger.debuglog(prevDockingState == 0 ? "Trying to dock " + OwnWorldId + " to " + targetWorld : "Trying To Undock  " + OwnWorldId + "  from " + targetWorld);

            if (prevDockingState == 0)
                DockToTargetWorld(targetWorld);
            else
                UnDockFromTargetWorld(targetWorld);
        }

        public void HandleUiDockingByDoor(int prevDockingState, int targetWorld, DockingDoor door)
        {
            SgtLogger.debuglog(prevDockingState == 0 ? "Trying to dock " + OwnWorldId + " with dedicated door to " + targetWorld : "Trying To Undock " + OwnWorldId + " from " + targetWorld);

            if (prevDockingState == 0)
                DockToTargetWorld(targetWorld, door);
            else
                UnDockFromTargetWorld(targetWorld);
            //DetailsScreen.Instance.Refresh(gameObject); ///deletes all others
        }

        public void DockToTargetWorld(int targetWorldId, DockingDoor OwnDoor = null)
        {
            if (!this.CanDock())
                return;

            var target = ModAssets.Dockables.Items.Find(mng => mng.OwnWorldId == targetWorldId);


            if (target == null || target.DockingDoors.Count == 0 || this.DockingDoors.Count==0 || !target.CanDock())
            {
                SgtLogger.debuglog("No doors found");
                return;
            }
            if(IsDockedTo(targetWorldId))
            {
                SgtLogger.debuglog("Already Docked");
                return;
            }
            ConnectTwo(this, target, OwnDoor);

            if (SpaceStationManager.WorldIsSpaceStationInterior(OwnWorldId))
            {
                ClusterManager.Instance.GetWorld(targetWorldId).SetParentIdx(OwnWorldId);
            }
            else if (SpaceStationManager.WorldIsSpaceStationInterior(targetWorldId))
            {
                ClusterManager.Instance.GetWorld(OwnWorldId).SetParentIdx(targetWorldId);
            }
            
            else
                ClusterManager.Instance.GetWorld(OwnWorldId).SetParentIdx(targetWorldId);

        }
        public void UndockAll()
        {

            SgtLogger.debuglog("Undocking all");

            foreach(int id in DockingDoors.Values.ToList())
            {
                SgtLogger.debuglog("from World: " + id);
                UnDockFromTargetWorld(id);
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


            door1mng.DockingDoors[door1] = door2mng.OwnWorldId;
            door2mng.DockingDoors[door2] = door1mng.OwnWorldId;

            door1.ConnecDoor(door2);
            door2.ConnecDoor(door1);
            door1.Teleporter.EnableTwoWayTarget(true);
            DetailsScreen.Instance.Refresh(door1.gameObject);
            DetailsScreen.Instance.Refresh(door2.gameObject);
        }


        //public void StartupConnect(DockingDoor door1, DockingDoor door2)
        //{
        //    door1.ConnecDoor(door2);
        //    door2.ConnecDoor(door1);
        //    door1.Teleporter.EnableTwoWayTarget(true);
        //}

        void InitPendingUndock(DockingDoor door, bool cleanup = false)
        {
            SgtLogger.l("Pending undock: " + door.GetMyWorldId()+" force: "+cleanup);

            if (cleanup)
                UndockDoor(door, true);
            else
            {
                if(!PendingUndocks.Contains(door) && !PendingUndocks.Contains(door.GetConnec())) 
                    PendingUndocks.Add(door);
            }
        }

        public static List<DockingDoor> PendingUndocks = new List<DockingDoor>();


        void UndockDoor(DockingDoor door, bool cleanup = false)
        {
            

            door.Teleporter.EnableTwoWayTarget(false);
            var door2 = door.GetConnec();

            door2.dManager.DockingDoors[door2] = -1;
            door.dManager.DockingDoors[door] = -1;

            door.DisconnecDoor(cleanup);
            door2.DisconnecDoor(cleanup);

            int targetWorldId = door2.GetMyWorldId();



            ClusterManager.Instance.GetWorld(targetWorldId).SetParentIdx(targetWorldId);
            ClusterManager.Instance.GetWorld(OwnWorldId).SetParentIdx(OwnWorldId);

            DetailsScreen.Instance.Refresh(door.gameObject);
            DetailsScreen.Instance.Refresh(door2.gameObject);
        }

        public void UnDockFromTargetWorld(int targetWorldId,bool cleanup=false)
        {
            SgtLogger.debuglog("TargetWorldToUndock: " + targetWorldId);

            if (targetWorldId == -1)
                return;

            var door = DockingDoors.First(d => d.Value == targetWorldId).Key;
            if(door == null)
            {
                Debug.LogWarning("No connection to undock from found");
                return;
            }
            InitPendingUndock(door, cleanup);
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
