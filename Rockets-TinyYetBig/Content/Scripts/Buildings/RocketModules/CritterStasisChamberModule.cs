using KSerialization;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Behaviours
{
	public class CritterStasisChamberModule : KMonoBehaviour, ISidescreenButtonControl, ISim1000ms, IUserControlledCapacity
	{
		[MyCmpGet]
		TreeFilterable treeFilterable;

		[Serialize]
		public List<CritterStorageInfo> storedCritters = new List<CritterStorageInfo>();
		private List<FetchOrder2> fetches;
		//private static StatusItem capacityStatusItem;
		[Serialize]
		public int CurrentMaxCapacity = Config.Instance.CritterStorageCapacity;

		private static readonly EventSystem.IntraObjectHandler<CritterStasisChamberModule> RefreshCreatureCountDelegate = new EventSystem.IntraObjectHandler<CritterStasisChamberModule>(((component, data) => component.RefreshCreatureCount(data)));
		public int CurrentCapacity => storedCritters.Count;

		public string GetStatusItem()
		{
			string returnValue = CurrentCapacity <= 0 ?
				STRINGS.BUILDING.STATUSITEMS.RTB_CRITTERMODULECONTENT.NOCRITTERS :
				STRINGS.BUILDING.STATUSITEMS.RTB_CRITTERMODULECONTENT.HASCRITTERS;

			foreach (var critter in storedCritters)
			{
				string critInfo = "\n";
				critInfo += critter.CreatureAge >= 0 ? STRINGS.BUILDING.STATUSITEMS.RTB_CRITTERMODULECONTENT.CRITTERINFO : STRINGS.BUILDING.STATUSITEMS.RTB_CRITTERMODULECONTENT.CRITTERINFOAGELESS;
				critInfo = critInfo.Replace("{CRITTERNAME}", Assets.GetPrefab(critter.CreatureTag).GetProperName());
				critInfo = critInfo.Replace("{AGE}", critter.CreatureAge.ToString("0.#"));
				returnValue += critInfo;
			}

			return returnValue;

		}

		void SpawnCritterFromStorage(CritterStorageInfo critterInfo)
		{
			Vector3 position = this.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);

			GameObject spawnedCritter = Util.KInstantiate(Assets.GetPrefab(critterInfo.CreatureTag), position);
			spawnedCritter.SetActive(true);
			var AgeMonitor = spawnedCritter.GetSMI<AgeMonitor.Instance>();
			if (AgeMonitor != null && critterInfo.CreatureAge >= 0)
				AgeMonitor.age.SetValue(critterInfo.CreatureAge);

			//spawnedCritter.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim((HashedString)"growup_pst");

			var wild = spawnedCritter.GetSMI<WildnessMonitor.Instance>();
			if (wild != null)
			{
				wild.wildness.SetValue(critterInfo.WildnessPercentage);
			}

			if (spawnedCritter.TryGetComponent<Klei.AI.Traits>(out var traits) && critterInfo.StoredTraitIDs != null)
			{
				traits.SetTraitIds(critterInfo.StoredTraitIDs);
			}
			if (spawnedCritter.TryGetComponent<Baggable>(out var baggable))
			{
				if (spawnedCritter.TryGetComponent<Trappable>(out var trap) && spawnedCritter.TryGetComponent<KAnimControllerBase>(out var animController) && !animController.HasAnimation("trussed"))
				{
					spawnedCritter.GetComponent<KPrefabID>().AddTag("trapped");
				}
				else
				{
					baggable.SetWrangled();
				}
			}
			storedCritters.Remove(critterInfo);
		}

		public void SpawnCrittersFromStorage()
		{
			int count = storedCritters.Count - 1;
			for (int i = count; i >= 0; --i)
			{
				SpawnCritterFromStorage(storedCritters[(i)]);
			}
			if (!storedCritters.IsNullOrDestroyed())
				storedCritters.Clear();

			treeFilterable.UpdateFilters([]);
			//DetailsScreen.Instance.Refresh(gameObject);///should refresh screen, Crashes, bad
			UpdateStatusItem();
		}
		public void AddCritterToStorage(GameObject critter)
		{
			var CritterInfoToStore = new CritterStorageInfo();
			CritterInfoToStore.CreatureTag = critter.GetComponent<KPrefabID>().PrefabTag;
			var ageMonitor = critter.GetSMI<AgeMonitor.Instance>();
			if (ageMonitor != null)
				CritterInfoToStore.CreatureAge = ageMonitor.age.value;
			else
				CritterInfoToStore.CreatureAge = -1;
			var wildnessCheck = critter.GetSMI<WildnessMonitor.Instance>();

			if (wildnessCheck != null)
			{
				CritterInfoToStore.WildnessPercentage = wildnessCheck.wildness.value;
			}
			if (critter.TryGetComponent<Klei.AI.Traits>(out var traits))
			{
				CritterInfoToStore.StoredTraitIDs = traits.GetTraitIds();
			}

			storedCritters.Add(CritterInfoToStore);
#if DEBUG
            SgtLogger.debuglog("Added {0} to critter stasis chamber, Age: {1}, Wildness: {2}".F(CritterInfoToStore.CreatureTag, CritterInfoToStore.CreatureAge, CritterInfoToStore.WildnessPercentage));
#endif
			UpdateStatusItem();
			critter.gameObject.DeleteObject();
		}

		private void UpdateStatusItem()
		{
			this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, ModAssets.StatusItems.RTB_CritterModuleContent, this);
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.fetches = new List<FetchOrder2>();
			treeFilterable.OnFilterChanged += new System.Action<HashSet<Tag>>(this.OnFilterChanged);
			Prioritizable.AddRef(this.gameObject);
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			this.Subscribe<CritterStasisChamberModule>((int)GameHashes.FixedCaptureComplete, CritterStasisChamberModule.RefreshCreatureCountDelegate);
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
			if (CurrentMaxCapacity == CurrentCapacity)
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
			HashSet<Tag> tags = treeFilterable.GetTags();
			ChoreType creatureFetch = Db.Get().ChoreTypes.CreatureFetch;
			Storage component = this.GetComponent<Storage>();
			int num1 = CurrentMaxCapacity - this.storedCritters.Count;
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
				float minimumFetchAmount = FetchChore.GetMinimumFetchAmount(tags);
				FetchOrder2 fetchOrder2 = new FetchOrder2(creatureFetch, tags, FetchChore.MatchCriteria.MatchID, GameTags.Creatures.Deliverable, (Tag[])null, component, minimumFetchAmount, Operational.State.Operational);
				fetchOrder2.validateRequiredTagOnTagChange = true;
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

		public string SidescreenButtonText => STRINGS.BUILDING.STATUSITEMS.RTB_CRITTERMODULECONTENT.DROPITBUTTON;

		public string SidescreenButtonTooltip => STRINGS.BUILDING.STATUSITEMS.RTB_CRITTERMODULECONTENT.DROPITBUTTONTOOLTIP;
		public LocString CapacityUnits => STRINGS.BUILDING.STATUSITEMS.RTB_CRITTERMODULECONTENT.UNITS;

		public float UserMaxCapacity
		{
			get => CurrentMaxCapacity;
			set
			{
				CurrentMaxCapacity = (int)value;

				int excess = CurrentCapacity - CurrentMaxCapacity;
				//SgtLogger.l("excess: " + excess);
				if (excess > 0)
				{
					int count = storedCritters.Count - 1;
					for (int i = count; i > count - excess; --i)
					{
						SpawnCritterFromStorage(storedCritters[(i)]);
					}
				}
				RebalanceFetches();
			}
		}

		public float AmountStored => CurrentCapacity;

		public float MinCapacity => 1;

		float IUserControlledCapacity.MaxCapacity => Config.Instance.CritterStorageCapacity;

		public bool WholeValues => true;



		public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();
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

		public int HorizontalGroupID()
		{
			return -1;
		}

		#endregion

		public struct CritterStorageInfo
		{
			public Tag CreatureTag;
			public float CreatureAge;
			public float WildnessPercentage;
			//public float EggPercentage;
			public List<string> StoredTraitIDs;

			public CritterStorageInfo(Tag _tag, float _age, float _wildPerc, List<string> _traits)//, float _egg)
			{
				CreatureTag = _tag;
				CreatureAge = _age;
				WildnessPercentage = _wildPerc;
				//EggPercentage = _egg;   
				StoredTraitIDs = _traits;
			}
		}

	}
}
