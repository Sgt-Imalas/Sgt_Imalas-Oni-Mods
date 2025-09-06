using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PoisNotIncluded.Content.Scripts
{

    public class CustomLightController : StateMachineComponent<CustomLightController.StatesInstance>
    {
        [MyCmpGet] Operational operational;

        [SerializeField] public string OnAnim = "on", OffAnim = "off";

        public override void OnSpawn()
        {
            smi.StartSM();
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, CustomLightController, object>.GameInstance
        {
            public StatesInstance(CustomLightController master) : base(master)
            {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, CustomLightController>
		{
			public State off;
			public State on;
			public override void InitializeStates(out BaseState default_state)
            {
                default_state = off;
				this.off
                    .PlayAnim(smi =>smi.master.OffAnim)
                    .EventTransition(GameHashes.OperationalChanged, this.on, (smi => smi.master.operational.IsOperational));
				this.on
					.PlayAnim(smi => smi.master.OnAnim)
					.EventTransition(GameHashes.OperationalChanged, this.off,(smi => !smi.master.operational.IsOperational))
                    .ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingLight)
                    .Enter("SetActive",(smi => smi.master.operational.SetActive(true)));

			}
		}
    }
}
