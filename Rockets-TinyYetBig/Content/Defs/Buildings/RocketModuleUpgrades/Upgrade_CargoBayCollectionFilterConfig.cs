using Rockets_TinyYetBig.Content.Defs.Buildings.StationParts;
using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ResearchTypes;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.RocketModuleUpgrades
{
	internal class Upgrade_CargoBayCollectionFilterConfig : IBuildingConfig
	{
		public override BuildingDef CreateBuildingDef() => RocketModuleUpgradeBase.CreateRocketModuleUpgrade(ModuleUpgradeDatabase.CargoBayFilter, "gas_germs_sensor_kanim");

		public override void ConfigureBuildingTemplate(GameObject go, Tag tag) => RocketModuleUpgradeBase.ConfigureBuildingTemplate(go, tag);
		public override void DoPostConfigureComplete(GameObject go) => RocketModuleUpgradeBase.DoPostConfigureComplete(go);
	}
}
