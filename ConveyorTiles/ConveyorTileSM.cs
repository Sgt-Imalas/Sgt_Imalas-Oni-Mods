using ConveyorTiles;
using Epic.OnlineServices.Platform;
using HarmonyLib;
using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI;
using static STRINGS.UI.DETAILTABS;

namespace ConveyorTiles
{
    internal class ConveyorTileSM : StateMachineComponent<ConveyorTileSM.StatesInstance>, ISaveLoadable, ICheckboxControl
    //, IGameObjectEffectDescriptor
    {
        [HarmonyPatch(typeof(Game))]
        [HarmonyPatch(nameof(Game.DestroyInstances))]
        public class Game_OnDestroy
        {
            public static void Postfix()
            {
                TileSMs.Clear();
            }
        }
        static StatusItem ConveyorTilePowerConsumption;

        public static void RegisterStatusItems()
        {
            ConveyorTilePowerConsumption = new StatusItem(
                      "CT_CONVEYORTILE_POWERCONSUMPTION",
                      "BUILDING",
                      "",
                      StatusItem.IconType.Info,
                      NotificationType.Neutral,
                      false,
                      OverlayModes.None.ID,
                      false
                      );

            ConveyorTilePowerConsumption.resolveStringCallback = delegate (string str, object data)
            {
                ConveyorTileSM conveyor = (ConveyorTileSM)data;

                string config = null;
                switch (conveyor.TileSpeedInternal)
                {
                    default:
                    case 1:
                        config = STRINGS.BUILDING.STATUSITEMS.CT_CONVEYORTILE_POWERCONSUMPTION.SPEEDBASE; break;
                    case 2:
                        config = STRINGS.BUILDING.STATUSITEMS.CT_CONVEYORTILE_POWERCONSUMPTION.SPEEDFAST; break;
                    case 3:
                        config = STRINGS.BUILDING.STATUSITEMS.CT_CONVEYORTILE_POWERCONSUMPTION.SPEEDEXPRESS; break;
                    case 4:
                        config = STRINGS.BUILDING.STATUSITEMS.CT_CONVEYORTILE_POWERCONSUMPTION.SPEEDRAPIT; break;

                }

                str = str.Replace("{CONFIG}", config);
                str = str.Replace("{WATTS}", GameUtil.GetFormattedWattage(conveyor.Wattage));
                return str;
            };
        }


        static Dictionary<int, ConveyorTileSM> TileSMs = new Dictionary<int, ConveyorTileSM>();

        public static readonly HashedString PORT_ID = (HashedString)nameof(ConveyorTileSM);
        public static readonly CellOffset RelevantDebrisTile = new CellOffset(0, 1);
        [MyCmpGet]
        private Operational operational;
        [MyCmpGet]
        private LogicPorts logicPorts;
        [MyCmpGet]
        private Rotatable rotatable;
        [MyCmpGet]
        private AnimTileable animTilable;
        [MyCmpGet]
        private KBatchedAnimController kbac;
        [MyCmpGet]
        private EnergyConsumer energyConsumer;
        [MyCmpAdd]
        CopyBuildingSettings buildingSettings;
        [MyCmpGet]
        KSelectable selectable;

        [Serialize] bool LogicControllsDirection = false;
        float TileSpeedInternal = 1;

        /// <summary>
        /// Factorio belt arrow colors
        /// </summary>
        Color tint_base = UIUtils.rgb(236, 163, 38), tint_fast = UIUtils.rgb(228, 49, 22), tint_express = UIUtils.rgb(15, 184, 252), tint_rapid = UIUtils.rgb(34, 196, 37);
        float speed_base = 1, speed_fast = 2f, speed_express = 3f, speed_rapid = 4f;

        Color gearTint = UIUtils.rgb(236, 163, 38);

        public float BeltSpeed => Config.Instance.SpeedMultiplier * TileSpeedInternal;
        public bool flipped => rotatable.IsRotated;
        public bool isOperational => operational.IsOperational;

        public string CheckboxTitleKey => "STRINGS.BUILDINGS.PREFABS.CT_CONVEYORTILE.CHECKBOX_LOGICDIRECTION.HEADER";

        public string CheckboxLabel => STRINGS.BUILDINGS.PREFABS.CT_CONVEYORTILE.CHECKBOX_LOGICDIRECTION.LABEL;

        public string CheckboxTooltip => STRINGS.BUILDINGS.PREFABS.CT_CONVEYORTILE.CHECKBOX_LOGICDIRECTION.TOOLTIP;

        private HandleVector<int>.Handle solidChangedEntry;
        private HandleVector<int>.Handle pickupablesChangedEntry;
        private HandleVector<int>.Handle floorSwitchActivatorChangedEntry;

        int moveItemCell, myCell;
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
                LogicControllsDirection = source.LogicControllsDirection;
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
            if (this.transform == null)
                return;


            if (animPercentage == -1)
            {
                animPercentage = kbac.GetElapsedTime();
            }
            else if (isOperational)
            {
                kbac.SetElapsedTime(animPercentage);
            }

            int left = Grid.CellLeft(myCell);
            int right = Grid.CellRight(myCell);

            if (propagateDirection != 2 && TileSMs.ContainsKey(left))
            {
                TileSMs[left]?.RefreshAnimState(1, animPercentage);
            }
            if (propagateDirection != 1 && TileSMs.ContainsKey(right))
            {
                TileSMs[right]?.RefreshAnimState(2, animPercentage);
            }
        }

