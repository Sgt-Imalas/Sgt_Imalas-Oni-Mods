using Cryopod.Buildings;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryopod
{
    class CryopodFreezeWorkable : Workable
    {
        [MyCmpReq]
        public Assignable assignable;
        private Chore freezeChore;


        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.assignable.OnAssign += new System.Action<IAssignableIdentity>(this.Assign);
            this.synchronizeAnims = false;
            this.overrideAnims = new KAnimFile[1]
            {
            Assets.GetAnim((HashedString) "anim_interacts_warp_portal_sender_kanim")
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
            if (this.freezeChore == null)
                return;
            this.freezeChore.Cancel("User cancelled");
            this.freezeChore = (Chore)null;
        }
        private void CompleteFreezeChore()
        {
            this.GetComponent<CryopodReusable>().FreezeChoreDone();
            this.freezeChore = (Chore)null;
            Game.Instance.userMenu.Refresh(this.gameObject);
        }
        public Chore CreateFreezeChore()
        {
            freezeChore = (Chore)new WorkChore<CryopodFreezeWorkable>(Db.Get().ChoreTypes.Migrate, (IStateMachineTarget)this, on_complete: ((System.Action<Chore>)(o => this.CompleteFreezeChore())), priority_class: PriorityScreen.PriorityClass.high);
            freezeChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, (object)this.assignable);
            return freezeChore;
        }
        protected override void OnStartWork(Worker worker) => base.OnStartWork(worker);

        protected override bool OnWorkTick(Worker worker, float dt)
        {
            if (!(worker != null))
                return base.OnWorkTick(worker, dt);
            GameObject gameObject1 = worker.gameObject;
            this.CompleteWork(worker);
            var cryopod = GetComponent<CryopodReusable>();
            foreach (SicknessInstance sickness in worker.GetComponent<MinionModifiers>().sicknesses)
            {
                cryopod.
                StoredSicknessIDs.Add(sickness.ExposureInfo.sicknessID);
            }
            
            var hp = worker.GetComponent<Health>();
            cryopod.storedDupeDamage = hp.maxHitPoints- hp.hitPoints; 
            cryopod.RefreshSideScreen();
            this.GetComponent<MinionStorage>().SerializeMinion(gameObject1);
            CompleteFreezeChore();
            return true;
        }
        protected override void OnStopWork(Worker worker) => base.OnStopWork(worker);

        protected override void OnCompleteWork(Worker worker)
        {    
        }
    }
}