using KSerialization;
using LogicSatellites.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LogicSatellites.Behaviours.ModAssets;

namespace LogicSatellites.Behaviours
{
    class SatelliteCarrierModule : StateMachineComponent<SatelliteCarrierModule.StatesInstance>, ISaveLoadable
	{
        [MyCmpReq] private KSelectable selectable;
        [MyCmpReq][Serialize]
		public Storage storage;



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

            public bool CanDeploySatellite(int type)
            {
				Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
				ClusterGridEntity atCurrentLocation = component.GetPOIAtCurrentLocation();
				if (!this.HoldingSatellite())
					return false;
				var locationRule = ModAssets.SatelliteConfigurations[type].AllowedLocation;

				if (locationRule == DeployLocation.anywhere)
				{
					return atCurrentLocation == null;
				}
				else if (locationRule == DeployLocation.orbital)
				{
					return atCurrentLocation == null && ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(component.Location, EntityLayer.Asteroid) != null;
				}
				else if (locationRule == DeployLocation.deepSpace)
				{
					return atCurrentLocation == null && ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(component.Location, EntityLayer.Asteroid) == null;
				}
				else if (locationRule == DeployLocation.temporalTear)
				{
					return atCurrentLocation.TryGetComponent<TemporalTear>(out var tear);
				}
				else return false;

			}

			public void EjectParts()
            {
				//Debug.Log("Holding a Satellite ? "+ HoldingSatellite());
				if (this.HoldingSatellite())
                {
					var satellite = storage.FindFirst(Tags.LS_Satellite);

					Debug.Log("Trying to drop Satellite");
					storage.Remove(satellite.gameObject);
					storage.items.Remove(satellite.gameObject);
					//satellite.gameObject.DeleteObject();
					GameObject.Destroy(satellite);
					
					var constructionPart = GameUtil.KInstantiate(Assets.GetPrefab(SatelliteComponentConfig.ID), gameObject.transform.position, Grid.SceneLayer.Ore);
					constructionPart.SetActive(true);
					var constructionPartElement = constructionPart.GetComponent<PrimaryElement>();
					constructionPartElement.Units = 20;
					sm.hasSatellite.Set(false, this);

				}
            }

            public ClusterGridEntity IsEntityAtLocation()
            {
				Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
				ClusterGridEntity atCurrentLocation = GetSatelliteAtCurrentLocation(component);
				return atCurrentLocation;
			}

			ClusterGridEntity GetSatelliteAtCurrentLocation(Clustercraft craft) { 
				var entity = craft.Status != Clustercraft.CraftStatus.InFlight || craft.IsFlightInProgress() ? (ClusterGridEntity)null : ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(craft.Location, EntityLayer.Payload);
				if(entity!= null && entity.GetComponent<SatelliteGridEntity>())
				{
					return entity;
                }
                else { return null; }

			}
			public bool CanRetrieveSatellite()
			{
				return IsEntityAtLocation()?.gameObject.GetComponent<SatelliteGridEntity>() != null;
			}


			public void TryRetrieveSatellite()
            {
                if (this.CanRetrieveSatellite()&& !this.HoldingSatellite())
				{
					var clusterSat = IsEntityAtLocation().gameObject;
					
					Debug.Log("Retrieving Satellite");
					int type = clusterSat.GetComponent<SatelliteGridEntity>().satelliteType;
					Debug.Log("SAT TYPE: " + type);
					clusterSat.DeleteObject();
                    sm.hasSatellite.Set(true, this);

                    //GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)SatelliteKitConfig.ID), this.transform.position);
					//storage.Store(sat.gameObject);
					//sat.GetComponent<Pickupable>().storage = storage;
					//sat.GetComponent<SatelliteTypeHolder>().SatelliteType = type;
				}
			}

            public void TryDeploySatellite(int type)
			{
				//Debug.Log(this.CanDeploySatellite() +" - " + this.HoldingSatellite());
				if (this.CanDeploySatellite(type) && this.HoldingSatellite())
				{
					Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
					//var satellite = storage.FindFirst(Tags.LS_Satellite);

					Debug.Log("Trying to deploy Satellite");
					Debug.Log("SAT TYPE: " + type);
					//Debug.Log(satellite);
					//storage.Remove(satellite.gameObject);
					//storage.items.Remove(satellite.gameObject);
					//satellite.gameObject.DeleteObject();
					//GameObject.Destroy(satellite);
					//Debug.Log(satellite.IsNullOrDestroyed() + "SHOULD BE TRUE");
					SpawnSatellite(component.Location,ModAssets.SatelliteConfigurations[type].GridID);
					sm.hasSatellite.Set(false,this);
				}
			}

			private void SpawnSatellite(AxialI location, string prefab)
            {
				Vector3 position = new Vector3(-1f, -1f, 0.0f);
				GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)prefab), position);
				sat.GetComponent<ClusterDestinationSelector>().SetDestination(location);
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
					.PlayAnim("satelite_build", KAnim.PlayMode.Loop);
				grounded.empty
					.PlayAnim("satelite_construction", KAnim.PlayMode.Loop)
					.Update((smi, dt) =>
					{
                        if (smi.storage.Has(SatelliteKitConfig.ID)&&hasSatellite.Get(smi) ==false)
                        {
							hasSatellite.Set(true,smi);
                        }
					})
					.ParamTransition<bool>(this.hasSatellite, this.grounded.loaded, IsTrue);


				not_grounded.DefaultState(this.not_grounded.loaded)
					.TagTransition(GameTags.RocketNotOnGround, this.grounded, true);

				not_grounded.loaded
					.PlayAnim("satelite_build", KAnim.PlayMode.Loop)
					.ParamTransition<bool>(this.hasSatellite, this.not_grounded.empty, IsFalse)
					.Update((smi, dt) =>
					{
						if(smi.storage.Has(SatelliteKitConfig.ID) && hasSatellite.Get(smi) == false)
						{
							hasSatellite.Set(true, smi);
						}

					});				
				not_grounded.empty
					.PlayAnim("satelite_missing", KAnim.PlayMode.Loop)
					.ParamTransition<bool>(this.hasSatellite, this.not_grounded.loaded, IsTrue)
					.Update((smi, dt) =>
					{
						if (smi.storage.Has(SatelliteKitConfig.ID) && hasSatellite.Get(smi) == false)
						{
							hasSatellite.Set(true, smi);
						}

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
