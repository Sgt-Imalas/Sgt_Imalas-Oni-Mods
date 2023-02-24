using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.LandingLegs
{
    internal class LandingLegModule :
    GameStateMachine<LandingLegModule, LandingLegModule.StatesInstance, IStateMachineTarget, LandingLegModule.Def>
    {
        public BoolParameter landedDeployed;
        public GroundedStates grounded;
        public NotGroundedStates not_grounded;
        //public IntParameter numVisualCapsules;

        public override void InitializeStates(out BaseState default_state)
        {
            default_state = grounded;

            //this.grounded
            //    .DefaultState(this.grounded.landedNormal)
            //    .TagTransition(GameTags.RocketNotOnGround, not_grounded);
            //this
            //    .grounded.landedNormal
            //    .PlayAnim(smi => smi.GetLoadingAnimName())
            //    .ParamTransition(hasCargo, this.grounded.empty, IsFalse)
            //    .OnAnimQueueComplete(this.grounded.loaded);
            //this.grounded.loaded
            //    .ParamTransition(hasCargo, this.grounded.empty, IsFalse)
            //    .EventTransition(GameHashes.OnStorageChange, this.grounded.loading, smi => smi.NeedsVisualUpdate());
            //this.grounded.empty
            //    .Enter(smi => this.numVisualCapsules.Set(0, smi))
            //    .PlayAnim("deployed").ParamTransition(hasCargo, this.grounded.loaded, IsTrue);
            //this.not_grounded
            //    .DefaultState(this.not_grounded.loaded)
            //    .TagTransition(GameTags.RocketNotOnGround, grounded, true);
            //this.not_grounded.loaded
            //    .PlayAnim("loaded")
            //    .ParamTransition(hasCargo, this.not_grounded.empty, IsFalse)
            //    .OnSignal(this.emptyCargo, this.not_grounded.emptying);
            //this.not_grounded.emptying
            //    .PlayAnim("deploying")
            //    .GoTo(this.not_grounded.empty);
            //this.not_grounded.empty
            //    .PlayAnim("deployed")
            //    .ParamTransition(hasCargo, this.not_grounded.loaded, IsTrue);
        }

        public class Def : BaseDef
        {
            //public float numCapsules;
        }

        public class NotGroundedStates :
          State
        {
            public State readyToLand;
            public State selectingLocation;
            public State empty;
        }

        public class GroundedStates :
          State
        {
            public State landedDeployed;
            public State landedNormal;
        }

        public class StatesInstance :
          GameInstance,
          IEmptyableCargo
        {
            [Serialize]
            private bool autoDeploy;

            public StatesInstance(IStateMachineTarget master, Def def)
              : base(master, def)
            {
                //this.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new LoadingCompleteCondition(this.storage));
                this.gameObject.Subscribe((int)GameHashes.OnStorageLockerSetupComplete, new Action<object>(this.SetupMeter));
            }

            private void SetupMeter(object obj)
            {
                KBatchedAnimTracker componentInChildren = this.gameObject.GetComponentInChildren<KBatchedAnimTracker>();
                componentInChildren.forceAlwaysAlive = true;
                componentInChildren.matchParentOffset = true;
            }

            public override void OnCleanUp()
            {
                this.gameObject.Unsubscribe((int)GameHashes.OnStorageLockerSetupComplete, new Action<object>(this.SetupMeter));
                base.OnCleanUp();
            }

            //public bool NeedsVisualUpdate()
            //{
            //    if (this.sm.numVisualCapsules.Get(this) >= Mathf.FloorToInt(this.storage.MassStored() / 200f))
            //        return false;
            //    this.sm.numVisualCapsules.Delta(1, this);
            //    return true;
            //}

            public string GetLoadingAnimName()
            {
                return null;
                //int num1 = this.sm.numVisualCapsules.Get(this);
                //int num2 = Mathf.RoundToInt(this.storage.capacityKg / 200f);
                //if (num1 == num2)
                //    return "loading6_full";
                //if (num1 == num2 - 1)
                //    return "loading5";
                //if (num1 == num2 - 2)
                //    return "loading4";
                //if (num1 == num2 - 3 || num1 > 2)
                //    return "loading3_repeat";
                //if (num1 == 2)
                //    return "loading2";
                //return num1 == 1 ? "loading1" : "deployed";
            }

            public void DeployCargoPods()
            {
                //Clustercraft component1 = this.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
                //ClusterGridEntity orbitAsteroid = component1.GetOrbitAsteroid();
                //if (orbitAsteroid != null)
                //{
                //    WorldContainer component2 = orbitAsteroid.GetComponent<WorldContainer>();
                //    int id = component2.id;
                //    Vector3 position = new Vector3(component2.minimumBounds.x + 1f, component2.maximumBounds.y, Grid.GetLayerZ(Grid.SceneLayer.Front));
                //    while ((double)this.storage.MassStored() > 0.0)
                //    {
                //        GameObject go = Util.KInstantiate(Assets.GetPrefab((Tag)"RailGunPayload"), position);
                //        go.GetComponent<Pickupable>().deleteOffGrid = false;
                //        float num = 0.0f;
                //        while ((double)num < 200.0 && (double)this.storage.MassStored() > 0.0)
                //            num += this.storage.Transfer(go.GetComponent<Storage>(), GameTags.Stored, 200f - num, hide_popups: true);
                //        go.SetActive(true);
                //        go.GetSMI<RailGunPayload.StatesInstance>().Travel(component1.Location, component2.GetMyWorldLocation());
                //    }
                //}
                //this.CheckIfLoaded();
            }

            private void ChooseLanderLocation()
            {
                ClusterGridEntity stableOrbitAsteroid = this.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().GetStableOrbitAsteroid();
                if (!((UnityEngine.Object)stableOrbitAsteroid != (UnityEngine.Object)null))
                    return;
                WorldContainer component1 = stableOrbitAsteroid.GetComponent<WorldContainer>();
                //Placeable component2 = this.landerContainer.FindFirst(this.def.landerPrefabID).GetComponent<Placeable>();
                //component2.restrictWorldId = component1.id;
                component1.LookAtSurface();
                ClusterManager.Instance.SetActiveWorld(component1.id);
                ManagementMenu.Instance.CloseAll();
                //PlaceTool.Instance.Activate(component2, new System.Action<Placeable, int>(this.OnLanderPlaced));
            }

            private void OnLanderPlaced(Placeable lander, int cell)
            {
                this.smi.sm.landedDeployed.Set(true,smi);

                //ManagementMenu.Instance.ToggleClusterMap();
                //ClusterMapScreen.Instance.SelectEntity(this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<ClusterGridEntity>(), true);
            }

            public bool IsValidDropLocation() => this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().GetOrbitAsteroid() != null;

            public bool AutoDeploy
            {
                get => this.autoDeploy;
                set => this.autoDeploy = value;
            }

            public bool CanAutoDeploy => true;

            public void EmptyCargo() => ChooseLanderLocation();

            public bool CanEmptyCargo() => this.IsValidDropLocation();

            public bool ChooseDuplicant => false;
            public MinionIdentity ChosenDuplicant
            {
                get => null;
                set
                {
                }
            }

            public bool ModuleDeployed => false;
        }
    }

}
