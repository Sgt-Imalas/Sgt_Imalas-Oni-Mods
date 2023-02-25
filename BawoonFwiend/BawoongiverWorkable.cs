using Klei.AI;
using Klei;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using HarmonyLib;
using System.Reflection;
using Database;

namespace BawoonFwiend
{
    internal class BawoongiverWorkable : Workable, IWorkerPrioritizable
    {
        [MyCmpReq]
        public Operational operational;
        public int basePriority = RELAXATION.PRIORITY.TIER5;

        ///public static Type VaricolouredBalloonsHelperType = Type.GetType("VaricolouredBalloons.VaricolouredBalloonsHelper, VaricolouredBalloons", false, false);
        [MyCmpGet]
        Bawoongiver bawoongiver;


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
            this.SetWorkTime(2f);
            ///ColorIntegration(this.gameObject);
        }

        public override void OnStartWork(Worker worker)
        {
            this.operational.SetActive(true);

            BalloonOverrideSymbol balloonOverride = this.bawoongiver.CurrentSkin;
            if (balloonOverride.animFile.IsNone())
            {
                //worker.gameObject.GetComponent<SymbolOverrideController>().TryRemoveSymbolOverride((HashedString)"body");
                //worker.gameObject.GetComponent<SymbolOverrideController>().AddSymbolOverride((HashedString)"body", Assets.GetAnim((HashedString)"balloon_anim_kanim").GetData().build.GetSymbol((KAnimHashedString)"body"));
            }
            else
            {
                worker.gameObject.GetComponent<SymbolOverrideController>().AddSymbolOverride((HashedString)"body", balloonOverride.symbol.Unwrap());
            }
        }

        public override void OnCompleteWork(Worker worker)
        {
            Storage component1 = this.GetComponent<Storage>();
            component1.ConsumeIgnoringDisease(ModAssets.Tags.BalloonGas, Bawoongiver.BloongasUsage);

            GameObject gameObject = Util.KInstantiate(Assets.GetPrefab((Tag)EquippableBalloonConfig.ID), worker.transform.GetPosition());
            gameObject.GetComponent<Equippable>().Assign((IAssignableIdentity)worker.GetComponent<MinionIdentity>());
            gameObject.GetComponent<Equippable>().isEquipped = true;
            gameObject.SetActive(true);
            base.OnCompleteWork(worker);
            
            gameObject.GetComponent<EquippableBalloon>().SetBalloonOverride(bawoongiver.CurrentSkin);


            if (worker.TryGetComponent<Effects>(out var component)&& Config.Instance.MachineGivenBalloonBuff<8)
            {
                component.Add(ModAssets.JustAMachine, true);
            }

            //Effects component2 = worker.GetComponent<Effects>();
            //if (!string.IsNullOrEmpty(BawoongiverWorkable.specificEffect))
            //    component2.Add(BawoongiverWorkable.specificEffect, true);
            //if (string.IsNullOrEmpty(BawoongiverWorkable.trackingEffect))
            //    return;
            //component2.Add(BawoongiverWorkable.trackingEffect, true);
        }
        //static uint GetSymbolOverrideIdx(GameObject go)
        //{
        //    if (VaricolouredBalloonsHelperType == null)
        //        return 0;
        //    var obj = go.gameObject.GetComponent(VaricolouredBalloonsHelperType);
        //    //foreach (var cmp in VaricolouredBalloonsHelperType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) 
        //    //   SgtLogger.l(cmp.Name.ToString(),"GET Field");
        //    //foreach (var cmp in VaricolouredBalloonsHelperType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
        //    //    SgtLogger.l(cmp.Name.ToString(), "GET method");

        //    var component = go.GetComponent(VaricolouredBalloonsHelperType);
        //    var fieldInfo = (uint)Traverse.Create(component).Method("get_ArtistBalloonSymbolIdx").GetValue();
        //    return fieldInfo;
        //}
        //static void SetSymbolOverrideIdx(GameObject go, uint val)
        //{
        //    if (VaricolouredBalloonsHelperType == null)
        //        return;
        //    //foreach (var cmp in go.gameObject.GetComponents<UnityEngine.Object>())
        //    //    SgtLogger.l(cmp.GetType().ToString(), "SET DEBUG");
        //    var component = go.GetComponent(VaricolouredBalloonsHelperType);
        //    Traverse.Create(component).Field("receiverballoonsymbolidx").SetValue(val);
        //}

        //static void ColorIntegration(GameObject go)
        //{
        //    if (VaricolouredBalloonsHelperType == null)
        //    {
        //        return;
        //    }
        //    var RandomizeMethod = VaricolouredBalloonsHelperType.GetMethod("RandomizeArtistBalloonSymbolIdx", BindingFlags.Instance | BindingFlags.NonPublic); 
        //    if (RandomizeMethod == null)
        //    {
        //        SgtLogger.logwarning("Method Not Found");
        //        return;
        //    }
        //    var component = go.GetComponent(VaricolouredBalloonsHelperType);
        //    Traverse.Create(component).Method("RandomizeArtistBalloonSymbolIdx").GetValue();
        //   // SgtLogger.l("integration called");

        //}
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
