using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Behaviours
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class DockingDoor : KMonoBehaviour, ISidescreenButtonControl
    {
        /// <summary>
        /// Transfer Storages
        /// </summary>



        [MyCmpGet]
        public Assignable assignable;


        [MyCmpGet]
        public NavTeleporter Teleporter;

        [Serialize]
        Ref<DockingDoor> connected = null;

        public CellOffset porterOffset = new CellOffset(0,0);

        public DockingManager dManager;

        public CraftModuleInterface GetCraftModuleInterface()
        {
            return GetWorldObject().GetComponent<CraftModuleInterface>();
        }
        public CraftModuleInterface GetDockedCraftModuleInterface()
        {
            if (connected == null)
                return null;
            else 
                return connected.Get().GetWorldObject().GetComponent<CraftModuleInterface>();
        }

        public void ConnecDoor(DockingDoor d)
        {
            // SgtLogger.debuglog("Door: " + d);
            this.Trigger((int)GameHashes.RocketLanded);
            d.Trigger((int)GameHashes.RocketLanded);
            connected = new Ref<DockingDoor>(d);
            Teleporter.SetTarget(d.Teleporter);
            if (!this.gameObject.IsNullOrDestroyed() && gameObject.TryGetComponent<KBatchedAnimController>(out var kanim))
            {
                kanim.Play("extending");
                kanim.Queue("extended");
            }
            assignable.enabled = true;
        }
        public DockingDoor GetConnec()
        {
            if(connected!=null)
                return connected.Get();
            return null;
        }

        public int GetConnectedTargetWorldId()
        {
            if (connected != null)
                return connected.Get().GetMyWorldId();
            return -1;
        }

        public void DisconnecDoor(bool skipanim = false)
        {
#if DEBUG
            SgtLogger.debuglog(dManager.GetWorldId() + " disconneccted from " + connected.Get().dManager.GetWorldId());
#endif

            this.Trigger((int)GameHashes.RocketLaunched);
            connected = null;
            assignable.enabled = false;
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
#if DEBUG
                SgtLogger.debuglog("Disconnecting due to flight");
#endif
                dManager.UnDockFromTargetWorld(connected.Get().dManager.GetWorldId());
            }
        }

        public CellOffset GetRotatedOffset()
        {
            var offset = porterOffset;
            if (TryGetComponent<Rotatable>(out var rotatable))
            {
                offset = rotatable.GetRotatedCellOffset(porterOffset);
            }
            return offset;
        }
        public int GetPorterCell()
        {
            return Grid.OffsetCell(Grid.PosToCell(this), GetRotatedOffset());
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            int worldId = ClusterUtil.GetMyWorldId(this.gameObject); 
            //dManager = ModAssets.Dockables.Items.Find(item => item.GetWorldId() == worldId);//GetRocket().GetComponent<DockingdManager>();
            dManager = GetWorldObject().AddOrGet<DockingManager>();

            Teleporter.offset = GetRotatedOffset();
            Teleporter.OnCellChanged();

            dManager.StartupID(worldId);
            dManager.AddDoor(this);
            dManager.SetManagerType();
            string startKanim = string.Empty;
            if (connected != null && connected.Get() != null && connected.Get().Teleporter != null)
            {
                Teleporter.SetTarget(connected.Get().Teleporter);
                startKanim = ("extended");
                assignable.enabled = true;
            }
            else
            {
                startKanim = ("retracted");
                assignable.enabled = false;
            }
            if(gameObject.TryGetComponent<KBatchedAnimController>(out var kanim))
            {
                kanim.Play(startKanim);
            }
        }

        public override void OnCleanUp()
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

            SelectTool.Instance.Activate();
        }

        public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();
        public bool SidescreenButtonInteractable() => assignable.enabled;

        public bool SidescreenEnabled()
        {
            return true;
        }

        public int HorizontalGroupID()
        {
            return -1;
        }
        #endregion
    }
}
