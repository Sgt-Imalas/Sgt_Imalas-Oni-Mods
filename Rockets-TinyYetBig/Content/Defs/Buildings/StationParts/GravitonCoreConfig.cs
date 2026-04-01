using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.StationParts
{
	internal class GravitonCoreConfig : IBuildingConfig
	{
		public static readonly string ID = "RTB_GravitonCore";
		public override BuildingDef CreateBuildingDef() => SpaceStationPartBase.CreateStationPartDef(ID, MATERIALS.REFINED_METALS, [1000]);

		public override void DoPostConfigureComplete(GameObject go) => SpaceStationPartBase.DoPostConfigureComplete(go);
	}
}
