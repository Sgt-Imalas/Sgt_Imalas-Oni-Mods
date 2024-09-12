using KSerialization;
using LogicSatellites.Entities;
using UnityEngine;
using static LogicSatellites.Behaviours.ModAssets;

namespace LogicSatellites.Behaviours
{
	class SatelliteCarrierModule : StateMachineComponent<SatelliteCarrierModule.StatesInstance>, ISaveLoadable
	{
		[MyCmpReq] private KSelectable selectable;
		[MyCmpReq]
		public Storage storage;

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.smi.StartSM();
		}

		#region StateMachine
		public class StatesInstance : GameStateMachine<States, StatesInstance, IStateMachineTarget, object>.GameInstance, ISatelliteCarrier
		{

			[MyCmpGet]
			public RocketModuleCluster rocketModuleCluster;

			Clustercraft craft;

			public bool HoldingSatellite()
			{
				return sm.hasSatellite.Get(this);
			}

			public bool CanDeploySatellite()
			{

				if (!this.HoldingSatellite() || craft.IsTravellingAndFueled())
					return false;

				ClusterGridEntity atCurrentLocation = craft.GetPOIAtCurrentLocation();

				var locationRule = ModAssets.SatelliteConfigurations[this.SatelliteType()].AllowedLocation;

				if (locationRule == DeployLocation.anywhere)
				{
					return atCurrentLocation == null;
				}
				else if (locationRule == DeployLocation.orbital)
				{
					return atCurrentLocation == null && ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(craft.Location, EntityLayer.Asteroid) != null;
				}
				else if (locationRule == DeployLocation.deepSpace)
				{
					return atCurrentLocation == null && ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(craft.Location, EntityLayer.Asteroid) == null;
				}
				else if (locationRule == DeployLocation.temporalTear)
				{
					return atCurrentLocation.TryGetComponent<TemporalTear>(out _);
				}
				else return false;

			}

