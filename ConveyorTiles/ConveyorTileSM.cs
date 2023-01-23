using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MoveDupeHere.ConveyorTileSM.States;

namespace MoveDupeHere
{
    internal class ConveyorTileSM : StateMachineComponent<ConveyorTileSM.StatesInstance>, ISaveLoadable
    //, IGameObjectEffectDescriptor
    {

        public static readonly HashedString PORT_ID = (HashedString)nameof(ConveyorTileSM);
        public static readonly CellOffset RelevantDebrisTile = new CellOffset(0, 1);
        [MyCmpGet]
        private Operational operational;
        [MyCmpGet]
        private Rotatable rotatable;

        private HandleVector<int>.Handle solidChangedEntry;
        private HandleVector<int>.Handle pickupablesChangedEntry;
        private HandleVector<int>.Handle floorSwitchActivatorChangedEntry;



        public override void OnSpawn()
        {
            base.OnSpawn();
            this.smi.StartSM();
            int cell = Grid.CellAbove(this.NaturalBuildingCell());
            //this.Subscribe(-801688580, new System.Action<object>(this.OnLogicValueChanged));
            //this.solidChangedEntry = GameScenePartitioner.Instance.Add("ConveyorTileSM.SolidChanged", (object) this.gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, new System.Action<object>(this.OnSolidChanged));
            //this.pickupablesChangedEntry = GameScenePartitioner.Instance.Add("ConveyorTileSM.PickupablesChanged", (object)this.gameObject, cell, GameScenePartitioner.Instance.pickupablesChangedLayer, new System.Action<object>(this.OnPickupablesChanged));
            //this.floorSwitchActivatorChangedEntry = GameScenePartitioner.Instance.Add("ConveyorTileSM.SwitchActivatorChanged", (object)this.gameObject, cell, GameScenePartitioner.Instance.floorSwitchActivatorChangedLayer, new System.Action<object>(this.OnActivatorsChanged));
        }


        private void OnPickupablesChanged(object data, float dt)
        {
            int cell = Grid.CellAbove(this.NaturalBuildingCell());
            ListPool<ScenePartitionerEntry, ConveyorTileSM>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ConveyorTileSM>.Allocate();
            GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(cell).x, Grid.CellToXY(cell).y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>)gathered_entries);
            for (int index = 0; index < gathered_entries.Count; ++index)
            {
                Pickupable pickupable = gathered_entries[index].obj as Pickupable;
                if (!((UnityEngine.Object)pickupable == (UnityEngine.Object)null) && !pickupable.wasAbsorbed)
                {
                   
                    KPrefabID component = pickupable.GetComponent<KPrefabID>();
                    if (!component.HasTag(GameTags.Creature) || component.HasTag(GameTags.Creatures.Walker) || component.HasTag(GameTags.Creatures.Flopping))
                    {
                        if (rotatable.IsRotated)
                        {
                            pickupable.gameObject.transform.position += transform.right * dt * 0.6f;
                        }
                        else
                        {
                            pickupable.gameObject.transform.position -= transform.right * dt * 0.6f;
                        }
                    }
                }
            }
            gathered_entries.Recycle();
        }


        private void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (logicValueChanged.portID != ConveyorTileSM.PORT_ID)
                return;
            bool logic_on = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
            operational.SetActive(logic_on);
        }


        public override void OnCleanUp()
        {
            //GameScenePartitioner.Instance.Free(ref this.solidChangedEntry);
            GameScenePartitioner.Instance.Free(ref this.pickupablesChangedEntry);
            GameScenePartitioner.Instance.Free(ref this.floorSwitchActivatorChangedEntry);
            //this.Unsubscribe(-801688580, new System.Action<object>(this.OnLogicValueChanged));
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
                    .EventTransition(GameHashes.OperationalChanged, On, smi => smi.master.operational.IsOperational);
                On
                    .PlayAnim("working",KAnim.PlayMode.Loop)
                    .EventTransition(GameHashes.OperationalChanged, Off, smi => !smi.master.operational.IsOperational)
                    .Update((smi, dt)=>
                    {
                        smi.master.OnPickupablesChanged(null,dt); 
                    })
                    ;
                    

            }
        }


        #endregion
    }
}


