
using KSerialization;
using STRINGS;
using UnityEngine;

namespace RoboPilot
{
    class PilotRoboStation : KMonoBehaviour
    {
        [Serialize]
        public Ref<KSelectable> Robo;
        [Serialize]
        public string storedName;
        private Operational.Flag dockedRobot = new Operational.Flag(nameof(dockedRobot), Operational.Flag.Type.Functional);
        private MeterController meter;
        [SerializeField]
        private Storage botMaterialStorage;

        private SchedulerHandle newRoboHandle;
        private static readonly EventSystem.IntraObjectHandler<PilotRoboStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<PilotRoboStation>((System.Action<PilotRoboStation, object>)((component, data) => component.OnOperationalChanged(data)));
        private int refreshRoboHandle = -1;
        private int BotNameChangeHandle = -1;


        protected override void OnPrefabInit()
        {
            this.Initialize(false);
            this.Subscribe<PilotRoboStation>(-592767678, PilotRoboStation.OnOperationalChangedDelegate);
        }
        public void SetStorages(Storage botMaterialStorage, Storage sweepStorage)
        {
            this.botMaterialStorage = botMaterialStorage;
        }

        protected void Initialize(bool use_logic_meter)
        {
            base.OnPrefabInit();
            this.GetComponent<Operational>().SetFlag(this.dockedRobot, false);
        }

        protected override void OnSpawn()
        {
            if (this.Robo == null || (UnityEngine.Object)this.Robo.Get() == (UnityEngine.Object)null)
            {
                this.RequestNewRobo();
            }
            this.UpdateNameDisplay();
        }

        private void RequestNewRobo(object data = null)
        {
            if ((UnityEngine.Object)this.botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, SweepBotConfig.MASS) == (UnityEngine.Object)null)
            {
                FetchList2 fetchList2 = new FetchList2(this.botMaterialStorage, Db.Get().ChoreTypes.Fetch);
                fetchList2.Add(GameTags.RefinedMetal, amount: SweepBotConfig.MASS);
                fetchList2.Submit((System.Action)null, true);
            }
            else
                this.MakeNewRobo();
        }

        private void MakeNewRobo(object data = null)
        {
            if (this.newRoboHandle.IsValid || (double)this.botMaterialStorage.GetAmountAvailable(GameTags.RefinedMetal) < (double)SweepBotConfig.MASS)
                return;
            PrimaryElement firstWithMass = this.botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, SweepBotConfig.MASS);
            if ((UnityEngine.Object)firstWithMass == (UnityEngine.Object)null)
                return;
            SimHashes sweepBotMaterial = firstWithMass.ElementID;
            firstWithMass.Mass -= SweepBotConfig.MASS;
            this.newRoboHandle = GameScheduler.Instance.Schedule("MakePilotRobo", 2f, (System.Action<object>)(obj =>
            {
                GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"RoboPilot"), Grid.CellToPos(Grid.CellRight(Grid.PosToCell(this.gameObject))), Grid.SceneLayer.Creatures);
                go.SetActive(true);
                this.Robo = new Ref<KSelectable>(go.GetComponent<KSelectable>());
                if (!string.IsNullOrEmpty(this.storedName))
                    this.Robo.Get().GetComponent<UserNameable>().SetName(this.storedName);
                this.UpdateNameDisplay();
                this.Robo.Get().GetComponent<PrimaryElement>().ElementID = sweepBotMaterial;
                this.RefreshRoboBotSubscription();
                this.newRoboHandle.ClearScheduler();
            }), (object)null, (SchedulerGroup)null);
            this.GetComponent<KBatchedAnimController>().Play((HashedString)"newsweepy");
        }
        private void RefreshRoboBotSubscription()
        {
            if (this.refreshRoboHandle != -1)
            {
                this.Robo.Get().Unsubscribe(this.refreshRoboHandle);
                this.Robo.Get().Unsubscribe(this.BotNameChangeHandle);
            }
            this.refreshRoboHandle = this.Robo.Get().Subscribe(1969584890, new System.Action<object>(this.RequestNewRobo));
            this.BotNameChangeHandle = this.Robo.Get().Subscribe(1102426921, new System.Action<object>(this.UpdateStoredName));
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
            if (this.newRoboHandle.IsValid)
                this.newRoboHandle.ClearScheduler();
            if (this.refreshRoboHandle == -1 || !((UnityEngine.Object)this.Robo.Get() != (UnityEngine.Object)null))
                return;
            this.Robo.Get().Unsubscribe(this.refreshRoboHandle);
        }

        private void OnOperationalChanged(object data)
        {
            Operational component = this.GetComponent<Operational>();
            if (component.Flags.ContainsValue(false))
                component.SetActive(false);
            else
                component.SetActive(true);
            if (this.Robo != null && !((UnityEngine.Object)this.Robo.Get() == (UnityEngine.Object)null))
                return;
            this.RequestNewRobo();
        }
    }
}
