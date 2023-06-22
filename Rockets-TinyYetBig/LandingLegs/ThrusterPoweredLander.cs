using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static AnimCommandFile;
using static Components;
using static ConduitFlowVisualizer.RenderMeshTask;

namespace Rockets_TinyYetBig.LandingLegs
{


    public class ThrusterPoweredLander : StateMachineComponent<ThrusterPoweredLander.StatesInstance>
    {
        [Serialize]
        public Tag previewTag;
        [Serialize]
        public bool deployOnLanding = true;
        [Serialize]
        public List<Type> cmpsToEnable;

        [Serialize]
        public float flightAnimOffset = 50f;

        [Serialize]
        public bool hasLandedBool = false;


        public float exhaustEmitRate = 50f;

        public float exhaustTemperature = 1263.15f;

        public SimHashes exhaustElement = SimHashes.CarbonDioxide;

        public GameObject landingPreview;
        public override void OnSpawn()
        {
            smi.StartSM();
        }


        public void ResetAnimPosition()
        {
            GetComponent<KBatchedAnimController>().Offset = Vector3.up * flightAnimOffset;
        }

        public void OnJettisoned()
        {
            bool hasLanded = smi.sm.isLanded.Get(smi);
            SgtLogger.l("onjettisoned");
            SgtLogger.l(hasLanded.ToString(), "haslanded");


            int cell = Grid.PosToCell(this.gameObject);
            flightAnimOffset = (float)(ClusterManager.Instance.GetWorld((int)Grid.WorldIdx[cell]).maximumBounds.y - (double)this.gameObject.transform.GetPosition().y) + 100.0f;
            SgtLogger.l(flightAnimOffset.ToString(), "StartingOffset");

            currentVelocity = -10.0f; // 100 m/s
            currentAcceleration = 9.81f; //9.81 m/s^2           
            landingSafetyMargin = 1.5f; // Soft landing starts at 3m altitude
            landingSpeed = -1f; // 1 m/s

        }

        public void ShowLandingPreview(bool show)
        {
            if (show)
            {
                landingPreview = Util.KInstantiate(Assets.GetPrefab(previewTag), base.transform.GetPosition(), Quaternion.identity, base.gameObject);
                landingPreview.SetActive(value: true);
            }
            else
            {
                landingPreview.DeleteObject();
                landingPreview = null;
            }
        }



        public float currentVelocity = -10.0f; // 100 m/s
        public float currentAcceleration = 9.81f; //9.81 m/s^2
        public float landingSafetyMargin = 3f; // Soft landing starts at 3m altitude
        public float landingSpeed = -1f; // 1 m/s

        public void LandingUpdate(float dt)
        {
            if (dt == 0 || dt == float.NaN || float.IsInfinity(dt))
                return;

            SgtLogger.l(string.Format("currentHeight: {0}, currentVelocity: {1}, currentAcceleration: {2}", flightAnimOffset, currentVelocity, currentAcceleration), "before");

            // Landing computer can control acceleration directly, so imagine 
            float targetVelocity = -flightAnimOffset + landingSafetyMargin;
            currentAcceleration = (targetVelocity - currentVelocity) / dt;

            currentVelocity += currentAcceleration * dt;
            if (currentVelocity > landingSpeed)
                currentVelocity = landingSpeed;

            flightAnimOffset += currentVelocity * dt;
            if (flightAnimOffset < 0)
                flightAnimOffset = 0;

            SgtLogger.l(string.Format("currentHeight: {0}, currentVelocity: {1}, currentAcceleration: {2}", flightAnimOffset, currentVelocity, currentAcceleration), "after");


            ResetAnimPosition();
            int num = Grid.PosToCell(base.gameObject.transform.GetPosition() + new Vector3(0f, flightAnimOffset, 0f));
            if (Grid.IsValidCell(num))
            {
                SimMessages.EmitMass(num, ElementLoader.GetElementIndex(exhaustElement), dt * exhaustEmitRate, exhaustTemperature, 0, 0);
            }
        }

        public void DoLand()
        {
            base.smi.master.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
            OccupyArea occupy = base.smi.GetComponent<OccupyArea>();
            if (occupy != null)
            {
                occupy.ApplyToCells = true;
            }

            if (deployOnLanding && CheckIfLoaded())
            {
                smi.sm.emptyCargo.Trigger(smi);
            }
            SgtLogger.l("Landed");
            if (cmpsToEnable != null && cmpsToEnable.Count > 0)
            {
                foreach (var cmp in cmpsToEnable)
                {
                    SgtLogger.l("Trying to enable " + cmp.ToString());
                    var component = gameObject.GetComponent(cmp);
                    if (component != null && component is KMonoBehaviour)
                    {
                        SgtLogger.l(component.ToString(), "Enabled");
                        (component as KMonoBehaviour).enabled = true;
                        if (component is OccupyArea)
                        {
                            (component as OccupyArea).UpdateOccupiedArea();
                        }

                    }
                }
            }

            base.smi.master.gameObject.Trigger(1591811118, this);
        }

