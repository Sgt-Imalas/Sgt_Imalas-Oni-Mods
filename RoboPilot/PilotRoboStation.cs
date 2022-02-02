
using KSerialization;
using STRINGS;
using UnityEngine;

namespace RoboPilot
{
    class PilotRoboStation : KMonoBehaviour
    
    //{
    //    [Serialize]
    //    public Ref<KSelectable> Robo;
    //    [Serialize]
    //    public string storedName = "robo duckie";
    //    private Operational.Flag dockedRobot = new Operational.Flag(nameof(dockedRobot), Operational.Flag.Type.Functional);
    //    [SerializeField]
    //    private Storage botMaterialStorage;

    //    private SchedulerHandle newRoboHandle;
    //    private static readonly EventSystem.IntraObjectHandler<PilotRoboStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<PilotRoboStation>((System.Action<PilotRoboStation, object>)((component, data) => component.OnOperationalChanged(data)));
    //    private int refreshRoboHandle = -1;
    //    private int BotNameChangeHandle = -1;


    //    protected override void OnPrefabInit()
    //    {
    //        this.Initialize();
    //        this.Subscribe<PilotRoboStation>(-592767678, PilotRoboStation.OnOperationalChangedDelegate);
    //    }
    //    public void SetStorages(Storage botMaterialStorage)
    //    {
    //        this.botMaterialStorage = botMaterialStorage;
    //    }

    //    protected void Initialize()
    //    {
    //        base.OnPrefabInit();
    //        this.GetComponent<Operational>().SetFlag(this.dockedRobot, false);
    //    }

    //    protected override void OnSpawn()
    //    {
    //        if (this.Robo == null || (UnityEngine.Object)this.Robo.Get() == (UnityEngine.Object)null)
    //        {

    //            this.RequestNewRobo();
    //        }
    //        else
    //        {
    //            this.RefreshRoboBotSubscription();
    //        }
    //        this.UpdateNameDisplay();
    //    }

    //    private void RequestNewRobo(object data = null)
    //    {
    //        if ((UnityEngine.Object)this.botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, PilotRoboConfig.MASS) == (UnityEngine.Object)null)
    //        {
    //            FetchList2 fetchList2 = new FetchList2(this.botMaterialStorage, Db.Get().ChoreTypes.Fetch);
    //            fetchList2.Add(GameTags.RefinedMetal, amount: PilotRoboConfig.MASS);
    //            fetchList2.Submit((System.Action)null, true);
    //        }
    //        else
    //            this.MakeNewRobo();
    //    }

    //    private void MakeNewRobo(object data = null)
    //    {
    //        if (this.newRoboHandle.IsValid || (double)this.botMaterialStorage.GetAmountAvailable(GameTags.RefinedMetal) < (double)PilotRoboConfig.MASS)
    //            return;
    //        PrimaryElement firstWithMass = this.botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, PilotRoboConfig.MASS);
    //        if ((UnityEngine.Object)firstWithMass == (UnityEngine.Object)null)
    //            return;
    //        SimHashes pilotBotMaterial = firstWithMass.ElementID;
    //        firstWithMass.Mass -= PilotRoboConfig.MASS;
    //        this.newRoboHandle = GameScheduler.Instance.Schedule("MakePilotRobo", 2f, (System.Action<object>)(obj =>
    //        {
    //            GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"PilotRobo"), Grid.CellToPos(Grid.CellRight(Grid.PosToCell(this.gameObject))), Grid.SceneLayer.Creatures);
    //            go.SetActive(true);
    //            this.Robo = new Ref<KSelectable>(go.GetComponent<KSelectable>());
    //            if (!string.IsNullOrEmpty(this.storedName))
    //                this.Robo.Get().GetComponent<UserNameable>().SetName(this.storedName);
    //            this.UpdateNameDisplay();
    //            this.Robo.Get().GetComponent<PrimaryElement>().ElementID = pilotBotMaterial;
    //            this.RefreshRoboBotSubscription();
    //            this.newRoboHandle.ClearScheduler();
    //            UpdateStoredName("Printed");
    //        }), (object)null, (SchedulerGroup)null);
    //        this.GetComponent<KBatchedAnimController>().Play((HashedString)"newsweepy");
    //    }
    //    private void RefreshRoboBotSubscription()
    //    {
    //        if (this.refreshRoboHandle != -1)
    //        {
    //            this.Robo.Get().Unsubscribe(this.refreshRoboHandle);
    //            this.Robo.Get().Unsubscribe(this.BotNameChangeHandle);
    //        }
    //        this.refreshRoboHandle = this.Robo.Get().Subscribe(1969584890, new System.Action<object>(this.RequestNewRobo));
    //        this.BotNameChangeHandle = this.Robo.Get().Subscribe(1102426921, new System.Action<object>(this.UpdateStoredName));
    //    }
    //    private void UpdateStoredName(object data)
    //    {
    //        this.storedName = (string)data;
    //        this.UpdateNameDisplay();
    //    }

