using Rockets_TinyYetBig.Buildings.CargoBays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static EdiblesManager;
using static Rockets_TinyYetBig.STRINGS.BUILDING.STATUSITEMS;
using static STRINGS.INPUT_BINDINGS;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
    public class FridgeModuleHatchGrabber : KMonoBehaviour, ISim1000ms
    {
        [MyCmpGet]
        Storage offlineStorage;
        [MyCmpGet]
        Operational operational;
        [MyCmpGet]
        TreeFilterable filter;
        [MyCmpGet]
        KSelectable selectable;
        private FilteredStorage filteredStorage;

        public MeterController storage_meter;

        private Guid StatusItemHandle;

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
            StatusItemHandle = selectable.AddStatusItem( ModAssets.StatusItems.RTB_AccessHatchStorage, (object)this);
            ModAssets.FridgeModuleGrabbers.Add(this); 
            GetAllMassDesc();
            this.filteredStorage.FilterChanged(); 
            CreateMeter();
            UpdateMeter();
        }


        public void CreateMeter()
        {
            storage_meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter_all", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, null);

        }
        public void UpdateMeter()
        {
            float positionPercent = Mathf.Clamp01(currentCapacity / totalCapacity);
            SgtLogger.l("current: " + positionPercent);
            if (storage_meter != null)
            {
                SgtLogger.l("meter: " + positionPercent);
                storage_meter.SetPositionPercent(positionPercent);
            }
        }
        float currentCapacity = 0, totalCapacity = 0;

        private List<int> refreshHandle = new List<int>();
        public float maxPullCapacityKG = 1f;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit(); 
            this.filteredStorage = new FilteredStorage((KMonoBehaviour)this, (Tag[])null, (IUserControlledCapacity)null, false, Db.Get().ChoreTypes.StorageFetch);

        }
        public override void OnCleanUp()
        {
            ModAssets.FridgeModuleGrabbers.Remove(this);
            craftModuleInterface.gameObject.Unsubscribe((int)GameHashes.RocketModuleChanged, UpdateModules);
            base.OnCleanUp();
        }

        public void Sim1000ms(float dt)
        {

            float neededStorage = maxPullCapacityKG - offlineStorage.MassStored();
            if (neededStorage < 0.001 || !operational.IsOperational) return;
            var filterArray = filter.acceptedTagSet.ToArray();

            //SgtLogger.l("needed: "+ neededStorage.ToString());
            if (ConnectedFridgeModules.Count > 0)
            {
                foreach (var module in ConnectedFridgeModules)
                {
                    //SgtLogger.l("modul: " + module.ToString());
                    if (module.MassStored() > 0.01)
                    {
                        foreach (var item in module.items)
                        {
                            //SgtLogger.l("item: " + item.ToString());
                            if (item.HasAnyTags(filterArray))
                            {
                                if (item.TryGetComponent<Pickupable>(out var pickupable))
                                {
                                    //SgtLogger.l("item2: " + pickupable.ToString());
                                    var TakenPickup = pickupable.Take(neededStorage);
                                    neededStorage -= TakenPickup.TotalAmount;
                                    offlineStorage.Store(TakenPickup.gameObject, true, true);
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

            foreach (var module in craftModuleInterface.ClusterModules)
            {
                if (module.Get().TryGetComponent<FridgeModule>(out var fridgeModule))
                {
                    //SgtLogger.l("found fridge");
                    ConnectedFridgeModules.Add(fridgeModule.fridgeStorage);
                }
            }
            UpdateMeter();
        }



        public float TotalKCAL => _totalKCal;
        float _totalKCal = 0;
        public string GetAllMassDesc()
        {
            currentCapacity = 0;
            totalCapacity = 0;

            var totalKCalNew = 0f;
            string infoText = string.Empty;
            if (ConnectedFridgeModules.Count > 0)
            {
                foreach (var module in ConnectedFridgeModules)
                {
                    currentCapacity += module.MassStored();
                    totalCapacity += module.capacityKg;

                    //SgtLogger.l("modul: " + module.ToString());
                    if (module.MassStored() > 0.01)
                    {
                        foreach (var item in module.items)
                        {
                            //SgtLogger.l("item: " + item.ToString());
                            if (item.TryGetComponent<Pickupable>(out var pickupable) && item.TryGetComponent<Edible>(out var edible))
                            {
                                float thisFoodsMass = edible.foodInfo.CaloriesPerUnit * pickupable.TotalAmount / 1000f;
                                totalKCalNew += thisFoodsMass;
                                infoText += string.Format(RTB_FOODSTORAGESTATUS.FOODINFO, edible.foodInfo.ConsumableName, thisFoodsMass.ToString());
                            }

                        }
                    }
                }
            }
            if(Mathf.RoundToInt(totalKCalNew) != Mathf.RoundToInt(_totalKCal))
            {
                _totalKCal = (totalKCalNew);
            }
            UpdateMeter();
            return infoText;
        }
    }
}