        public bool CheckIfLoaded()
        {
            bool flag = false;
            MinionStorage component = GetComponent<MinionStorage>();
            if (component != null)
            {
                flag |= component.GetStoredMinionInfo().Count > 0;
            }

            Storage component2 = GetComponent<Storage>();
            if (component2 != null && !component2.IsEmpty())
            {
                flag = true;
            }

            if (flag != smi.sm.hasCargo.Get(smi))
            {
                smi.sm.hasCargo.Set(flag, smi);
            }

            return flag;
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, ThrusterPoweredLander, object>.GameInstance
        {
            public StatesInstance(ThrusterPoweredLander master) : base(master)
            {
            }
        }


        public class States : GameStateMachine<States, StatesInstance, ThrusterPoweredLander>
        {
            public class CrashedStates : State
            {
                public State loaded;

                public State emptying;

                public State empty;
            }

            public BoolParameter hasCargo;

            public Signal emptyCargo;

            public State init;

            public State stored;

            public State landing;

            public State land;

            public CrashedStates grounded;

            public BoolParameter isLanded = new BoolParameter(default_value: false);

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = init;
                base.serializable = SerializeType.ParamsOnly;
                root
                    .Enter((smi) => smi.sm.isLanded.Set(smi.master.hasLandedBool, smi))
                    .InitializeOperationalFlag(RocketModule.landedFlag)
                    .Enter(delegate (StatesInstance smi)
                {
                    SgtLogger.l("Root " + smi.master.hasLandedBool);
                    smi.master.CheckIfLoaded();
                }).EventHandler(GameHashes.OnStorageChange, delegate (StatesInstance smi)
                {
                    smi.master.CheckIfLoaded();
                });
                init
                    .Enter((smi) => smi.sm.isLanded.Set(smi.master.hasLandedBool, smi))
                    .Enter((smi) => SgtLogger.l("init " + smi.master.hasLandedBool))
                    .ParamTransition(isLanded, grounded, IsTrue)
                    .GoTo(stored);
                stored.TagTransition(GameTags.Stored, landing, on_remove: true).EventHandler(GameHashes.JettisonedLander, delegate (StatesInstance smi)
                {
                });
                landing.PlayAnim("landing", KAnim.PlayMode.Loop).Enter(delegate (StatesInstance smi)
                {
                    smi.master.ShowLandingPreview(show: true);
                }).Exit(delegate (StatesInstance smi)
                {
                    smi.master.ShowLandingPreview(show: false);
                })
                    .Enter(delegate (StatesInstance smi)
                    {
                        smi.master.OnJettisoned();
                        smi.master.ResetAnimPosition();
                    })
                    .Update(delegate (StatesInstance smi, float dt)
                    {
                        smi.master.LandingUpdate(dt);
                    }, UpdateRate.SIM_33ms)
                    .Exit((smi) =>
                    {
                        SgtLogger.l("launchpad landed");
                        smi.master.DoLand();
                    })
                    .UpdateTransition(land, (smi, dt) => smi.master.flightAnimOffset <= 0.3f);
                land.PlayAnim("grounded_pre").OnAnimQueueComplete(grounded);
                grounded.DefaultState(grounded.loaded)
                    .ToggleOperationalFlag(RocketModule.landedFlag)
                    .Enter(delegate (StatesInstance smi)
                    {
                        smi.master.CheckIfLoaded();
                    })
                    .Enter(delegate (StatesInstance smi)
                    {
                        smi.master.hasLandedBool = true;
                        smi.sm.isLanded.Set(value: true, smi);
                    });
                grounded.loaded.PlayAnim("grounded")
                    .ParamTransition(hasCargo, grounded.empty, IsFalse)
                    .OnSignal(emptyCargo, grounded.emptying)
                    ;
                grounded.emptying.PlayAnim("deploying").TriggerOnEnter(GameHashes.JettisonCargo).OnAnimQueueComplete(grounded.empty);
                grounded.empty.PlayAnim("deployed").ParamTransition(hasCargo, grounded.loaded, IsTrue);
            }

        }
    }
}
