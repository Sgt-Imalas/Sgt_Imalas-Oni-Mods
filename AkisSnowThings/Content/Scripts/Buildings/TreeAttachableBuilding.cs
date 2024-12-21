using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisSnowThings.Content.Scripts.Buildings
{
	internal class TreeAttachableBuilding:KMonoBehaviour
	{


		[MyCmpReq]
		private Building building;
		[MyCmpReq]
		BuildingAttachPoint buildingAttachPoint;
		public override void OnSpawn()
		{
			base.OnSpawn();
			ToggleConnectedPlantHarvest(false);
		}

		private void ToggleConnectedPlantHarvest(bool enabled)
		{
			var pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(building.GetExtents(), GameScenePartitioner.Instance.plants, pooledList);

			foreach (var scenePartitionerEntry in pooledList)
			{
				if (scenePartitionerEntry.obj is KMonoBehaviour kMonoBehaviour && kMonoBehaviour.TryGetComponent(out Harvestable harvestable))
				{
					harvestable.harvestDesignatable.harvestWhenReady = enabled;
					harvestable.SetCanBeHarvested(enabled);
					if(!enabled)
						harvestable.ForceCancelHarvest();
				}
			}
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			ToggleConnectedPlantHarvest(true);
		}
	}
}
