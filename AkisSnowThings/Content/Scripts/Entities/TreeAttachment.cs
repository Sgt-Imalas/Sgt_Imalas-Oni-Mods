using AkisSnowThings.Content.Scripts.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisSnowThings.Content.Scripts.Entities
{
	public class TreeAttachment : KMonoBehaviour
	{
		[MyCmpReq]
		BuildingAttachPoint buildingAttachPoint;

		[MyCmpReq]
		OccupyArea occupyArea;	

		Growing growth;
		public override void OnSpawn()
		{
			base.OnSpawn();
			growth = this.GetComponent<Growing>();
		}

		public bool CanAttachToTree()
		{
			return growth == null || growth.IsGrown();
		}
		public override void OnCleanUp()
		{
			DeconstructAttachedDecor();
			base.OnCleanUp();
		}

		private void DeconstructAttachedDecor()
		{
			var pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			foreach (var attachmentSlot in buildingAttachPoint.points)
			{
				AttachableBuilding attachedBuilding = attachmentSlot.attachedBuilding;
				if (attachedBuilding != null && attachedBuilding.TryGetComponent<Deconstructable>(out var deconstructable))
				{
					deconstructable.ForceDestroyAndGetMaterials();
				}
			}
		}
	}
}