        public static bool HasTileableNeighbor(ConveyorTileSM a, ConveyorTileSM b)
        {
            return a.flipped == b.flipped && a.isOperational == b.isOperational && Mathf.Approximately(a.TileSpeedInternal, b.TileSpeedInternal);
        }

        public void RefreshEndCaps()
        {
            if (this.transform == null) return;

            int left = Grid.CellLeft(myCell);
            int right = Grid.CellRight(myCell);
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

        public void UpdateEndCaps() => animTilable.UpdateEndCaps();

        public override void OnSpawn()
        {
            base.OnSpawn();
            selectable.AddStatusItem(ConveyorTilePowerConsumption, this);
            myCell = this.NaturalBuildingCell();
            moveItemCell = Grid.CellAbove(myCell);
            AdjustTileSpeed();
            this.smi.StartSM();
            //this.Subscribe(-801688580, new System.Action<object>(this.OnLogicValueChanged));
            this.Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
            TileSMs[myCell] = this;

            if (!Config.Instance.NoLogicInputs)
                this.Subscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);
        }
        private void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (logicValueChanged.portID != LogicOperationalController.PORT_ID)
                return;
            ReevaluateLogicState();
        }
        void ReevaluateLogicState()
        {
            if (LogicControllsDirection && logicPorts != null && logicPorts.IsPortConnected(LogicOperationalController.PORT_ID))
            {
                var inputBitsInt = logicPorts.GetInputValue(LogicOperationalController.PORT_ID);
                bool shouldBeFlipped = LogicCircuitNetwork.IsBitActive(1, inputBitsInt);
                if (shouldBeFlipped != flipped)
                {
                    Reverse();
                }
                float targetInternalSpeed = Config.Instance.SpeedMultiplier;

                bool thirdBit = LogicCircuitNetwork.IsBitActive(2, inputBitsInt);
                bool fourthBit = LogicCircuitNetwork.IsBitActive(3, inputBitsInt);
                if (thirdBit)
                {
                    targetInternalSpeed = fourthBit ? targetInternalSpeed * speed_rapid : targetInternalSpeed * speed_fast;
                    gearTint = fourthBit ? tint_rapid : tint_fast;
                }
                else
                {
                    targetInternalSpeed = fourthBit ? targetInternalSpeed * speed_express : targetInternalSpeed * speed_base;
                    gearTint = fourthBit ? tint_express : tint_base;
                }
                if (!Mathf.Approximately(targetInternalSpeed, TileSpeedInternal))
                {
                    TileSpeedInternal = targetInternalSpeed;
                    AdjustTileSpeed();
                }
            }
        }

        public int Wattage => Mathf.RoundToInt(BeltSpeed * Config.Instance.TileWattage);

        void AdjustTileSpeed()
        {
            if (Config.Instance.GearTint)
                kbac.SetSymbolTint("gear", gearTint);
            kbac.PlaySpeedMultiplier = BeltSpeed;
            float currentAnimProgression = kbac.GetElapsedTime();
            var currentAnim = kbac.GetCurrentAnim();
            var mode = kbac.GetMode();
            kbac.Play(currentAnim.name, mode);
            energyConsumer.BaseWattageRating = Wattage;

            RefreshAnimState(animPercentage: currentAnimProgression);
            RefreshEndCaps();
        }

        void TogglePause(bool pause) => kbac.stopped = pause;
        bool ValidItemCell(int cell) => Grid.IsValidCell(cell) && !Grid.IsSolidCell(cell);

        private void OnPickupablesChanged(object data, float dt)
        {
            if (!ValidItemCell(moveItemCell))
            {
                return;
            }
            bool liquidCellAbove = Grid.IsLiquid(moveItemCell);
            ListPool<ScenePartitionerEntry, ConveyorTileSM>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ConveyorTileSM>.Allocate();
            GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(moveItemCell).x, Grid.CellToXY(moveItemCell).y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>)gathered_entries);
            Vector3 newItemPos;
            for (int index = 0; index < gathered_entries.Count; ++index)
            {
                if (gathered_entries[index].obj is Pickupable pickupable && !pickupable.wasAbsorbed)
                {

                    if (pickupable.TryGetComponent<KPrefabID>(out var component))
                    {
                        ///Fishes that are not flopping (have to check via cell bc flopping tag is always there) and flyers are immune
                        if ((component.HasTag(GameTags.Creatures.Swimmer) && liquidCellAbove) || component.HasTag(GameTags.Creatures.Flyer) || component.HasTag(GameTags.Creatures.Hoverer))
                        {
                            continue;
                        }
                        if (component.HasTag(GameTags.Creatures.Walker) && Config.Instance.ImmuneCritters)
                        {
                            continue;
                        }
                        ///If config disables conveyor for non asleep dupes
                        if (component.HasTag(GameTags.DupeBrain) && Config.Instance.Immunes && !component.HasTag(GameTags.Asleep))
                        {
                            continue;
                        }
                        ///Working dupes that have started working are immune (doesnt affect walkers)
                        if (pickupable.TryGetComponent<Worker>(out var worker) && worker.workable != null && worker.workable.GetPercentComplete() > 0)
                        {
                            continue;
                        }
                    }

                    var transf = pickupable.transform;
                    newItemPos = transf.position + (rotatable.IsRotated ? Vector3.right : Vector3.left) * dt * 0.535f * BeltSpeed;
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
            GameScenePartitioner.Instance.Free(ref this.pickupablesChangedEntry);
            GameScenePartitioner.Instance.Free(ref this.floorSwitchActivatorChangedEntry);

            if (!Config.Instance.NoLogicInputs)
                Unsubscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);
            Unsubscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
            Unsubscribe(493375141, OnRefreshUserMenuDelegate);
            TileSMs.Remove(myCell);
            base.OnCleanUp();
        }

        public bool GetCheckboxValue() => LogicControllsDirection;

        public void SetCheckboxValue(bool value)
        {
            LogicControllsDirection = value;
            ReevaluateLogicState();
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
                    .Enter(smi => smi.master.RefreshEndCaps())
                    .EventTransition(GameHashes.OperationalChanged, On, smi => smi.master.operational.IsOperational)
                    .Exit(smi => smi.master.RefreshAnimState())
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


