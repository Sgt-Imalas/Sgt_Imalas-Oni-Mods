using KSerialization;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Behaviours
{
    [SerializationConfig(MemberSerialization.Invalid)]
    class DockingManager : KMonoBehaviour, IListableOption
    {
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
            Debug.Log("CraftType set to: "+ Type);
#endif
        }


        public Sprite GetDockingIcon()
        {
            Sprite returnVal =null;
            switch (Type)
            {
                case DockableType.Rocket:
                    returnVal = Assets.GetSprite("rocket_landing");
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

        protected override void OnSpawn()
        {
            base.OnSpawn();
            ModAssets.Dockables.Add(this);
#if DEBUG
            Debug.Log("AddedDockable");
#endif
        }
        protected override void OnCleanUp()
        {
            ModAssets.Dockables.Remove(this);
            base.OnCleanUp();
        }

        public string GetUiDoorInfo()
        {
            StringBuilder sb = new StringBuilder();
            int count = AvailableConnections();
            sb.Append(count);
            sb.Append(count!=1?" available connections":" available connection");
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
#if DEBUG
            Debug.Log("ADDED DOOR!, ID: " + OwnWorldId+", Doorcount: "+DockingDoors.Count());
#endif
        }
        public void RemoveDoor(DockingDoor door)
        {
            if (DockingDoors.ContainsKey(door))
            {
#if DEBUG
                Debug.Log(door + "<-> " + door.GetMyWorldId());
#endif
                UnDockFromTargetWorld(door.GetConnectedTargetWorldId(), true);
                ///Disconecc;
                //door.DisconnecDoor();
                DockingDoors.Remove(door);
            }
        }

        public bool CanDock()
        {
            return DockingDoors.Any(k => k.Key.GetConnec() == null)&&HasDoors();
        }
        public bool IsDockedToAny()
        {
            return DockingDoors.Any(k => k.Key.GetConnec() != null);
        }

        public bool HasDoors()
        {
            //Debug.Log("HAs Doors: " + DockingDoors.Count);
            return DockingDoors.Count>0;
        }

        public bool IsDockedTo(int WorldID)
        {
            return DockingDoors.ContainsValue(WorldID);
        }
        public void HandleUiDocking(int prevDockingState,int targetWorld)
        {
#if DEBUG
            Debug.Log(prevDockingState == 0 ? "Trying to dock to " + targetWorld : "Trying To Undock from " + targetWorld);
#endif
            if (prevDockingState == 0)
                DockToTargetWorld(targetWorld);
            else
                UnDockFromTargetWorld(targetWorld);
            //DetailsScreen.Instance.Refresh(gameObject); ///deletes all others
        }

        public void DockToTargetWorld(int targetWorldId)
        {
            if (!this.CanDock())
                return;

            var target = ModAssets.Dockables.Items.Find(mng => mng.OwnWorldId == targetWorldId);


            if (target == null || target.DockingDoors.Count == 0 || this.DockingDoors.Count==0 || !target.CanDock())
            {
                Debug.Log("No doors found");
                return;
            }
            if(IsDockedTo(targetWorldId))
            {
                Debug.Log("Already Docked");
                return;
            }
            ConnectTwo(this, target);
        }
        public void UndockAll()
        {
#if DEBUG
            Debug.Log("Undocking all");
#endif
            foreach(int id in DockingDoors.Values.ToList())
            {
#if DEBUG
                Debug.Log("World: " + id);
#endif
                UnDockFromTargetWorld(id);
            }
        }

        public static void ConnectTwo(DockingManager door1mng, DockingManager door2mng)
        {
            var door1 = door1mng.DockingDoors.First(k => k.Value == -1).Key;
            var door2 = door2mng.DockingDoors.First(k => k.Value == -1).Key;
            if (door1 == null || door2 == null)
                return;


            door1mng.DockingDoors[door1] = door2mng.OwnWorldId;
            door2mng.DockingDoors[door2] = door1mng.OwnWorldId;

            door1.ConnecDoor(door2);
            door2.ConnecDoor(door1);
            door1.Teleporter.EnableTwoWayTarget(true);
        }


        //public void StartupConnect(DockingDoor door1, DockingDoor door2)
        //{
        //    door1.ConnecDoor(door2);
        //    door2.ConnecDoor(door1);
        //    door1.Teleporter.EnableTwoWayTarget(true);
        //}

        public void UnDockFromTargetWorld(int targetWorldId,bool cleanup=false)
        {
#if DEBUG
            Debug.Log("TargetWorldToUndock: " + targetWorldId);
#endif
            if (targetWorldId == -1)
                return;

            var door = DockingDoors.First(d => d.Value == targetWorldId).Key;
            if(door == null)
            {
                Debug.LogWarning("No connection to undock from found");
            }
            door.Teleporter.EnableTwoWayTarget(false);
            var door2 = door.GetConnec();
            door2.dManager.DockingDoors[door2] = -1;
            door2.DisconnecDoor();
            DockingDoors[door] = -1;
            door.DisconnecDoor(cleanup);

        }

        public string GetProperName() => this.GetComponent<Clustercraft>().name;

        
    }
    enum DockableType
    {
        Rocket = 0,
        SpaceStation = 1,
        Derelict = 2
    }
}
