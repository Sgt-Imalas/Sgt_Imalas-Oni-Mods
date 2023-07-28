using KSerialization;
using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Docking
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class IDockable : KMonoBehaviour
    {
        [Serialize]
        public Ref<IDockable> connected = null;

        [MyCmpGet]
        public Assignable assignable;
        [MyCmpGet]
        public NavTeleporter Teleporter;
        [MyCmpGet]
        public MoveToDocked MoveTo;

        public bool HasDupeTeleporter => Teleporter != null && MoveTo != null && assignable != null ;


        public DockingManager dManager;

        public virtual CraftModuleInterface GetCraftModuleInterface()
        {
            return GetWorldObject().GetComponent<CraftModuleInterface>();
        }
        public virtual CraftModuleInterface GetDockedCraftModuleInterface()
        {
            if (connected == null)
                return null;
            else
            {
                if (connected.Get() != null && connected.Get().GetWorldObject() != null && connected.Get().GetWorldObject().TryGetComponent<CraftModuleInterface>(out var interfac))
                    return interfac;
                return null;
            }
        }

        public virtual void ConnecDockable(IDockable d)
        {
            this.Trigger((int)GameHashes.RocketLanded);
            d.Trigger((int)GameHashes.RocketLanded);
            this.Trigger((int)GameHashes.ChainedNetworkChanged);
            connected = new Ref<IDockable>(d);
        }

        public bool IsConnected => GetConnec() != null;

        public virtual IDockable GetConnec()
        {
            if (connected != null)
                return connected.Get();
            return null;
        }

        public virtual int GetConnectedTargetWorldId()
        {
            if (connected != null)
                return connected.Get().GetMyWorldId();
            return -1;
        }

        public virtual void DisconnecDoor(bool skipanim = false)
        {
#if DEBUG
            SgtLogger.debuglog(dManager.GetWorldId() + " disconneccted from " + connected.Get().dManager.GetWorldId());
#endif

            this.Trigger((int)GameHashes.RocketLaunched);
            this.Trigger((int)GameHashes.ChainedNetworkChanged);
            if (this.gameObject.IsNullOrDestroyed())
            {
                SgtLogger.l("wasdestroyed");
                return;
            }

            if (gameObject == null) return;

            connected = null;

        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            dManager = gameObject.TryGetComponent<RocketModuleCluster>(out var module) 
                ? module.CraftInterface.gameObject.AddOrGet<DockingManager>() 
                : ClusterUtil.GetMyWorld(this.gameObject).gameObject.AddOrGet<DockingManager>();
            dManager.AddDockable(this);
            dManager.SetManagerType();
        }

        public override void OnCleanUp()
        {
            dManager.RemoveDockable(this);
            base.OnCleanUp();
        }

        public virtual GameObject GetWorldObject()
        {
            WorldContainer world = ClusterManager.Instance.GetWorld(dManager.WorldId);
            return world == null ? null : world.gameObject;
        }
    }
}
