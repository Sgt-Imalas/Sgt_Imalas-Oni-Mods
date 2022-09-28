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
    class DockingDoor : KMonoBehaviour, ISidescreenButtonControl
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
            Teleporter.SetTarget(d.Teleporter);
            if (!this.gameObject.IsNullOrDestroyed() && gameObject.TryGetComponent<KBatchedAnimController>(out var kanim))
            {
                kanim.Play("extending");
                kanim.Queue("extended");
            }
        }
        public DockingDoor GetConnec()
        {
            if(connected!=null)
                return connected.Get();
            return null;
        }

        public void DisconnecDoor(bool skipanim = false)
        {
#if DEBUG
            Debug.Log(dManager.GetWorldId() + " disconneccted from " + connected.Get().dManager.GetWorldId());
#endif
            connected = null;

            Teleporter.SetTarget(null);
            if (gameObject.TryGetComponent<KBatchedAnimController>(out var kanim)&&!skipanim)
            {
                kanim.Play("retracting");
                kanim.Queue("retracted");
            }
        }

        private void UnDockOnFlight()
        {
            if (connected != null)
            {
                Debug.Log("Disconnecting due to flight");
                dManager.UnDockFromTargetWorld(connected.Get().dManager.GetWorldId());
            }
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            int worldId = ClusterUtil.GetMyWorldId(this.gameObject); 
            //dManager = ModAssets.Dockables.Items.Find(item => item.GetWorldId() == worldId);//GetRocket().GetComponent<DockingdManager>();
            dManager = GetWorldObject().AddOrGet<DockingManager>();
            dManager.StartupID(worldId);
            dManager.AddDoor(this);
            string startKanim = string.Empty;
            if (connected != null && connected.Get() != null && connected.Get().Teleporter != null )
            {
                Teleporter.SetTarget(connected.Get().Teleporter);
                startKanim=("extended");
            }
            else
                startKanim = ("retracted");
            if(gameObject.TryGetComponent<KBatchedAnimController>(out var kanim))
            {
                kanim.Play(startKanim);
            }
        }

        protected override void OnCleanUp()
        {
            dManager.RemoveDoor(this);
            base.OnCleanUp();
        }

        private GameObject GetWorldObject()
        {
            WorldContainer world = ClusterUtil.GetMyWorld(this.gameObject);
            return (UnityEngine.Object)world == (UnityEngine.Object)null ? (GameObject)null : world.gameObject.GetComponent<ClusterGridEntity>().gameObject;
        }

        #region Sidescreen

        public string SidescreenButtonText => STRINGS.UI_MOD.DOCKINGUI.BUTTON;

        public string SidescreenButtonTooltip => STRINGS.UI_MOD.DOCKINGUI.BUTTONINFO;

        public int ButtonSideScreenSortOrder()
        {
            return 20;
        }

        public void OnSidescreenButtonPressed()
        {
            ClusterManager.Instance.SetActiveWorld(connected.Get().GetMyWorld().id);
            ManagementMenu.Instance.CloseAll();
        }

        public bool SidescreenButtonInteractable()

        => connected != null;

        public bool SidescreenEnabled()
        {
            return true;
        }
        #endregion
    }
}
