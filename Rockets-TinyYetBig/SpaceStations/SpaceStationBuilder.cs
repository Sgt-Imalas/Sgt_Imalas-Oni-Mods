using KSerialization;
using Rockets_TinyYetBig.SpaceStations.Construction;

namespace Rockets_TinyYetBig.SpaceStations
{
	class SpaceStationBuilder : KMonoBehaviour, ISim1000ms//, ISidescreenButtonControl
	{
		[MyCmpGet] RocketModuleCluster rocketModuleCluster;
		[MyCmpGet] Storage storage;

		[Serialize]
		PartProject CurrentProject = null;
		[Serialize]
		SpaceConstructable CurrentSite = null;


		public bool ConstructionTimes(out bool constructionProcess, out float remainingTime)
		{
			constructionProcess = true;
			remainingTime = -1;
			if (HasProject)
			{
				constructionProcess = CurrentProject.IsConstructionProcess;
				remainingTime = CurrentProject.TotalConstructionTime - CurrentProject.CurrentConstructionTime;
				return true;
			}
			return false;
		}

		public bool HasResources(PartProject project)
		{
			if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
			{
				return true;
			}
			//var cmi = rocketModuleCluster.CraftInterface;
			//if (!cmi.HasCargoModule)
			//    return false;

			//var CargoBays = ListPool<CargoBayCluster, SpaceStationBuilder>.Allocate();
			//foreach (var clusterModule in cmi.ClusterModules)
			//{
			//    if (clusterModule.Get().TryGetComponent<CargoBayCluster>(out var cargoBay))
			//    {
			//        CargoBays.Add(cargoBay);
			//    }
			//}
			//CargoBays.Dispose();
			return storage.FindFirstWithMass(project.ResourceTag, project.ResourceAmountMass) != null;  //TODO!

		}

		public void Sim1000ms(float dt)
		{
			if (HasProject)
			{
				if (CurrentProject.IsConstructionProcess)
					ProgressConstruction(dt);
				else
					ProgressDeconstruction(dt);
			}
		}
		private void ProgressDeconstruction(float dt)
		{
			if (CurrentProject.CurrentConstructionTime >= 0)
			{
				CurrentProject.CurrentConstructionTime += dt;
				if (CurrentProject.CurrentConstructionTime >= CurrentProject.TotalConstructionTime)
				{
					CurrentSite.FinishConstruction(CurrentProject);
					CurrentSite.PutInConstructionStorage(CurrentProject, storage);
					CurrentProject = null;
				}
			}
		}
		private void ProgressConstruction(float dt)
		{
			if (CurrentProject.CurrentConstructionTime >= 0)
			{
				CurrentProject.CurrentConstructionTime += dt;
				if (CurrentProject.CurrentConstructionTime >= CurrentProject.TotalConstructionTime)
				{
					CurrentSite.FinishConstruction(CurrentProject);
					CurrentSite.PutInConstructionStorage(CurrentProject, storage);
					CurrentProject = null;
				}
			}
		}
		public void FindSite()
		{
			if (rocketModuleCluster.CraftInterface.TryGetComponent<ClusterGridEntity>(out ClusterGridEntity entity))
			{
				var entities = ClusterGrid.Instance.GetEntitiesOfLayerAtCell(entity.Location, EntityLayer.POI);

				foreach (var poi in entities)
				{
					if (poi.TryGetComponent<SpaceConstructable>(out SpaceConstructable spaceConstructable))
					{
						CurrentSite = spaceConstructable;
						return;
					}
				}
			}
		}

		bool HasProject => CurrentProject != null && CurrentSite != null;

		public void ResetConstruction(object data)
		{
			if (HasProject)
			{
				if (CurrentProject.IsConstructionProcess)
					CurrentSite.CancelConstruction(CurrentProject);
				else
					CurrentSite.CancelDeconstruction(CurrentProject);

				CurrentProject = null;
			}
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.GetComponent<RocketModuleCluster>().CraftInterface.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.ResetConstruction));
			this.GetComponent<RocketModuleCluster>().CraftInterface.gameObject.Subscribe((int)GameHashes.ClusterDestinationChanged, new System.Action<object>(this.ResetConstruction));
		}
		public override void OnCleanUp()
		{
			this.GetComponent<RocketModuleCluster>().CraftInterface.gameObject.Unsubscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.ResetConstruction));
			this.GetComponent<RocketModuleCluster>().CraftInterface.gameObject.Unsubscribe((int)GameHashes.ClusterDestinationChanged, new System.Action<object>(this.ResetConstruction));
			base.OnCleanUp();
		}

	}
}
