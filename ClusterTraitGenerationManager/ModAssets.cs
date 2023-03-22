using HarmonyLib;
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
        public static GameObject TraitPopup;
        public static void LoadAssets()
        {
            var bundle = AssetUtils.LoadAssetBundle("clustergenerationsettingsmanager_menuassets");
            var Assets = bundle.LoadAsset<GameObject>("Assets/CGMExport.prefab"); 

            //UIUtils.ListAllChildren(Assets.transform);

            var TMPConverter = new TMPConverter();
            TMPConverter.ReplaceAllText(Assets);

            CustomPlanetSideScreen = Assets.transform.Find("IndividualSettings").gameObject;
            TraitPopup = Assets.transform.Find("TraitPopup").gameObject;
            //var TMPConverter = new TMPConverter();
            //TMPConverter.ReplaceAllText(wallSidescreenPrefab);
            //TMPConverter.ReplaceAllText(settingsDialogPrefab);
        }
    }
}
