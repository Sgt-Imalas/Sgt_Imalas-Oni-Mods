using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class LiquidOxidizerLoaderConfig : IBuildingConfig
    {
        public const string ID = "RTB_LiquidOxidizerLoader";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef() => BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort(ID, "conduit_port_liquid_loader_kanim", ConduitType.Liquid, true, width: 1,height: 2);
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) 
        { 
            BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Liquid, 10f, true);
            go.AddOrGet<FuelLoaderComponent>().loaderType = FuelLoaderComponent.LoaderType.LiquidOx;
        }
        public override void DoPostConfigureComplete(GameObject go) 
        { 
            BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go, true); 
        }
    }
}
