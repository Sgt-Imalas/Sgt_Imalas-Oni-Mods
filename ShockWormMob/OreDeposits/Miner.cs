using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClusterMapRocketAnimator.UtilityStates;
using UnityEngine;
using KSerialization;
using static STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.EXPANSION1ACTIVE.LEVELS;
using Klei.AI;
using static ElementConverter;

namespace ShockWormMob.OreDeposits
{

    //Converted the class to a state machine component because thats *fancier*
    //(its a nice change as it allows to model the different states the drill is in much better)
    public class Miner : StateMachineComponent<Miner.StatesInstance>
    {
        /// <summary>
        /// Tag we add to the craftable drillbit material so we can define the material
        /// </summary>
        public static Tag DrillbitMaterial = TagManager.Create("DepositDrillBitConsumable");

        [MyCmpGet] AttachableBuilding attatchableBuilding;
        [MyCmpGet] PrimaryElement primaryElement;
        OreDeposit Deposit;


        /// <summary>
        /// storage for our drill input material;
        /// </summary>
        public Storage drillbitStorage;

        /// <summary>
        /// do we dump the material to the world or store it for shipping? can be set in the DrillBuildingConfig
        /// </summary>
        [Serialize] public bool DumpMaterialToWorld = true;

        /// <summary>
        /// Tile where the resources get dropped out - can be adjusted via changing InputCellOffset;
        /// </summary>
        [Serialize] private int materialOutputCell;
        public CellOffset outputCellOffset = new CellOffset(0,0);


        /// <summary>
        /// The output storage if we ship the output via rail instead of dumping it to the world
        /// </summary>
        public Storage outputStorage = null;


        /// <summary>
        /// Rate at which the drillbit is consumed. Default: 1 drillbit per 500kg mined - can be adjusted in building def
        /// </summary>
        public float drillbitConsumptionRate = 1f / 500f;

        /// <summary>
        /// kg per second mined
        /// </summary>
        public float BaseMiningSpeed = 10f;

        



        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        public override void OnSpawn()
        {
            //getting that refererence
            GetDepositReference();
            //getting the output cell
            materialOutputCell = Grid.OffsetCell(Grid.PosToCell(this), outputCellOffset);

            //starting the state machine
            smi.StartSM();
        }


        public bool IsMiningPossible(float dt = 1f)
        {
            if (DumpMaterialToWorld)
                return drillbitStorage.MassStored() >= BaseMiningSpeed * drillbitConsumptionRate * dt;
            else
                return drillbitStorage.MassStored() >= BaseMiningSpeed * drillbitConsumptionRate * dt && outputStorage.RemainingCapacity() >= BaseMiningSpeed * dt; //rn this blocks production if the output is full, can be changed if needed

        }
        public void MineResources(float dt)
        {
            //Getting the "current" element + rate multiplier
            var ToProduce = Deposit.GetCurrentMiningElement();
            SimHashes minedMaterial = ToProduce.Key;

            float ElementMassToProduce = dt * ToProduce.Value * BaseMiningSpeed;
            float DrillBitToConsume = dt * drillbitConsumptionRate * BaseMiningSpeed;

            //consuming the drillbit 
            drillbitStorage.ConsumeIgnoringDisease(DrillbitMaterial, DrillBitToConsume);

            Element elementByHash = ElementLoader.FindElementByHash(minedMaterial);
            if (DumpMaterialToWorld)
            {

                //creates output material and yeets it into the world
                if (elementByHash.IsGas || elementByHash.IsLiquid)
                    SimMessages.AddRemoveSubstance(materialOutputCell, minedMaterial, CellEventLogger.Instance.ElementEmitted, ElementMassToProduce, primaryElement.Temperature, byte.MaxValue, 0);
                else if (elementByHash.IsSolid)
                {
                   var chunk =  elementByHash.substance.SpawnResource(Grid.CellToPos(materialOutputCell
                       ,CellAlignment.Center,Grid.SceneLayer.Ore 
                       ), ElementMassToProduce, primaryElement.Temperature, byte.MaxValue, 0);
                    chunk.SetActive(true);
                    Debug.Log(chunk);
                    Debug.Log(chunk.transform.position.ToString());
                }

            }
            else
            {
                //creates output material and puts it into output storage
                switch (elementByHash.state & Element.State.Solid)
                {
                    case Element.State.Gas:
                        outputStorage.AddGasChunk(minedMaterial, ElementMassToProduce, primaryElement.Temperature, byte.MaxValue, 0, false);
                        break;
                    case Element.State.Liquid:
                        outputStorage.AddLiquid(minedMaterial, ElementMassToProduce, primaryElement.Temperature, byte.MaxValue, 0);
                        break;
                    case Element.State.Solid:
                        outputStorage.AddOre(minedMaterial, ElementMassToProduce, primaryElement.Temperature, byte.MaxValue, 0);
                        break;
                }
            }
        }

