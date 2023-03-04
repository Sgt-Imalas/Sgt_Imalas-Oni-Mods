using Rockets_TinyYetBig.Buildings.CargoBays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
    internal class FridgeModuleHatchGrabber : KMonoBehaviour
    {
        [MyCmpGet]
        RocketModuleCluster clusterModule;


        List<Storage> ConnectedFridgeModules = new List<Storage>();

        public override void OnSpawn()
        {
            base.OnSpawn();
            UpdateModules();


        }
        void UpdateModules()
        {
            ConnectedFridgeModules.Clear();

            foreach(var module in clusterModule.CraftInterface.ClusterModules)
            {
                if(module.Get().TryGetComponent<FridgeModule>(out var fridgeModule))
                {
                    ConnectedFridgeModules.Add(fridgeModule.fridgeStorage);
                }
            }
        }

    }
}
