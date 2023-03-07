using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace ClusterTraitGenerationManager
{
    internal class CGSMClusterManager
    {
        public static void InstantiateClusterSelectionView()
        {
            var window = Util.KInstantiateUI(LockerNavigator.Instance.kleiInventoryScreen.gameObject);
            window.SetActive(false);
            var copy = window.transform;
            UnityEngine.Object.Destroy(window);
            var newScreen = Util.KInstantiateUI(copy.gameObject, Global.Instance.globalCanvas, true);
            newScreen.name = "ClusterSelectionView";
            newScreen.AddComponent(typeof(FeatureSelectionScreen));

            UIUtils.ListAllChildren(newScreen.transform);
            LockerNavigator.Instance.PushScreen(newScreen);
        }
    }
}