    //    private void UpdateNameDisplay()
    //    {
    //        if (string.IsNullOrEmpty(this.storedName))
    //            this.GetComponent<KSelectable>().SetName(string.Format((string)BUILDINGS.PREFABS.PilotRoboStation.NAMEDSTATION, (object)ROBOTS.MODELS.SWEEPBOT.NAME));
    //        else
    //            this.GetComponent<KSelectable>().SetName(string.Format((string)BUILDINGS.PREFABS.SWEEPBOTSTATION.NAMEDSTATION, (object)this.storedName));
    //        NameDisplayScreen.Instance.UpdateName(this.gameObject);
    //    }

    //    public void DockRobot(bool docked) => this.GetComponent<Operational>().SetFlag(this.dockedRobot, docked);

    //    public void StartCharging()
    //    {
    //        this.GetComponent<KBatchedAnimController>().Queue((HashedString)"sleep_pre");
    //        this.GetComponent<KBatchedAnimController>().Queue((HashedString)"sleep_idle", KAnim.PlayMode.Loop);
    //    }

    //    public void StopCharging()
    //    {
    //        this.GetComponent<KBatchedAnimController>().Play((HashedString)"sleep_pst");
    //        this.UpdateNameDisplay();
    //    }

    //    protected override void OnCleanUp()
    //    {
    //        if (this.newRoboHandle.IsValid)
    //            this.newRoboHandle.ClearScheduler();
    //        if (this.refreshRoboHandle == -1 || !((UnityEngine.Object)this.Robo.Get() != (UnityEngine.Object)null))
    //            return;
    //        this.Robo.Get().Unsubscribe(this.refreshRoboHandle);
    //    }

    //    private void OnOperationalChanged(object data)
    //    {
    //        Operational component = this.GetComponent<Operational>();
    //        if (component.Flags.ContainsValue(false))
    //            component.SetActive(false);
    //        else
    //            component.SetActive(true);
    //        if (this.Robo != null && !((UnityEngine.Object)this.Robo.Get() == (UnityEngine.Object)null))
    //            return;
    //        this.RequestNewRobo();
    //    }
    //}

    {
        [Serialize]
        public Ref<KSelectable> sweepBot;
        [Serialize]
        public string storedName;
        private Operational.Flag dockedRobot = new Operational.Flag(nameof(dockedRobot), Operational.Flag.Type.Functional);
        private MeterController meter;
        [SerializeField]
        private Storage botMaterialStorage;
        [SerializeField]
        private Storage sweepStorage;
        private SchedulerHandle newSweepyHandle;
        private static readonly EventSystem.IntraObjectHandler<PilotRoboStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<PilotRoboStation>((System.Action<PilotRoboStation, object>)((component, data) => component.OnOperationalChanged(data)));
        private int refreshSweepbotHandle = -1;
        private int sweepBotNameChangeHandle = -1;

        public void SetStorages(Storage botMaterialStorage, Storage sweepStorage)
        {
            this.botMaterialStorage = botMaterialStorage;
            this.sweepStorage = sweepStorage;
        }

        protected override void OnPrefabInit()
        {
            this.Initialize(false);
            this.Subscribe<PilotRoboStation>(-592767678, PilotRoboStation.OnOperationalChangedDelegate);
        }

        protected void Initialize(bool use_logic_meter)
        {
            base.OnPrefabInit();
            this.GetComponent<Operational>().SetFlag(this.dockedRobot, false);
        }

        protected override void OnSpawn()
        {
            this.Subscribe(-1697596308, new System.Action<object>(this.OnStorageChanged));
            this.meter = new MeterController((KAnimControllerBase)this.gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[2]
            {
      "meter_frame",
      "meter_level"
            });
            if (this.sweepBot == null || (UnityEngine.Object)this.sweepBot.Get() == (UnityEngine.Object)null)
            {
                this.RequestNewSweepBot();
            }
            else
            {
                StorageUnloadMonitor.Instance smi = this.sweepBot.Get().GetSMI<StorageUnloadMonitor.Instance>();
                smi.sm.sweepLocker.Set(this.sweepStorage, smi);
                this.RefreshSweepBotSubscription();
            }
            this.UpdateMeter();
            this.UpdateNameDisplay();
        }

        private void RequestNewSweepBot(object data = null)
        {
            if ((UnityEngine.Object)this.botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, PilotRoboConfig.MASS) == (UnityEngine.Object)null)
            {
                FetchList2 fetchList2 = new FetchList2(this.botMaterialStorage, Db.Get().ChoreTypes.Fetch);
                fetchList2.Add(GameTags.RefinedMetal, amount: PilotRoboConfig.MASS);
                fetchList2.Submit((System.Action)null, true);
            }
            else
                this.MakeNewSweepBot();
        }

