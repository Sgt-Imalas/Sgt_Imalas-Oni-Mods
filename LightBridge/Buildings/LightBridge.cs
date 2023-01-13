using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace LightBridge.Buildings
{
    internal class LightBridge : StateMachineComponent<LightBridge.SMInstance>, ISaveLoadable, ISingleSliderControl, ISliderControl
    //, IGameObjectEffectDescriptor
    {
        [MyCmpReq] protected Operational operational;
        [MyCmpReq] private KSelectable selectable;
        [MyCmpGet] private Rotatable rotatable;
        [MyCmpGet] private Building component;

        [MyCmpGet] private LessUpdatedFakeFloor fakeFloor;

        const int MaxLength = 9;
        [Serialize]
        int CurrentLength = MaxLength;

        public StatusItem _LaserBridgeStatusItem;

        public List<CellOffset> LightBridgeArea;
        public MeterController LightBridgeMeter { get; private set; }

        #region slider
        public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.LIGHTBRIDGESIDESCREEN.TITLE";

        public string SliderUnits => STRINGS.UI.UISIDESCREENS.LIGHTBRIDGESIDESCREEN.TILEUNITS;
        public int SliderDecimalPlaces(int index) => 0;
        public float GetSliderMin(int index) => 0;
        public float GetSliderMax(int index) => MaxLength;

        public float GetSliderValue(int index) => CurrentLength;

        public void SetSliderValue(float percent, int index)
        {
            CurrentLength = (int)percent; 
            UpdateBrigdeState();
        }

        public string GetSliderTooltipKey(int index)=> "STRINGS.UI.UISIDESCREENS.LIGHTBRIDGESIDESCREEN.DESC";

        public string GetSliderTooltip() => STRINGS.UI.UISIDESCREENS.LIGHTBRIDGESIDESCREEN.DESC;
#endregion

        int lastLength = 0;

        #region Spawn&Cleanup
        protected override void OnSpawn()
        {
            SetupLightBridgeArea();
            this.LightBridgeMeter = new MeterController(this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, Array.Empty<string>());
            this.LightBridgeMeter.SetPositionPercent(0f);
            base.OnSpawn();
            smi.StartSM(); 
        }
        public void EnableBridge()
        {
            int BridgeLength = GetBridgeLength();
            fakeFloor.floorOffsets = LightBridgeArea.Take(BridgeLength).ToArray();
            LightBridgeMeter.SetPositionPercent(((float)BridgeLength / (float)MaxLength));
            fakeFloor.SetFloor(true);
        }
        public void ShutDownBridge()
        {
            this.LightBridgeMeter.SetPositionPercent(0f);
            fakeFloor.SetFloor(false);
        }
        public void UpdateBrigdeState()
        {
            int BridgeLength = GetBridgeLength();
            //Debug.Log("BridgeLength: " + BridgeLength);;

            if (BridgeLength == lastLength)
                return;

            fakeFloor.UpdateFloorCells(LightBridgeArea.Take(BridgeLength).ToArray());
            LightBridgeMeter.SetPositionPercent(((float)BridgeLength / (float)MaxLength));
            lastLength = BridgeLength;
        }


        int GetBridgeLength()
        {
            int cell = Grid.PosToCell(this);
            for (int i = 0; i < CurrentLength; i++)
            {
                CellOffset rotatedOffset = component.GetRotatedOffset(LightBridgeArea[i]);
                int num = Grid.OffsetCell(cell, rotatedOffset);
                //Debug.Log("Cell: " + cell + ", IsSolid: " + Grid.IsSolidCell(cell) + ", IsLiquid: " + Grid.IsLiquid(cell) + ", IsGas: " + Grid.IsGas(cell));
                if (Grid.IsValidCell(num)&&Grid.AreCellsInSameWorld(num, cell) && Grid.IsSolidCell(num)
                   // && !Grid.Transparent[num]
                    )
                {
                    return i;
                }
            }
            return CurrentLength;
        }
        public void SetupLightBridgeArea()
        {

            LightBridgeArea = new List<CellOffset>();
            for (int i = 2; i < MaxLength + 2; i++)
            {
                LightBridgeArea.Add(new CellOffset(i, 0));
            }

        }

        
        #endregion


        #region StateMachine
        public class SMInstance : GameStateMachine<States, SMInstance, LightBridge, object>.GameInstance
        {
            private readonly Operational _operational;
            public readonly KSelectable _selectable;

            public SMInstance(LightBridge master) : base(master)
            {
                _operational = master.GetComponent<Operational>();
                _selectable = master.GetComponent<KSelectable>();
            }
            public bool IsOperational => _operational.IsFunctional && _operational.IsOperational;

        }

        public class States : GameStateMachine<States, SMInstance, LightBridge>
        {
            public State Emmitting;
            public State ShuttingOff;
            public State Off;
            public override void InitializeStates(out BaseState defaultState)
            {

                defaultState = Off;

                Off
                    .QueueAnim("off")
                    .EventTransition(GameHashes.OperationalChanged, Emmitting, smi => smi.IsOperational);


                Emmitting
                    .Enter((smi)=>smi.master.EnableBridge())
                    .Update((smi, dt) =>
                    {
                        smi.master.UpdateBrigdeState();
                    }, UpdateRate.SIM_200ms)
                    .PlayAnim("working_pre")
                    .QueueAnim("working", true)
                    .EventTransition(GameHashes.OperationalChanged, ShuttingOff, smi => !smi.IsOperational);
                ShuttingOff
                    .Enter((smi) => smi.master.ShutDownBridge())
                    .PlayAnim("working_post")
                    .OnAnimQueueComplete(Off);

            }
        }
        #endregion
    }
}

