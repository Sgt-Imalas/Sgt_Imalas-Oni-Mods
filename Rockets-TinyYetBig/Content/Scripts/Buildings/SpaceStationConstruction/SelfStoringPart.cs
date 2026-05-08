using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction
{
	internal class SelfStoringPart : AttachableBuildingUpgrade
	{
		public override void OnConstructionFinished()
		{
			if (attachableBuilding == null)
				return;
			var stationBuilder = attachableBuilding.GetAttachedTo();
			if (stationBuilder == null || stationBuilder.gameObject == null || !stationBuilder.TryGetComponent<SpaceStationAttachablePartStorage>(out var partStorage))
			{
				SgtLogger.error("no SpaceStationAttachablePartStorage on attached " + stationBuilder.name);
				return;
			}
			partStorage.StoreNewPart(StoredStationPart.Create(gameObject));
		}
	}
}
