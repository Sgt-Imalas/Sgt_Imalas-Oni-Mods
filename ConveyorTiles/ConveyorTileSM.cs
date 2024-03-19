using ConveyorTiles;
using Epic.OnlineServices.Platform;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace ConveyorTiles
{
    internal class ConveyorTileSM : StateMachineComponent<ConveyorTileSM.StatesInstance>, ISaveLoadable
    //, IGameObjectEffectDescriptor
    {
        public static ModHashes Refresh = new ModHashes("CT_RefreshAnim");
        static Dictionary<int,ConveyorTileSM> TileSMs = new Dictionary<int,ConveyorTileSM>();


        public static readonly HashedString PORT_ID = (HashedString)nameof(ConveyorTileSM);
        public static readonly CellOffset RelevantDebrisTile = new CellOffset(0, 1);
        [MyCmpGet]
        private Operational operational;
        [MyCmpGet]
        private Rotatable rotatable;
        [MyCmpGet]
        private AnimTileable animTilable;
        [MyCmpGet]
        private KBatchedAnimController kbac;
        [MyCmpAdd]
        CopyBuildingSettings buildingSettings;

        public bool flipped => rotatable.IsRotated;
        public bool isOperational => operational.IsOperational;

        private HandleVector<int>.Handle solidChangedEntry;
        private HandleVector<int>.Handle pickupablesChangedEntry;
        private HandleVector<int>.Handle floorSwitchActivatorChangedEntry;

        int cell;
        private static readonly EventSystem.IntraObjectHandler<ConveyorTileSM> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ConveyorTileSM>((component, data) => component.OnCopySettings(data));
        private static readonly EventSystem.IntraObjectHandler<ConveyorTileSM> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ConveyorTileSM>((System.Action<ConveyorTileSM, object>)((component, data) => component.OnRefreshUserMenu(data)));
        public override void OnPrefabInit() => this.Subscribe<ConveyorTileSM>(493375141, OnRefreshUserMenuDelegate);
        private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_mirror", STRINGS.MODBUTTONS.FLIPBUTTON.LABEL, new System.Action(this.Reverse), Action.BuildingUtility1, tooltipText: STRINGS.MODBUTTONS.FLIPBUTTON.TOOLTIP));

        private void OnCopySettings(object obj)
        {
            if (obj != null && obj is GameObject go && go.GetComponent<ConveyorTileSM>())
            {
                var source = go.GetComponent<ConveyorTileSM>();
                rotatable.SetOrientation(source.rotatable.GetOrientation());
                RefreshEndCaps();
            }
        }
        void Reverse()
        {
            rotatable.SetOrientation(flipped ? Orientation.Neutral : Orientation.FlipH);
            RefreshEndCaps();
        }

        private void RefreshAnimState(int propagateDirection = 0, float animPercentage = -1)
        {
            if(animPercentage == -1)
            {
                animPercentage = kbac.GetElapsedTime();
            }
            else if (isOperational)
            {
                kbac.SetElapsedTime(animPercentage);
            }

            int left = Grid.CellLeft(this.NaturalBuildingCell());
            int right = Grid.CellRight(this.NaturalBuildingCell());

            if (propagateDirection != 2 && TileSMs.ContainsKey(left))
            {
                TileSMs[left]?.RefreshAnimState(1, animPercentage);
            }
            if (propagateDirection != 1 && TileSMs.ContainsKey(right))
            {
                TileSMs[right]?.RefreshAnimState(2, animPercentage);
            }
        }

        public void RefreshEndCaps()
        {
            int left = Grid.CellLeft(this.NaturalBuildingCell());
            int right = Grid.CellRight(this.NaturalBuildingCell());
            if (TileSMs.ContainsKey(left))
            {
                TileSMs[left]?.UpdateEndCaps();
            }
            if (TileSMs.ContainsKey(right))
            {
                TileSMs[right]?.UpdateEndCaps();
            }
            UpdateEndCaps();
        }

        public void UpdateEndCaps()=> animTilable.UpdateEndCaps();

        public override void OnSpawn()
        {
            base.OnSpawn();
            kbac.PlaySpeedMultiplier = Config.Instance.SpeedMultiplier;
            RefreshAnimState();
            this.smi.StartSM();
            cell = Grid.CellAbove(this.NaturalBuildingCell());
            this.Subscribe(-801688580, new System.Action<object>(this.OnLogicValueChanged));
            this.Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
            TileSMs.Add(this.NaturalBuildingCell(), this);
            //this.solidChangedEntry = GameScenePartitioner.Instance.Add("ConveyorTileSM.SolidChanged", (object) this.gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, new System.Action<object>(this.OnSolidChanged));
            //var s =                  GameScenePartitioner.Instance.Add("LadderBed.Constructor", (object)this.gameObject, cell, GameScenePartitioner.Instance.pickupablesChangedLayer, new System.Action<object>(this.OnMoverChanged)));
            //this.pickupablesChangedEntry = GameScenePartitioner.Instance.Add("ConveyorTileSM.PickupablesChanged", (object)this.gameObject, cell, GameScenePartitioner.Instance.pickupablesChangedLayer, new System.Action<object>(this.OnPickupablesChanged));
            //this.floorSwitchActivatorChangedEntry = GameScenePartitioner.Instance.Add("ConveyorTileSM.SwitchActivatorChanged", (object)this.gameObject, cell, GameScenePartitioner.Instance.floorSwitchActivatorChangedLayer, new System.Action<object>(this.OnActivatorsChanged));
            
        }
        private void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            //if (logicValueChanged.portID != ConveyorTileSM.PORT_ID)
            //    return;

            bool logic_on = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
            //operational.SetActive(logic_on);
            //RefreshEndCaps();
        }
        void TogglePause(bool pause) =>kbac.stopped = pause;
        bool ValidItemCell(int cell) => Grid.IsValidCell(cell) && !Grid.IsSolidCell(cell);

        private void OnPickupablesChanged(object data, float dt)
        {
            if (!ValidItemCell(cell))
            {
                return;
            }

            ListPool<ScenePartitionerEntry, ConveyorTileSM>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ConveyorTileSM>.Allocate();
            GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(cell).x, Grid.CellToXY(cell).y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>)gathered_entries);
            Vector3 newItemPos;
            for (int index = 0; index < gathered_entries.Count; ++index)
            {
                if (gathered_entries[index].obj is Pickupable pickupable && !pickupable.wasAbsorbed)
                {

                    if (pickupable.TryGetComponent<KPrefabID>(out var component)
                        && component.HasTag(GameTags.DupeBrain) 
                        && (Config.Instance.Immunes || !component.HasTag(GameTags.Asleep))
                        && (Config.Instance.Immunes || (pickupable.TryGetComponent<Worker>(out var worker) && worker.workable != null && worker.workable.GetPercentComplete()>0))
                        )
                    {
                        continue;
                    }


                    var transf = pickupable.transform;
                    newItemPos = transf.position + (rotatable.IsRotated ? Vector3.right : Vector3.left) * dt * 0.535f * Config.Instance.SpeedMultiplier;
                    if (ValidItemCell(Grid.PosToCell(newItemPos)))
                    {
                        transf.SetPosition(newItemPos);
                    }

                }
            }
            gathered_entries.Recycle();
        }



        public override void OnCleanUp()
        {
            //GameScenePartitioner.Instance.Free(ref this.solidChangedEntry);
            GameScenePartitioner.Instance.Free(ref this.pickupablesChangedEntry);
            GameScenePartitioner.Instance.Free(ref this.floorSwitchActivatorChangedEntry);
            this.Unsubscribe(-801688580, new System.Action<object>(this.OnLogicValueChanged));
            this.Unsubscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
            TileSMs.Remove(this.NaturalBuildingCell());
            base.OnCleanUp();
        }
        #region StateMachine

        public class StatesInstance : GameStateMachine<States, StatesInstance, ConveyorTileSM>.GameInstance
        {
            public StatesInstance(ConveyorTileSM master) : base(master)
            {
            }
        }

        public class States : GameStateMachine<ConveyorTileSM.States, ConveyorTileSM.StatesInstance, ConveyorTileSM, object>
        {
            public State On;
            public State Off;


            public override void InitializeStates(out BaseState defaultState)
            {

                defaultState = Off;

                Off
                    .PlayAnim("off")
                    .Enter(smi=> smi.master.RefreshEndCaps())
                    .EventTransition(GameHashes.OperationalChanged, On, smi => smi.master.operational.IsOperational )  
                    .Exit(smi=>smi.master.RefreshAnimState())
                    ;
                On
                    .Enter(smi => smi.master.RefreshEndCaps())
                    .Enter(smi => smi.master.operational.SetActive(true))
                    .Exit(smi => smi.master.operational.SetActive(false))
                    .PlayAnim("working", KAnim.PlayMode.Loop)

                    .EventTransition(GameHashes.OperationalChanged, Off, smi => !smi.master.operational.IsOperational)
                    .Update((smi, dt) =>
                    {
                        smi.master.OnPickupablesChanged(null, dt);
                    }, UpdateRate.SIM_33ms)
                    ;


            }
        }



        #endregion
    }
}


