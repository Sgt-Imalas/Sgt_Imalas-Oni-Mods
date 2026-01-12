using Rockets_TinyYetBig.Content.Defs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Grid;
using static ResearchTypes;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings
{

	public class BunkerLaunchpadDigger : StateMachineComponent<BunkerLaunchpadDigger.StatesInstance>
	{
		public CellOffset diggableAreaMin, diggableAreaMax;
		GameObject SawbladeVis;
		KBatchedAnimController SawbladeKbac;
		int sawFrameTurnpoint = 28;

		public override void OnSpawn()
		{
			InitializeSawbladeVis();
			smi.StartSM();
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			if (SawbladeVis != null)
				Destroy(SawbladeVis);
		}
		void InitializeSawbladeVis()
		{
			Vector3 worldPos = transform.position;
			SawbladeVis = GameUtil.KInstantiate(Assets.GetPrefab(BunkerLaunchpadSawbladeConfig.ID), worldPos, Grid.SceneLayer.GasFront);
			SawbladeVis.SetActive(true);
			SawbladeVis.TryGetComponent(out SawbladeKbac);
			//SawbladeKbac.Play("working_loop", KAnim.PlayMode.Loop);
		}


		public class StatesInstance : GameStateMachine<States, StatesInstance, BunkerLaunchpadDigger, object>.GameInstance
		{
			public StatesInstance(BunkerLaunchpadDigger master) : base(master)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, BunkerLaunchpadDigger>
		{
			State waitingForDebris;
			DebrisDetected debrisDetected;
			public class DebrisDetected : State
			{
				public State ClearingPre;
				public State ClearingLoop;
				public State ClearingPost;
			}

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = waitingForDebris;
			}
		}
	}
}
