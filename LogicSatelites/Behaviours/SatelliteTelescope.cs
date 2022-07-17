using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LogicSatelites.Behaviours
{
    class SatelliteTelescope :
    GameStateMachine<SatelliteTelescope, SatelliteTelescope.Instance, IStateMachineTarget, SatelliteTelescope.Def>
    {
        public GameStateMachine<SatelliteTelescope, SatelliteTelescope.Instance, IStateMachineTarget, SatelliteTelescope.Def>.State all_work_complete;
        public GameStateMachine<SatelliteTelescope, SatelliteTelescope.Instance, IStateMachineTarget, SatelliteTelescope.Def>.State working;
        private GameObject telescopeTargetMarker;
        private AxialI currentTarget;
        public override void InitializeStates(out StateMachine.BaseState default_state)
        {

            default_state = (StateMachine.BaseState)this.working;
            this.working.
                EventTransition(GameHashes.ClusterFogOfWarRevealed,
                (smi => Game.Instance),
                this.all_work_complete,
                (smi => !smi.CheckHasAnalyzeTarget()))
                .Update((smi, dt) =>
                {                                                                                      //TUNING.ROCKETRY.CLUSTER_FOW.DEFAULT_CYCLES_PER_REVEAL         
                    float detectionIncrease = dt * ((float)((double)TUNING.ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL / (double)Config.Instance.SatelliteScannerSpeed / 600.0));

                    smi.currentPercentage = smi.m_fowManager.GetRevealCompleteFraction(smi.m_analyzeTarget)*100;
                    //Debug.Log(smi.currentPercentage+ " <-> "+ detectionIncrease);
                    if (smi.currentPercentage == 0.01f)
                    {
                        currentTarget = smi.m_analyzeTarget;
                        if (!(bool)(UnityEngine.Object)ClusterGrid.Instance.GetEntityOfLayerAtCell(currentTarget, EntityLayer.Telescope))
                        {
                            this.telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"TelescopeTarget"), Grid.SceneLayer.Background);
                            this.telescopeTargetMarker.SetActive(true);
                            this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(this.currentTarget);
                            this.telescopeTargetMarker.name = "Satellite Scan Target";

                        }
                    }
                    if (smi.currentPercentage + detectionIncrease >= 100f)
                    {
                        if ((UnityEngine.Object)this.telescopeTargetMarker != (UnityEngine.Object)null)
                            Util.KDestroyGameObject(this.telescopeTargetMarker);
                    }
                    smi.m_fowManager.EarnRevealPointsForLocation(this.currentTarget, detectionIncrease);
                });


        }

        public class Def : StateMachine.BaseDef
        {
            public int analyzeClusterRadius = Config.Instance.SatelliteScannerRange;
        }
        public new class Instance :
          GameStateMachine<SatelliteTelescope, SatelliteTelescope.Instance, IStateMachineTarget, SatelliteTelescope.Def>.GameInstance
        {
            [Serialize]
            private bool m_hasAnalyzeTarget;
            [Serialize]
            public AxialI m_analyzeTarget;
            [Serialize]
            public float currentPercentage; 

            public ClusterFogOfWarManager.Instance m_fowManager;

            public Instance(IStateMachineTarget smi, SatelliteTelescope.Def def) : base(smi, def)
            {
                m_fowManager = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
                currentPercentage = m_fowManager.GetRevealCompleteFraction(m_analyzeTarget);
            }

            public bool CheckHasAnalyzeTarget()
            {
                ClusterFogOfWarManager.Instance smi = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
                if (this.m_hasAnalyzeTarget && !smi.IsLocationRevealed(this.m_analyzeTarget))
                    return true;
                AxialI myWorldLocation = this.GetAxialLocation();
                this.m_hasAnalyzeTarget = smi.GetUnrevealedLocationWithinRadius(myWorldLocation, this.def.analyzeClusterRadius, out this.m_analyzeTarget);
                return this.m_hasAnalyzeTarget;
            }
            public AxialI GetAxialLocation()
            {
                return this.GetComponent<SatelliteGridEntity>().Location;
            }
            public AxialI GetAnalyzeTarget()
            {
                Debug.Assert(this.m_hasAnalyzeTarget, (object)"GetAnalyzeTarget called but this telescope has no target assigned.");
                return this.m_analyzeTarget;
            }
        }

    }
}
