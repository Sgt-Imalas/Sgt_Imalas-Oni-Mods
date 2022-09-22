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
        /// <summary>
        /// My door + connectedDoor
        /// </summary>
        int OwnWorldId=-1;

        public Dictionary<DockingDoor, DockingManager> DockingDoors = new Dictionary<DockingDoor, DockingManager>();
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
            base.OnLoadLevel();
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
                DockingDoors.Add(door,null);
            }
            Debug.Log("ADDED DOOR!, ID: " + OwnWorldId);
        }
        public void RemoveDoor(DockingDoor door)
        {
            if (DockingDoors.ContainsKey(door))
            {
                ///Disconecc;
                DockingDoors.Remove(door);
            }
        }

        public bool CanDock()
        {
            return DockingDoors.Any(k => k.Value == null);
        }

        public bool HasDoors()
        {
            return DockingDoors.Count>0;
        }

        public bool IsDockedTo(DockingManager target)
        {
            return DockingDoors.ContainsValue(target);
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
            if(IsDockedTo(target))
            {
                Debug.Log("Already Docked");
                return;
            }
            ConnectTwo(this, target);
        }
        public static void ConnectTwo(DockingManager door1mng, DockingManager door2mng)
        {
            var door1 = door1mng.DockingDoors.First(k => k.Value == null).Key;
            var door2 = door2mng.DockingDoors.First(k => k.Value == null).Key;

            door1mng.DockingDoors[door1] = door2mng;
            door2mng.DockingDoors[door2] = door1mng;

            door1.ConnecDoor(door2);
            door2.ConnecDoor(door1);

            door1.Teleporter.EnableTwoWayTarget(true);

        }
        public void UnDockFromTargetWorld(int targetWorldId)
        {
            var door = DockingDoors.Keys.First(d => d.GetConnec().Manager.OwnWorldId == targetWorldId);
            if(door == null)
            {
                Debug.LogWarning("No connection to undock from found");
            }
            var door2 = door.GetConnec();
            door2.Manager.DockingDoors[door2] = null;
            door2.DisconnecDoor();

            DockingDoors[door] = null;
            door.DisconnecDoor();

            door.Teleporter.EnableTwoWayTarget(false);
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
