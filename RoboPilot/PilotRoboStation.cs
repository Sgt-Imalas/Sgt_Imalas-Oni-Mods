
using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoboPilot
{
    class PilotRoboStation :  
  RocketControlStation,
  IGameObjectEffectDescriptor
{
  public static List<Tag> CONTROLLED_BUILDINGS = new List<Tag>();
    private const int UNNETWORKED_VALUE = 1;
    [Serialize]
    public float TimeRemaining;
    [Serialize]
    private bool m_restrictWhenGrounded;
    public static readonly HashedString PORT_ID = (HashedString)"LogicUsageRestriction";
    private static readonly EventSystem.IntraObjectHandler<RocketControlStation> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<RocketControlStation>((System.Action<RocketControlStation, object>)((component, data) => component.OnLogicValueChanged(data)));
    private static readonly EventSystem.IntraObjectHandler<RocketControlStation> OnRocketRestrictionChanged = new EventSystem.IntraObjectHandler<RocketControlStation>((System.Action<RocketControlStation, object>)((component, data) => component.UpdateRestrictionAnimSymbol(data)));

    public bool RestrictWhenGrounded
    {
        get => this.m_restrictWhenGrounded;
        set
        {
            this.m_restrictWhenGrounded = value;
            this.Trigger(1861523068, (object)null);
        }
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
        this.smi.StartSM();
        Components.RocketControlStations.Add(this);
        this.Subscribe<RocketControlStation>(-801688580, RocketControlStation.OnLogicValueChangedDelegate);
        this.Subscribe<RocketControlStation>(1861523068, RocketControlStation.OnRocketRestrictionChanged);
        this.UpdateRestrictionAnimSymbol();
    }

    protected override void OnCleanUp()
    {
        base.OnCleanUp();
        Components.RocketControlStations.Remove(this);
    }

    public bool BuildingRestrictionsActive
    {
        get
        {
            if (this.IsLogicInputConnected())
                return this.m_logicUsageRestrictionState;
            GameObject gameObject = this.smi.sm.clusterCraft.Get(this.smi);
            return this.RestrictWhenGrounded && (UnityEngine.Object)gameObject != (UnityEngine.Object)null && gameObject.gameObject.HasTag(GameTags.RocketOnGround);
        }
    }

    public bool IsLogicInputConnected() => this.GetNetwork() != null;

    public void OnLogicValueChanged(object data)
    {
        if (!(((LogicValueChanged)data).portID == RocketControlStation.PORT_ID))
            return;
        LogicCircuitNetwork network = this.GetNetwork();
        this.m_logicUsageRestrictionState = LogicCircuitNetwork.IsBitActive(0, network != null ? network.OutputValue : 1);
        this.Trigger(1861523068, (object)null);
    }

    public void OnTagsChanged(object obj)
    {
        if (!(((TagChangedEventData)obj).tag == GameTags.RocketOnGround))
            return;
        this.Trigger(1861523068, (object)null);
    }

    private LogicCircuitNetwork GetNetwork() => Game.Instance.logicCircuitManager.GetNetworkForCell(this.GetComponent<LogicPorts>().GetPortCell(RocketControlStation.PORT_ID));

    private void UpdateRestrictionAnimSymbol(object o = null) => this.GetComponent<KAnimControllerBase>().SetSymbolVisiblity((KAnimHashedString)"restriction_sign", this.BuildingRestrictionsActive);

    public List<Descriptor> GetDescriptors(GameObject go)
    {
        List<Descriptor> descriptors = new List<Descriptor>();
        descriptors.Add(new Descriptor((string)UI.BUILDINGEFFECTS.ROCKETRESTRICTION_HEADER, (string)UI.BUILDINGEFFECTS.TOOLTIPS.ROCKETRESTRICTION_HEADER));
        string newValue = string.Join(", ", RocketControlStation.CONTROLLED_BUILDINGS.Select<Tag, string>((Func<Tag, string>)(t => Strings.Get("STRINGS.BUILDINGS.PREFABS." + t.Name.ToUpper() + ".NAME").String)).ToArray<string>());
        descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.ROCKETRESTRICTION_BUILDINGS.text.Replace("{buildinglist}", newValue), UI.BUILDINGEFFECTS.TOOLTIPS.ROCKETRESTRICTION_BUILDINGS.text.Replace("{buildinglist}", newValue)));
        return descriptors;
    }

        List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
        {
            throw new NotImplementedException();
        }

        public class States :
      GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation>
    {
        public StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.TargetParameter clusterCraft;
        private GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State unoperational;
        private GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State operational;
        private GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State running;
        private RocketControlStation.States.ReadyStates ready;
        private RocketControlStation.States.LaunchStates launch;
        public StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Signal pilotSuccessful;
        public StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.FloatParameter timeRemaining;

        public override void InitializeStates(out StateMachine.BaseState default_state)
        {
            this.serializable = StateMachine.SerializeType.ParamsOnly;
            default_state = (StateMachine.BaseState)this.unoperational;
            this.root.Enter("SetTarget", (StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State.Callback)(smi =>
            {
                this.clusterCraft.Set(this.GetRocket(smi), smi);
                this.clusterCraft.Get(smi).Subscribe(-1582839653, new System.Action<object>(smi.master.OnTagsChanged));
            })).Target(this.masterTarget).Exit((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State.Callback)(smi => this.SetRocketSpeedModifiers(smi, 0.5f)));
            this.unoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational);
            double num1;
            this.operational.Enter((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State.Callback)(smi => this.SetRocketSpeedModifiers(smi, 1f, smi.pilotSpeedMult))).PlayAnim("on").TagTransition(GameTags.Operational, this.unoperational, true).Transition((GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State)this.ready, new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.IsInFlight), UpdateRate.SIM_4000ms).Target(this.clusterCraft).EventTransition(GameHashes.RocketRequestLaunch, (GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State)this.launch, new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.RocketReadyForLaunch)).EventTransition(GameHashes.LaunchConditionChanged, (GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State)this.launch, new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.RocketReadyForLaunch)).Target(this.masterTarget).Exit((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State.Callback)(smi => num1 = (double)this.timeRemaining.Set(120f, smi)));
            this.launch.Enter((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State.Callback)(smi => this.SetRocketSpeedModifiers(smi, 1f, smi.pilotSpeedMult))).ToggleChore(new Func<RocketControlStation.StatesInstance, Chore>(this.CreateLaunchChore), this.operational).Transition(this.launch.fadein, new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.IsInFlight)).Target(this.clusterCraft).EventTransition(GameHashes.RocketRequestLaunch, this.operational, GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Not(new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.RocketReadyForLaunch))).EventTransition(GameHashes.LaunchConditionChanged, (GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State)this.launch, GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Not(new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.RocketReadyForLaunch))).Target(this.masterTarget);
            this.launch.fadein.Enter((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State.Callback)(smi =>
            {
                if (CameraController.Instance.cameraActiveCluster != this.clusterCraft.Get(smi).GetComponent<WorldContainer>().id)
                    return;
                CameraController.Instance.FadeIn();
            }));
            double num2;
            this.running.PlayAnim("on").TagTransition(GameTags.Operational, this.unoperational, true).Transition(this.operational, GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Not(new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.IsInFlight))).ParamTransition<float>((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Parameter<float>)this.timeRemaining, (GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State)this.ready, (StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Parameter<float>.Callback)((smi, p) => (double)p <= 0.0)).Enter((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State.Callback)(smi => this.SetRocketSpeedModifiers(smi, 1f, smi.pilotSpeedMult))).Update("Decrement time", new System.Action<RocketControlStation.StatesInstance, float>(this.DecrementTime)).Exit((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State.Callback)(smi => num2 = (double)this.timeRemaining.Set(30f, smi)));
            this.ready.TagTransition(GameTags.Operational, this.unoperational, true).DefaultState(this.ready.idle).ToggleChore(new Func<RocketControlStation.StatesInstance, Chore>(this.CreateChore), this.ready.post, (GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State)this.ready).Transition(this.operational, GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Not(new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.IsInFlight))).OnSignal(this.pilotSuccessful, this.ready.post).Update("Decrement time", new System.Action<RocketControlStation.StatesInstance, float>(this.DecrementTime));
            this.ready.idle.PlayAnim("on", KAnim.PlayMode.Loop).WorkableStartTransition((Func<RocketControlStation.StatesInstance, Workable>)(smi => (Workable)smi.master.GetComponent<RocketControlStationIdleWorkable>()), this.ready.working).ParamTransition<float>((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Parameter<float>)this.timeRemaining, this.ready.warning, (StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Parameter<float>.Callback)((smi, p) => (double)p <= 15.0));
            this.ready.warning.PlayAnim("on_alert", KAnim.PlayMode.Loop).WorkableStartTransition((Func<RocketControlStation.StatesInstance, Workable>)(smi => (Workable)smi.master.GetComponent<RocketControlStationIdleWorkable>()), this.ready.working).ToggleMainStatusItem(Db.Get().BuildingStatusItems.PilotNeeded).ParamTransition<float>((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Parameter<float>)this.timeRemaining, this.ready.autopilot, (StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Parameter<float>.Callback)((smi, p) => (double)p <= 0.0));
            this.ready.autopilot.PlayAnim("on_failed", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().BuildingStatusItems.AutoPilotActive).WorkableStartTransition((Func<RocketControlStation.StatesInstance, Workable>)(smi => (Workable)smi.master.GetComponent<RocketControlStationIdleWorkable>()), this.ready.working).Enter((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State.Callback)(smi => this.SetRocketSpeedModifiers(smi, 0.5f, smi.pilotSpeedMult)));
            this.ready.working.PlayAnim("working_pre").QueueAnim("working_loop", true).Enter((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State.Callback)(smi => this.SetRocketSpeedModifiers(smi, 1f, smi.pilotSpeedMult))).WorkableStopTransition((Func<RocketControlStation.StatesInstance, Workable>)(smi => (Workable)smi.master.GetComponent<RocketControlStationIdleWorkable>()), this.ready.idle);
            double num3;
            this.ready.post.PlayAnim("working_pst").OnAnimQueueComplete(this.running).Exit((StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State.Callback)(smi => num3 = (double)this.timeRemaining.Set(120f, smi)));
        }

        private void DecrementTime(RocketControlStation.StatesInstance smi, float dt)
        {
            double num = (double)this.timeRemaining.Delta(-dt, smi);
        }

        private bool RocketReadyForLaunch(RocketControlStation.StatesInstance smi)
        {
            Clustercraft component = this.clusterCraft.Get(smi).GetComponent<Clustercraft>();
            return component.LaunchRequested && component.CheckReadyToLaunch();
        }

        private GameObject GetRocket(RocketControlStation.StatesInstance smi) => ClusterManager.Instance.GetWorld(smi.GetMyWorldId()).gameObject.GetComponent<Clustercraft>().gameObject;

        private void SetRocketSpeedModifiers(
          RocketControlStation.StatesInstance smi,
          float autoPilotSpeedMultiplier,
          float pilotSkillMultiplier = 1f)
        {
            this.clusterCraft.Get(smi).GetComponent<Clustercraft>().AutoPilotMultiplier = autoPilotSpeedMultiplier;
            this.clusterCraft.Get(smi).GetComponent<Clustercraft>().PilotSkillMultiplier = pilotSkillMultiplier;
        }

        private Chore CreateChore(RocketControlStation.StatesInstance smi)
        {
            Workable component = (Workable)smi.master.GetComponent<RocketControlStationIdleWorkable>();
            WorkChore<RocketControlStationIdleWorkable> chore = new WorkChore<RocketControlStationIdleWorkable>(Db.Get().ChoreTypes.RocketControl, (IStateMachineTarget)component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Work, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
            chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object)Db.Get().SkillPerks.CanUseRocketControlStation);
            chore.AddPrecondition(ChorePreconditions.instance.IsRocketTravelling);
            return (Chore)chore;
        }

        private Chore CreateLaunchChore(RocketControlStation.StatesInstance smi)
        {
            Workable component = (Workable)smi.master.GetComponent<RocketControlStationLaunchWorkable>();
            WorkChore<RocketControlStationLaunchWorkable> launchChore = new WorkChore<RocketControlStationLaunchWorkable>(Db.Get().ChoreTypes.RocketControl, (IStateMachineTarget)component, ignore_schedule_block: true, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.topPriority);
            launchChore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object)Db.Get().SkillPerks.CanUseRocketControlStation);
            return (Chore)launchChore;
        }

        public void LaunchRocket(RocketControlStation.StatesInstance smi) => this.clusterCraft.Get(smi).GetComponent<Clustercraft>().Launch();

        public bool IsInFlight(RocketControlStation.StatesInstance smi) => this.clusterCraft.Get(smi).GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.InFlight;

        public bool IsLaunching(RocketControlStation.StatesInstance smi) => this.clusterCraft.Get(smi).GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.Launching;

        public class ReadyStates :
          GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State
        {
            public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State idle;
            public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State working;
            public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State post;
            public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State warning;
            public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State autopilot;
        }

        public class LaunchStates :
          GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State
        {
            public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State launch;
            public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State fadein;
        }
    }

    public class StatesInstance :
      GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.GameInstance
    {
        public float pilotSpeedMult = 1f;

        public StatesInstance(RocketControlStation smi)
          : base(smi)
        {
        }

        public void LaunchRocket() => this.sm.LaunchRocket(this);

        public void SetPilotSpeedMult(Worker pilot)
        {
            this.pilotSpeedMult = 1f;
        }
    }
}
