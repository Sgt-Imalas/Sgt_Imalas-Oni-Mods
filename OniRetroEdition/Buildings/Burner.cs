//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace OniRetroEdition.Buildings
//{

//    public class Burner : StateMachineComponent<Burner.SMInstance>
//    {
//        private const float BURN_PERIOD = 1f;
//        private const float FUEL_PER_SECOND = 0.001f;
//        private const float TOTAL_ENERGY = 246000f;
//        private const float ENERGY_FOR_1KG_GAS = 10f;
//        private const int FLAME_LENGTH = 3;
//        public Vector3 flameSourceOffset = new Vector3(0.0f, 1f, 0.0f);
//        public Tag fuelTag;
//        public int lastFlameLength = 1;
//        [MyCmpReq]
//        private Storage storage;
//        [MyCmpReq]
//        private Operational operational;
//        [MyCmpReq]
//        private Rotatable rotatable;
//        [MyCmpReq]
//        private Switch switchCmp;
//        [MyCmpReq]
//        private ElementEmitter elementEmitter;
//        private static StatusItem statusHasFuel;
//        private static StatusItem statusBurningFuel;

//        private void CreateStatusItems()
//        {
//            if (Burner.statusBurningFuel != null)
//                return;
//            Burner.statusHasFuel = new StatusItem("HasFuel", (string)BUILDING.STATUSITEMS.BURNER.HAS_FUEL.NAME, (string)BUILDING.STATUSITEMS.BURNER.HAS_FUEL.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
//            Burner.statusHasFuel.resolveStringCallback = (Func<string, object, string>)((str, obj) =>
//            {
//                Burner.SMInstance smi = (Burner.SMInstance)obj;
//                return string.Format(str, GameUtil.GetFormattedPercent(smi.sm.fuelRemaining.Get(smi) * 100f));
//            });
//            Burner.statusBurningFuel = new StatusItem("BurningFuel", (string)BUILDING.STATUSITEMS.BURNER.BURNING_FUEL.NAME, (string)BUILDING.STATUSITEMS.BURNER.BURNING_FUEL.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
//            Burner.statusBurningFuel.resolveStringCallback = (Func<string, object, string>)((str, obj) =>
//            {
//                Burner.SMInstance smi = (Burner.SMInstance)obj;
//                return string.Format(str, GameUtil.GetFormattedPercent(smi.sm.fuelRemaining.Get(smi) * 100f));
//            });
//        }

//        public override void OnPrefabInit()
//        {
//            base.OnPrefabInit();
//            this.switchCmp.OnToggle += new Action<bool>(this.OnToggle);
//            this.CreateStatusItems();
//        }

//        public override void OnSpawn()
//        {
//            base.OnSpawn();
//            this.smi.StartSM();
//        }

//        private void OnToggle(bool on) => this.smi.sm.toggledOn.Set(on, this.smi);

//        private bool HasFuelBrick() => (double)this.storage.GetAmountAvailable(this.fuelTag) >= 1.0;

//        private void ConsumeFuelBrick()
//        {
//            if (!this.HasFuelBrick())
//                return;
//            this.storage.Consume(this.fuelTag, 1f);
//            double num = (double)this.smi.sm.fuelRemaining.Delta(1f, this.smi);
//        }

//        private bool CanBurn()
//        {
//            return this.operational.IsFunctional && this.smi.sm.toggledOn.Get(this.smi) && (double)this.smi.sm.fuelRemaining.Get(this.smi) > 0.0;
//        }

//        private void BurnFuel(float dt)
//        {
//            double num = (double)this.smi.sm.fuelRemaining.Delta((float)(-(double)dt * (1.0 / 1000.0)), this.smi);
//            CellOffset rotatedCellOffset1 = this.rotatable.GetRotatedCellOffset(new CellOffset((Vector2)this.flameSourceOffset));
//            CellOffset rotatedCellOffset2 = this.rotatable.GetRotatedCellOffset(new CellOffset(1, 0));
//            int index = Grid.OffsetCell(Grid.PosToCell(this.transform.position), rotatedCellOffset1);
//            for (int a = 0; a <= 3; ++a)
//            {
//                this.lastFlameLength = Mathf.Max(a, 1);
//                Element element = Grid.Element[index];
//                float kilojoules = !element.IsGas ? (float)(246000.0 * (double)dt * (1.0 / 1000.0)) : 10f * Grid.Mass[index];
//                SimMessages.ModifyEnergy(index, kilojoules, SimMessages.EnergySourceID.Burner);
//                if (element.IsSolid)
//                    break;
//                index = Grid.OffsetCell(index, rotatedCellOffset2);
//            }
//        }

//        public class SMInstance : GameStateMachine<Burner.States, Burner.SMInstance, Burner>.GameInstance
//        {
//            public SMInstance(Burner master)
//              : base(master)
//            {
//            }
//        }

//        public class States : GameStateMachine<Burner.States, Burner.SMInstance, Burner>
//        {
//            public StateMachine<Burner.States, Burner.SMInstance, Burner>.FloatParameter fuelRemaining;
//            public StateMachine<Burner.States, Burner.SMInstance, Burner>.BoolParameter toggledOn;
//            public StateMachine<Burner.States, Burner.SMInstance, Burner>.ObjectParameter<KBatchedAnimController> flameAnim;
//            public StateMachine<Burner.States, Burner.SMInstance, Burner>.ObjectParameter<KBatchedAnimController> flameContactAnim;
//            public GameStateMachine<Burner.States, Burner.SMInstance, Burner>.State empty;
//            public GameStateMachine<Burner.States, Burner.SMInstance, Burner>.State full;
//            public GameStateMachine<Burner.States, Burner.SMInstance, Burner>.State burning;
//            public GameStateMachine<Burner.States, Burner.SMInstance, Burner>.State expired;

