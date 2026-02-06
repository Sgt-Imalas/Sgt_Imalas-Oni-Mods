using Database;
using Klei.AI;
using SetStartDupes.DuplicityEditing;
using SetStartDupes.DuplicityEditing.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static SetStartDupes.DupeTraitManager;
using static SetStartDupes.STRINGS.UI.PRESETWINDOW;

namespace SetStartDupes
{
	internal class UnityDuplicitySelectionScreen : FScreen
	{
#pragma warning disable IDE0051 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members
		public static UnityDuplicitySelectionScreen Instance = null;

		public LocText ToReplaceName;
		public Image ToReplaceColour;

		public GameObject PresetListContainer;
		public GameObject PresetListPrefab;
		public GameObject BodypartPrefab;

		public FButton ClearSearchBar;
		public FInputField2 Searchbar;

		public bool CurrentlyActive;

		OpenedFrom openedFrom;
		AccessorySlot openedFromSlot;
		Tag MinionModel;


		Dictionary<Trait, GameObject> TraitContainers = new Dictionary<Trait, GameObject>();
		Dictionary<SkillGroup, GameObject> DupeInterestContainers = new Dictionary<SkillGroup, GameObject>();
		Dictionary<Effect, GameObject> EffectContainers = new Dictionary<Effect, GameObject>();
		Dictionary<AccessorySlot, Dictionary<HashedString, GameObject>> BodypartContainers = new();

		Dictionary<OpenedFrom, List<GameObject>> CategoryEntries = new();

		public enum OpenedFrom
		{
			Undefined = 0,
			Interest,
			Trait,
			Effect,
			Bodypart,

		}


