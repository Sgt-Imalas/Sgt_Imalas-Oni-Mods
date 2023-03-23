using Rockets_TinyYetBig.Buildings.CargoBays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
    internal class FridgeModuleHatchGrabber : KMonoBehaviour, ISim1000ms
    {
        [MyCmpGet]
        Storage offlineStorage;
        [MyCmpGet]
        Operational operational;
        [MyCmpGet]
        TreeFilterable filter;


        CraftModuleInterface craftModuleInterface;

        private static readonly EventSystem.IntraObjectHandler<FridgeModuleHatchGrabber> OnRocketModulesChangend = new EventSystem.IntraObjectHandler<FridgeModuleHatchGrabber>((System.Action<FridgeModuleHatchGrabber, object>)((component, data) => component.UpdateModules(data)));


        List<Storage> ConnectedFridgeModules = new List<Storage>();

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.GetMyWorld().TryGetComponent<CraftModuleInterface>(out craftModuleInterface);
            SgtLogger.Assert("craftModuleInterface", craftModuleInterface);
            //SgtLogger.l("AAAA");

            craftModuleInterface.gameObject.Subscribe((int)GameHashes.RocketModuleChanged, UpdateModules);
            UpdateModules(null);
        }
        private List<int> refreshHandle = new List<int>();
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();

        }
        public override void OnCleanUp()
        {
            craftModuleInterface.gameObject.Unsubscribe((int)GameHashes.RocketModuleChanged, UpdateModules);
            base.OnCleanUp();
        }

        public void Sim1000ms(float dt)
        {
            float neededStorage = offlineStorage.RemainingCapacity();
            if (neededStorage < 0.001|| !operational.IsOperational) return;
            var filterArray = filter.acceptedTagSet.ToArray();

            //SgtLogger.l("needed: "+ neededStorage.ToString());
            if (ConnectedFridgeModules.Count > 0)
            {
                foreach(var module in ConnectedFridgeModules)
                {
                    //SgtLogger.l("modul: " + module.ToString());
                    if (module.MassStored() > 0.01)
                    {
                        foreach(var item in module.items)
                        {
                            //SgtLogger.l("item: " + item.ToString());
                            if (item.HasAnyTags(filterArray))
                            {
                                if(item.TryGetComponent<Pickupable>(out var pickupable))
                                {
                                    //SgtLogger.l("item2: " + pickupable.ToString());
                                    var TakenPickup = pickupable.Take(neededStorage);
                                    neededStorage -= TakenPickup.TotalAmount;
                                    offlineStorage.Store(TakenPickup.gameObject);
                                }
                                if (neededStorage < 0.001f)
                                    break;
                            }
                        }
                    }
                    if (neededStorage < 0.001f)
                        break;
                }
            }
        }

        void UpdateModules(object o)
        {
            SgtLogger.l("Redoing Referenced Modules");
            ConnectedFridgeModules.Clear();

            foreach(var module in craftModuleInterface.ClusterModules)
            {
                if(module.Get().TryGetComponent<FridgeModule>(out var fridgeModule))
                {
                    //SgtLogger.l("found fridge");
                    ConnectedFridgeModules.Add(fridgeModule.fridgeStorage);
                }
            }
        }

    }
}
