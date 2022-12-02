using KSerialization;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ResearchTypes;

namespace Rockets_TinyYetBig.Behaviours
{
    public class CritterStasisChamberModule : KMonoBehaviour, ISidescreenButtonControl ,ISim1000ms
    {
        [Serialize]
        public List<CritterStorageInfo> storedCritters = new List<CritterStorageInfo>();
        private List<FetchOrder2> fetches;
        //private static StatusItem capacityStatusItem;

        private static readonly EventSystem.IntraObjectHandler<CritterStasisChamberModule> RefreshCreatureCountDelegate = new EventSystem.IntraObjectHandler<CritterStasisChamberModule>((System.Action<CritterStasisChamberModule, object>)((component, data) => component.RefreshCreatureCount(data)));
        public int CurrentCapacity => storedCritters.Count;

        public string GetStatusItem()
        {
            string returnValue = CurrentCapacity <= 0 ?
                STRINGS.BUILDING.STATUSITEMS.RTB_CRITTERMODULECONTENT.NOCRITTERS :
                STRINGS.BUILDING.STATUSITEMS.RTB_CRITTERMODULECONTENT.HASCRITTERS;
            
            foreach(var critter in storedCritters)
            {
                string critInfo = "\n";
                critInfo += STRINGS.BUILDING.STATUSITEMS.RTB_CRITTERMODULECONTENT.CRITTERINFO;
                critInfo = critInfo.Replace("{CRITTERNAME}", Assets.GetPrefab(critter.CreatureTag).GetProperName());
                critInfo = critInfo.Replace("{AGE}", critter.CreatureAge.ToString("0.#"));
                returnValue += critInfo;
            }
            
            return returnValue;

        }

        public void SpawnCrittersFromStorage()
        {
            Vector3 position = this.transform.GetPosition();
            position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
            foreach (var critterInfo in storedCritters)
            {
                GameObject spawnedCritter = Util.KInstantiate(Assets.GetPrefab(critterInfo.CreatureTag), position);
                spawnedCritter.SetActive(true);
                spawnedCritter.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim((HashedString)"growup_pst");
                spawnedCritter.GetSMI<AgeMonitor.Instance>().age.SetValue(critterInfo.CreatureAge);
                var wild = spawnedCritter.GetSMI<WildnessMonitor.Instance>();
                if (wild != null)
                {
                    wild.wildness.SetValue(critterInfo.WildnessPercentage);
                }

                Baggable component2 = spawnedCritter.GetComponent<Baggable>();
                if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
                    component2.SetWrangled();
            }
            // DetailsScreen.Instance.Refresh(gameObject);
            //this.GetComponent<TreeFilterable>().UpdateFilters(null);
            if (!storedCritters.IsNullOrDestroyed())
                storedCritters.Clear();

            UpdateStatusItem();
        }
        public void AddCritterToStorage(GameObject critter)
        {
            var CritterInfoToStore = new CritterStorageInfo();
            CritterInfoToStore.CreatureTag = critter.GetComponent<KPrefabID>().PrefabTag;
            CritterInfoToStore.CreatureAge = critter.GetSMI<AgeMonitor.Instance>().age.value;
            CritterInfoToStore.WildnessPercentage = critter.GetSMI<WildnessMonitor.Instance>().wildness.value;
            storedCritters.Add(CritterInfoToStore);
#if DEBUG
            Debug.Log("Added {0} to critter stasis chamber, Age: {1}, Wildness: {2}".F(CritterInfoToStore.CreatureTag, CritterInfoToStore.CreatureAge, CritterInfoToStore.WildnessPercentage));
#endif
            UpdateStatusItem();
            critter.gameObject.DeleteObject();
        }

