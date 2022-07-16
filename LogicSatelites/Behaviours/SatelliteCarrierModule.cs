using LogicSatelites.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LogicSatelites.Behaviours.ModAssets;

namespace LogicSatelites.Behaviours
{
    class SatelliteCarrierModule : StateMachineComponent<SatelliteCarrierModule.StatesInstance>, ISaveLoadable
	{
        [MyCmpReq] private KSelectable selectable;
        [MyCmpReq] public Storage storage;


		protected override void OnSpawn()
		{
			base.OnSpawn();
			this.smi.StartSM();
		}

		#region StateMachine
		public class StatesInstance : GameStateMachine<States, StatesInstance, IStateMachineTarget, object>.GameInstance, ISatelliteCarrier
		{       
			public bool HoldingSatellite()
			{
				return sm.hasSatellite.Get(this);
			}

            public bool CanDeploySatellite()
            {
				return IsEntityAtLocation()==null; // && freeSpace
            }

			public ClusterGridEntity IsEntityAtLocation()
            {
				Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
				ClusterGridEntity atCurrentLocation = component.GetPOIAtCurrentLocation();
				return atCurrentLocation;
			}

            public bool CanRetrieveSatellite()
			{
				return IsEntityAtLocation()?.gameObject.GetComponent<SatelliteGridEntity>() != null;
			}
			public void OnButtonClicked()
			{
                if (ModeIsDeployment)
                {
					TryDeploySatellite();
					Debug.Log("Deploying Satellite");
				}
				else
                {
					TryRetrieveSatellite();
					Debug.Log("Retrieving Satellite");
				}
			}

			public void TryRetrieveSatellite()
            {
                if (this.CanRetrieveSatellite()&& !this.HoldingSatellite())
				{
					var clusterSat = IsEntityAtLocation().gameObject;
					clusterSat.DeleteObject();
					sm.hasSatellite.Set(true, this);

					Vector3 position = new Vector3(-1f, -1f, 0.0f);
					GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)"LS_ClusterSateliteLogic"), position);
					storage.Store(sat.gameObject);
				}
			}

            public void TryDeploySatellite()
			{
				Debug.Log(this.CanDeploySatellite() +" - " + this.HoldingSatellite());
				if (this.CanDeploySatellite() && this.HoldingSatellite())
				{
					Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
					var satellite = storage.FindFirst(Tags.LS_Satellite);
					Debug.Log(satellite);
					storage.Remove(satellite);
					Util.KDestroyGameObject(satellite);
					SpawnSatellite(component.Location);
					sm.hasSatellite.Set(false,this);
				}
			}

			private void SpawnSatellite(AxialI location)
            {
				Vector3 position = new Vector3(-1f, -1f, 0.0f);
				GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)"LS_SatelliteGrid"), position);
				sat.name = ModAssets.GetSatelliteNameRandom();
				sat.GetComponent<ClusterGridEntity>().Location = location;
				sat.SetActive(true);			
			}


            public bool ModeIsDeployment
			{ 
				get;
				set; 
			}

            public Storage storage;
			public StatesInstance(IStateMachineTarget master) : base(master)
			{
				this.storage = master.GetComponent<Storage>();
			}
        }

		public class States : GameStateMachine<States, StatesInstance, IStateMachineTarget>
		{
			public GameStateMachine<States, StatesInstance, IStateMachineTarget>.BoolParameter hasSatellite;

			public GroundedStates grounded;
			public NotGroundedStates not_grounded;
			public override void InitializeStates(out BaseState defaultState)
			{
				defaultState = grounded; 

				grounded.DefaultState(this.grounded.loaded)
						.TagTransition(GameTags.RocketNotOnGround, this.not_grounded);

				grounded.loaded
					.ParamTransition<bool>(this.hasSatellite, this.grounded.empty, IsFalse)
					.PlayAnim("ready_to_launch", KAnim.PlayMode.Loop);
				grounded.empty
					.PlayAnim("satelite_construction",KAnim.PlayMode.Loop)
					.Update((smi, dt) =>
					{
                        if (smi.storage.Has(SatelliteLogicConfig.ID)&&hasSatellite.Get(smi) ==false)
                        {
							hasSatellite.Set(true,smi);
                        }
					})
					.ParamTransition<bool>(this.hasSatellite, this.grounded.loaded, IsTrue);


				not_grounded.DefaultState(this.not_grounded.loaded)
					.TagTransition(GameTags.RocketNotOnGround, this.grounded, true);

				not_grounded.loaded
					.ParamTransition<bool>(this.hasSatellite, this.not_grounded.empty, IsFalse)
					.Update((smi, dt) =>
					{
						
					});				
				not_grounded.empty
					.ParamTransition<bool>(this.hasSatellite, this.not_grounded.loaded, IsTrue)
					.Update((smi, dt) =>
					{

					});
			}


			public class NotGroundedStates : State
			{
				public State loaded;
				public State empty;
			}
			public class GroundedStates : State
			{
				public State loaded;
				public State empty;
			}

		}
		#endregion
	}
}
