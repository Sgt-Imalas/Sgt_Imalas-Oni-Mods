using Database;
using Klei.AI;
using SetStartDupes.DuplicityEditing.Helpers;
using SetStartDupes.DuplicityEditing.ScreenComponents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using static SetStartDupes.DupeTraitManager;
using static STRINGS.DUPLICANTS;
using static STRINGS.UI;
using static STRINGS.UI.TOOLS;

namespace SetStartDupes.DuplicityEditing
{
	internal class DuplicityMainScreen : FScreen
	{
		public enum Tab
		{
			undefined,
			Attributes,
			Appearance,
			Health,
			Skills,
			Effects
		}

#pragma warning disable IDE0051 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members


		public static DuplicityMainScreen Instance = null;
		ConfirmDialogScreen ConfirmDialog = null;



		public bool CurrentlyActive;

		GameObject MinionButtonContainer, MinionButtonPrefab;
		Dictionary<MinionAssignablesProxy, MinionSelectButton> MinionButtons = new Dictionary<MinionAssignablesProxy, MinionSelectButton>();
		Dictionary<Tab, List<GameObject>> CategoryGameObjects = new Dictionary<Tab, List<GameObject>>();
		MinionAssignablesProxy SelectedMinion;
		public DuplicantEditableStats Stats;

		Dictionary<Tab, FToggleButton> Tabs = new Dictionary<Tab, FToggleButton>();
		Tab lastCategory = Tab.undefined;

		//Preset buttons
		FButton OpenCrewPresets, OpenStatPresets;

		//Prefabs:
		NumberInput NumberInputPrefabWide, NumberInputPrefab;
		HeaderMain HeaderMainPrefab;
		HeaderDescriptor HeaderDescriptorPrefab;
		CheckboxInput CheckboxInputPrefab;
		SliderInput SliderInputPrefab;
		DeletableNumberInputListEntry DeletableNumberInputListEntryPrefab;
		AppearanceEntry AppearanceEntryPrefab;

		GameObject ParentContainer;

		//Details Header:
		LocText HeaderLabel;

		//Attribute-Tab:
		Dictionary<Klei.AI.Attribute, NumberInput> attributeEditors;
		Dictionary<string, DeletableListEntry> TraitEntries = new();
		Dictionary<HashedString, DeletableListEntry> AptitudeEntries = new();
		FButton AddNewTrait, AddNewAptitude;
		GameObject TraitContainer, AptitudeContainer;
		DeletableListEntry TraitPrefab, AptitudePrefab;


		//Health-Tab:
		Dictionary<Amount, SliderInput> AmountSliders = new();

		//Skills-Tab:
		NumberInput XP;
		Dictionary<Skill, CheckboxInput> SkillToggles = new();

		//Effects-Tab:
		Dictionary<string, DeletableNumberInputListEntry> EffectEntries = new();
		FButton AddNewEffectButton;

		//Appearance-Tab:
		public GameObject AnimEntryContainer;
		Dictionary<AccessorySlot, AppearanceEntry> MinionAnimCategories = new();

		//Footer
		FButton CloseBtn, ResetBtn, SaveBtn, CleanSlateBtn;

		public static GameObject ParentGO => GameScreenManager.Instance.transform.Find("ScreenSpaceOverlayCanvas/MiddleCenter - InFrontOfEverything").gameObject;

