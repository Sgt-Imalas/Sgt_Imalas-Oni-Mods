using Cryopod.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryopod
{
    class OpenCryopodWorkable : Workable
    {
        private Chore openChore;
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.synchronizeAnims = false;
            this.overrideAnims = new KAnimFile[1]
            {
            Assets.GetAnim((HashedString) "anim_interacts_warp_portal_sender_kanim")
            };
            this.SetWorkTime(3f);
            this.showProgressBar = false;
        }
        
        public void CancelFreezeChore(object param = null)
        {
            if (this.openChore == null)
                return;
            this.openChore.Cancel("User cancelled");
            this.openChore = (Chore)null;
        }
        private void CompleteOpenChore()
        {
            this.GetComponent<CryopodReusable>().OpenChoreDone();
            this.openChore = (Chore)null;
            Game.Instance.userMenu.Refresh(this.gameObject);
        }
        public Chore CreateOpenChore()
        {
            openChore = (Chore)new WorkChore<OpenCryopodWorkable>(Db.Get().ChoreTypes.Migrate, (IStateMachineTarget)this, on_complete: ((System.Action<Chore>)(o => this.CompleteOpenChore())), priority_class: PriorityScreen.PriorityClass.high);
            
            return openChore;
        }
        protected override void OnStartWork(Worker worker) => base.OnStartWork(worker);

        protected override bool OnWorkTick(Worker worker, float dt)
        {
            base.OnWorkTick(worker, dt);
            Debug.Log(worker);
            return true;
        }
        protected override void OnStopWork(Worker worker) => base.OnStopWork(worker);

        protected override void OnCompleteWork(Worker worker)
        {
            CompleteOpenChore();
        }
    }
}
