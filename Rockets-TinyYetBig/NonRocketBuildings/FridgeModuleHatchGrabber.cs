using PeterHan.PLib.Core;
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

        static readonly EventSystem.IntraObjectHandler<FridgeModuleHatchGrabber> OnRocketModulesChangend = new EventSystem.IntraObjectHandler<FridgeModuleHatchGrabber>((System.Action<FridgeModuleHatchGrabber, object>)((component, data) => component.UpdateModules(data)));


        List<CargoBayCluster> ConnectedFridgeModules = new List<CargoBayCluster>();

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.GetMyWorld().TryGetComponent<CraftModuleInterface>(out craftModuleInterface);
            SgtLogger.Assert("craftModuleInterface", craftModuleInterface);
            //SgtLogger.l("AAAA");

            craftModuleInterface.gameObject.Subscribe((int)GameHashes.RocketModuleChanged, UpdateModules);
            UpdateModules(null);
            StatusItemHandle = selectable.AddStatusItem(ModAssets.StatusItems.RTB_AccessHatchStorage, (object)this);
            ModAssets.FridgeModuleGrabbers.Add(this);
            //GetAllMassDesc();
            //this.filteredStorage.FilterChanged();
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
            if (storage_meter != null)
            {
                storage_meter.SetPositionPercent(positionPercent);
            }
        }
        float currentCapacity = 0, totalCapacity = 0;

        private List<int> refreshHandle = new List<int>();
        public float maxPullCapacityKG = 1f;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            //this.filteredStorage = new FilteredStorage((KMonoBehaviour)this, new Tag[1]
            //{
            //    GameTags.Compostable
            //}, (IUserControlledCapacity)null, true, Db.Get().ChoreTypes.StorageFetch);

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


            if (!operational.IsOperational || operational.Flags.ContainsKey(RocketUsageRestriction.rocketUsageAllowed) && operational.GetFlag(RocketUsageRestriction.rocketUsageAllowed) == false)
                return;
            var filterArray = filter.acceptedTagSet.ToArray();

            currentCapacity = 0;
            totalCapacity = 0;

            //SgtLogger.l("needed: "+ neededStorage.ToString());
            if (ConnectedFridgeModules.Count > 0)
            {

                foreach (var module in ConnectedFridgeModules)
                {
                    currentCapacity += module.storage.MassStored();
                    totalCapacity += module.userMaxCapacity;
                }


                if (neededStorage > 0)
                {
                    foreach (var module in ConnectedFridgeModules)
                    {
                        if (module.storage.MassStored() > 0)
                        {
                            for (int i = module.storage.items.Count - 1; i >= 0; i--)
                            {
                                var item = module.storage.items[i];
                                if (item.TryGetComponent(out KPrefabID prefab ) && prefab.HasAnyTags(filterArray))
                                {
                                    if (item.TryGetComponent<Pickupable>(out var pickupable))
                                    {
                                        //SgtLogger.l("item2: " + pickupable.ToString());
                                        var TakenPickup = pickupable.Take(neededStorage);

                                        if(TakenPickup != null)
                                        {
                                            neededStorage -= TakenPickup.TotalAmount;
                                            offlineStorage.Store(TakenPickup.gameObject, true, false);
                                        }
                                    }
                                    if (neededStorage <= 0)
                                        break;
                                }
                            }
                        }
                        if (neededStorage <= 0)
                            break;
                    }
                }

                return;
                //else 
                //    if (neededStorage < 0)
                //{
                //    //push some back into module REDO THIS, ITS SCUFFED AF!
                //    neededStorage *= -1;

                //    for (int i = offlineStorage.items.Count - 1; i >= 0; i--)
                //    {
                //        var itemToPutInStorage = offlineStorage.items[i];
                //        foreach (var module in ConnectedFridgeModules)
                //        {
                //            float remainingCapacity = module.storage.RemainingCapacity();

                //            if (remainingCapacity <= 0)
                //                continue;
                //            if (itemToPutInStorage.HasAnyTags(module.storage.storageFilters.ToArray()))
                //            {
                //                if (itemToPutInStorage.TryGetComponent<Pickupable>(out var pickupable))
                //                {
                //                    var currentAmount = pickupable.TotalAmount;

                //                    float amount = Mathf.Min(remainingCapacity, neededStorage);

                //                    if(amount == 0)
                //                        continue;

                //                    //SgtLogger.l($"{amount}, {remainingCapacity}, {neededStorage} -> {pickupable}");
                //                    var TakenPickup = pickupable.Take(amount);

                //                    if (TakenPickup != null)
                //                    {
                //                        remainingCapacity -= TakenPickup.TotalAmount;
                //                        neededStorage -= TakenPickup.TotalAmount;
                //                        module.storage.Store(TakenPickup.gameObject, true, false);
                //                    }
                //                    if (currentAmount <= amount)
                //                    {
                //                        break;
                //                    }

                //                }

                //                if (neededStorage <= 0)
                //                    break;
                //            }
                //        }
                //    }
                //}

            }

        }

        void UpdateModules(object o)
        {
            SgtLogger.l("Redoing Referenced Modules");
            ConnectedFridgeModules.Clear();

            foreach (var module in craftModuleInterface.ClusterModules)
            {
                if (module.Get().TryGetComponent<FridgeModule>(out var fridgeModule) && module.Get().TryGetComponent<CargoBayCluster>(out var clusterbay))
                {
                    //SgtLogger.l("found fridge");
                    ConnectedFridgeModules.Add(clusterbay);
                }
            }
            //GetAllMassDesc();
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
                    if(module.storage==null)
                        continue;

                    currentCapacity += module.storage.MassStored();
                    totalCapacity += module.storage.capacityKg;

                    //SgtLogger.l("modul: " + module.ToString());
                    if (module.storage.MassStored() > 0f)
                    {
                        for (int i = module.storage.items.Count - 1; i >= 0; i--)
                        {
                            if (module.storage.items[i] != null
                                && !module.storage.items[i].IsNullOrDestroyed()
                                && module.storage.items[i].gameObject != null
                                && module.storage.items[i].gameObject.TryGetComponent<Pickupable>(out var pickupable) 
                                && module.storage.items[i].gameObject.TryGetComponent<Edible>(out var edible))
                            {
                                float thisFoodsMass = edible.foodInfo.CaloriesPerUnit * pickupable.TotalAmount / 1000f;
                                totalKCalNew += thisFoodsMass;
                                infoText += string.Format(RTB_FOODSTORAGESTATUS.FOODINFO, edible.foodInfo.ConsumableName, thisFoodsMass.ToString());
                            }
                        }
                    }
                }
            }
            if (Mathf.RoundToInt(totalKCalNew) != Mathf.RoundToInt(_totalKCal))
            {
                _totalKCal = (totalKCalNew);
            }
            UpdateMeter();
            return infoText;
        }
    }
}
