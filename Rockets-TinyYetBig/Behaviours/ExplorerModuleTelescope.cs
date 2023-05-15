using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Behaviours
{
    public class ExplorerModuleTelescope :
    GameStateMachine<ExplorerModuleTelescope, ExplorerModuleTelescope.Instance, IStateMachineTarget, ExplorerModuleTelescope.Def>
    {
        public class WorkingStates: State
        {
            public State working;
            public State all_work_complete;

        }
        public WorkingStates workingStates;
        public State grounded;
        private GameObject telescopeTargetMarker;
        private AxialI currentTarget;
        public override void InitializeStates(out BaseState default_state)
        {

            default_state = (BaseState)this.grounded;
            this.grounded
                //.Enter((smi) => SgtLogger.l("ENTERED STATE: grounded", "ExplorerSMI"))
                .Enter((smi) => smi.DestroyTelescope())
                .TagTransition(GameTags.RocketNotOnGround, workingStates);


            this.workingStates
                .DefaultState(this.workingStates.working)
                //.Enter((smi) => SgtLogger.l("ENTERED STATE: workingStates", "ExplorerSMI"))
                .TagTransition(GameTags.RocketNotOnGround, grounded, true);

            this.workingStates.working
                //.Enter((smi) => SgtLogger.l("ENTERED STATE: workingStates.working", "ExplorerSMI"))
                .
                EventTransition(GameHashes.ClusterFogOfWarRevealed,
                (smi => Game.Instance),
                this.workingStates.all_work_complete,
                (smi => !smi.CheckHasAnalyzeTarget()))
                .Update((smi, dt) =>
                {                                                                                      //TUNING.ROCKETRY.CLUSTER_FOW.DEFAULT_CYCLES_PER_REVEAL         
                    float detectionIncrease = dt * ((float)((double)TUNING.ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL /
                    //0.001f
                    (double)Config.Instance.ScannerModuleScanSpeed 
                    / 600.0)) ;

                    //Debug.Log(smi.currentPercentage+ " <-> "+ detectionIncrease);
                    //if (smi.currentPercentage == 0.01f)
                    //{
                    currentTarget = smi.m_analyzeTarget;
                    if (!ClusterGrid.Instance.GetEntityOfLayerAtCell(currentTarget, EntityLayer.Telescope))
                    {
                        this.telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"TelescopeTarget"), Grid.SceneLayer.Background);
                        this.telescopeTargetMarker.SetActive(true);
                        this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(this.currentTarget);

                    }
                    smi.currentPercentage = smi.m_fowManager.GetRevealCompleteFraction(smi.m_analyzeTarget) * 100f;
                    //}
                    if (smi.currentPercentage + detectionIncrease >= 99.9f)
                    {
                        smi.m_fowManager.RevealCellIfValid(this.currentTarget);
                        smi.DestroyTelescope();
                    }
                    smi.m_fowManager.EarnRevealPointsForLocation(this.currentTarget, detectionIncrease);
                });
            this.workingStates.all_work_complete
                //.Enter((smi) => SgtLogger.l("ENTERED STATE: all_work_complete", "ExplorerSMI"))

                .Enter((smi) => smi.DestroyTelescope())
                .EventTransition(GameHashes.ClusterLocationChanged,(smi => Game.Instance),this.workingStates,(smi => smi.CheckHasAnalyzeTarget())).
                EventTransition(GameHashes.ClusterFogOfWarRevealed,
                (smi => Game.Instance),
                this.workingStates,
                (smi => smi.CheckHasAnalyzeTarget()));


        }

        public class Def : BaseDef
        {
            public int analyzeClusterRadius = Config.Instance.ScannerModuleRangeRadius;
        }
        public new class Instance :
          GameInstance
        {
            [Serialize]
            private bool m_hasAnalyzeTarget;
            [Serialize]
            public AxialI m_analyzeTarget;
            [Serialize]
            public float currentPercentage;

            public void DestroyTelescope()
            {
                if ((UnityEngine.Object)sm.telescopeTargetMarker != (UnityEngine.Object)null)
                    Util.KDestroyGameObject(sm.telescopeTargetMarker);
            }
            public ClusterFogOfWarManager.Instance m_fowManager;

            public Instance(IStateMachineTarget smi, Def def) : base(smi, def)
            {
                m_fowManager = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
            }
            public override void OnCleanUp()
            {
                DestroyTelescope();
                base.OnCleanUp();
            }
            public bool Grounded()
            {
                var clusterCraft = this.GetComponent<RocketModuleCluster>().CraftInterface.m_clustercraft;
                if(clusterCraft != null)
                {
                    return !(clusterCraft.status == Clustercraft.CraftStatus.InFlight);
                }
                return false;
            }

            public bool CheckHasAnalyzeTarget()
            {
                ClusterFogOfWarManager.Instance smi = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();

                AxialI myWorldLocation = this.GetAxialLocation();
                if (this.m_hasAnalyzeTarget && !smi.IsLocationRevealed(this.m_analyzeTarget))
                {
                    if (ClusterGrid.Instance.IsInRange(myWorldLocation, m_analyzeTarget, Config.Instance.ScannerModuleRangeRadius))
                    {
                        return true;
                    }
                    else
                    {
                        DestroyTelescope();
                    }
                }
                this.m_hasAnalyzeTarget = smi.GetUnrevealedLocationWithinRadius(myWorldLocation, this.def.analyzeClusterRadius, out this.m_analyzeTarget);
                return this.m_hasAnalyzeTarget;
            }
            public AxialI GetAxialLocation()
            {
                return this.GetComponent<RocketModuleCluster>().CraftInterface.m_clustercraft.Location;
            }
            public AxialI GetAnalyzeTarget()
            {
                Debug.Assert(this.m_hasAnalyzeTarget, (object)"GetAnalyzeTarget called but this telescope has no target assigned.");
                return this.m_analyzeTarget;
            }
        }

    }
}
