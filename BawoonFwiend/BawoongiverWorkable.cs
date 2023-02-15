using Klei.AI;
using Klei;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace BawoonFwiend
{
    internal class BawoongiverWorkable : Workable, IWorkerPrioritizable
    {
        [MyCmpReq]
        public Operational operational;
        public int basePriority = RELAXATION.PRIORITY.TIER5;
        public static string trackingEffect = "RecentlyRecDrink";



        public BawoongiverWorkable() => this.SetReportType(ReportManager.ReportType.PersonalTime);

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.overrideAnims = new KAnimFile[1]
            {
                Assets.GetAnim((HashedString) "anim_interacts_balloon_receiver_kanim")
            };
            this.showProgressBar = true;
            this.resetProgressOnStop = true;
            this.synchronizeAnims = false;
            this.SetWorkTime(4f);
        }

        public override void OnStartWork(Worker worker) => this.operational.SetActive(true);

        public override void OnCompleteWork(Worker worker)
        {
            Storage component1 = this.GetComponent<Storage>();
            component1.ConsumeIgnoringDisease(ModAssets.Tags.BalloonGas, Bawoongiver.BloongasUsage);

            GameObject gameObject = Util.KInstantiate(Assets.GetPrefab((Tag)"EquippableBalloon"), worker.transform.GetPosition());
            gameObject.GetComponent<Equippable>().Assign((IAssignableIdentity)worker.GetComponent<MinionIdentity>());
            gameObject.GetComponent<Equippable>().isEquipped = true;
            gameObject.SetActive(true);
            //var bloon = gameObject.GetSMI<EquippableBalloon>();
            //bloon.smi.transitionTime = GameClock.Instance.GetTime() + 300;

            base.OnCompleteWork(worker);

            //Effects component2 = worker.GetComponent<Effects>();
            //if (!string.IsNullOrEmpty(BawoongiverWorkable.specificEffect))
            //    component2.Add(BawoongiverWorkable.specificEffect, true);
            //if (string.IsNullOrEmpty(BawoongiverWorkable.trackingEffect))
            //    return;
            //component2.Add(BawoongiverWorkable.trackingEffect, true);
        }

        public override void OnStopWork(Worker worker) => this.operational.SetActive(false);

        public bool GetWorkerPriority(Worker worker, out int priority)
        {
            priority = RELAXATION.PRIORITY.TIER5;
            worker.TryGetComponent<Effects>(out var component);
            if (component.HasEffect("HasBalloon"))
            {
                priority = 0;
                return false;
            }
            return true;
            }
        }

}
