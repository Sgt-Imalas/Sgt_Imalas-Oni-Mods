using Cryopod.Buildings;
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
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.synchronizeAnims = false;
            this.overrideAnims = new KAnimFile[1]
            {
      Assets.GetAnim((HashedString) "anim_interacts_incubator_kanim")
            };
            this.SetWorkTime(float.PositiveInfinity);
            this.showProgressBar = false;
        }

        protected override void OnStartWork(Worker worker) => base.OnStartWork(worker);

        protected override bool OnWorkTick(Worker worker, float dt)
        {
            if (!(worker != null))
                return base.OnWorkTick(worker, dt);
            GameObject gameObject1 = worker.gameObject;
            this.CompleteWork(worker);
            this.GetComponent<MinionStorage>().SerializeMinion(gameObject1);
            return true;
        }

        protected override void OnStopWork(Worker worker) => base.OnStopWork(worker);

        protected override void OnCompleteWork(Worker worker)
        {    
        }
    }
}