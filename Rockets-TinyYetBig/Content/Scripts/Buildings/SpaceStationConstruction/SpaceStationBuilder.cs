using KSerialization;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction
{
	class SpaceStationBuilder : KMonoBehaviour, ISim1000ms
	{
		[MyCmpGet] RocketModuleCluster rocketModuleCluster;
		[MyCmpGet] SpaceStationAttachablePartStorage storage;

		[Serialize]
		public StoredStationPart? CurrentProject = null;
		//[Serialize]
		//SpaceConstructable CurrentSite = null;


		public void Sim1000ms(float dt)
		{
			//if (HasProject)
			//{
			//	if (CurrentProject.Value.IsConstructionProcess)
			//		ProgressConstruction(dt);
			//	else
			//		ProgressDeconstruction(dt);
			//}
		}
		//private void ProgressDeconstruction(float dt)
		//{
		//	var project = CurrentProject.Value;
		//	if (project.CurrentConstructionTime >= 0)
		//	{
		//		project.CurrentConstructionTime += dt;
		//		if (project.CurrentConstructionTime >= project.TotalConstructionTime)
		//		{
		//			CurrentSite.FinishConstruction(project);
		//			CurrentSite.PutInConstructionStorage(project, storage);
		//			CurrentProject = null;
		//		}
		//	}
		//}
		//private void ProgressConstruction(float dt)
		//{
		//	var project = CurrentProject.Value;
		//	if (project.CurrentConstructionTime >= 0)
		//	{
		//		project.CurrentConstructionTime += dt;
		//		if (project.CurrentConstructionTime >= project.TotalConstructionTime)
		//		{
		//			CurrentSite.FinishConstruction(project);
		//			CurrentSite.PutInConstructionStorage(project, storage);
		//			CurrentProject = null;
		//		}
		//	}
		//}
		public void FindSite()
		{
			if (rocketModuleCluster.CraftInterface.TryGetComponent(out ClusterGridEntity entity))
			{
				var entities = ClusterGrid.Instance.GetEntitiesOfLayerAtCell(entity.Location, EntityLayer.POI);

				foreach (var poi in entities)
				{
					//if (poi.TryGetComponent(out SpaceConstructable spaceConstructable))
					//{
					//	CurrentSite = spaceConstructable;
					//	return;
					//}
				}
			}
		}

		bool HasProject => CurrentProject != null;// && CurrentSite != null;
				
		//public void ResetConstruction(object data)
		//{
		//	if (HasProject)
		//	{
		//		var project = CurrentProject.Value;
		//		if (project.IsConstructionProcess)
		//			CurrentSite.CancelConstruction(project);
		//		else
		//			CurrentSite.CancelDeconstruction(project);

		//		CurrentProject = null;
		//	}
		//}

		//public override void OnSpawn()
		//{
		//	base.OnSpawn();
		//	this.GetComponent<RocketModuleCluster>().CraftInterface.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.ResetConstruction));
		//	this.GetComponent<RocketModuleCluster>().CraftInterface.gameObject.Subscribe((int)GameHashes.ClusterDestinationChanged, new System.Action<object>(this.ResetConstruction));
		//}
		//public override void OnCleanUp()
		//{
		//	this.GetComponent<RocketModuleCluster>().CraftInterface.gameObject.Unsubscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.ResetConstruction));
		//	this.GetComponent<RocketModuleCluster>().CraftInterface.gameObject.Unsubscribe((int)GameHashes.ClusterDestinationChanged, new System.Action<object>(this.ResetConstruction));
		//	base.OnCleanUp();
		//}
	}
}
