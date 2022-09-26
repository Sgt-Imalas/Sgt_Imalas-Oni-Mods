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

        public DockingManager dManager;

        public void ConnecDoor(DockingDoor d)
        {
           // Debug.Log("Door: " + d);
            connected = new Ref<DockingDoor>(d);
            Debug.Log(dManager.GetWorldId() +" conneccted to " + d.dManager.GetWorldId());
            Teleporter.SetTarget(d.Teleporter);
            ///DoStuffUpdateidk;
        }
        public DockingDoor GetConnec()
        {
            if(connected!=null)
                return connected.Get();
            return null;
        }

        public void DisconnecDoor()
        {
            Debug.Log(dManager.GetWorldId() + " disconneccted from " + connected.Get().dManager.GetWorldId());
            connected = null;

            Teleporter.SetTarget(null);
            ///DoStuffUpdateidk;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            int worldId = ClusterUtil.GetMyWorldId(this.gameObject); 
            //dManager = ModAssets.Dockables.Items.Find(item => item.GetWorldId() == worldId);//GetRocket().GetComponent<DockingdManager>();
            dManager = GetRocket().AddOrGet<DockingManager>();
            dManager.StartupID(worldId);
            dManager.AddDoor(this);
        }
        protected override void OnCleanUp()
        {
            dManager.RemoveDoor(this);
            base.OnCleanUp();
        }

        private GameObject GetRocket()
        {
            WorldContainer world = ClusterUtil.GetMyWorld(this.gameObject);
            return (UnityEngine.Object)world == (UnityEngine.Object)null ? (GameObject)null : world.gameObject.GetComponent<Clustercraft>().gameObject;
        }
    }
}
