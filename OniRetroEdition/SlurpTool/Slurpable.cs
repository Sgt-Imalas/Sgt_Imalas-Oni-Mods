using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.SlurpTool
{
    internal class Slurpable : Workable, ISim1000ms, ISim200ms
    {
        [MyCmpReq]
        private KSelectable Selectable;
        [MyCmpAdd]
        private Prioritizable prioritizable;
        public float amountMoppedPerTick = 1000f;
        private HandleVector<int>.Handle partitionerEntry;
        private SchedulerHandle destroyHandle;
        private float amountMopped;
        private MeshRenderer childRenderer;
        private CellOffset[] offsets = new CellOffset[]
        {
            new CellOffset(0, 0),
            new CellOffset(1, 0),
            new CellOffset(-1, 0)
        };
        private static readonly EventSystem.IntraObjectHandler<Slurpable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Slurpable>((System.Action<Slurpable, object>)((component, data) => component.OnRefreshUserMenu(data)));
        private static readonly EventSystem.IntraObjectHandler<Slurpable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Slurpable>((System.Action<Slurpable, object>)((component, data) => component.OnReachableChanged(data)));

        private Slurpable() => this.showProgressBar = false;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.workerStatusItem = Db.Get().DuplicantStatusItems.Mopping;
            this.requiredSkillPerk = Db.Get().SkillPerks.CanDoPlumbing.Id;
            this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
            this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
            this.skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
            this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
            this.childRenderer = this.GetComponentInChildren<MeshRenderer>();
            Prioritizable.AddRef(this.gameObject);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            if (!this.IsThereLiquid())
            {
                this.gameObject.DeleteObject();
            }
            else
            {
                Grid.Objects[Grid.PosToCell(this.gameObject), 8] = this.gameObject;
                WorkChore<Slurpable> workChore = new WorkChore<Slurpable>(Db.Get().ChoreTypes.Mop, (IStateMachineTarget)this);
                this.SetWorkTime(float.PositiveInfinity);
                this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForMop);
                this.Subscribe<Slurpable>(493375141, Slurpable.OnRefreshUserMenuDelegate);
                this.overrideAnims = null;
                faceTargetWhenWorking = true;
                multitoolContext = "fetchliquid";
                multitoolHitEffectTag = WaterSuckEffect.ID;
                this.partitionerEntry = GameScenePartitioner.Instance.Add("Slurpable.OnSpawn", (object)this.gameObject, new Extents(Grid.PosToCell((KMonoBehaviour)this), new CellOffset[1]
                {
                    new CellOffset(0, 0)
                }), GameScenePartitioner.Instance.liquidChangedLayer, new System.Action<object>(this.OnLiquidChanged));
                this.Refresh();
                this.Subscribe<Slurpable>(-1432940121, Slurpable.OnReachableChangedDelegate);
                new ReachabilityMonitor.Instance((Workable)this).StartSM();
                SimAndRenderScheduler.instance.Remove((object)this);
                SetOffsetTable(OffsetGroups.InvertedStandardTable);
            }
        }

        private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("icon_cancel", global::STRINGS.UI.USERMENUACTIONS.CANCELMOP.NAME, new System.Action(this.OnCancel), tooltipText: ((string)global::STRINGS.UI.USERMENUACTIONS.CANCELMOP.TOOLTIP)));

        private void OnCancel()
        {
            DetailsScreen.Instance.Show(false);
            this.gameObject.Trigger(2127324410);
        }

        public override void OnStartWork(Worker worker)
        {
            SimAndRenderScheduler.instance.Add((object)this);
            this.Refresh();
            this.MopTick(this.amountMoppedPerTick);
        }

        public override void OnStopWork(Worker worker)
        {
            SimAndRenderScheduler.instance.Remove((object)this);
            worker.GetComponent<Storage>().DropAll();
        }

        public override void OnCompleteWork(Worker worker)
        {
            SimAndRenderScheduler.instance.Remove((object)this);
            worker.GetComponent<Storage>().DropAll();
        }

        public override bool InstantlyFinish(Worker worker)
        {
            this.MopTick(1000f);
            return true;
        }

        public void Sim1000ms(float dt)
        {
            if ((double)this.amountMopped <= 0.0)
                return;
            PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, GameUtil.GetFormattedMass(-this.amountMopped), this.transform);
            this.amountMopped = 0.0f;
        }

        public void Sim200ms(float dt)
        {
            if (!((UnityEngine.Object)this.worker != (UnityEngine.Object)null))
                return;
            this.Refresh();
            this.MopTick(this.amountMoppedPerTick);
        }

        private void OnCellMopped(Sim.MassConsumedCallback mass_cb_info, object data)
        {
            if (this == null || mass_cb_info.mass <= 0.0f)
                return;
            this.amountMopped += mass_cb_info.mass;
            int cell = Grid.PosToCell(this);
            SubstanceChunk chunk = LiquidSourceManager.Instance.CreateChunk(ElementLoader.elements[(int)mass_cb_info.elemIdx], mass_cb_info.mass, mass_cb_info.temperature, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount, Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore));

            if(worker!=null && worker.TryGetComponent<Storage>(out var storage))
            {
                storage.Store(chunk.gameObject);
            }
            else
            {
                SgtLogger.warning("worker was null on slurpable");
            }
            //chunk.transform.SetPosition(chunk.transform.GetPosition() + new Vector3((float)(((double)UnityEngine.Random.value - 0.5) * 0.5), 0.0f, 0.0f));
        }

        public static void MopCell(int cell, float amount, System.Action<Sim.MassConsumedCallback, object> cb)
        {
            if (!Grid.Element[cell].IsLiquid)
                return;
            int callbackIdx = -1;
            if (cb != null)
                callbackIdx = Game.Instance.massConsumedCallbackManager.Add(cb, (object)null, nameof(Slurpable)).index;
            SimMessages.ConsumeMass(cell, Grid.Element[cell].id, amount, (byte)1, callbackIdx);
        }

        private void MopTick(float mopAmount)
        {
            int cell1 = Grid.PosToCell((KMonoBehaviour)this);
            for (int index = 0; index < this.offsets.Length; ++index)
            {
                int cell2 = Grid.OffsetCell(cell1, this.offsets[index]);
                if (Grid.Element[cell2].IsLiquid)
                    Slurpable.MopCell(cell2, mopAmount, new System.Action<Sim.MassConsumedCallback, object>(this.OnCellMopped));
            }
        }

        private bool IsThereLiquid()
        {
            int cell = Grid.PosToCell((KMonoBehaviour)this);
            bool flag = false;
            for (int index = 0; index < this.offsets.Length; ++index)
            {
                int i = Grid.OffsetCell(cell, this.offsets[index]);
                if (Grid.Element[i].IsLiquid)
                    flag = true;
            }
            return flag;
        }

        private void Refresh()
        {
            if (!this.IsThereLiquid())
            {
                if (this.destroyHandle.IsValid)
                    return;
                this.destroyHandle = GameScheduler.Instance.Schedule("DestroySlurpable", 1f, (System.Action<object>)(Slurpable => this.TryDestroy()), (object)this, (SchedulerGroup)null);
            }
            else
            {
                if (!this.destroyHandle.IsValid)
                    return;
                this.destroyHandle.ClearScheduler();
            }
        }

        private void OnLiquidChanged(object data) => this.Refresh();

        private void TryDestroy()
        {
            if (!((UnityEngine.Object)this != (UnityEngine.Object)null))
                return;
            this.gameObject.DeleteObject();
        }

        public override void OnCleanUp()
        {
            base.OnCleanUp();
            GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
            this.Unsubscribe<Slurpable>(493375141, Slurpable.OnRefreshUserMenuDelegate);
        }

        private void OnReachableChanged(object data)
        {
            if (!((UnityEngine.Object)this.childRenderer != (UnityEngine.Object)null))
                return;
            Material material = this.childRenderer.material;
            bool flag = (bool)data;
            if (material.color == Game.Instance.uiColours.Dig.invalidLocation)
                return;
            KSelectable component = this.GetComponent<KSelectable>();
            if (flag)
            {
                material.color = Game.Instance.uiColours.Dig.validLocation;
                component.RemoveStatusItem(Db.Get().BuildingStatusItems.MopUnreachable);
            }
            else
            {
                component.AddStatusItem(Db.Get().BuildingStatusItems.MopUnreachable, (object)this);
                GameScheduler.Instance.Schedule("Locomotion Tutorial", 2f, (System.Action<object>)(obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Locomotion)), (object)null, (SchedulerGroup)null);
                material.color = Game.Instance.uiColours.Dig.unreachable;
            }
        }
    }
}