			public void EjectParts()
			{
				//Debug.Log("Holding a Satellite ? "+ HoldingSatellite());
				if (this.HoldingSatellite())
				{
					var satellite = storage.FindFirst(Tags.LS_Satellite);

					//Debug.Log("Trying to drop Satellite");
					storage.Remove(satellite.gameObject);
					storage.items.Remove(satellite.gameObject);
					////satellite.gameObject.DeleteObject();
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
				if (gameObject.TryGetComponent<RocketModuleCluster>(out var module) && module.CraftInterface.TryGetComponent<Clustercraft>(out var craft))
					return GetSatelliteAtCurrentLocation(craft);
				return null;
			}

			ClusterGridEntity GetSatelliteAtCurrentLocation(Clustercraft craft)
			{
				var entity = craft.Status != Clustercraft.CraftStatus.InFlight || craft.IsFlightInProgress() ? (ClusterGridEntity)null : ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(craft.Location, EntityLayer.Payload);
				if (entity != null && entity.GetComponent<SatelliteGridEntity>())
				{
					return entity;
				}
				else { return null; }

			}
			public bool CanRetrieveSatellite()
			{
				if (this.gameObject.IsNullOrDestroyed())
					return false;

				var entity = IsEntityAtLocation();
				return (entity != null && entity.TryGetComponent<SatelliteGridEntity>(out _) && !HoldingSatellite());
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
				if (this.CanRetrieveSatellite() && !this.HoldingSatellite())
				{
					var clusterSat = IsEntityAtLocation().gameObject;

					Debug.Log("Retrieving Satellite");
					int type = clusterSat.GetComponent<SatelliteGridEntity>().satelliteType;
					Debug.Log("SAT TYPE: " + type);
					clusterSat.DeleteObject();
					sm.hasSatellite.Set(true, this);



					//GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)SatelliteKitConfig.ID), this.transform.GetPosition(), Grid.SceneLayer.Ore);
					//go.SetActive(true);
					//this.storage.Store(go);

					GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)SatelliteKitConfig.ID), this.transform.GetPosition());
					sat.SetActive(true);
					sat.name = Assets.GetPrefab((Tag)SatelliteKitConfig.ID).GetProperName();
					storage.Store(sat.gameObject);
					SetSatelliteType(type);
				}
			}

			public void TryDeploySatellite()
			{
				//Debug.Log(this.CanDeploySatellite() +" - " + this.HoldingSatellite());
				if (this.CanDeploySatellite() && this.HoldingSatellite())
				{
					Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
					var satellite = storage.FindFirst(Tags.LS_Satellite);

					int satType = this.SatelliteType();
					Debug.Log("Trying to deploy Satellite");
					Debug.Log("SAT TYPE: " + satType);
					//Debug.Log(satellite);
					storage.Remove(satellite.gameObject);
					storage.items.Remove(satellite.gameObject);
					GameObject.Destroy(satellite);
					//Debug.Log(satellite.IsNullOrDestroyed() + "SHOULD BE TRUE");

					SpawnSatellite(component.Location, ModAssets.SatelliteConfigurations[satType].GridID);
					sm.hasSatellite.Set(false, this);
				}
			}

			private void SpawnSatellite(AxialI location, string prefab)
			{
				Vector3 position = new Vector3(-1f, -1f, 0.0f);
				GameObject sat = Util.KInstantiate(Assets.GetPrefab((Tag)prefab), position);
				sat.name = Assets.GetPrefab((Tag)prefab).GetProperName();
				sat.GetComponent<ClusterDestinationSelector>().SetDestination(location);
				sat.GetComponent<ClusterGridEntity>().Location = location;
				sat.SetActive(true);
			}
			public int SatelliteType()
			{
				int type = -1;
				var satellite = storage.FindFirst(Tags.LS_Satellite);
				if (satellite != null && satellite.TryGetComponent<SatelliteTypeHolder>(out var typeHolder))
				{
					type = typeHolder.SatelliteType;
				}
				return type;
			}

			public void SetSatelliteType(int type)
			{
				var satellite = storage.FindFirst(Tags.LS_Satellite);
				if (satellite != null && satellite.TryGetComponent<SatelliteTypeHolder>(out var typeHolder))
				{
					typeHolder.SatelliteType = type;
				}
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
				craft = rocketModuleCluster.CraftInterface.m_clustercraft;
			}
		}

		public class States : GameStateMachine<States, StatesInstance, IStateMachineTarget>
		{
			[Serialize]
			public GameStateMachine<States, StatesInstance, IStateMachineTarget>.BoolParameter hasSatellite = new BoolParameter(false);

			public GroundedStates grounded;
			public NotGroundedStates not_grounded;
			public override void InitializeStates(out BaseState defaultState)
			{
				defaultState = grounded;

				grounded
					.DefaultState(this.grounded.loaded)
					.Update((smi, dt) =>
					{
						hasSatellite.Set(smi.storage.Has(SatelliteKitConfig.ID), smi);
					})
					.TagTransition(GameTags.RocketNotOnGround, this.not_grounded);

				grounded.loaded
					.ParamTransition<bool>(this.hasSatellite, this.grounded.empty, IsFalse)
					.PlayAnim("satelite_build", KAnim.PlayMode.Loop);
				grounded.empty
					.PlayAnim("satelite_construction", KAnim.PlayMode.Loop)
					.ParamTransition<bool>(this.hasSatellite, this.grounded.loaded, IsTrue);


				not_grounded.DefaultState(this.not_grounded.loaded)
					.Update((smi, dt) =>
					{
						hasSatellite.Set(smi.storage.Has(SatelliteKitConfig.ID), smi);
					})
					.TagTransition(GameTags.RocketNotOnGround, this.grounded, true);

				not_grounded.loaded
					.PlayAnim("satelite_build", KAnim.PlayMode.Loop)
					.ParamTransition<bool>(this.hasSatellite, this.not_grounded.empty, IsFalse);
				not_grounded.empty
					.PlayAnim("satelite_missing", KAnim.PlayMode.Loop)
					.ParamTransition<bool>(this.hasSatellite, this.not_grounded.loaded, IsTrue);
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
