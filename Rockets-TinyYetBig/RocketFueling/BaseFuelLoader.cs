using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ResearchTypes;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class BaseFuelLoader
    {
        public static BuildingDef CreateBaseFuelLoaderPort(string id, string anim, ConduitType conduitType, int width = 1, int height = 2)
        {
            BuildingDef buildingDef = BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort(id, anim, conduitType, true, width, height);
            buildingDef.PowerInputOffset = new CellOffset(0, 0);

            return buildingDef;
        }
        public static void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag, ConduitType conduitType, float storageSize, FuelLoaderComponent.LoaderType loaderType)
        {
            BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, conduitType, storageSize, true);
            go.AddOrGet<FuelLoaderComponent>().loaderType = loaderType;
        }
    }
}
