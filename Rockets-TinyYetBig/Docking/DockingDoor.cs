using KSerialization;
using Rockets_TinyYetBig.Docking;
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
    public class DockingDoor : IDockable
    {
        /// <summary>
        /// Transfer Storages
        /// </summary>

        public CellOffset porterOffset = new CellOffset(0, 0);

        public override void ConnecDockable(IDockable d)
        {
            base.ConnecDockable (d);

            if (connected.Get().HasDupeTeleporter)
            {

                Teleporter.SetTarget(d.Teleporter);
               // assignable.canBeAssigned = true;
            }
            if (!this.gameObject.IsNullOrDestroyed() && gameObject.TryGetComponent<KBatchedAnimController>(out var kanim))
            {
                kanim.Play("extending");
                kanim.Queue("extended");
            }
            //DetailsScreen.Instance.Refresh(gameObject);
        }


        public override void DisconnecDoor(bool skipanim = false)
        {
            base.DisconnecDoor(skipanim);

            //assignable.Unassign();
            //assignable.canBeAssigned = false;
            Teleporter.SetTarget(null);

            if (skipanim)
                return;

            if (gameObject.TryGetComponent<KBatchedAnimController>(out var kanim))
            {
                SgtLogger.l("cleanup: "+skipanim);
                if (!skipanim)
                {
                    kanim.Play("retracting");
                    kanim.Queue("retracted");
                }
                else
                {
                    kanim.Play("retracted");
                }
            }
            //DetailsScreen.Instance.Refresh(gameObject);
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
        //public override GameObject GetWorldObject()
        //{
        //    //return this.GetMyWorld().gameObject;
        //}

        public override void OnSpawn()
        {
            base.OnSpawn();
            string startKanim = string.Empty;
            if (connected != null && connected.Get() != null)
            {
                if (connected.Get().HasDupeTeleporter)
                    Teleporter.SetTarget(connected.Get().Teleporter);
                startKanim = ("extended");
              //  assignable.canBeAssigned = true;
            }
            else
            {
                startKanim = ("retracted");
                //assignable.Unassign();
               // assignable.canBeAssigned = false;
            }
            if(gameObject.TryGetComponent<KBatchedAnimController>(out var kanim))
            {
                kanim.Play(startKanim);
            }
        }
    }
}
