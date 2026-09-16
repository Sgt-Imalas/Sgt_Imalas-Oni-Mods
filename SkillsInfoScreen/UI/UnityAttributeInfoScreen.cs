using SkillsInfoScreen.UI.UIComponents;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using static SkillsInfoScreen.STRINGS.ATTRIBUTESCREEN_DROPDOWN;

namespace SkillsInfoScreen.UI
{
	internal class UnityAttributeInfoScreen : KScreen
	{
		public enum OrderBy
		{
			NameAs, NameDes,
			XpAs, XpDes,
			AttributeAs, AttributeDes,
		}
		OrderBy CurrentSort = OrderBy.NameAs;
		Klei.AI.Attribute CurrentSortAttribute = null;


		public UnityAttributeInfoScreen() : base()
		{
			ConsumeMouseScroll = true;
		}

		public static UnityAttributeInfoScreen Instance = null;

		GameObject ItemContainer;
		WorldHeaderEntry WorldHeaderPrefab;
		DuplicantEntry DuplicantEntryPrefab;
		FToggle SplitByAsteroids;
		FOrderByParamToggle SortByName, SortByXP;
		GameObject SortByContainer, SortByAttributePrefab, SortBySpacerPrefab;
		Dictionary<string, FOrderByParamToggle> SortByAttributes = [];
		FButton Close;

		Dictionary<string, Klei.AI.Attribute> Columns = [];
		Dictionary<int, WorldHeaderEntry> WorldHeaders = [];
		Dictionary<IAssignableIdentity, DuplicantEntry> MinionEntries = [];
		FMultiSelectDropdown Options;

		public static void DestroyInstance() { Instance = null; }

