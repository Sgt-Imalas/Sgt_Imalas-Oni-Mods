using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static SetStartDupes.DupeTraitManager;
using static SetStartDupes.STRINGS.UI.PRESETWINDOW;
using static STRINGS.DUPLICANTS;

namespace SetStartDupes
{
	/// <summary>
	/// copied from trait screen, responsible for selecting a reroll trait 
	/// </summary>
	internal class UnityTraitRerollingScreen : FScreen
	{
#pragma warning disable IDE0051 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members
		public static UnityTraitRerollingScreen Instance = null;

		public LocText ToReplaceName;
		public Image ToReplaceColour;

		public GameObject PresetListContainer;
		public GameObject PresetListPrefab;

		public FButton ClearSearchBar;
		public FInputField2 Searchbar;

		public bool CurrentlyActive;

		NextType TraitCategory;


		Dictionary<Trait, GameObject> TraitContainers = new Dictionary<Trait, GameObject>();
		GameObject NothingEntry;
		CharacterContainer OpenedFrom;


		public static void ShowWindow(System.Action onClose, CharacterContainer opener)
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.TraitsWindowPrefab, ModAssets.ParentScreen, true);
				Instance = screen.AddOrGet<UnityTraitRerollingScreen>();
				Instance.Init();
			}
			Instance.OpenedFrom = opener;
			Instance.SetOpenedType();
			Instance.Show(true);
			Instance.ConsumeMouseScroll = true;
			Instance.transform.SetAsLastSibling();
			Instance.OnCloseAction = onClose;
			Instance.Searchbar.Text = string.Empty;
		}

		private bool init;
		private System.Action OnCloseAction;


		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
			{
				this.Show(false);
			}
			if (e.TryConsume(Action.DebugToggleClusterFX))
			{
				Searchbar.ExternalStartEditing();
			}

			base.OnKeyDown(e);
		}

		public static string GetTraitName(CharacterContainer instance)
		{
			if (GuaranteedTraitRoll.ContainsKey(instance))
			{
				return GuaranteedTraitRoll[instance].GetName();
			}
			return CONGENITALTRAITS.NONE.NAME;
		}
		public static string GetTraitId(CharacterContainer instance)
		{
			if (GuaranteedTraitRoll.ContainsKey(instance))
			{
				return GuaranteedTraitRoll[instance].Id;
			}
			return "None";
		}

		private void SetOpenedType()
		{
			TraitCategory = NextType.allTraits;

			List<string> allowedTraits = ModAssets.TryGetTraitsOfCategory(NextType.allTraits, OpenedFrom.stats.personality.model).Select(t => t.id).ToList();

			Trait currentTrait = null;
			if (GuaranteedTraitRoll.ContainsKey(OpenedFrom))
			{
				currentTrait = GuaranteedTraitRoll[OpenedFrom];
			}


			if (currentTrait != null)
			{
				var next = ModAssets.GetTraitListOfTrait(currentTrait.Id);
				TraitCategory = next;
				ToReplaceName.text = GetTraitName(currentTrait);
				ToReplaceColour.color = ModAssets.GetColourFromType(next);
			}
			else
			{
				ToReplaceName.text = global::STRINGS.DUPLICANTS.CONGENITALTRAITS.NONE.NAME;
				ToReplaceColour.color = ModAssets.Colors.grey;
			}
			foreach (var go in TraitContainers)
			{
				go.Value.SetActive(allowedTraits.Contains(go.Key.Id));
			}
			ApplyFilter();
		}


		string GetTraitName(Trait trait)
		{
			return trait.Name;
		}

		private void AddUIContainer(Trait trait2, NextType next)
		{
			if (trait2 != null)
				AddUiContainer(
				GetTraitName(trait2),
				ModAssets.GetTraitTooltip(trait2, trait2.Id),
				trait: trait2,
				traitType: next);

		}



		private GameObject AddUiContainer(string name, string description, Trait trait = null, NextType traitType = default)
		{
			var PresetHolder = Util.KInstantiateUI(PresetListPrefab, PresetListContainer, true);
			UIUtils.TryChangeText(PresetHolder.transform, "Label", name);
			if (description != null && description.Length > 0)
			{
				UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("Label"), description, true, onBottom: true);
			}

			PresetHolder.transform.FindOrAddComponent<FButton>().OnClick += () => ChoseThis(trait);
			PresetHolder.transform.Find("Background").FindOrAddComponent<Image>().color = ModAssets.GetColourFromType(traitType);
			if (trait != null && !TraitContainers.ContainsKey(trait))
			{
				TraitContainers[trait] = PresetHolder;
			}

			return PresetHolder;
		}

		public static Dictionary<CharacterContainer, Trait> GuaranteedTraitRoll = new Dictionary<CharacterContainer, Trait>();
		private void ChoseThis(Trait trait)
		{
			if (trait != null)
			{
				GuaranteedTraitRoll[OpenedFrom] = trait;
				ModAssets.LockModelSelection(OpenedFrom);
			}
			else
			{
				if (GuaranteedTraitRoll.ContainsKey(OpenedFrom))
					GuaranteedTraitRoll.Remove(OpenedFrom);
			}

			if (OnCloseAction != null)
				this.OnCloseAction.Invoke();
			this.Show(false);
		}

		private void Init()
		{
			if (init)
			{
				return;
			}
			SgtLogger.l("Initializing TraitSelectionWindow");


			ToReplaceName = transform.Find("ToReplace/CurrentlyActive/Label").FindComponent<LocText>();
			ToReplaceColour = transform.Find("ToReplace/CurrentlyActive/Background").FindComponent<Image>();

			Searchbar = transform.Find("SearchBar/Input").FindOrAddComponent<FInputField2>();
			Searchbar.OnValueChanged.AddListener(ApplyFilter);
			Searchbar.Text = string.Empty;


			ClearSearchBar = transform.Find("SearchBar/DeleteButton").FindOrAddComponent<FButton>();
			ClearSearchBar.OnClick += () => Searchbar.Text = string.Empty;

			UIUtils.AddSimpleTooltipToObject(ClearSearchBar.transform, HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.CLEARTOOLTIP);

			PresetListContainer = transform.Find("ScrollArea/Content").gameObject;
			PresetListPrefab = transform.Find("ScrollArea/Content/PresetEntryPrefab").gameObject;
			PresetListPrefab.SetActive(false);
			transform.Find("ScrollArea/Content/CarePackagePrefab").gameObject.SetActive(false);


			var CloserButton = transform.Find("CloseButton").gameObject;
			CloserButton.FindOrAddComponent<FButton>().OnClick += () => this.Show(false);
			CloserButton.transform.Find("Text").GetComponent<LocText>().text = STRINGS.UI.PRESETWINDOW.HORIZONTALLAYOUT.ITEMINFO.BUTTONS.CLOSEBUTTON.TEXT;

			transform.Find("ScrollArea/Content/DupeSkinPartPrefab").gameObject.SetActive(false);
			InitAllContainers();
			init = true;
		}

		private void InitAllContainers()
		{
			var traitsDb = Db.Get().traits;

			NothingEntry = AddUiContainer(CONGENITALTRAITS.NONE.NAME, CONGENITALTRAITS.NONE.DESC);
			NothingEntry.SetActive(true);

			foreach (var type in (NextType[])Enum.GetValues(typeof(NextType)))
			{
				if (type != NextType.posTrait && type != NextType.negTrait && type != NextType.bionic_boost && type != NextType.bionic_bug) continue;

				var TraitsOfCategory = ModAssets.TryGetTraitsOfCategory(type, null);

				var sortedTraits = TraitsOfCategory.OrderBy(traitval => traitsDb.TryGet(traitval.id)?.GetName());

                foreach (DUPLICANTSTATS.TraitVal item in sortedTraits)
				{
					var trait = traitsDb.TryGet(item.id);

					if (ModAssets.TraitAllowedInCurrentDLC(item))
						AddUIContainer(traitsDb.TryGet(item.id), type);
				}
			}
		}


		public void ApplyFilter(string filterstring = "")
		{
			var allowedTraits = ModAssets.TryGetTraitsOfCategory(NextType.allTraits, OpenedFrom?.stats?.personality?.model ?? null).Select(t => t.id).ToList();
			foreach (var go in TraitContainers)
			{
				bool Contained = allowedTraits.Contains(go.Key.Id);
				go.Value.SetActive(filterstring == string.Empty ? Contained : Contained && ShowInFilter(filterstring, [go.Key.GetName(), go.Key.description ]));
			}
			NothingEntry.SetActive(true);
		}

		bool ShowInFilter(string filtertext, string stringsToInclude)
		{
			return ShowInFilter(filtertext, [stringsToInclude]);
		}

		bool ShowInFilter(string filtertext, string[] stringsToInclude)
		{
			bool show = false;
			filtertext = filtertext.ToLowerInvariant();

			foreach (var text in stringsToInclude)
			{
				if (text != null && text.Length > 0 && text.ToLowerInvariant().Contains(filtertext))
				{
					show = true;
					break;
				}
			}
			return show;
		}



		public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (!init)
			{
				Init();
			}
			CurrentlyActive = show;
		}
	}
}
