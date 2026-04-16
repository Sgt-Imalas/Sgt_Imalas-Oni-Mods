using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction
{
	internal class SelfStoringPart : KMonoBehaviour
	{
		[MyCmpReq] AttachableBuilding attachableBuilding;
		[MyCmpReq] BuildingComplete building;
		[MyCmpReq] Deconstructable deconstructable;
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (attachableBuilding == null)
				return;
			var stationBuilder = attachableBuilding.GetAttachedTo();
			if(stationBuilder == null || stationBuilder.gameObject == null || !stationBuilder.TryGetComponent<SpaceStationAttachablePartStorage>(out var partStorage))
			{
				SgtLogger.error("no SpaceStationAttachablePartStorage on attached " + stationBuilder.name);
				return; 
			}
			partStorage.StoreNewPart(StoredStationPart.Create(gameObject));
			Destroy(gameObject);
		}
	}
}