		public static void InitScreen(GameObject parent)
		{
			if (Instance == null)
			{
				Instance = Util.KInstantiateUI<UnityAttributeInfoScreen>(ModAssets.ScreenGO, parent, true);
				Instance.Init();
				Instance.Show(false);
			}
		}
		public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (show)
			{
				transform.SetAsLastSibling();
				Refresh();
			}
		}
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Init();
		}

		bool init;
		void Init()
		{
			if (init)
				return;
			init = true;

			ItemContainer = transform.Find("ScrollArea/Content").gameObject;
			WorldHeaderPrefab = transform.Find("ScrollArea/Content/WorldPrefab").gameObject.AddOrGet<WorldHeaderEntry>();
			WorldHeaderPrefab.gameObject.SetActive(false);
			DuplicantEntryPrefab = transform.Find("ScrollArea/Content/DupePrefab").gameObject.AddOrGet<DuplicantEntry>();
			DuplicantEntryPrefab.gameObject.SetActive(false);
			SplitByAsteroids = transform.Find("Info/ShowAsteroids/Checkbox").gameObject.AddOrGet<FToggle>();

			SortByName = transform.Find("Info/Filter_Name").gameObject.AddOrGet<FOrderByParamToggle>();
			SortByName.SetActions(() => SetOrderedBy(OrderBy.NameAs), () => SetOrderedBy(OrderBy.NameDes));
			SortByName.ActivateToggle();

			SortByXP = transform.Find("Info/Filter_XP").gameObject.AddOrGet<FOrderByParamToggle>();
			SortByXP.StartDescending = true;
			SortByXP.SetActions(() => SetOrderedBy(OrderBy.XpAs), () => SetOrderedBy(OrderBy.XpDes));

			SortByContainer = transform.Find("Info").gameObject;
			SortByAttributePrefab = transform.Find("Info/Attribute_Filter").gameObject;
			SortByAttributePrefab.gameObject.SetActive(false);
			SortBySpacerPrefab = transform.Find("Info/spacer").gameObject;

			Close = transform.Find("TopBar/CloseButton").gameObject.AddOrGet<FButton>();
			Close.OnClick += () => ManagementMenu.Instance.CloseAll();
			Close.PlayClickSound = false;

			Options = transform.Find("TopBar/FilterButton").gameObject.AddOrGet<FMultiSelectDropdown>();
			Options.DropDownEntries = [
				new FMultiSelectDropdown.FDropDownEntry(TEMP_EFFECT_TOGGLE.NAME,SetTempEffectInclude,true, TEMP_EFFECT_TOGGLE.TOOLTIP),
				new FMultiSelectDropdown.FDropDownEntry(TINT_VALUES.NAME,SetValueTint,true, TINT_VALUES.TOOLTIP),
				//new FMultiSelectDropdown.FDropDownEntry(TINT_XP.NAME,SetXpTint,true, TINT_VALUES.TOOLTIP),
				new FMultiSelectDropdown.FDropDownButtonEntry(RESETPOS.NAME, OnResetPos),
				new FMultiSelectDropdown.FDropDownButtonEntry(RESETSIZE.NAME, OnResetSize)
				];

			if (DlcManager.IsExpansion1Active())
				Options.DropDownEntries.Insert(0, new FMultiSelectDropdown.FDropDownEntry(SHOW_ASTEROIDS.NAME, SetAsteroidSplit, ModAssets.Config.SortPerWorld, SHOW_ASTEROIDS.TOOLTIP));

			Options.InitializeDropDown();

			var drag = transform.Find("TopBar").gameObject.AddOrGet<DraggablePanel>();
			drag.Target = transform;
			drag.OnDragged = OnMoved;

			var resize = transform.Find("ResizeKnob").gameObject.AddOrGet<ResizeDragKnob>();
			resize.Target = transform;
			resize.OnResized = OnResized;
			transform.rectTransform().sizeDelta = ModAssets.Config.SizeDelta;
			transform.localPosition = ModAssets.Config.LocalPosition;
			InitAttributeFilters();
		}

		void SetOrderedBy(OrderBy mode, Klei.AI.Attribute attribute = null)
		{
			CurrentSort = mode;
			CurrentSortAttribute = attribute;
			Refresh();
		}

		void SetAsteroidSplit(bool enable)
		{
			ModAssets.Config.SortPerWorld = enable;
			ModAssets.Config.Write();
			Refresh();
			Refresh();
		}
		void SetTempEffectInclude(bool include)
		{
			ModAssets.Config.IncludeTemporaryEffects = include;
			ModAssets.Config.Write();
			Refresh();
		}
		void SetValueTint(bool on)
		{
			ModAssets.Config.TintValue = on;
			ModAssets.Config.Write();
			Refresh();
		}
		//void SetXpTint(bool on)
		//{
		//	ModAssets.Config.TintXP = on;
		//	ModAssets.Config.Write();
		//	Refresh();
		//}
		void OnResized()
		{
			ModAssets.Config.OnResized(transform.rectTransform());
		}
		void OnMoved()
		{
			var pos = transform.localPosition;
			var canvas = transform.GetComponentInParent<Canvas>().pixelRect;
			var scale = transform.GetComponentInParent<CanvasScaler>().scaleFactor;
			pos.y = Mathf.Clamp(pos.y, -((canvas.height / scale) - 180), 60);
			pos.x = Mathf.Clamp(pos.x, -((canvas.width / scale) - 150), 20);

			transform.SetLocalPosition(pos);
			ModAssets.Config.OnMoved(transform);
		}
		void OnResetSize(bool _)
		{
			transform.rectTransform().sizeDelta = new(1400, 720);
			OnResized();
		}
		void OnResetPos(bool _)
		{
			transform.SetLocalPosition(new(0, -45f));
			OnMoved();
		}

		void Refresh()
		{
			RefreshAsteroidHeaders();
			RefreshMinionEntries();
			RefreshOrderByToggles();
		}
		void RefreshOrderByToggles()
		{
			if (CurrentSort != OrderBy.NameAs && CurrentSort != OrderBy.NameDes)
				SortByName.DeactivateToggle();
			if (CurrentSort != OrderBy.XpAs && CurrentSort != OrderBy.XpDes)
				SortByXP.DeactivateToggle();

			string currentAttribute = CurrentSortAttribute?.Id ?? string.Empty;
			foreach (var toggleKVP in SortByAttributes)
			{
				if (toggleKVP.Key != currentAttribute)
					toggleKVP.Value.DeactivateToggle();
			}
		}

		void RefreshAsteroidHeaders()
		{
			if (DlcManager.IsPureVanilla())
				return;
			foreach (var entry in WorldHeaders.Values)
			{
				entry.gameObject.SetActive(false);
			}
			if (!ModAssets.Config.SortPerWorld)
				return;
			foreach (var worldContainer in ClusterManager.Instance.WorldContainers)
			{
				if (!worldContainer.isDiscovered)
					continue;
				if (!Components.LiveMinionIdentities.Any(id => id.GetMyWorldId() == worldContainer.id) && !Components.MinionStorages.Any(storage => storage.GetMyWorldId() == worldContainer.id))
					continue;
				int id = worldContainer.id;
				var header = AddOrGetWorldHeader(id);
				header.Refresh();
			}
		}
		object GetCurrentSort(IAssignableIdentity minion)
		{
			switch (CurrentSort)
			{
				case OrderBy.NameAs:
				case OrderBy.NameDes:
					return minion.GetProperName();
				case OrderBy.XpAs:
				case OrderBy.XpDes:
					if (minion is StoredMinionIdentity st)
						return st.TotalExperienceGained;
					if (minion is MinionIdentity i)
						return i.GetComponent<MinionResume>().TotalExperienceGained;
					break;
				case OrderBy.AttributeAs:
				case OrderBy.AttributeDes:
					return ModAssets.GetAttributeLevel(minion, CurrentSortAttribute);
			}
			return 0;
		}
		int WorldSort(IAssignableIdentity minion)
		{
			var worldsSorted = ClusterManager.Instance.GetWorldIDsSorted();

			if (minion is MinionIdentity i)
				return worldsSorted.IndexOf(i.GetMyWorldId());
			else if (minion is StoredMinionIdentity st)
			{
				int world = st.assignableProxy.Get().GetMyWorldId();
				return worldsSorted.IndexOf(world);
			}
			return 0;
		}

		void RefreshMinionEntries()
		{
			foreach (var entry in MinionEntries.Values)
			{
				entry.gameObject.SetActive(false);
			}
			IOrderedEnumerable<IAssignableIdentity> minions = null;

			var allMinions = new List<IAssignableIdentity>();
			allMinions.AddRange(Components.LiveMinionIdentities);
			foreach (MinionStorage storage in Components.MinionStorages)
			{
				foreach (var stored in storage.GetStoredMinionInfo())
					if (stored.serializedMinion != null)
						allMinions.Add(stored.serializedMinion.Get<StoredMinionIdentity>());
			}

			
			minions = allMinions.OrderBy(WorldSort);

			foreach (var minion in minions)
			{
				SgtLogger.l("WORLD: " + minion.GetProperName() + ": " + WorldSort(minion));
			}
			bool descendingSort = (int)CurrentSort % 2 == 0;

			if (ModAssets.Config.SortPerWorld)
			{
				if (descendingSort)
					minions = minions.OrderBy(WorldSort).ThenByDescending(GetCurrentSort);
				else
					minions = minions.OrderBy(WorldSort).ThenBy(GetCurrentSort);
			}
			else
			{
				//without asteroid headers, the ordering is inverted
				if (descendingSort)
					minions = minions.OrderBy(GetCurrentSort);
				else
					minions = minions.OrderByDescending(GetCurrentSort);
			}

			foreach (IAssignableIdentity minion in minions)
			{
				var entry = AddOrGetMinionEntry(minion);
				entry.Refresh();

				int worldId = 0;
				if (minion is MinionIdentity m)
					worldId = m.GetMyWorldId();
				else if (minion is StoredMinionIdentity id)
				{
					MinionStorage storage = Components.MinionStorages.Items.FirstOrDefault(s => s.GetStoredMinionInfo().Any(info => info.serializedMinion.Get()== id.GetComponent<KPrefabID>()));
					if(storage != null)
						worldId = storage.GetMyWorldId();
				}

				if (DlcManager.IsExpansion1Active() && ModAssets.Config.SortPerWorld && WorldHeaders.TryGetValue(worldId, out WorldHeaderEntry header))
				{
					entry.transform.SetSiblingIndex(header.transform.GetSiblingIndex() + 1);
					entry.gameObject.SetActive(header.Expanded);
				}
				else
				{
					entry.transform.SetAsLastSibling();
				}
			}

		}

		void InitAttributeFilters()
		{
			var attributeDb = Db.Get().Attributes;
			var skillGroupsDb = Db.Get().SkillGroups;
			ModAssets.MaxSkillLevels = [];

			var stats = DUPLICANTSTATS.ALL_ATTRIBUTES.OrderBy(id => global::STRINGS.UI.StripLinkFormatting(attributeDb.TryGet(id)?.Name ?? "unknown"));

			foreach (var attributeId in stats)
			{
				if (attributeId == "SpaceNavigation" && !DlcManager.IsExpansion1Active())
					continue;
				ModAssets.MaxSkillLevels[attributeId] = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL;

				var attribute = attributeDb.TryGet(attributeId);
				Columns[attributeId] = attribute;

				var filterGO = Util.KInstantiateUI(SortByAttributePrefab, SortByContainer);
				var toggle = filterGO.AddOrGet<FOrderByParamToggle>();
				toggle.Label = attribute.Name;
				toggle.StartDescending = true;
				toggle.SetActions(() => SetOrderedBy(OrderBy.AttributeAs, attribute), () => SetOrderedBy(OrderBy.AttributeDes, attribute));

				var relevantSkillGroup = skillGroupsDb.resources.FirstOrDefault(r => r.relevantAttributes.Contains(attribute));
				if (relevantSkillGroup != null)
				{
					toggle.SetCompactIcon(Assets.GetSprite(relevantSkillGroup.archetypeIcon), 55);
				}
				filterGO.SetActive(true);

				SortByAttributes[attributeId] = toggle;

				if (attributeId != stats.Last())
					Util.KInstantiateUI(SortBySpacerPrefab, SortByContainer, true);
				SgtLogger.l("Adding column for " + attributeId);
			}
			transform.Find("Info/EndSpacer")?.SetAsLastSibling();
		}

		DuplicantEntry AddOrGetMinionEntry(IAssignableIdentity minion)
		{
			if (!MinionEntries.TryGetValue(minion, out var value))
			{
				value = Util.KInstantiateUI<DuplicantEntry>(DuplicantEntryPrefab.gameObject, ItemContainer, true);
				value.Init(minion);
				MinionEntries.Add(minion, value);
			}
			value.gameObject.SetActive(true);
			return value;
		}
		WorldHeaderEntry AddOrGetWorldHeader(int worldId)
		{
			if (!WorldHeaders.TryGetValue(worldId, out var value))
			{
				value = Util.KInstantiateUI<WorldHeaderEntry>(WorldHeaderPrefab.gameObject, ItemContainer, true);
				value.Init(ClusterManager.Instance.GetWorld(worldId), Refresh);
				value.transform.SetAsLastSibling();
				WorldHeaders.Add(worldId, value);
			}
			value.gameObject.SetActive(true);
			return value;
		}


		public override void Show(bool show = true)
		{
			base.Show(show);
		}
	}
}