        public void GetDepositReference()
        {
            //foreach will only run once as there is only one attached gameobject- the deposit
            foreach (var deposit in AttachableBuilding.GetAttachedNetwork(attatchableBuilding))
            {
                //setting local reference variable so we dont have to run getComponent each time we want something from it
                deposit.TryGetComponent(out Deposit);
            }
        }



        public class StatesInstance : GameStateMachine<States, StatesInstance, Miner, object>.GameInstance
        {
            public Operational operational;
            public StatesInstance(Miner master) : base(master)
            {
                operational = master.GetComponent<Operational>();

            }
        }


        /// <summary>
        /// States- this is where our spicy stuff takes place.
        /// We define different behaviors on how the building should function, based on external and internal conditions
        /// Also we can easily set state specific animations
        /// </summary>

        public class States : GameStateMachine<States, StatesInstance, Miner>
        {
            //sub states, allows more branched out specific logic while having overarching processes in the main state
            public class EnabledStates : State
            {
                public State waiting;
                public State drilling;
            }

            State disabled;
            EnabledStates enabled;


            public override void InitializeStates(out BaseState default_state)
            {
                default_state = disabled;
                disabled
                    //missing power and logic signals change the operational and emit the event OperationalChanged, this here will automatically trigger, check that new value and move to the correct state when that happens
                    .EventTransition(GameHashes.OperationalChanged, this.enabled, (smi => smi.operational.IsOperational))
                    ;
                enabled
                    .EventTransition(GameHashes.OperationalChanged, this.enabled, (smi => !smi.operational.IsOperational))
                    ;
                enabled.defaultState = enabled.waiting; //start off in waiting to check if we can mine at all 
                enabled.waiting
                    //only recheck when something enters or leaves storage, as thats the only thing that can change the condition
                    .EventTransition(GameHashes.OnStorageChange, enabled.drilling, (smi) => smi.master.IsMiningPossible())
                    ;
                enabled.drilling
                    .Update((smi, dt) =>
                    {
                        //going back to waiting if mining is not possible, otherwise mine resources
                        if (!smi.master.IsMiningPossible(dt))
                        {
                            smi.GoTo(enabled.waiting);
                        }
                        else
                        {
                            smi.master.MineResources(dt);
                        }

                    }, UpdateRate.SIM_200ms //executes 5 times a second, can be changed
                    );

            }
        }


        #region Accumulator
        ///No longer in use
        //private void CreatingNewAccumulators()
        //{

        //    for (int i = 0; i < converter.consumedElements.Length; i++)
        //    {
        //        converter.consumedElements[i].Accumulator = Game.Instance.accumulators.Add("ElementsConsumed", converter);
        //    }

        //    for (int j = 0; j < converter.outputElements.Length; j++)
        //    {
        //        converter.outputElements[j].accumulator = Game.Instance.accumulators.Add("OutputElements", converter);
        //    }
        //}
        //private void CleaningUpOldAccumulators()
        //{
        //    for (int i = converter.consumedElements.Length - 1; i >= 0; i--)
        //    {
        //        Game.Instance.accumulators.Remove(converter.consumedElements[i].Accumulator);
        //    }

        //    for (int i = converter.outputElements.Length - 1; i >= 0; i--)
        //    {
        //        Game.Instance.accumulators.Remove(converter.outputElements[i].accumulator);
        //    }
        //}
        
        #endregion    
    }
}
