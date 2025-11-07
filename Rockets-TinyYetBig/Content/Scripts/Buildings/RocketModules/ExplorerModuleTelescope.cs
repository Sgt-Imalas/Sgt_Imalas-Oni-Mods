using KSerialization;
using System;
using UnityEngine;

namespace Rockets_TinyYetBig.Behaviours
{
	public class ExplorerModuleTelescope :
	GameStateMachine<ExplorerModuleTelescope, ExplorerModuleTelescope.Instance, IStateMachineTarget, ExplorerModuleTelescope.Def>
	{
		public class WorkingStates : State
		{
			public State working;
			public State all_work_complete;

		}
		public WorkingStates workingStates;
		public State grounded;
		private AxialI currentTarget;
		public override void InitializeStates(out BaseState default_state)
		{

			default_state = (BaseState)this.grounded;
			this.grounded
				//.Enter((smi) => SgtLogger.l("ENTERED STATE: grounded", "ExplorerSMI"))
				.Enter(smi => smi.DropStoredDatabanks())
				.Enter((smi) => smi.DestroyTelescope())
				.TagTransition(GameTags.RocketNotOnGround, workingStates);


			this.workingStates
				.DefaultState(this.workingStates.working)
				//.Enter((smi) => SgtLogger.l("ENTERED STATE: workingStates", "ExplorerSMI"))
				.TagTransition(GameTags.RocketNotOnGround, grounded, true);

			this.workingStates.working
				//.Enter((smi) => SgtLogger.l("ENTERED STATE: workingStates.working", "ExplorerSMI"))
				.EventTransition(GameHashes.ClusterFogOfWarRevealed,
				(smi => Game.Instance),
				this.workingStates.all_work_complete,
				(smi => !smi.CheckHasAnalyzeTarget()))
				.Update((smi, dt) =>
				{
					smi.ScanSpaceLocation(dt);
				});
			this.workingStates.all_work_complete
				//.Enter((smi) => SgtLogger.l("ENTERED STATE: all_work_complete", "ExplorerSMI"))

				.Enter((smi) => smi.DestroyTelescope())
				.UpdateTransition(workingStates.working, (smi, dt) => smi.CheckHasAnalyzeTarget(), update_rate: UpdateRate.SIM_1000ms)
				//.Enter((smi) => smi.CheckHasAnalyzeTarget())

				.EventTransition(GameHashes.ClusterLocationChanged, (smi => Game.Instance), this.workingStates, (smi => smi.CheckHasAnalyzeTarget()))
				.EventTransition(GameHashes.ClusterFogOfWarRevealed,
				(smi => Game.Instance),
				this.workingStates,
				(smi => smi.CheckHasAnalyzeTarget()));


		}

		public class Def : BaseDef
		{
			public int analyzeClusterRadius = Config.Instance.ScannerModuleRangeRadius;
		}
		public new class Instance :
		  GameInstance, IHexCellCollector
		{
			[Serialize]
			private bool m_hasAnalyzeTarget;
			[Serialize]
			public AxialI m_analyzeTarget;

			Storage databankStorage;
			private GameObject telescopeTargetMarker;

			internal void ScanSpaceLocation(float dt)
			{
				float detectionIncrease = dt * TUNING.ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL / Config.Instance.ScannerModuleScanSpeed / 600f;

				if (!ClusterGrid.Instance.GetEntityOfLayerAtCell(m_analyzeTarget, EntityLayer.Telescope))
				{
					this.telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab("TelescopeTarget"), Grid.SceneLayer.Background);
					this.telescopeTargetMarker.SetActive(true);
					this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(m_analyzeTarget);
				}
				if (smi.m_fowManager.EarnRevealPointsForLocation(m_analyzeTarget, detectionIncrease))
				{
					Game.Instance.BoxingTrigger((int)GameHashes.ClusterFogOfWarRevealed, m_analyzeTarget);
					DestroyTelescope();
					SpawnDatabanks(3);
				}
			}
			internal void SpawnDatabanks(int count)
			{
				if (databankStorage.IsFull())
					return;

				for (int i = 0; i < count; i++)
				{
					var db = Util.KInstantiate(Assets.GetPrefab(DatabankHelper.ID), new(-1, -1), Quaternion.identity, gameLayer: 23);					
					db.SetActive(true);
					databankStorage.Store(db, true);
				}
			}
			internal void DropStoredDatabanks()
			{
				databankStorage.DropAll();
			}
			public void DestroyTelescope()
			{
				if (!telescopeTargetMarker.IsNullOrDestroyed())
					Util.KDestroyGameObject(telescopeTargetMarker);
			}
			public ClusterFogOfWarManager.Instance m_fowManager;

			public Instance(IStateMachineTarget smi, Def def) : base(smi, def)
			{
				m_fowManager = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
			}

			public override void StartSM()
			{
				databankStorage = GetComponent<Storage>();
				base.StartSM();
			}
			public override void OnCleanUp()
			{
				DestroyTelescope();
				base.OnCleanUp();
			}
			public bool Grounded()
			{
				var clusterCraft = this.GetComponent<RocketModuleCluster>().CraftInterface.m_clustercraft;
				if (clusterCraft != null)
				{
					return !(clusterCraft.status == Clustercraft.CraftStatus.InFlight);
				}
				return false;
			}

			public bool CheckHasAnalyzeTarget()
			{
				if (this.def.analyzeClusterRadius <= 0) return false;


				ClusterFogOfWarManager.Instance smi = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();

				AxialI myWorldLocation = this.GetAxialLocation();
				if (this.m_hasAnalyzeTarget && !smi.IsLocationRevealed(this.m_analyzeTarget))
				{
					if (ClusterGrid.Instance.IsInRange(myWorldLocation, m_analyzeTarget, Config.Instance.ScannerModuleRangeRadius))
					{
						return true;
					}
					else
					{
						DestroyTelescope();
					}
				}
				this.m_hasAnalyzeTarget = smi.GetUnrevealedLocationWithinRadius(myWorldLocation, this.def.analyzeClusterRadius, out this.m_analyzeTarget);
				return this.m_hasAnalyzeTarget;
			}
			public AxialI GetAxialLocation()
			{
				this.gameObject.TryGetComponent<RocketModuleCluster>(out var module);
				return module.CraftInterface.m_clustercraft.Location;
			}
			public AxialI GetAnalyzeTarget()
			{
				Debug.Assert(this.m_hasAnalyzeTarget, "GetAnalyzeTarget called but this telescope has no target assigned.");
				return this.m_analyzeTarget;
			}

			public bool CheckIsCollecting() => m_hasAnalyzeTarget;

			public string GetProperName() => this.GetComponent<RocketModuleCluster>().GetProperName();

			public Sprite GetUISprite() => global::Def.GetUISprite( this.master.gameObject.GetComponent<KPrefabID>().PrefabID()).first;

			public float GetCapacity() => databankStorage.Capacity();

			public float GetMassStored() => databankStorage.MassStored();

			/// screen takes 4 seconds to completely fill the progress bar
			public float TimeInState() => m_fowManager.GetRevealCompleteFraction(smi.m_analyzeTarget) * 4f;

			public string GetCapacityBarText() => $"{GameUtil.GetFormattedUnits(this.GetMassStored())} / {GameUtil.GetFormattedUnits(this.GetCapacity())}";
		}

	}
}
