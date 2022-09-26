using KSerialization;
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

        public Dictionary<DockingDoor,int> DockingDoors = new Dictionary<DockingDoor, int>();
        public int GetWorldId() => OwnWorldId;

        DockableType Type = DockableType.Rocket;

        public DockableType GetType => Type;

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
            Debug.Log("AddedDockable");
        }
        protected override void OnCleanUp()
        {
            ModAssets.Dockables.Remove(this);
            base.OnCleanUp();
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
            Debug.Log("ADDED DOOR!, ID: " + OwnWorldId+", Doorcount: "+DockingDoors.Count());
        }
        public void RemoveDoor(DockingDoor door)
        {
            if (DockingDoors.ContainsKey(door))
            {
                Debug.Log(door + "<-> " + door.GetMyWorldId());
                UnDockFromTargetWorld(door.GetConnec().GetMyWorldId());
                ///Disconecc;
                //door.DisconnecDoor();
                DockingDoors.Remove(door);
            }
        }

        public bool CanDock()
        {
            return DockingDoors.Any(k => k.Key.GetConnec() == null);
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
            Debug.Log(prevDockingState == 0 ? "Trying to dock to " + targetWorld : "Trying To Undock from " + targetWorld);
            if (prevDockingState == 0)
                DockToTargetWorld(targetWorld);
            else
                UnDockFromTargetWorld(targetWorld);
        }

        public void DockToTargetWorld(int targetWorldId)
        {
            var target = ModAssets.Dockables.Items.Find(mng => mng.OwnWorldId == targetWorldId);
            if (target == null || target.DockingDoors.Count == 0 || this.DockingDoors.Count==0)
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
        public static void ConnectTwo(DockingManager door1mng, DockingManager door2mng)
        {
            var door1 = door1mng.DockingDoors.First(k => k.Value == -1).Key;
            var door2 = door2mng.DockingDoors.First(k => k.Value == -1).Key;

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

        public void UnDockFromTargetWorld(int targetWorldId)
        {
            Debug.Log("TargetWorldToUndock: " + targetWorldId);
            var door = DockingDoors.Keys.First(d => d.GetConnec().GetMyWorldId() == targetWorldId);
            if(door == null)
            {
                Debug.LogWarning("No connection to undock from found");
            }
            door.Teleporter.EnableTwoWayTarget(false);
            var door2 = door.GetConnec();
            door2.dManager.DockingDoors[door2] = -1;
            door2.DisconnecDoor();
            DockingDoors[door] = -1;
            door.DisconnecDoor();

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
