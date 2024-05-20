using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2
{
    internal class ModAssets
    {
        public static GameObject BlueprintSelectionScreen;
        public static void LoadAssets()
        {
            var bundle = AssetUtils.LoadAssetBundle("blueprints_ui", platformSpecific: true);
            BlueprintSelectionScreen = bundle.LoadAsset<GameObject>("Assets/UIs/blueprintSelector.prefab");

            //UIUtils.ListAllChildren(Assets.transform);

            var TMPConverter = new TMPConverter();
            TMPConverter.ReplaceAllText(BlueprintSelectionScreen);
        }
    }
}
