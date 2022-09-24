using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Behaviours
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class DockingDoor : KMonoBehaviour
    {
        [MyCmpGet]
        public NavTeleporter Teleporter;

        [Serialize]
        Ref<DockingDoor> connected = null;

        public DockingManager Manager;

        public void ConnecDoor(DockingDoor d)
        {
           // Debug.Log("Door: " + d);
            connected = new Ref<DockingDoor>(d);
            Debug.Log(Manager.GetWorldId() +" conneccted to " + d.Manager.GetWorldId());
            Teleporter.SetTarget(d.Teleporter);
            ///DoStuffUpdateidk;
        }
        public DockingDoor GetConnec()
        {
            return connected.Get();
        }

        public void DisconnecDoor()
        {
            Debug.Log(Manager.GetWorldId() + " disconneccted from " + connected.Get().Manager.GetWorldId());
            connected = null;

            Teleporter.SetTarget(null);
            ///DoStuffUpdateidk;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            int worldId = ClusterUtil.GetMyWorldId(this.gameObject); 
            //Manager = ModAssets.Dockables.Items.Find(item => item.GetWorldId() == worldId);//GetRocket().GetComponent<DockingManager>();
            Manager = GetRocket().AddOrGet<DockingManager>();
            Manager.AddDoor(this);
        }
        protected override void OnCleanUp()
        {
            Manager.RemoveDoor(this);
            base.OnCleanUp();
        }

        private GameObject GetRocket()
        {
            WorldContainer world = ClusterUtil.GetMyWorld(this.gameObject);
            return (UnityEngine.Object)world == (UnityEngine.Object)null ? (GameObject)null : world.gameObject.GetComponent<Clustercraft>().gameObject;
        }
        public void Disconnect()
        {

        }
        public void Connect(DockingDoor door)
        {

        }
    }
}
