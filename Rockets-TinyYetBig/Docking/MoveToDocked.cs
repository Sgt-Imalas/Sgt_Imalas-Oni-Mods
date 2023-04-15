using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Docking
{
    class MoveToDocked : Workable
    {
        [MyCmpReq]
        public Assignable assignable;
        [MyCmpReq]
        public DockingDoor door;
        [MyCmpReq]
        public NavTeleporter port;

        private Chore MoveChore;
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.assignable.OnAssign += new System.Action<IAssignableIdentity>(this.Assign);
            this.synchronizeAnims = false;
            this.overrideAnims = new KAnimFile[1]
            {
            Assets.GetAnim((HashedString) "anim_sleep_bed_kanim")
            };
            this.SetWorkTime(float.PositiveInfinity);
            this.showProgressBar = false;
        }
        private void Assign(IAssignableIdentity new_assignee)
        {
            this.CancelFreezeChore();
            if (new_assignee == null)
                return;
            this.CreateFreezeChore();
        }
        public void CancelFreezeChore(object param = null)
        {
            if (this.MoveChore == null)
                return;
            this.MoveChore.Cancel("User cancelled");
            this.MoveChore = (Chore)null;
        }
        private void CompleteFreezeChore()
        {
            this.MoveChore = (Chore)null;
            Game.Instance.userMenu.Refresh(this.gameObject);
        }
        public Chore CreateFreezeChore()
        {
            MoveChore = (Chore)new WorkChore<MoveToDocked>(Db.Get().ChoreTypes.Migrate, (IStateMachineTarget)this, on_complete: ((System.Action<Chore>)(o => this.CompleteFreezeChore())), priority_class: PriorityScreen.PriorityClass.high);
            MoveChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, (object)this.assignable);
            return MoveChore;
        }
        public override void OnStartWork(Worker worker) => base.OnStartWork(worker);

        public override bool OnWorkTick(Worker worker, float dt)
        {
            var connectedDoor = door.GetConnec();

            if (connectedDoor != null)
            {
                var nav = worker.GetComponent<Navigator>();
                int targetCell = Grid.PosToCell(connectedDoor);

                MoveToLocationMonitor.Instance smi = nav.GetSMI<MoveToLocationMonitor.Instance>();
                if (nav.CanReach(targetCell) && smi != null)
                {
                    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
                    smi.MoveToLocation(targetCell);
                    SelectTool.Instance.Activate();
                    assignable.Unassign();
                }
                else
                    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));

            }

            if (!(worker != null))
                return base.OnWorkTick(worker, dt);

            this.CompleteWork(worker);
            CompleteFreezeChore();
            return true;
        }
        public override void OnStopWork(Worker worker) => base.OnStopWork(worker);

        public override void OnCompleteWork(Worker worker)
        {
            if(door.dManager!= null) { }

            base.OnCompleteWork(worker);
        }
    }
}
