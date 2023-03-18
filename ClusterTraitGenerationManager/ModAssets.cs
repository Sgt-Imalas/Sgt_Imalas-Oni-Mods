using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.CREATURES.STATS;

namespace ClusterTraitGenerationManager
{
    internal class ModAssets
    {
        Transform Screen;
        public static GameObject CustomPlanetSideScreen;
        public static void LoadAssets()
        {
            var bundle = AssetUtils.LoadAssetBundle("clustergenerationsettingsmanager_menuassets");
            CustomPlanetSideScreen = bundle.LoadAsset<GameObject>("Assets/Controls/Layout/CGMExpor.prefab");

            var TMPConverter = new TMPConverter();
            TMPConverter.ReplaceAllText(CustomPlanetSideScreen);
            //var TMPConverter = new TMPConverter();
            //TMPConverter.ReplaceAllText(wallSidescreenPrefab);
            //TMPConverter.ReplaceAllText(settingsDialogPrefab);
        }
    }
}
