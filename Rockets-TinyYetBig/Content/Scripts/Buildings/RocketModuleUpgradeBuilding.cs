using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings
{
	internal class RocketModuleUpgradeBuilding : AttachableBuildingUpgrade
	{
		[MyCmpReq] Deconstructable deconstructable;
		public override void OnConstructionFinished()
		{
			if (attachableBuilding == null)
				return;
			var rocketModule = RocketAttachableSocket.Get(attachableBuilding);
			if (rocketModule == null || rocketModule.gameObject == null || !rocketModule.TryGetComponent<RocketModuleUpgradeStorage>(out var moduleUpgradeStorage))
			{
				SgtLogger.error("no SpaceStationAttachablePartStorage on attached " + rocketModule.name);
				return;
			}
			moduleUpgradeStorage.AddUpgrade(RocketModuleUpgradeInstance.Create(gameObject));
		}
	}
}
