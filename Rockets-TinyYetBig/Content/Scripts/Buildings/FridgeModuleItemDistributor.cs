//using Rockets_TinyYetBig.Behaviours;
//using Rockets_TinyYetBig.Buildings.CargoBays;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.UI;
//using UtilLibs;
//using static Rockets_TinyYetBig.STRINGS.BUILDING.STATUSITEMS;

//namespace Rockets_TinyYetBig.NonRocketBuildings
//{
//    public class FridgeModuleItemDistributor : KMonoBehaviour, ISim200ms
//    {
//        [MyCmpReq]
//        Storage OwnStorage;
//        [MyCmpReq]
//        KSelectable selectable;

//        public MeterController storage_meter;

//        private Guid StatusItemHandle;
//        private FilteredStorage filteredStorage;

//        CraftModuleInterface craftModuleInterface;

//        private static readonly EventSystem.IntraObjectHandler<FridgeModuleItemDistributor> OnRocketModulesChangend = new EventSystem.IntraObjectHandler<FridgeModuleItemDistributor>((System.Action<FridgeModuleItemDistributor, object>)((component, data) => component.UpdateModules(data)));


//        List<Tuple<CargoBayCluster, FakeStorage>> ConnectedFridgeModules = new List<Tuple<CargoBayCluster,FakeStorage>>();
//        public override void OnPrefabInit() => this.filteredStorage = new FilteredStorage(this, new Tag[1]
//        {
//            GameTags.Compostable
//        }, null, true, Db.Get().ChoreTypes.FoodFetch);

//        public override void OnSpawn()
//        {
//            base.OnSpawn();
//            this.GetMyWorld().TryGetComponent<CraftModuleInterface>(out craftModuleInterface);
//            SgtLogger.Assert("craftModuleInterface was null on fridge access hatch?!", craftModuleInterface);
//            //SgtLogger.l("AAAA");
//            ModAssets.FridgeModuleDistributors.Add(this);
//            craftModuleInterface.gameObject.Subscribe((int)GameHashes.RocketModuleChanged, UpdateModules);
//            UpdateModules(null);
//            //StatusItemHandle = selectable.AddStatusItem(ModAssets.StatusItems.RTB_AccessHatchStorage, (object)this);

//            this.filteredStorage.FilterChanged();
//            CreateMeter();
//            UpdateMeter();
//        }
//        public void CreateMeter()
//        {
//            storage_meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter_all", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, null);

//        }
//        public void UpdateMeter()
//        {
//            float positionPercent = Mathf.Clamp01(OwnStorage.MassStored() / OwnStorage.capacityKg);
//            if (storage_meter != null)
//            {
//                storage_meter.SetPositionPercent(positionPercent);
//            }
//        }
//        public override void OnCleanUp()
//        {

//            ModAssets.FridgeModuleDistributors.Remove(this);
//            base.OnCleanUp();

//        }

//        public void Sim200ms(float dt)
//        {
//        }

//        void UpdateModules(object o)
//        {
//            SgtLogger.l("Redoing Referenced Modules");
//            ConnectedFridgeModules.Clear();
//            float capacity = 0;
//            foreach (var module in craftModuleInterface.ClusterModules)
//            {
//                if (module.Get().TryGetComponent<FridgeModule>(out _) 
//                    && module.Get().TryGetComponent<CargoBayCluster>(out var clusterbay)
//                    && module.Get().TryGetComponent<FakeStorage>(out var fakeStorage)
//                    &&fakeStorage.LinkType == FakeStorage.RocketModuleLinkType.FreezerModule)
//                {

//                    capacity += clusterbay.storage.capacityKg;
//                    //SgtLogger.l("found fridge");
//                    fakeStorage.InitLink(OwnStorage,clusterbay.storage);

//                    ConnectedFridgeModules.Add(new Tuple<CargoBayCluster, FakeStorage>(clusterbay,fakeStorage));
//                }
//            }
//            OwnStorage.capacityKg = capacity;
//            UpdateMeter();
//        }


//    }
//}
