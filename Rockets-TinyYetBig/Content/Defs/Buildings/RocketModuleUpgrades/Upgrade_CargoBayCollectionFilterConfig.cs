using Rockets_TinyYetBig.Content.ModDb;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.RocketModuleUpgrades
{
	internal class Upgrade_CargoBayCollectionFilterConfig : IBuildingConfig
	{
		public override BuildingDef CreateBuildingDef() => RocketModuleUpgradeBase.CreateRocketModuleUpgrade(ModuleUpgradeDatabase.CargoBayFilter, "gas_germs_sensor_kanim");

		public override void ConfigureBuildingTemplate(GameObject go, Tag tag) => RocketModuleUpgradeBase.ConfigureBuildingTemplate(go, tag);
		public override void DoPostConfigureComplete(GameObject go) => RocketModuleUpgradeBase.DoPostConfigureComplete(go);
	}
}