        private void MakeNewSweepBot(object data = null)
        {
            if (this.newSweepyHandle.IsValid || (double)this.botMaterialStorage.GetAmountAvailable(GameTags.RefinedMetal) < (double)PilotRoboConfig.MASS)
                return;
            PrimaryElement firstWithMass = this.botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, PilotRoboConfig.MASS);
            if ((UnityEngine.Object)firstWithMass == (UnityEngine.Object)null)
                return;
            SimHashes sweepBotMaterial = firstWithMass.ElementID;
            firstWithMass.Mass -= PilotRoboConfig.MASS;
            this.UpdateMeter();
            this.newSweepyHandle = GameScheduler.Instance.Schedule("MakePilot", 2f, (System.Action<object>)(obj =>
            {
                GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"PilotRobo"), Grid.CellToPos(Grid.CellRight(Grid.PosToCell(this.gameObject))), Grid.SceneLayer.Creatures);
                go.SetActive(true);
                this.sweepBot = new Ref<KSelectable>(go.GetComponent<KSelectable>());
                if (!string.IsNullOrEmpty(this.storedName))
                    this.sweepBot.Get().GetComponent<UserNameable>().SetName(this.storedName);
                this.UpdateNameDisplay();
                StorageUnloadMonitor.Instance smi = go.GetSMI<StorageUnloadMonitor.Instance>();
                smi.sm.sweepLocker.Set(this.sweepStorage, smi);
                this.sweepBot.Get().GetComponent<PrimaryElement>().ElementID = sweepBotMaterial;
                this.RefreshSweepBotSubscription();
                this.newSweepyHandle.ClearScheduler();
            }), (object)null, (SchedulerGroup)null);
            this.GetComponent<KBatchedAnimController>().Play((HashedString)"newsweepy");
        }

        private void RefreshSweepBotSubscription()
        {
            if (this.refreshSweepbotHandle != -1)
            {
                this.sweepBot.Get().Unsubscribe(this.refreshSweepbotHandle);
                this.sweepBot.Get().Unsubscribe(this.sweepBotNameChangeHandle);
            }
            this.refreshSweepbotHandle = this.sweepBot.Get().Subscribe(1969584890, new System.Action<object>(this.RequestNewSweepBot));
            this.sweepBotNameChangeHandle = this.sweepBot.Get().Subscribe(1102426921, new System.Action<object>(this.UpdateStoredName));
        }

        private void UpdateStoredName(object data)
        {
            this.storedName = (string)data;
            this.UpdateNameDisplay();
        }

        private void UpdateNameDisplay()
        {
            if (string.IsNullOrEmpty(this.storedName))
                this.GetComponent<KSelectable>().SetName(string.Format((string)BUILDINGS.PREFABS.SWEEPBOTSTATION.NAMEDSTATION, (object)ROBOTS.MODELS.SWEEPBOT.NAME));
            else
                this.GetComponent<KSelectable>().SetName(string.Format((string)BUILDINGS.PREFABS.SWEEPBOTSTATION.NAMEDSTATION, (object)this.storedName));
            NameDisplayScreen.Instance.UpdateName(this.gameObject);
        }

        public void DockRobot(bool docked) => this.GetComponent<Operational>().SetFlag(this.dockedRobot, docked);

        public void StartCharging()
        {
            this.GetComponent<KBatchedAnimController>().Queue((HashedString)"sleep_pre");
            this.GetComponent<KBatchedAnimController>().Queue((HashedString)"sleep_idle", KAnim.PlayMode.Loop);
        }

        public void StopCharging()
        {
            this.GetComponent<KBatchedAnimController>().Play((HashedString)"sleep_pst");
            this.UpdateNameDisplay();
        }

        protected override void OnCleanUp()
        {
            if (this.newSweepyHandle.IsValid)
                this.newSweepyHandle.ClearScheduler();
            if (this.refreshSweepbotHandle == -1 || !((UnityEngine.Object)this.sweepBot.Get() != (UnityEngine.Object)null))
                return;
            this.sweepBot.Get().Unsubscribe(this.refreshSweepbotHandle);
        }

        private void UpdateMeter()
        {
            float minusStorageMargin = this.GetMaxCapacityMinusStorageMargin();
            float percent_full = Mathf.Clamp01(this.GetAmountStored() / minusStorageMargin);
            if (this.meter == null)
                return;
            this.meter.SetPositionPercent(percent_full);
        }

        private void OnStorageChanged(object data)
        {
            this.UpdateMeter();
            if (this.sweepBot == null || (UnityEngine.Object)this.sweepBot.Get() == (UnityEngine.Object)null)
                this.RequestNewSweepBot();
            KBatchedAnimController component = this.GetComponent<KBatchedAnimController>();
            if (component.currentFrame >= component.GetCurrentNumFrames())
                this.GetComponent<KBatchedAnimController>().Play((HashedString)"remove");
            for (int idx = 0; idx < this.sweepStorage.Count; ++idx)
                this.sweepStorage[idx].GetComponent<Clearable>().MarkForClear(allowWhenStored: true);
        }

        private void OnOperationalChanged(object data)
        {
            Operational component = this.GetComponent<Operational>();
            if (component.Flags.ContainsValue(false))
                component.SetActive(false);
            else
                component.SetActive(true);
            if (this.sweepBot != null && !((UnityEngine.Object)this.sweepBot.Get() == (UnityEngine.Object)null))
                return;
            this.RequestNewSweepBot();
        }

        private float GetMaxCapacityMinusStorageMargin() => this.sweepStorage.Capacity() - this.sweepStorage.storageFullMargin;

        private float GetAmountStored() => this.sweepStorage.MassStored();
    }
}
