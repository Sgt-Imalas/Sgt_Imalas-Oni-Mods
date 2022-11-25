using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class GasFuelLoaderConfig : IBuildingConfig
    {
        public const string ID = "RTB_GasFuelLoader";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef() => BaseFuelLoader.CreateBaseFuelLoaderPort(ID, "conduit_port_gas_loader_kanim", ConduitType.Gas, width: 1,height: 2);

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => BaseFuelLoader.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Gas, 10f, FuelLoaderComponent.LoaderType.Fuel);
        public override void DoPostConfigureComplete(GameObject go) 
        { 
            BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go, true); 
        }
    }
}