		public static void ShowWindow(Tag minionModel,OpenedFrom from, System.Action<object> OnSelect, System.Action onClose, AccessorySlot accessorySlot = null)
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.TraitsWindowPrefab, DuplicityMainScreen.Instance.transform.parent.gameObject, true);
				Instance = screen.AddOrGet<UnityDuplicitySelectionScreen>();
				Instance.Init();
			}
			Instance.OnSelect = OnSelect;
			Instance.SetOpenedType(minionModel,from, accessorySlot);
			Instance.Show(true);
			Instance.ConsumeMouseScroll = true;
			Instance.transform.SetAsLastSibling();
			Instance.OnCloseAction = onClose;
			Instance.Searchbar.Text = string.Empty;

		}

		private bool init;
		private System.Action OnCloseAction;
		private System.Action<object> OnSelect;


		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
			{
				this.Show(false);
			}
			if (e.TryConsume(Action.Find))
			{
				Searchbar.ExternalStartEditing();
			}

			base.OnKeyDown(e);
		}
		private void SetOpenedType(Tag minionModel,OpenedFrom from = OpenedFrom.Undefined, AccessorySlot slot = null)
		{
			MinionModel = minionModel;
			openedFrom = from;
			openedFromSlot = slot;
			foreach (var cat in CategoryEntries)
			{
				bool enable = cat.Key == openedFrom;

				if (!enable)
				{
					foreach (var entry in cat.Value)
					{
						entry.SetActive(false);
					}
				}
			}

			ApplyFilter();
		}


		string GetTraitName(Trait trait)
		{
			return trait.Name;
		}

		private GameObject AddUIContainer(SkillGroup group)
		{
			if (group != null && !DupeInterestContainers.ContainsKey(group))
			{
				DupeInterestContainers[group] = AddUiContainer(
				ModAssets.GetSkillgroupName(group),
				ModAssets.GetSkillgroupDescription(group),
				() => SelectItem(group.Id));
				return DupeInterestContainers[group];
			}
			return null;
		}

		private GameObject AddUIContainer(Trait trait2, NextType next)
		{
			if (trait2 != null && !TraitContainers.ContainsKey(trait2))
			{
				TraitContainers[trait2] = AddUiContainer(
				GetTraitName(trait2),
				ModAssets.GetTraitTooltip(trait2, trait2.Id),
				() => SelectItem(trait2.Id),
				 ModAssets.GetColourFromType(next));
				return TraitContainers[trait2];
			}
			return null;

		}



		private GameObject AddUiContainer(string name, string description, System.Action onClickAction, Color overrideColor = default, GameObject prefabOverride = null, Sprite placeImage = null)
		{
			if (prefabOverride == null)
				prefabOverride = PresetListPrefab;

			var PresetHolder = Util.KInstantiateUI(prefabOverride, PresetListContainer, true);

			UIUtils.TryChangeText(PresetHolder.transform, "Label", name);
			if (description != null && description.Length > 0)
			{
				UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("Label"), description, true, onBottom: true);
			}
			if (placeImage != null)
			{
				var image = PresetHolder.transform.Find("Image").FindOrAddComponent<Image>();
				image.sprite = placeImage;
				UnityEngine.Rect rect = image.sprite.rect;
				if (rect.width > rect.height)
				{
					var size = (rect.height / rect.width) * 55;
					image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
				}
				else
				{
					var size = (rect.width / rect.height) * 55;
					image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
				}
			}

			PresetHolder.transform.FindOrAddComponent<FButton>().OnClick += onClickAction;
			if (overrideColor != default)
				PresetHolder.transform.Find("Background").FindOrAddComponent<Image>().color = overrideColor;

			return PresetHolder;
		}

		private void SelectItem(object obj)
		{
			OnSelect(obj);
			if (OnCloseAction != null)
				this.OnCloseAction.Invoke();
			this.Show(false);
		}

		public string SkillGroup(SkillGroup group)
		{
			return Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + group.relevantAttributes.First().Id.ToUpperInvariant() + ".NAME");
		}

		private void Init()
		{
			if (init) { return; }
			SgtLogger.l("Initializing DupeEditorSelectionWindow");

			transform.Find("ToReplace").gameObject.SetActive(false);
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

			SgtLogger.Assert("PresetListPrefab was null", PresetListPrefab);
			SgtLogger.Assert("PresetListContainer was null", PresetListContainer);
			SgtLogger.l("initializing ListEntries");

			BodypartPrefab = transform.Find("ScrollArea/Content/DupeSkinPartPrefab").gameObject;
			BodypartPrefab.SetActive(false);
			//hairEntry.gameObject.SetActive(false);
			InitAllContainers();


			init = true;
		}

		private void InitAllContainers()
		{
			CategoryEntries.Add(OpenedFrom.Interest, new());
			CategoryEntries.Add(OpenedFrom.Trait, new());
			CategoryEntries.Add(OpenedFrom.Effect, new());
			CategoryEntries.Add(OpenedFrom.Bodypart, new());


			var traitsDb = Db.Get().traits;
			var interests = Db.Get().SkillGroups.resources;

			var orderedNextTypes = new List<NextType>()
			{
				NextType.joy,
				NextType.stress,
				NextType.special,
				NextType.geneShufflerTrait,
				NextType.posTrait,
				NextType.bionic_boost,
				NextType.needTrait,
				NextType.bionic_bug,
				NextType.negTrait,
				NextType.undefined,
			};

			foreach (var type in orderedNextTypes)
			{
				if (type == NextType.allTraits) continue;

				var TraitsOfCategory = ModAssets.TryGetTraitsOfCategory(type, null);
				foreach (var item in TraitsOfCategory)
				{
					if (ModAssets.TraitAllowedInCurrentDLC(item))
						CategoryEntries[OpenedFrom.Trait].Add(AddUIContainer(traitsDb.TryGet(item.id), type));
					else
						SgtLogger.l(item.id, "Filtered, not active dlc");

				}
			}
			foreach (var item in interests)
			{
				if(item.choreGroupID == null) continue;

				CategoryEntries[OpenedFrom.Interest].Add(AddUIContainer(item));
			}
			HashSet<string> validMinionAttributes = new(AttributeHelper.GetValidMinionAttributeIDs());
			foreach (var effect in Db.Get().effects.resources)
			{
				//filter out any effects that 
				if (effect.SelfModifiers.Count == 0 || effect.SelfModifiers.Any(modifier => !validMinionAttributes.Contains(modifier.AttributeId)))
				{
					//SgtLogger.l("skipping effect " + effect.Id + ": " + effect.Name);
					continue;
				}

				if (effect != null && !EffectContainers.ContainsKey(effect))
				{
					string effectName = effect.Name.Contains("MISSING.STRINGS") ? effect.Id : effect.Name;

					EffectContainers[effect] = AddUiContainer(
					effectName,
					effect.description,
					() => SelectItem(effect.Id));
					CategoryEntries[OpenedFrom.Effect].Add(EffectContainers[effect]);
				}
			}
			foreach (var slot in AccessorySlotHelper.GetAllChangeableSlot())
			{
				BodypartContainers[slot] = new Dictionary<HashedString, GameObject>();
				foreach (var accessory in slot.accessories)
				{
					if (!BodypartContainers[slot].ContainsKey(accessory.IdHash))
					{
						BodypartContainers[slot].Add(accessory.IdHash,
							AddUiContainer(
								accessory.Id,
								"",
								() => SelectItem(accessory),
								prefabOverride: BodypartPrefab,
								placeImage: AccessorySlotHelper.GetSpriteFrom(accessory.symbol, accessory.slot)
							));
						CategoryEntries[OpenedFrom.Bodypart].Add(BodypartContainers[slot][accessory.Id]);
					}
				}
			}
		}


		public void ApplyFilter(string filterstring = "")
		{
			if (openedFrom == OpenedFrom.Interest)
			{
				var forbidden = DuplicityMainScreen.Instance.CurrentInterestIDs();// new List<SkillGroup>();// ReferencedStats.skillAptitudes.Keys.ToList();
				foreach (var go in DupeInterestContainers)
				{
					bool notYetAdded = !forbidden.Contains(go.Key.Id);
					go.Value.SetActive(filterstring == string.Empty ? notYetAdded : notYetAdded && ShowInFilter(filterstring, go.Key.Name));
				}
			}
			else if (openedFrom == OpenedFrom.Trait)
			{
				var allowedTraits = GetAllowedTraits();
				foreach (var go in TraitContainers)
				{
					bool Contained = allowedTraits.Contains(go.Key.Id);
					go.Value.SetActive(filterstring == string.Empty ? Contained : Contained && ShowInFilter(filterstring, new string[] { go.Key.Name, go.Key.description }));
				}
			}
			else if (openedFrom == OpenedFrom.Effect)
			{
				var forbiddenEffects = DuplicityMainScreen.Instance.CurrentEffectIDs();
				foreach (var go in EffectContainers)
				{
					bool notYetAdded = !forbiddenEffects.Contains(go.Key.Id);
					go.Value.SetActive(filterstring == string.Empty ? notYetAdded : notYetAdded && ShowInFilter(filterstring, new string[] { go.Key.Name, go.Key.description }));
				}
			}
			else if (openedFrom == OpenedFrom.Bodypart)
			{
				foreach (var kvp in BodypartContainers)
				{
					foreach (var entry in kvp.Value)
					{
						if (openedFromSlot != kvp.Key)
						{
							entry.Value.SetActive(false);
						}
						else
						{
							entry.Value.SetActive(ShowInFilter(filterstring, new string[] { entry.Key.ToString() }));
						}
					}
				}
			}

		}

		bool ShowInFilter(string filtertext, string stringsToInclude)
		{
			return ShowInFilter(filtertext, new string[] { stringsToInclude });
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


		List<string> GetAllowedTraits()
		{
			var traits = ModAssets.TryGetTraitsOfCategory(NextType.allTraits, MinionModel, overrideShowAll: true);

			DuplicityMainScreen.Instance.ReactionInfo(out var hasJoy, out var hasStress);

			if (!hasJoy)
				traits = ModAssets.TryGetTraitsOfCategory(NextType.joy, MinionModel).Concat(traits).ToList();
			if (!hasStress)
				traits = ModAssets.TryGetTraitsOfCategory(NextType.stress, MinionModel).Concat(traits).ToList();


			var allowedTraits = traits.Select(t => t.id).ToList();
			var finalTraits = new List<string>();
			var forbiddenTraits = DuplicityMainScreen.Instance.CurrentTraitIDs();
			//ReferencedStats.Traits.Count > 0 ? ReferencedStats.Traits.Select(allowedTraits => allowedTraits.Id).ToList() : 

			foreach (string existing in allowedTraits)
			{
				if (!forbiddenTraits.Contains(existing))
				{
					finalTraits.Add(existing);
				}
			}
			return finalTraits;
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
