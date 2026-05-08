using Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings
{
	internal abstract class AttachableBuildingUpgrade : KMonoBehaviour
	{
		[MyCmpReq] protected AttachableBuilding attachableBuilding;
		public override void OnSpawn()
		{
			base.OnSpawn();
			OnConstructionFinished();
			Destroy(gameObject);
		}
		public abstract void OnConstructionFinished();
	}
}