        private void UpdateStatusItem()
        {
            this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, ModAssets.StatusItems.RTB_CritterModuleContent, (object)this);
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.fetches = new List<FetchOrder2>();
            this.GetComponent<TreeFilterable>().OnFilterChanged += new System.Action<HashSet<Tag>>(this.OnFilterChanged);
            //this.GetComponent<Storage>().SetOffsets(this.deliveryOffsets);
            Prioritizable.AddRef(this.gameObject);
            //if (CritterStasisChamberModule.capacityStatusItem == null)
            //{
            //    CritterStasisChamberModule.capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
            //    CritterStasisChamberModule.capacityStatusItem.resolveStringCallback = (Func<string, object, string>)((str, data) =>
            //    {
            //        //Debug.Log("TEstsst"+ str + data);
            //        string newValue1 = Util.FormatWholeNumber(this.CurrentCapacity);
            //        string newValue2 = Util.FormatWholeNumber(Config.Instance.CritterStorageCapacity);
            //        str = str.Replace("{Stored}", newValue1).Replace("{Capacity}", newValue2).Replace("{Units}", global::STRINGS.UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.UNITS_SUFFIX);
            //        return str;
            //    });
            //}
            //this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, CritterStasisChamberModule.capacityStatusItem, (object)this);
        }
        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.Subscribe<CritterStasisChamberModule>(643180843, CritterStasisChamberModule.RefreshCreatureCountDelegate);
            this.RefreshCreatureCount(); 
            UpdateStatusItem();
        }
        private void OnFilterChanged(HashSet<Tag> tags)
        {
            this.ClearFetches();
            this.RebalanceFetches();
        }

        private void RefreshCreatureCount(object data = null)
        {
            int storedCreatureCount = storedCritters.Count;
            if (Config.Instance.CritterStorageCapacity == storedCreatureCount)
                return;
            this.RebalanceFetches();
        }
        private void ClearFetches()
        {
            for (int index = this.fetches.Count - 1; index >= 0; --index)
                this.fetches[index].Cancel("clearing all fetches");
            this.fetches.Clear();
        }

        private void RebalanceFetches()
        {
            HashSet<Tag> tags = this.GetComponent<TreeFilterable>().GetTags();
            ChoreType creatureFetch = Db.Get().ChoreTypes.CreatureFetch;
            Storage component = this.GetComponent<Storage>();
            int num1 = Config.Instance.CritterStorageCapacity - this.storedCritters.Count;
            int count = this.fetches.Count;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            for (int index = this.fetches.Count - 1; index >= 0; --index)
            {
                if (this.fetches[index].IsComplete())
                {
                    this.fetches.RemoveAt(index);
                    ++num2;
                }
            }
            int num6 = 0;
            for (int index = 0; index < this.fetches.Count; ++index)
            {
                if (!this.fetches[index].InProgress)
                    ++num6;
            }
            if (num6 == 0 && this.fetches.Count < num1)
            {
                FetchOrder2 fetchOrder2 = new FetchOrder2(creatureFetch, tags, FetchChore.MatchCriteria.MatchID, GameTags.Creatures.Deliverable, (Tag[])null, component, 1f, Operational.State.Operational);
                fetchOrder2.Submit(new System.Action<FetchOrder2, Pickupable>(this.OnFetchComplete), false, new System.Action<FetchOrder2, Pickupable>(this.OnFetchBegun));
                this.fetches.Add(fetchOrder2);
                int num7 = num3 + 1;
            }
            int num8 = this.fetches.Count - num1;
            for (int index = this.fetches.Count - 1; index >= 0 && num8 > 0; --index)
            {
                if (!this.fetches[index].InProgress)
                {
                    this.fetches[index].Cancel("fewer creatures in room");
                    this.fetches.RemoveAt(index);
                    --num8;
                    ++num4;
                }
            }
            while (num8 > 0 && this.fetches.Count > 0)
            {
                this.fetches[this.fetches.Count - 1].Cancel("fewer creatures in room");
                this.fetches.RemoveAt(this.fetches.Count - 1);
                --num8;
                ++num5;
            }
        }

        private void OnFetchComplete(FetchOrder2 fetchOrder, Pickupable fetchedItem)
        {
            this.RebalanceFetches();
        }

        private void OnFetchBegun(FetchOrder2 fetchOrder, Pickupable fetchedItem) => this.RebalanceFetches();



        #region button

        public string SidescreenButtonText => "Dröp Critters Bröther";

        public string SidescreenButtonTooltip => "Drop it like its hot";

        public bool SidescreenEnabled() => true;

        public bool SidescreenButtonInteractable() => storedCritters.Count > 0;

        public void OnSidescreenButtonPressed()
        {
            SpawnCrittersFromStorage();
        }

        public int ButtonSideScreenSortOrder() => 21;

        public void Sim1000ms(float dt)
        {
            Storage component = this.GetComponent<Storage>();
            foreach (var item in component.items)
            {
                if (!item.IsNullOrDestroyed())
                    AddCritterToStorage(item);
            }
        }

        #endregion

        public struct CritterStorageInfo
        {
            public Tag CreatureTag;
            public float CreatureAge;
            public float WildnessPercentage;
            //public float EggPercentage;

            public CritterStorageInfo(Tag _tag, float _age, float _wildPerc)//, float _egg)
            {
                CreatureTag = _tag;    
                CreatureAge = _age;
                WildnessPercentage = _wildPerc;
                //EggPercentage = _egg;   
            }
        }

    }
}
