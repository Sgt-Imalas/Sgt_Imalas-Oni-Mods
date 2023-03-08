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
        static GameObject Screen=null;
        public static void InstantiateClusterSelectionView(System.Action onClose = null)
        {
            if(Screen == null)
            {
                LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.kleiInventoryScreen);
                LockerNavigator.Instance.PopScreen();
                
                var window = Util.KInstantiateUI(LockerNavigator.Instance.kleiInventoryScreen.gameObject);
                window.SetActive(false);
                var copy = window.transform;
                UnityEngine.Object.Destroy(window);
                var newScreen = Util.KInstantiateUI(copy.gameObject, Global.Instance.globalCanvas);
                newScreen.name = "ClusterSelectionView";
                newScreen.AddComponent(typeof(FeatureSelectionScreen));
                Screen = newScreen;
                LockerNavigator.Instance.PushScreen(newScreen,onClose);
            }
            else
            {
                LockerNavigator.Instance.PushScreen(Screen, onClose);
            }

        }
    }
}