//            public override void InitializeStates(out StateMachine.BaseState default_state)
//            {
//                default_state = (StateMachine.BaseState)this.empty;
//                this.serializable = true;
//                this.empty.EventTransition(GameHashes.OnStorageChange, this.full, (StateMachine<Burner.States, Burner.SMInstance, Burner>.Transition.ConditionCallback)(smi => smi.master.HasFuelBrick())).ToggleComponent<ManualDeliveryAmount>().PlayAnim("empty", KAnim.PlayMode.Loop).Exit((StateMachine<Burner.States, Burner.SMInstance, Burner>.State.Callback)(smi => smi.master.ConsumeFuelBrick()));
//                this.full.ParamTransition<bool>((StateMachine<Burner.States, Burner.SMInstance, Burner>.Parameter<bool>)this.toggledOn, this.burning, (StateMachine<Burner.States, Burner.SMInstance, Burner>.Parameter<bool>.Callback)((smi, p) => smi.master.CanBurn())).EventTransition(GameHashes.FunctionalChanged, this.burning, (StateMachine<Burner.States, Burner.SMInstance, Burner>.Transition.ConditionCallback)(smi => smi.master.CanBurn())).PlayAnim("on", KAnim.PlayMode.Loop).ToggleMainStatusItem(Burner.statusHasFuel);
//                this.burning.ParamTransition<bool>((StateMachine<Burner.States, Burner.SMInstance, Burner>.Parameter<bool>)this.toggledOn, this.full, (StateMachine<Burner.States, Burner.SMInstance, Burner>.Parameter<bool>.Callback)((smi, p) => !smi.master.CanBurn())).EventTransition(GameHashes.FunctionalChanged, this.full, (StateMachine<Burner.States, Burner.SMInstance, Burner>.Transition.ConditionCallback)(smi => !smi.master.CanBurn())).ParamTransition<float>((StateMachine<Burner.States, Burner.SMInstance, Burner>.Parameter<float>)this.fuelRemaining, this.expired, (StateMachine<Burner.States, Burner.SMInstance, Burner>.Parameter<float>.Callback)((smi, p) => (double)p <= 0.0)).PlayAnim("burning", KAnim.PlayMode.Loop).ToggleSchedulePeriodic("Burn Fuel", 1f, (Action<Burner.SMInstance>)(smi => smi.master.BurnFuel(1f))).ToggleMainStatusItem(Burner.statusBurningFuel).Enter((StateMachine<Burner.States, Burner.SMInstance, Burner>.State.Callback)(smi =>
//                {
//                    smi.master.elementEmitter.SetEmitting(true);
//                    GameObject go1 = GameUtil.KInstantiate(EffectPrefabs.Instance.BurnerFlame, smi.master.transform.position, Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX));
//                    go1.AddOrGet<LoopingSounds>();
//                    go1.AddComponent<Rotatable>().Match(smi.master.rotatable);
//                    KBatchedAnimController component1 = go1.GetComponent<KBatchedAnimController>();
//                    component1.Play("loop", KAnim.PlayMode.Loop);
//                    this.flameAnim.Set(component1, smi);
//                    GameObject go2 = GameUtil.KInstantiate(EffectPrefabs.Instance.BurnerFlameContact, smi.master.transform.position, Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX));
//                    go2.AddOrGet<LoopingSounds>();
//                    go2.AddComponent<Rotatable>().Match(smi.master.rotatable);
//                    KBatchedAnimController component2 = go2.GetComponent<KBatchedAnimController>();
//                    component2.Play("loop", KAnim.PlayMode.Loop);
//                    this.flameContactAnim.Set(component2, smi);
//                })).Update((StateMachine<Burner.States, Burner.SMInstance, Burner>.State.Callback)(smi =>
//                {
//                    float lastFlameLength = (float)smi.master.lastFlameLength;
//                    Vector3 vector3_1 = smi.master.transform.position + smi.master.flameSourceOffset + new Vector3(0.0f, 0.5f, 0.0f);
//                    this.flameAnim.Get(smi).GetBatchInstanceData().SetClipRadius(vector3_1.x, vector3_1.y, lastFlameLength * lastFlameLength, true);
//                    Vector3 vector3_2 = vector3_1 + smi.master.rotatable.GetRotatedCellOffset(new CellOffset(smi.master.lastFlameLength, 0)).ToVector3();
//                    this.flameContactAnim.Get(smi).transform.position = vector3_2;
//                })).Exit((StateMachine<Burner.States, Burner.SMInstance, Burner>.State.Callback)(smi =>
//                {
//                    smi.master.elementEmitter.SetEmitting(false);
//                    UnityEngine.Object.Destroy((UnityEngine.Object)this.flameAnim.Get(smi).gameObject);
//                    UnityEngine.Object.Destroy((UnityEngine.Object)this.flameContactAnim.Get(smi).gameObject);
//                }));
//                this.expired.PlayAnim("expired", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Expired);
//            }
//        }
//    }

//}