		public static void ShowWindow(GameObject SourceDupe, System.Action onClose)
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.DuplicityWindowPrefab, ParentGO, true);
				Instance = screen.AddOrGet<DuplicityMainScreen>();
				Instance.Init();
				Instance.name = "DSS_DuplicityEditor_MainScreen";
			}
			//Instance.SetOpenedType(currentGroup, currentTrait, DupeTraitManager, openedFrom);
			Instance.Show(true);
			Instance.ConsumeMouseScroll = true;
			Instance.transform.SetAsLastSibling();
			Instance.OnCloseAction = onClose;

			Instance.UpdateMinionButtons(true);
			if (SourceDupe.TryGetComponent<MinionIdentity>(out var identity))
			{
				Instance.TryChangeMinion(identity.assignableProxy.Get());
			}
		}

		private bool init;
		private System.Action OnCloseAction;


		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.Consumed)
				return;

			if (e.TryConsume(Action.MouseRight) || e.TryConsume(Action.Escape))
			{
				if (HasOpenDialogue())
					CloseConfirmDialog();
				else
					TryClose();
				//Debug.Log("consumed closing action");
			}
			base.OnKeyDown(e);
		}

		void GenerateMinionEditStats(MinionAssignablesProxy minion)
		{
			SelectedMinion = minion;
			Stats = DuplicantEditableStats.GenerateFromMinion(minion);
			HeaderLabel.SetText(string.Format(STRINGS.UI.DUPEEDITING.DETAILS.HEADER.LABEL_FILLED, SelectedMinion.GetProperName()));
			UpdateMinionButtons();
			RefreshDetailsUI();
		}
		
		void TryClose()
		{
			if (PendingChanges() && !HasOpenDialogue())
			{
				ConfirmDialog = DialogUtil.CreateConfirmDialog(STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.TITLE,
				   STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.TEXT,
			STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.APPLYCHANGES,
				   () => ApplyAndClose(),
			STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.DISCARDCHANGES,
				   () => DiscardAndClose(),
				   STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.CANCEL,
				   () => { }, parent: ParentGO);
			}
			else
				DiscardAndClose();

		}
		void ClearAll()
		{
			Stats.ClearAll();
			RefreshDetailsUI();
		}

		void ApplyAndClose()
		{
			Stats.Apply(SelectedMinion);
			Stats = null;
			Show(false);
		}
		void DiscardAndClose()
		{
			Stats = null;
			Show(false);
		}

		private bool PendingChanges() => Stats != null && Stats.EditsPending;

		private void Init()
		{
			if (init) { return; }
			SgtLogger.l("Initializing Duplicity Dupe editing");
			MinionButtonContainer = transform.Find("Categories/Content/ScrollRectContainer").gameObject;
			MinionButtonPrefab = MinionButtonContainer.transform.Find("Item").gameObject;
			MinionButtonPrefab.SetActive(false);

			SaveBtn = transform.Find("Details/Footer/Buttons/SaveChangesButton").gameObject.AddOrGet<FButton>();
			SaveBtn.OnClick += () => Stats.Apply(SelectedMinion);

			ResetBtn = transform.Find("Details/Footer/Buttons/ResetButton").gameObject.AddOrGet<FButton>();
			ResetBtn.OnClick += () => GenerateMinionEditStats(SelectedMinion);

			CloseBtn = transform.Find("Details/Footer/Buttons/ExitButton").gameObject.AddOrGet<FButton>();
			CloseBtn.OnClick += TryClose;

			CleanSlateBtn = transform.Find("Details/Footer/Buttons/CleanSlateButton").gameObject.AddOrGet<FButton>();
			UIUtils.AddSimpleTooltipToObject(CleanSlateBtn.gameObject, STRINGS.UI.DUPEEDITING.DETAILS.FOOTER.BUTTONS.CLEANSLATEBUTTON.TOOLTIP);
			CleanSlateBtn.OnClick += ClearAll;

			InitPrefabs();

			InitTabs();
			init = true;
		}
		private void InitPrefabs()
		{
			NumberInputPrefabWide = transform.Find("Details/Content/ScrollRectContainer/NumberInputPrefabWide").FindOrAddComponent<NumberInput>();
			NumberInputPrefabWide.gameObject.SetActive(false);

			NumberInputPrefab = transform.Find("Details/Content/ScrollRectContainer/NumberInputPrefab").FindOrAddComponent<NumberInput>();
			NumberInputPrefab.gameObject.SetActive(false);

			HeaderMainPrefab = transform.Find("Details/Content/ScrollRectContainer/HeaderMain").FindOrAddComponent<HeaderMain>();
			HeaderMainPrefab.gameObject.SetActive(false);

			HeaderDescriptorPrefab = transform.Find("Details/Content/ScrollRectContainer/HeaderDescriptor").FindOrAddComponent<HeaderDescriptor>();
			HeaderDescriptorPrefab.gameObject.SetActive(false);

			CheckboxInputPrefab = transform.Find("Details/Content/ScrollRectContainer/CheckboxPrefab").FindOrAddComponent<CheckboxInput>();
			CheckboxInputPrefab.gameObject.SetActive(false);

			SliderInputPrefab = transform.Find("Details/Content/ScrollRectContainer/SliderPrefab").FindOrAddComponent<SliderInput>();
			SliderInputPrefab.gameObject.SetActive(false);

			DeletableNumberInputListEntryPrefab = transform.Find("Details/Content/ScrollRectContainer/DeletableNumberInputPrefab").FindOrAddComponent<DeletableNumberInputListEntry>();
			DeletableNumberInputListEntryPrefab.gameObject.SetActive(false);

			AppearanceEntryPrefab = transform.Find("Details/Content/ScrollRectContainer/Appearence/Item").gameObject.AddOrGet<AppearanceEntry>();
			AppearanceEntryPrefab.gameObject.SetActive(false);

			ParentContainer = transform.Find("Details/Content/ScrollRectContainer").gameObject;

			HeaderLabel = transform.Find("Details/Header/Label").GetComponent<LocText>();

			OpenCrewPresets = transform.Find("Categories/Header/ExportPreset").gameObject.AddOrGet<FButton>();
			OpenCrewPresets.OnClick += OpenCrewPresetScreen;
			OpenStatPresets = transform.Find("Details/Header/ExportPreset").gameObject.AddOrGet<FButton>();
			OpenStatPresets.OnClick += OpenStatPresetScreen;


			//temp:
			//transform.Find("Details/Content/ScrollRectContainer/Appearence").gameObject.SetActive(false);
			transform.Find("Details/Content/ScrollRectContainer/SingleListPrefab").gameObject.SetActive(false);
			//transform.Find("Details/Content/ScrollRectContainer/TraitInterestContainer").gameObject.SetActive(false);
		}

		void OpenStatPresetScreen()
		{
			ModAssets.ParentScreen = this.transform.parent.gameObject;
			UnityPresetScreen.ShowWindow(null, Stats, RefreshDetailsUI);
		}
		void OpenCrewPresetScreen()
		{
			ModAssets.ParentScreen = this.transform.parent.gameObject;
			UnityCrewPresetScreen.ShowWindow(null,RefreshDetailsUI);
		}

		private void InitTabs()
		{
			Tabs.Add(Tab.Attributes, transform.Find("Details/Header/Buttons/AttributeButton").FindOrAddComponent<FToggleButton>());
			Tabs.Add(Tab.Appearance, transform.Find("Details/Header/Buttons/AppearanceButton").FindOrAddComponent<FToggleButton>());
			Tabs.Add(Tab.Health, transform.Find("Details/Header/Buttons/HealthButton").FindOrAddComponent<FToggleButton>());
			Tabs.Add(Tab.Skills, transform.Find("Details/Header/Buttons/SkillsButton").FindOrAddComponent<FToggleButton>());
			Tabs.Add(Tab.Effects, transform.Find("Details/Header/Buttons/EffectsButton").FindOrAddComponent<FToggleButton>());

			foreach (var tab in Tabs)
			{
				tab.Value.OnClick += () => ShowCategory(tab.Key);
			}
			foreach (var category in Tabs.Keys)
			{
				CategoryGameObjects[category] = new List<GameObject>();
			}
			InitAttributeTab();
			InitAppearanceTab();
			InitHealthTab();
			InitSkillsTab();
			InitEffectsTab();

		}
		void TrySetSkillLearned(Skill target, bool learned)
		{
			Stats?.SetSkillLearned(target, learned);
		}
		void TryChangeAmount(float newAmount, Amount target)
		{
			Stats?.SetAmount(target, newAmount);
		}
		void TryChangeXP(string number)
		{
			if (!float.TryParse(number, out var newVal))
			{
				XP.SetInputFieldValue("0");
				return;
			}

			Stats?.SetExperience(newVal);
		}
		void TryChangeEffectLength(string effectID, string number)
		{
			if (!float.TryParse(number, out var newVal))
			{
				return;
			}
			if (newVal <= 0)
				newVal = 1;

			Stats?.EditEffect(effectID, newVal);
			RebuildEffects();
		}

		void TryChangeAttribute(string number, Klei.AI.Attribute attribute)
		{
			if (!float.TryParse(number, out var newVal))
			{
				attributeEditors[attribute].SetInputFieldValue("0");
				return;
			}

			if (newVal > 10000) newVal = 10000;

			Stats?.SetAttributeLevel(attribute, Mathf.RoundToInt(newVal));
		}
		void InitEffectsTab()
		{
			//header
			var tableHeader = Util.KInstantiateUI<HeaderDescriptor>(HeaderDescriptorPrefab.gameObject, ParentContainer);
			tableHeader.TextLeft = STRINGS.UI.DUPEEDITING.DETAILS.CONTENT.SCROLLRECTCONTAINER.EFFECTS.EFFECT;
			tableHeader.TextRight = STRINGS.UI.DUPEEDITING.DETAILS.CONTENT.SCROLLRECTCONTAINER.EFFECTS.TIMEREMAINING;
			CategoryGameObjects[Tab.Effects].Add(tableHeader.gameObject);

			//AddNewButton

			AddNewEffectButton = transform.Find("Details/Content/ScrollRectContainer/NewButtonPrefab").gameObject.AddOrGet<FButton>();
			AddNewEffectButton.OnClick += () => UnityDuplicitySelectionScreen.ShowWindow(UnityDuplicitySelectionScreen.OpenedFrom.Effect, (obj) => OnAddEffect((string)obj), RebuildEffects);
			CategoryGameObjects[Tab.Effects].Add(AddNewEffectButton.gameObject);
		}
		void InitAppearanceTab()
		{
			AnimEntryContainer = transform.Find("Details/Content/ScrollRectContainer/Appearence").gameObject;
			CategoryGameObjects[Tab.Appearance].Add(AnimEntryContainer);
			foreach (var slot in AccessorySlotHelper.GetAllChangeableSlot())
			{
				var entry = Util.KInstantiateUI<AppearanceEntry>(AppearanceEntryPrefab.gameObject, AnimEntryContainer);
				entry.CategoryText = slot.Id;
				entry.OnEntryClicked = () => UnityDuplicitySelectionScreen.ShowWindow(UnityDuplicitySelectionScreen.OpenedFrom.Bodypart, (obj) => OnChangeAccessory(slot, obj), () => RebuildAccessories(), slot);
				entry.gameObject.SetActive(true);
				MinionAnimCategories[slot] = entry;
				CategoryGameObjects[Tab.Appearance].Add(entry.gameObject);
			}
		}
		void OnChangeAccessory(AccessorySlot slot, object newAccessoryObj)
		{
			if (newAccessoryObj is Accessory accessory)
			{
				Stats.SetAccessory(slot, accessory);
			}
			UpdateMinionButtons();
		}

		void InitSkillsTab()
		{
			//xp input
			XP = Util.KInstantiateUI<NumberInput>(NumberInputPrefabWide.gameObject, ParentContainer);
			XP.Text = STRINGS.UI.DUPEEDITING.DETAILS.CONTENT.SCROLLRECTCONTAINER.SKILLS.EXPERIENCE;
			XP.OnInputChanged += (text) => TryChangeXP(text);
			XP.WholeNumbers = false;
			CategoryGameObjects[Tab.Skills].Add(XP.gameObject);

			//header
			var tableHeader = Util.KInstantiateUI<HeaderDescriptor>(HeaderDescriptorPrefab.gameObject, ParentContainer);
			tableHeader.TextLeft = STRINGS.UI.DUPEEDITING.DETAILS.CONTENT.SCROLLRECTCONTAINER.SKILLS.SKILL;
			tableHeader.TextRight = STRINGS.UI.DUPEEDITING.DETAILS.CONTENT.SCROLLRECTCONTAINER.SKILLS.MASTERY;
			CategoryGameObjects[Tab.Skills].Add(tableHeader.gameObject);

			foreach (var skill in SkillHelper.GetAllSkills())
			{
				var input = Util.KInstantiateUI<CheckboxInput>(CheckboxInputPrefab.gameObject, ParentContainer);
				input.Text = skill.Name;
				input.OnCheckboxToggled += (bool skillActive) => TrySetSkillLearned(skill, skillActive);
				SkillToggles[skill] = input;
				CategoryGameObjects[Tab.Skills].Add(input.gameObject);
			}
		}
		void InitHealthTab()
		{
			foreach (Amount amount in AmountHelper.GetAllEditableAmounts())
			{
				var input = Util.KInstantiateUI<SliderInput>(SliderInputPrefab.gameObject, ParentContainer);
				input.Text = amount.Name;
				input.wholeNumbers = false;
				input.SetMinMaxCurrent(amount.minAttribute.BaseValue, amount.maxAttribute.BaseValue, amount.minAttribute.BaseValue);
				input.OnSliderValueChanged += (newVal) => TryChangeAmount(newVal, amount);
				input.TrailingNumbersCount = 2;

				AmountSliders[amount] = input;
				CategoryGameObjects[Tab.Health].Add(input.gameObject);
			}
		}
		void InitAttributeTab()
		{
			var TraitsInterestContainer = transform.Find("Details/Content/ScrollRectContainer/TraitInterestContainer").gameObject;
			TraitsInterestContainer.SetActive(true);

			AptitudeContainer = TraitsInterestContainer.transform.Find("Content/grp/InterestContainer/ScrollArea/Content").gameObject;
			TraitContainer = TraitsInterestContainer.transform.Find("Content/grp/TraitContainer/ScrollArea/Content").gameObject;

			TraitPrefab = TraitContainer.transform.Find("ListViewEntryPrefab").gameObject.AddOrGet<DeletableListEntry>();
			TraitPrefab.gameObject.SetActive(false);

			AptitudePrefab = AptitudeContainer.transform.Find("ListViewEntryPrefab").gameObject.AddOrGet<DeletableListEntry>();
			AptitudePrefab.gameObject.SetActive(false);

			TraitsInterestContainer.SetActive(false);
			TraitsInterestContainer.transform.SetAsFirstSibling();
			CategoryGameObjects[Tab.Attributes].Add(TraitsInterestContainer);

			attributeEditors = new();

			AddNewTrait = TraitsInterestContainer.transform.Find("Content/grp2/AddTraitButton").gameObject.AddOrGet<FButton>();
			AddNewTrait.OnClick += () => UnityDuplicitySelectionScreen.ShowWindow(UnityDuplicitySelectionScreen.OpenedFrom.Trait, (obj) => OnAddTrait((string)obj), () => RebuildTraitsAptitudes());
			AddNewAptitude = TraitsInterestContainer.transform.Find("Content/grp2/AddInterestButton").gameObject.AddOrGet<FButton>();
			AddNewAptitude.OnClick += () => UnityDuplicitySelectionScreen.ShowWindow(UnityDuplicitySelectionScreen.OpenedFrom.Interest, (obj) => OnAddAptitude((string)obj), () => RebuildTraitsAptitudes());

			foreach (var attribute in AttributeHelper.GetEditableAttributes(null))
			{
				var attributeInput = Util.KInstantiateUI<NumberInput>(NumberInputPrefab.gameObject, ParentContainer);
				attributeInput.Text = attribute.Name;
				attributeInput.OnInputChanged += (text) => TryChangeAttribute(text, attribute);
				attributeEditors[attribute] = attributeInput;

				CategoryGameObjects[Tab.Attributes].Add(attributeInput.gameObject);
			}
		}

		private void ShowCategory(Tab key)
		{
			lastCategory = key;
			foreach (var tab in Tabs)
			{
				tab.Value.SetIsSelected(tab.Key == key);
			}

			foreach (var tabtype in (Tab[])Enum.GetValues(typeof(Tab)))
			{
				if (CategoryGameObjects.ContainsKey(tabtype))
				{
					bool setActive = tabtype == key;

					foreach (var item in CategoryGameObjects[tabtype])
					{
						item.SetActive(setActive);
					}
				}
			}


			switch (key)
			{
				default:
					break;

				case Tab.Attributes:
					RefreshAttributeTab();
					break;
				case Tab.Health:
					RefreshHealthTab();
					break;
				case Tab.Skills:
					RefreshSkillsTab();
					break;
				case Tab.Appearance:
					RebuildAccessories();
					break;
				case Tab.Effects:
					RebuildEffects();
					break;
			}
		}

		public List<HashedString> CurrentInterestIDs()
		{
			var list = new List<HashedString>();
			if (Stats != null)
			{
				list.AddRange(Stats.AptitudeBySkillGroup.Keys);
			}

			return list;
		}
		public List<string> CurrentEffectIDs()
		{
			var list = new List<string>();
			if (Stats != null)
			{
				list.AddRange(Stats.Effects.Keys);
			}

			return list;
		}

		public List<string> CurrentTraitIDs()
		{
			var list = new List<string>();
			if (Stats != null)
				list.AddRange(Stats.Traits);

			return list;
		}

		public void ReactionInfo(out bool hasJoy, out bool hasStress)
		{
			hasJoy = true;
			hasStress = true;

			if (Stats != null)
			{
				hasJoy = Stats.HasJoyTrait;
				hasStress = Stats.HasStressTrait;
			}
		}
		private void RefreshHealthTab()
		{
			SgtLogger.Assert("stats were null", Stats);
			if (Stats == null)
				return;
			foreach (var amount in AmountHelper.GetAllEditableAmounts())
			{
				bool isValidAmount = AmountHelper.IsValidModelAmount(amount,Stats.Model);

				AmountSliders[amount].gameObject.SetActive(isValidAmount);

				if (!isValidAmount)
					continue;

				var instance = amount.Lookup(SelectedMinion.GetTargetGameObject());
				AmountSliders[amount].SetMinMaxCurrent(instance.GetMin(), instance.GetMax(), instance.value);
			}
		}
		private void RefreshSkillsTab()
		{
			SgtLogger.Assert("stats were null", Stats);
			if (Stats == null)
				return;
			XP.SetInputFieldValue(Stats.GetExperience().ToString());

			foreach(var toggle in SkillToggles.Values)
			{
				toggle.gameObject.SetActive(false);
			}

			foreach (var skill in SkillHelper.GetAllSkills(Stats.Model.ToString()))
			{
				SkillToggles[skill].gameObject.SetActive(true);

                SkillToggles[skill].SetCheckboxValue(Stats.HasMasteredSkill(skill));
			}
		}
		private void RefreshAttributeTab()
		{
			SgtLogger.Assert("stats were null", Stats);
			if (Stats == null)
				return;
			RebuildTraitsAptitudes();

			foreach(var attribute in attributeEditors)
			{
				attribute.Value.gameObject.SetActive(false);
			}

			foreach (var attribute in AttributeHelper.GetEditableAttributes(Stats.Model))
			{
                attributeEditors[attribute].gameObject.SetActive(true);
                attributeEditors[attribute].SetInputFieldValue(Stats.GetAttributeLevel(attribute).ToString());
			}
		}


		private void RebuildEffects()
		{
			foreach (var entry in EffectEntries.Values)
			{
				entry.gameObject.SetActive(false);
			}
			if (Stats == null)
				return;
			foreach (var effect in Stats.Effects)
			{
				var effectContainer = AddOrGetEffectContainer(effect.Key);
				if (effectContainer == null)
					continue;

				effectContainer.gameObject.SetActive(true);

				if (effect.Value > 0)
				{
					effectContainer.SetInputFieldVisibility(true);
					effectContainer.SetInputFieldValue(effect.Value.ToString());
				}
				else
				{
					effectContainer.SetInputFieldVisibility(false);
				}

				effectContainer.transform.SetAsLastSibling();
			}
			AddNewEffectButton.transform.SetAsLastSibling();

		}
		private void RebuildAccessories()
		{
			if (Stats == null)
				return;
			foreach (var entry in MinionAnimCategories)
			{
				entry.Value.SetItemIcon(Assets.GetSprite("unknown"));
				entry.Value.SetItemName("None");

			}

			foreach (var slotEntry in Stats.Accessories)
			{
				if (MinionAnimCategories.ContainsKey(slotEntry.Key))
				{
					var entry = MinionAnimCategories[slotEntry.Key];
					entry.SetItemIcon(AccessorySlotHelper.GetSpriteFrom(slotEntry.Value.symbol, slotEntry.Key));
					entry.SetItemName(slotEntry.Value.Name);
				}
			}

		}
		private void RebuildTraitsAptitudes()
		{
			foreach (var traitEntry in TraitEntries.Values)
			{
				traitEntry.gameObject.SetActive(false);
			}
			foreach (var traitEntry in AptitudeEntries.Values)
			{
				traitEntry.gameObject.SetActive(false);
			}
			if (Stats == null)
				return;

			if (Stats.HasStressTrait)
			{
				var stress = AddOrGetTraitContainer(Stats.StressTraitId);
				stress.gameObject.SetActive(true);
				stress.transform.SetAsFirstSibling();
			}

			if (Stats.HasJoyTrait)
			{
				var joy = AddOrGetTraitContainer(Stats.JoyTraitId);
				joy.gameObject.SetActive(true);
				joy.transform.SetAsFirstSibling();
			}

			foreach (var trait in Stats.Traits)
			{
				if (ModAssets.IsMinionBaseTrait(trait))
					continue;

				var traitInfo = AddOrGetTraitContainer(trait);
				traitInfo.gameObject.SetActive(true);
			}
			foreach (var apt in Stats.AptitudeBySkillGroup.Keys)
			{
				var aptitudeInfo = AddOrGetAptitudeContainer(apt);
				aptitudeInfo.gameObject.SetActive(true);
			}
		}
		DeletableListEntry AddOrGetTraitContainer(string traitID)
		{
			var trait = Db.Get().traits.TryGet(traitID);
			if (trait == null)
			{
				SgtLogger.error("trait with the id " + traitID + " not found!");
				return null;
			}

			if (!TraitEntries.ContainsKey(traitID))
			{
				var Type = ModAssets.GetTraitListOfTrait(traitID);

				var go = Util.KInstantiateUI(TraitPrefab.gameObject, TraitContainer);
				var entry = go.AddOrGet<DeletableListEntry>();
				entry.Text = trait.Name;
				entry.Tooltip = ModAssets.GetTraitTooltip(trait, trait.Id);
				entry.backgroundColor = ModAssets.GetColourFromType(Type);
				if (Type == NextType.undefined || Type == NextType.special )
				{
					entry.HideDelete = true;
				}
				else
				{
					entry.OnDeleteClicked = () => OnRemoveTrait(traitID);
				}
				go.SetActive(true);
				TraitEntries[traitID] = entry;
			}

			return TraitEntries[traitID];
		}


		DeletableNumberInputListEntry AddOrGetEffectContainer(string effectId)
		{
			var effectFromDb = Db.Get().effects.TryGet(effectId);
			string effectTooltip = string.Empty, effectName = effectId;

			if (effectFromDb != null)
			{
				effectTooltip = effectFromDb.description;
				effectName = effectFromDb.Name.Contains("MISSING.STRINGS") ? effectFromDb.Id : effectFromDb.Name;
			}
			else
			{
				SgtLogger.warning("effect with the id " + effectId + " not found!");
				if (Strings.TryGet($"STRINGS.DUPLICANTS.MODIFIERS.{effectId.ToUpperInvariant()}.NAME", out var nameEntry))
				{
					effectName = nameEntry.String;
				}
				if (Strings.TryGet($"STRINGS.DUPLICANTS.MODIFIERS.{effectId.ToUpperInvariant()}.TOOLTIP", out var ttEntry))
				{
					effectTooltip = ttEntry.String;
					effectTooltip += "\n\n";
				}
				effectTooltip += STRINGS.UI.DUPEEDITING.DETAILS.CONTENT.SCROLLRECTCONTAINER.EFFECTS.DYNAMIC_EFFECT_TOOLTIP;
			}

			if (!EffectEntries.ContainsKey(effectId))
			{
				var go = Util.KInstantiateUI(DeletableNumberInputListEntryPrefab.gameObject, ParentContainer);
				var entry = go.AddOrGet<DeletableNumberInputListEntry>();
				entry.Text = effectName;
				entry.Tooltip = effectTooltip;
				entry.OnDeleteClicked = () => OnRemoveEffect(effectId);
				entry.OnInputChanged = newVal => TryChangeEffectLength(effectId, newVal);
				go.SetActive(true);
				EffectEntries[effectId] = entry;
				CategoryGameObjects[Tab.Effects].Add(entry.gameObject);
			}
			return EffectEntries[effectId];
		}
		void OnRemoveTrait(string id)
		{
			Stats?.RemoveTrait(id);
			RebuildTraitsAptitudes();
		}
		void OnAddTrait(string id)
		{
			Stats?.AddTrait(id);
			RebuildTraitsAptitudes();
		}


		DeletableListEntry AddOrGetAptitudeContainer(HashedString aptiudeID)
		{
			var aptitude = Db.Get().SkillGroups.TryGet(aptiudeID);
			if (aptitude == null)
			{
				SgtLogger.error("aptitude with the id " + aptitude + " not found!");
				return null;
			}

			if (!AptitudeEntries.ContainsKey(aptiudeID))
			{
				var go = Util.KInstantiateUI(AptitudePrefab.gameObject, AptitudeContainer);
				var entry = go.AddOrGet<DeletableListEntry>();
				entry.Text = ModAssets.GetSkillgroupName(aptitude);
				entry.Tooltip = ModAssets.GetSkillgroupDescription(aptitude);
				entry.OnDeleteClicked = () => OnRemoveAptitude(aptiudeID);
				AptitudeEntries[aptiudeID] = entry;
				go.SetActive(true);
			}

			return AptitudeEntries[aptiudeID];
		}
		void OnRemoveAptitude(HashedString id)
		{
			Stats?.RemoveAptitude(id);
			RebuildTraitsAptitudes();
		}
		void OnAddAptitude(string id)
		{
			Stats?.AddAptitude(id);
			RebuildTraitsAptitudes();
		}
		void OnRemoveEffect(string id)
		{
			Stats?.RemoveEffect(id);
			RebuildEffects();
		}
		void OnAddEffect(string id)
		{
			Stats?.AddEffect(id);
			RebuildEffects();
		}
		bool HasOpenDialogue() => ConfirmDialog != null;
		void CloseConfirmDialog()
		{
			if (ConfirmDialog != null)
			{
				ConfirmDialog.Deactivate();
				ConfirmDialog = null;
			}
		}

		public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (!init)
			{
				Init();
			}
			CurrentlyActive = show;
			ConfirmDialog = null;

			OnResize();
			if (show)
				ScreenResize.Instance.OnResize += () => OnResize();
			else
				ScreenResize.Instance.OnResize -= () => OnResize();

			if (!show)
				SelectedMinion = null;

		}
		public void OnResize()
		{
			var rectMain = this.rectTransform();
			rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, UnityEngine.Screen.width * (1f / (rectMain.lossyScale.x)));
			rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, UnityEngine.Screen.height * (1f / (rectMain.lossyScale.y)));
		}
		private void RefreshDetailsUI()
		{
			if (lastCategory == Tab.undefined)
				lastCategory = Tab.Attributes;
			ShowCategory(lastCategory);
		}

		private void UpdateMinionButtons(bool refreshPortraits = false)
		{
			HashSet<MinionAssignablesProxy> proxies = new HashSet<MinionAssignablesProxy>();
			HashSet<MinionAssignablesProxy> proxiesToRemove = new HashSet<MinionAssignablesProxy>();

			foreach (MinionIdentity minion in Components.LiveMinionIdentities)
			{
				UpdateMinionButton(refreshPortraits, minion);
				proxies.Add(minion.assignableProxy.Get());
			}
			foreach (MinionStorage minionStorage in Components.MinionStorages.Items)
			{
				foreach (MinionStorage.Info info in minionStorage.GetStoredMinionInfo())
				{
					if (info.serializedMinion != null)
					{
						StoredMinionIdentity storedMinionIdentity = info.serializedMinion.Get<StoredMinionIdentity>();
						UpdateMinionButton(refreshPortraits, null, storedMinionIdentity);
						proxies.Add(storedMinionIdentity.assignableProxy.Get());

					}
				}
			}

			//check all buttons if the dupe still exist, remove from list if yes
			foreach (var entry in MinionButtons.Keys)
			{
				if (!proxies.Contains(entry))
				{
					proxiesToRemove.Add(entry);
				}
			}
			//remove minion buttons that no longer exist
			foreach (var entry in proxiesToRemove)
			{
				RemoveMinionButton(entry);
			}
		}

		private MinionSelectButton AddOrGetMinionButton(MinionIdentity identity = null, StoredMinionIdentity identityStored = null)
		{
			MinionAssignablesProxy proxy = identity != null ? identity.assignableProxy.Get() : identityStored.assignableProxy.Get();
			if (!MinionButtons.ContainsKey(proxy))
			{
				GameObject button = Util.KInstantiateUI(MinionButtonPrefab, MinionButtonContainer, true);
				var component = button.AddOrGet<MinionSelectButton>();

				var img = button.transform.Find("Image").gameObject;
				img.SetActive(false);
				var minionImage = Util.KInstantiateUI(MinionPortraitHelper.GetCrewPortraitPrefab(), button, true);
				minionImage.TryGetComponent<MinionPortraitHelper>(out var helper);
				//minionImage.TryGetComponent<LayoutElement>(out var layout);
				//layout.preferredHeight = 60;
				//layout.preferredWidth = 60;

				minionImage.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 4, 55);
				minionImage.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 4, 55);

				//minionImage.useLabels = false;
				//minionImage.SetIdentityObject(proxy, false);
				component.Init(helper, () => TryChangeMinion(proxy));

				MinionButtons[proxy] = component;
			}
			return MinionButtons[proxy];
		}
		private void RemoveMinionButton(MinionAssignablesProxy proxy)
		{
			if (MinionButtons.ContainsKey(proxy))
			{
				UnityEngine.Object.Destroy(MinionButtons[proxy].gameObject);
				MinionButtons.Remove(proxy);
			}
		}
		public void TryChangeMinion(MinionAssignablesProxy newMinion)
		{
			foreach (var btn in MinionButtons)
			{
				btn.Value.SetActiveState(btn.Key == SelectedMinion);
			}


			if (newMinion == SelectedMinion)
				return;

			if (PendingChanges() && !HasOpenDialogue())
			{
				ConfirmDialog = DialogUtil.CreateConfirmDialog(
				   STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.TITLE,
				   STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.TEXT,
			STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.APPLYCHANGES,
				   () =>
				   {
					   Stats.Apply(SelectedMinion);
					   GenerateMinionEditStats(newMinion);
				   },
			STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.DISCARDCHANGES,
				   () => GenerateMinionEditStats(newMinion),
			STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.CANCEL,
				   () => { } ,parent: ParentGO);
			}
			else
			{
				GenerateMinionEditStats(newMinion);
			}

			if (Stats.Model == GameTags.Minions.Models.Bionic)
			{
				AddNewAptitude.SetInteractable(false);
				//AddNewTrait.SetInteractable(false);
			}
			else
			{

				AddNewAptitude.SetInteractable(true);
				//AddNewTrait.SetInteractable(true);
			}
		}

		private List<KeyValuePair<string, string>> GetAccessoryIDs(MinionIdentity identity = null, StoredMinionIdentity identityStored = null)
		{

			List<KeyValuePair<string, string>> accessories = new List<KeyValuePair<string, string>>();
			List<ResourceRef<Accessory>> accessoriesOrigin = new();

			if (identityStored != null)
			{
				accessoriesOrigin = identityStored.accessories;
			}
			else if (identity != null && identity.TryGetComponent<Accessorizer>(out var accessorizer))
			{
				accessoriesOrigin = accessorizer.accessories;
			}

			foreach (var accessory in accessoriesOrigin)
			{
				if (accessory.Get() != null)
				{
					accessories.Add(new KeyValuePair<string, string>(accessory.Get().slot.Id, accessory.Get().Id));
				}
			}
			return accessories;
		}

		private void UpdateMinionButton(bool refreshPortraits, MinionIdentity identity = null, StoredMinionIdentity identityStored = null)
		{
			bool storedMinion = identityStored != null;

			MinionAssignablesProxy proxy = identity != null ? identity.assignableProxy.Get() : identityStored.assignableProxy.Get();

			var button = AddOrGetMinionButton(identity, identityStored);
			if (refreshPortraits)
				button.UpdatePortrait(GetAccessoryIDs(identity, identityStored));

			//button.helper.ForceRefresh(); 
			button.UpdateName(identity, identityStored);
			button.UpdateState(storedMinion, proxy == SelectedMinion);
		}
	}
}
