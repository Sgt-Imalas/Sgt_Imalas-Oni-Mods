using Beached_ModAPI;
using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace SetStartDupes
{
	public class DupeTraitManager : KMonoBehaviour
	{

		public static GameObject AttributeEditPrefab = null;

		#region UI_Handing
		public bool CurrentlyEditing
		{
			get
			{
				return _currentlyEditing;
			}
			set
			{
				_currentlyEditing = value;
				if (value)
				{

				}
			}
		}

		class UI_InterestLogic : KMonoBehaviour
		{
			public SkillGroup SkillGroup;
			public KButton plusButton, minusButton;
			public LocText Label;
		}


		Dictionary<Trait, GameObject> UI_TraitEntries = new Dictionary<Trait, GameObject>();
		Dictionary<SkillGroup, UI_InterestLogic> UI_InterestEntries = new Dictionary<SkillGroup, UI_InterestLogic>();




		GameObject ListEntryButtonPrefab;
		GameObject ListEntryButtonContainer;
		GameObject SpacerPrefab;
		//Cached UI parts:

		GameObject InterestBonusHeaderGO;
		LocText InterestBonusHeader;
		ToolTip interestBonusTooltipCMP;
		string InterestBonusTooltip;


		LocText TraitBalanceHeader;
		ToolTip TraitBalanceTooltipCMP;
		string TraitBalanceTooltip;


		GameObject AttributeContainer;

		GameObject InterestContainer;
		GameObject TraitContainer;
		GameObject OverjoyedContainer;
		GameObject StressContainer;
		GameObject LifeGoalContainer;

		GameObject AddNewTrait;
		GameObject AddNewInterest;




		LocText JoyLabel;
		ToolTip JoyTT;
		LocText StressLabel;
		ToolTip StressTT;
		LocText GoalLabel;
		ToolTip GoalTT;

		private bool _currentlyEditing = false;

		public GameObject InitContainer(string name, string title)
		{
			GameObject go = Util.KInstantiateUI(TraitContainer, TraitContainer.transform.parent.gameObject, true);
			go.name = name;
			UIUtils.TryChangeText(go.transform, "Title", title);
			return go;
		}
		public void InitUI()
		{
			UIUtils.FindAndDestroy(transform, "Top");
			UIUtils.FindAndDestroy(transform, "AttributeScores");
			//UIUtils.FindAndDestroy(transform, "Scroll/Content/TraitsAndAptitudes/AptitudeContainer");
			InterestContainer = transform.Find("Scroll/Content/TraitsAndAptitudes/AptitudeContainer").gameObject;
			TraitContainer = transform.Find("Scroll/Content/TraitsAndAptitudes/TraitContainer").gameObject;


			UIUtils.FindAndDestroy(InterestContainer.transform, "AptitudeGroup", true);
			UIUtils.FindAndDestroy(InterestContainer.transform, "AttributeLabelTrait", true);
			UIUtils.FindAndDestroy(TraitContainer.transform, "TraitGroupGood", true);
			UIUtils.FindAndDestroy(TraitContainer.transform, "TraitGroupBad", true);



			Debug.Assert(TraitContainer, "traitcontainer was null!");

			AttributeContainer = InitContainer("AttributeContainer", global::STRINGS.UI.DETAILTABS.STATS.GROUPNAME_ATTRIBUTES);
			AttributeContainer.transform.SetSiblingIndex(InterestContainer.transform.GetSiblingIndex());
			AttributeContainer.SetActive(false);

			OverjoyedContainer = InitContainer("OverjoyedContainer", string.Format(global::STRINGS.UI.CHARACTERCONTAINER_JOYTRAIT, string.Empty));
			OverjoyedContainer.SetActive(!Config.Instance.NoJoyReactions);

			StressContainer = InitContainer("StressContainer", string.Format(global::STRINGS.UI.CHARACTERCONTAINER_STRESSTRAIT, string.Empty));
			StressContainer.SetActive(!Config.Instance.NoStressReactions);

			LifeGoalContainer = InitContainer("LifeGoalContainer", string.Format(Strings.Get("STRINGS.UI.CHARACTERCONTAINER_LIFEGOAL_TRAIT"), string.Empty));
			LifeGoalContainer.SetActive(ModAssets.Beached_LifegoalsActive);

			if (InterestContainer.transform.gameObject.TryGetComponent<LayoutElement>(out LayoutElement layoutElement))
			{
				layoutElement.preferredHeight = -1;
			}


			//UIUtils.FindAndDestroy(transform, "Scroll/Content/TraitsAndAptitudes/TraitContainer");
			UIUtils.FindAndDestroy(transform, "Scroll/Content/ExpectationsGroupAlt");
			UIUtils.FindAndDestroy(transform, "Scroll/Content/DescriptionGroup");

			ListEntryButtonContainer = transform.Find("Scroll/Content/TraitsAndAptitudes").gameObject;

			Transform overallSize = transform.Find("Scroll");
			overallSize.TryGetComponent<LayoutElement>(out var SizeSetter);
			SizeSetter.flexibleHeight = 600;
			overallSize.TryGetComponent<KScrollRect>(out var scrollerCmp);
			//scrollerCmp.elasticity = 0;
			scrollerCmp.inertia = false;

			ListEntryButtonContainer.TryGetComponent<VerticalLayoutGroup>(out var vlg);
			vlg.spacing = 3;
			vlg.padding = new RectOffset(3, 1, 0, 0);

			ListEntryButtonPrefab = Util.KInstantiateUI(ModAssets.NextButtonPrefab);

			ListEntryButtonPrefab.GetComponent<KButton>().enabled = true;
			var right = Util.KInstantiateUI(ModAssets.RemoveFromTraitsButtonPrefab, ListEntryButtonPrefab);
			UIUtils.TryFindComponent<ToolTip>(right.transform).toolTip = STRINGS.UI.BUTTONS.REMOVEFROMSTATS;
			//UIUtils.TryFindComponent<ToolTip>(right.transform,"Image").toolTip="Cycle to next";
			right.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 2.5f, 25);
			right.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 2.5f, 25);
			right.SetActive(false);

			var AddOnSpacerInterestUP = Util.KInstantiateUI(ModAssets.RemoveFromTraitsButtonPrefab, ListEntryButtonPrefab);
			//UIUtils.TryFindComponent<ToolTip>(prefabParent.transform).toolTip = STRINGS.UI.BUTTONS.ADDTOSTATS;
			//UIUtils.TryFindComponent<ToolTip>(right.transform,"Image").toolTip="Cycle to next";
			AddOnSpacerInterestUP.name = "InterestUP";
			AddOnSpacerInterestUP.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 2.5f, 25);
			AddOnSpacerInterestUP.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 2.5f, 25);
			AddOnSpacerInterestUP.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("icon_positive");
			AddOnSpacerInterestUP.transform.Find("Image").rectTransform().Rotate(new Vector3(0, 0, 180));
			AddOnSpacerInterestUP.SetActive(false);

			var AddOnSpacerInterestDown = Util.KInstantiateUI(ModAssets.RemoveFromTraitsButtonPrefab, ListEntryButtonPrefab);
			//UIUtils.TryFindComponent<ToolTip>(prefabParent.transform).toolTip = STRINGS.UI.BUTTONS.ADDTOSTATS;
			//UIUtils.TryFindComponent<ToolTip>(right.transform,"Image").toolTip="Cycle to next";
			AddOnSpacerInterestDown.name = "InterestDOWN";
			AddOnSpacerInterestDown.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 2.5f, 25);
			AddOnSpacerInterestDown.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 2.5f, 25);
			AddOnSpacerInterestDown.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("icon_negative");
			AddOnSpacerInterestDown.SetActive(false);


			var renameLabel = ListEntryButtonPrefab.transform.Find("SelectLabel");
			if (renameLabel != null)
			{
				renameLabel.name = "Label";
			}
			var selectLabel = ListEntryButtonPrefab.transform.Find("Label");
			ListEntryButtonPrefab.TryGetComponent<LayoutElement>(out var LE);
			LE.minHeight = 30;
			LE.preferredHeight = 30;

			SpacerPrefab = Util.KInstantiateUI(selectLabel.gameObject);
			SpacerPrefab.AddOrGet<LayoutElement>().minHeight = 25;



			InterestBonusHeaderGO = Util.KInstantiateUI(SpacerPrefab, InterestContainer, true);
			InterestBonusHeaderGO.name = "InterestBonusPointInfoHeader";
			InterestBonusHeader = InterestBonusHeaderGO.GetComponent<LocText>();
			interestBonusTooltipCMP = UIUtils.AddSimpleTooltipToObject(InterestBonusHeaderGO.gameObject, "tt");

			AddNewInterest = Util.KInstantiateUI(ModAssets.AddNewToTraitsButtonPrefab, InterestContainer, Config.Instance.AddAndRemoveTraitsAndInterests);
			AddNewInterest.TryGetComponent<LayoutElement>(out var addbtnLE);
			addbtnLE.preferredWidth = 262;
			var imgAddinterest = AddNewInterest.transform.Find("Image").rectTransform();
			imgAddinterest.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (addbtnLE.preferredWidth / 2f) - (25f / 2f), 25);
			UIUtils.AddActionToButton(AddNewInterest.transform, "", () => UnityTraitScreen.ShowWindow(ToEditMinionStats, () => UpdateUI(), DupeTraitManager: this, openedFrom: UnityTraitScreen.OpenedFrom.Interest));
			AddNewInterest.TryGetComponent<ToolTip>(out var tt1);
			tt1.enabled = true;
			tt1.SetSimpleTooltip(STRINGS.UI.BUTTONS.ADDTOSTATS);



			AddNewTrait = Util.KInstantiateUI(ModAssets.AddNewToTraitsButtonPrefab, TraitContainer, Config.Instance.AddAndRemoveTraitsAndInterests);
			AddNewTrait.TryGetComponent<LayoutElement>(out var addtraitbtnLE);
			addtraitbtnLE.preferredWidth = 262;
			var imgAdd = AddNewTrait.transform.Find("Image").rectTransform();
			imgAdd.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (addbtnLE.preferredWidth / 2f) - (25f / 2f), 25);
			UIUtils.AddActionToButton(AddNewTrait.transform, "", () => UnityTraitScreen.ShowWindow(ToEditMinionStats, () => UpdateUI(), DupeTraitManager: this, openedFrom: UnityTraitScreen.OpenedFrom.Trait));
			AddNewTrait.TryGetComponent<ToolTip>(out var tt2);
			tt2.enabled = true;
			tt2.SetSimpleTooltip(STRINGS.UI.BUTTONS.ADDTOSTATS);

			var TraitBalanceHeaderGO = Util.KInstantiateUI(SpacerPrefab, TraitContainer, true);
			TraitBalanceHeaderGO.name = "TraitBalanceInfoHeader";
			TraitBalanceHeader = TraitBalanceHeaderGO.GetComponent<LocText>();
			TraitBalanceTooltipCMP = UIUtils.AddSimpleTooltipToObject(TraitBalanceHeaderGO.gameObject, "tt");
			RebuildUI();
		}


		GameObject AddInterestEditToggle(GameObject parent)
		{
			var interestToggle = Util.KInstantiateUI(ListEntryButtonPrefab, parent, true);
			interestToggle.name = "ToggleInterestEditing";

			interestToggle.TryGetComponent<LayoutElement>(out var LE);
			LE.preferredWidth = 270;
			UIUtils.AddSimpleTooltipToObject(interestToggle.transform, "Toggle raw attribute editing\nThis disables interest point redistribution when active.", true, onBottom: true);


			interestToggle.GetComponent<KButton>().enabled = true;
			UIUtils.AddActionToButton(interestToggle.transform, "", () =>
			{

			});
			//ModAssets.ApplyTraitStyleByKey(interestToggle.GetComponent<KImage>(), Nexty);

			//UIUtils.TryChangeText(interestToggle.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, trait.Name));
			interestToggle.transform.Find("RemoveButton").gameObject.SetActive(false);

			return interestToggle;
		}

		void UpdateReactions()
		{
			if (JoyLabel == null)
			{
				JoyLabel = AddTraitContainerUI(ToEditMinionStats.joyTrait, OverjoyedContainer, NextType.joy, false).transform.Find("Label").GetComponent<LocText>();
			}
			else
			{
				JoyLabel.text = ToEditMinionStats.joyTrait.Name;
			}
			if (!Config.Instance.NoJoyReactions)
			{
				if (JoyTT == null)
				{
					JoyTT = JoyLabel.transform.parent.gameObject.GetComponent<ToolTip>();
				}
				JoyTT.SetSimpleTooltip(ModAssets.GetTraitTooltip(ToEditMinionStats.joyTrait, ToEditMinionStats.joyTrait.Id));
			}


			if (StressLabel == null)
			{
				StressLabel = AddTraitContainerUI(ToEditMinionStats.stressTrait, StressContainer, NextType.stress, false).transform.Find("Label").GetComponent<LocText>();
			}
			else
			{
				StressLabel.text = ToEditMinionStats.stressTrait.Name;
			}

			if (!Config.Instance.NoStressReactions)
			{
				if (StressTT == null)
				{
					StressTT = StressLabel.transform.parent.gameObject.GetComponent<ToolTip>();
				}
				StressTT.SetSimpleTooltip(ModAssets.GetTraitTooltip(ToEditMinionStats.stressTrait, ToEditMinionStats.stressTrait.Id));
			}

			if (ModAssets.Beached_LifegoalsActive)
			{

				Trait LifeGoalTrait = Beached_API.GetCurrentLifeGoal.Invoke(ToEditMinionStats);
				if (GoalLabel == null)
				{
					GoalLabel = AddTraitContainerUI(LifeGoalTrait, LifeGoalContainer, NextType.Beached_LifeGoal, false).transform.Find("Label").GetComponent<LocText>();
				}
				else
				{
					GoalLabel.text = LifeGoalTrait.Name;
				}
				if (GoalTT == null)
				{
					GoalTT = GoalLabel.transform.parent.gameObject.GetComponent<ToolTip>();
				}
				GoalTT.SetSimpleTooltip(ModAssets.GetTraitTooltip(LifeGoalTrait, LifeGoalTrait.Id));
			}
		}


		void RebuildTraits()
		{
			foreach (var entry in UI_TraitEntries.Values)
			{
				entry.SetActive(false);
			}
			foreach (Trait t in ToEditMinionStats.Traits)
			{
				AddTraitUI(t);
			}
		}

		void RebuildInterestPointTooltip()
		{
			if (ToEditMinionStats == null)
				return;


			InterestBonusHeader.text = STRINGS.UI.DUPESETTINGSSCREEN.TRAITBONUSPOOL + " " + PointPool;

			InterestBonusTooltip = string.Empty;

			InterestBonusTooltip = string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAITBONUSPOOLTOOLTIP, Config.Instance.BalanceAddRemove ? AdditionalSkillPoints : "∞");

			if (!Config.Instance.BalanceAddRemove)
				InterestBonusTooltip += "\n" + string.Format(global::STRINGS.UI.MODIFIER_ITEM_TEMPLATE, STRINGS.UI.DUPESETTINGSSCREEN.CONFIGBALANCINGDISABLED, UIUtils.ColorText("∞", UIUtils.number_green));


			foreach (var trait in ToEditMinionStats.Traits)
			{
				var thisOnesInterest = ModAssets.GetTraitStatBonusTooltip(trait, false);
				if (thisOnesInterest != string.Empty)
				{
					InterestBonusTooltip += "\n" + string.Format(global::STRINGS.UI.MODIFIER_ITEM_TEMPLATE, trait.Name, thisOnesInterest);
				}
			}

			if (ExternalModPoints != 0)
				InterestBonusTooltip += "\n" + string.Format(global::STRINGS.UI.MODIFIER_ITEM_TEMPLATE, STRINGS.UI.DUPESETTINGSSCREEN.OTHERMODORIGINNAME, UIUtils.ColorNumber(ExternalModPoints));

			interestBonusTooltipCMP.SetSimpleTooltip(InterestBonusTooltip);
		}

		void RemoveInterestUI(SkillGroup interest)
		{
			if (UI_InterestEntries.ContainsKey(interest))
			{
				UI_InterestEntries[interest].gameObject.SetActive(false);
			}
			UpdateInterestSorting();
		}
		void UpdateInterestLabels()
		{
			foreach (var entry in UI_InterestEntries)
			{
				var interest = entry.Key;
				var logic = entry.Value;

				logic.Label.text = string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY2, GetSkillGroupName(interest), FirstSkillGroupStat(interest), this.GetBonusValue(interest));
				logic.minusButton.isInteractable = CanReduceInterest(interest);
				logic.plusButton.isInteractable = CanIncreaseInterest();
			}
			RebuildInterestPointTooltip();
		}

		void AddInterestUI(SkillGroup interest)
		{
			if (UI_InterestEntries.ContainsKey(interest))
			{
				UI_InterestEntries[interest].gameObject.SetActive(true);
				UpdateInterestLabels();
			}
			else
			{
				var AptitudeEntry = Util.KInstantiateUI(ListEntryButtonPrefab, InterestContainer, true);
				AptitudeEntry.name = "AptitudeEntry_" + interest.Id;

				AptitudeEntry.TryGetComponent<LayoutElement>(out var LE);
				LE.minHeight = 55;
				LE.preferredHeight = 55;
				LE.preferredWidth = 262;


				UIUtils.AddActionToButton(AptitudeEntry.transform, "", () =>
				{
					UnityTraitScreen.ShowWindow(ToEditMinionStats, () => UpdateUI(), currentGroup: interest, DupeTraitManager: this);
				});
				UIUtils.AddSimpleTooltipToObject(AptitudeEntry.transform, ModAssets.GetSkillgroupDescription(interest, ToEditMinionStats), true, onBottom: true);
				AptitudeEntry.GetComponent<KButton>().enabled = true;
				ModAssets.ApplyDefaultStyle(AptitudeEntry.GetComponent<KImage>());
				UIUtils.TryChangeText(AptitudeEntry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY2, GetSkillGroupName(interest), FirstSkillGroupStat(interest), this.GetBonusValue(interest)));

				var removeButton = AptitudeEntry.transform.Find("RemoveButton");
				removeButton.gameObject.SetActive(Config.Instance.AddAndRemoveTraitsAndInterests);
				UIUtils.AddActionToButton(AptitudeEntry.transform, "RemoveButton", () =>
				{
					this.RemoveInterest(interest);
					//InstantiateOrGetDupeModWindow(parent, ToEditMinionStats, hide);
				}
				);

				AptitudeEntry.transform.Find("InterestDOWN").gameObject.SetActive(true);
				UIUtils.AddActionToButton(AptitudeEntry.transform, "InterestDOWN", () =>
				{
					this.ReduceInterest(interest);
					UpdateInterestLabels();
					//InstantiateOrGetDupeModWindow(parent, ToEditMinionStats, hide);
				});
				AptitudeEntry.transform.Find("InterestUP").gameObject.SetActive(true);

				UIUtils.AddActionToButton(AptitudeEntry.transform, "InterestUP", () =>
				{
					this.IncreaseInterest(interest);
					UpdateInterestLabels();
					//InstantiateOrGetDupeModWindow(parent, ToEditMinionStats, hide);
				});


				var textLabel = AptitudeEntry.transform.Find("Label").GetComponent<LocText>();
				textLabel.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 26, LE.preferredWidth - (2 * 26f));


				AptitudeEntry.transform.Find("InterestUP").gameObject.TryGetComponent<KButton>(out var interestPlusBtn);
				AptitudeEntry.transform.Find("InterestDOWN").gameObject.TryGetComponent<KButton>(out var interestMinusBtn);

				var logic = AptitudeEntry.AddOrGet<UI_InterestLogic>();
				logic.Label = textLabel;
				logic.plusButton = interestPlusBtn;
				logic.minusButton = interestMinusBtn;
				logic.SkillGroup = interest;

				UI_InterestEntries[interest] = logic;

				removeButton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 2.5f, 50);
				removeButton.Find("Image").rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 23);
			}
			UpdateInterestSorting();
		}

		void RemoveTraitUI(Trait trait)
		{
			if (UI_TraitEntries.ContainsKey(trait))
			{
				UI_TraitEntries[trait].SetActive(false);
			}
			UpdateTraitSorting();
		}
		void AddTraitUI(Trait trait)
		{
			if (UI_TraitEntries.ContainsKey(trait))
			{
				UI_TraitEntries[trait].SetActive(true);
			}
			else
			{
				if (ModAssets.IsMinionBaseTrait(trait.Id))
					return;

				var type = ModAssets.GetTraitListOfTrait(trait.Id);

				//bool bionicTrait = (type == NextType.bionic_boost || type == NextType.bionic_bug);
				var traitEntry = AddTraitContainerUI(trait, TraitContainer, type);
				traitEntry.TryGetComponent<LayoutElement>(out var LE);
				UI_TraitEntries[trait] = traitEntry;

				var textLabel = traitEntry.transform.Find("Label").GetComponent<LocText>();
				if (type != NextType.undefined && type != NextType.special)
					textLabel.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 26, LE.preferredWidth - 25);
			}
			UpdateTraitSorting();
		}
		GameObject AddTraitContainerUI(Trait trait, GameObject parent, NextType type, bool enableDeleteButton = true)
		{
			var traitEntry = Util.KInstantiateUI(ListEntryButtonPrefab, parent, true);
			traitEntry.name = "TraitEntry_" + trait.Id;

			traitEntry.TryGetComponent<LayoutElement>(out var LE);
			LE.preferredWidth = 270;
			UIUtils.AddSimpleTooltipToObject(traitEntry.transform, ModAssets.GetTraitTooltip(trait, trait.Id), true, onBottom: true);

			ModAssets.ApplyTraitStyleByKey(traitEntry.GetComponent<KImage>(), type);

			bool notEditable = (type == NextType.undefined || type == NextType.special);


			traitEntry.GetComponent<KButton>().enabled = !notEditable;
			UIUtils.AddActionToButton(traitEntry.transform, "", () =>
			{
				UnityTraitScreen.ShowWindow(ToEditMinionStats,
					() => UpdateUI(),
					 DupeTraitManager: this,
					currentTrait: trait);
			});

			UIUtils.TryChangeText(traitEntry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, trait.Name));
			traitEntry.transform.Find("RemoveButton").gameObject.SetActive(Config.Instance.AddAndRemoveTraitsAndInterests && !notEditable && enableDeleteButton);

			ModAssets.ApplyTraitStyleByKey(traitEntry.transform.Find("RemoveButton").gameObject.GetComponent<KImage>(), type);

			UIUtils.AddActionToButton(traitEntry.transform, "RemoveButton", () =>
			{
				RemoveTrait(trait);
				//InstantiateOrGetDupeModWindow(parent, referencedStats, hide);
			}
			);
			return traitEntry;
		}

		void RebuildTypeSpecifics()
		{
			if (ToEditMinionStats == null)
				return;

			bool isBionic = ToEditMinionStats.personality.model == BionicMinionConfig.MODEL;

			AddNewTrait.SetActive(!isBionic || Config.Instance.BionicNormalTraits);

			InterestContainer.SetActive(!isBionic);
		}

		void RebuildInterests()
		{
			foreach (var entry in UI_InterestEntries.Values)
			{
				entry.gameObject.SetActive(false);
			}

			foreach (SkillGroup a in GetInterestsWithStats())
			{
				AddInterestUI(a);
			}
		}

		void UpdateUI()
		{
			UpdateInterestLabels();
			UpdateReactions();
			UpdateTraitSorting();
			UpdateInterestSorting();
		}
		void UpdateTraitSorting()
		{
			var traitsSorted = UI_TraitEntries.Keys.OrderBy(t => ModAssets.GetTraitListOfTrait(t)).ThenBy(t => t.Name).ToList();
			foreach (var t in traitsSorted)
			{
				UI_TraitEntries[t].transform.SetAsLastSibling();
			}

			AddNewTrait.transform.SetAsLastSibling();
			UpdateTraitInfoHeader();
		}
		void UpdateTraitInfoHeader()
		{
			if (ToEditMinionStats == null || TraitBalanceHeader == null)
				return;
			TraitBalanceHeader.transform.SetSiblingIndex(2);


			TraitBalanceTooltip = string.Empty;

			int totalRarityBalance = 0;
			foreach (Trait trait in ToEditMinionStats.Traits)
			{
				var traitVal = DUPLICANTSTATS.GetTraitVal(trait.Id);

				if (traitVal.id == DUPLICANTSTATS.INVALID_TRAIT_VAL.id)
					continue;

				int rarity = trait.PositiveTrait ? traitVal.rarity : -traitVal.rarity;

				totalRarityBalance += rarity;

				if (rarity.ToString() != string.Empty)
				{
					TraitBalanceTooltip += "\n  " + string.Format(global::STRINGS.UI.MODIFIER_ITEM_TEMPLATE, trait.Name, ModAssets.GetTraitRarityString(rarity));
				}
			}

			//int balanceThreshold = 3;
			//string balanceString=string.Empty;
			//if (totalRarityBalance >= -balanceThreshold && totalRarityBalance <= balanceThreshold)
			//    balanceString = UIUtils.ColorText(STRINGS.UI.DUPESETTINGSSCREEN.BALANCE_BALANCED, UIUtils.number_green);
			//else if(totalRarityBalance > balanceThreshold)
			//    balanceString = UIUtils.ColorText(STRINGS.UI.DUPESETTINGSSCREEN.BALANCE_WEAKER, UIUtils.number_red);
			//else if (totalRarityBalance < -balanceThreshold)
			//    balanceString = UIUtils.ColorText(STRINGS.UI.DUPESETTINGSSCREEN.BALANCE_STRONGER, UIUtils.number_red);


			//TraitBalanceTooltip = string.Format(STRINGS.UI.DUPESETTINGSSCREEN.BALANCE_TOOLTIP, balanceString,totalRarityBalance, -balanceThreshold,balanceThreshold) + TraitBalanceTooltip; 
			TraitBalanceTooltip = string.Format(STRINGS.UI.DUPESETTINGSSCREEN.BALANCE_TOOLTIP_SIMPLE, totalRarityBalance) + TraitBalanceTooltip;
			TraitBalanceHeader.text = string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAITBALANCEHEADER, totalRarityBalance);
			TraitBalanceTooltipCMP.SetSimpleTooltip(TraitBalanceTooltip);
		}

		void UpdateInterestSorting()
		{
			var interestsSorted = UI_InterestEntries.Keys.OrderBy(t => t.Name).ThenBy(t => t.Id).ToList();
			foreach (var t in interestsSorted)
			{
				UI_InterestEntries[t].transform.SetAsLastSibling();
			}

			AddNewInterest.transform.SetAsLastSibling();
			UpdateInterestLabels();
		}

		void RebuildUI()
		{
			if (ToEditMinionStats == null)
				return;
			SgtLogger.l("Rebuilding UI");

			RebuildTypeSpecifics();
			RebuildInterests();
			RebuildTraits();
			RebuildInterestPointTooltip();

			UpdateUI();
		}

		static string GetSkillGroupName(SkillGroup Group) => ModAssets.GetChoreGroupNameForSkillgroup(Group);
		static string FirstSkillGroupStat(SkillGroup Group) => Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + Group.relevantAttributes.First()?.Id.ToUpperInvariant() + ".NAME");


		#endregion

		public Dictionary<string, int> USEDSTATS = new();

		public List<string> currentTraitIds = new();
		public int strengthSkillHeightHolder = -1;


		public List<SkillGroup> ActiveInterests = new();

		MinionStartingStats ToEditMinionStats = null;
		public MinionStartingStats Stats => ToEditMinionStats;




		int FallBack = -1;

		int additionalSkillPoints = 0;
		int skillPointPool = 0;
		public int SkillPointPool => skillPointPool;
		public int AdditionalSkillPoints => additionalSkillPoints;

		public int ExternalModPoints => _externalModPoints;
		int _externalModPoints = 0;

		public string PointPool => Config.Instance.BalanceAddRemove ? UIUtils.ColorNumber(skillPointPool) : UIUtils.ColorText("∞", UIUtils.number_green);


		public enum NextType
		{
			//chatty + ancient knowledge
			special,
			//purple traits from gene shuffler
			geneShufflerTrait,

			//bionic dupe traits
			bionic_boost,
			bionic_bug,

			//regular dupe traits
			posTrait,
			needTrait,
			negTrait,

			//reactions
			joy,
			stress,


			undefined, //catch
			cogenital,  //unused

			//fill type for all
			allTraits,

			//mod integration types
			Beached_LifeGoal, //lifegoal from akis beached mod
			RainbowFart //rainbow fart from rainbow farts mod
		}
		internal void SetReferenceStats(MinionStartingStats referencedStats)
		{
			if (ToEditMinionStats != referencedStats)
			{
				SgtLogger.l("Redoing Reference");
				if (ToEditMinionStats != null && ModAssets.DupeTraitManagers.ContainsKey(ToEditMinionStats))
					ModAssets.DupeTraitManagers.Remove(ToEditMinionStats);

				ToEditMinionStats = referencedStats;

				ModAssets.DupeTraitManagers.Add(referencedStats, this);
				RecalculateAll();

			}
		}

		public void RecalculateAll()
		{
			ExternalModBonusPointCalculation();
			CalculateAdditionalSkillPoints();
			RebuildUI();

		}

		void Beached_RecalculateLifeGoal()
		{
			if (ToEditMinionStats == null || ModAssets.Beached_LifegoalsActive)
			{
				Trait currentGoal = Beached_API.GetCurrentLifeGoal(ToEditMinionStats);
				Beached_API.RemoveLifeGoal(ToEditMinionStats);
				Beached_API.SetLifeGoal(ToEditMinionStats, currentGoal, false);
			}
		}


		public void CalculateAdditionalSkillPoints() => CalculateAdditionalSkillPointsTrueIfChanged();

		public bool CalculateAdditionalSkillPointsTrueIfChanged()
		{
			var newValue = ModAssets.GetTraitBonus(ToEditMinionStats);
			var oldValue = additionalSkillPoints;

			additionalSkillPoints = newValue;
			return newValue != oldValue;
		}
		public void ExternalModBonusPointCalculation()
		{
			SgtLogger.l("Initializing External Bonus Point Calculation for " + ToEditMinionStats.Name);

			_externalModPoints = 0;
			int PointsPerInterest = ModAssets.MinimumPointsPerInterest(ToEditMinionStats);

			SgtLogger.l("Minimum points per interest: " + PointsPerInterest);
			Dictionary<string, int> relevantAttributes = new Dictionary<string, int>();
			foreach (var interest in ToEditMinionStats.skillAptitudes)
			{
				if (interest.Value > 0)
				{
					foreach (var attr in interest.Key.relevantAttributes)
					{
						if (!relevantAttributes.ContainsKey(attr.Id))
						{
							relevantAttributes.Add(attr.Id, 1);
						}
						else
						{
							relevantAttributes[attr.Id] += 1;
						}

					}
				}
			}

			foreach (var startingLevel in ToEditMinionStats.StartingLevels)
			{
				if (relevantAttributes.ContainsKey(startingLevel.Key))
				{
					int pointsThatAttributeShouldHave = relevantAttributes[startingLevel.Key] * PointsPerInterest;

					_externalModPoints += Math.Max(0, (startingLevel.Value - pointsThatAttributeShouldHave)); ///collecting all bonus points in that attribute
				}
			}
			SgtLogger.l("Total bonus gathered from starting levels " + _externalModPoints);
			SgtLogger.l("Total trait bonus " + ModAssets.GetTraitBonus(ToEditMinionStats));
			int TraitBonus = ModAssets.GetTraitBonus(ToEditMinionStats);

			if (_externalModPoints > 0)
				_externalModPoints -= TraitBonus;

			SgtLogger.l("Final Value: " + _externalModPoints);


			if (_externalModPoints != 0)
			{
				SgtLogger.l("Registering additional mod points");
				ModAssets.OtherModBonusPoints.Add(ToEditMinionStats, _externalModPoints);

			}
		}

		public void DeltaPointPool(int delta)
		{
			skillPointPool += delta;
		}
		public void ResetPool()
		{
			skillPointPool = 0;
		}
		public void RemoveLifeGoal()
		{
			if (ToEditMinionStats != null && ModAssets.Beached_LifegoalsActive)
			{
				Beached_API.RemoveLifeGoal(ToEditMinionStats);
			}
		}
		public void AddLifeGoal(Trait trait)
		{
			if (ToEditMinionStats != null && ModAssets.Beached_LifegoalsActive)
			{
				Beached_API.SetLifeGoal(ToEditMinionStats, trait, true);
			}
		}


		public void RemoveTrait(Trait trait)
		{
			if (ToEditMinionStats != null && ToEditMinionStats.Traits.Contains(trait))
			{
				ToEditMinionStats.Traits.Remove(trait);
				RedoStatpointBonus(trait);
				RemoveTraitUI(trait);
				RebuildInterestPointTooltip();
			}

		}
		public void AddTrait(Trait trait)
		{
			if (ToEditMinionStats != null)
			{
				ToEditMinionStats.Traits.Add(trait);
				RedoStatpointBonus(trait);
				AddTraitUI(trait);
				RebuildInterestPointTooltip();
			}
		}
		public void RedoStatpointBonus(Trait trait)
		{
			ModAssets.GetTraitListOfTrait(trait.Id, out var list);
			SgtLogger.l(trait.Name + ": " + list);
			if (list == null)
				return;

			var traitBonusHolder = list.Find(traitTo => traitTo.id == trait.Id);

			if (traitBonusHolder.statBonus == 0)
				return;

			if (CalculateAdditionalSkillPointsTrueIfChanged())
			{
				RecalculateSkillPoints();
				ResetPool();
			}
		}


		public void IncreaseInterest(SkillGroup interest)
		{
			if (CanIncreaseInterest())
			{
				foreach (var attribute in interest.relevantAttributes)
				{
					if (ToEditMinionStats.StartingLevels.ContainsKey(attribute.Id))
					{
						ToEditMinionStats.StartingLevels[attribute.Id]++;
					}
				}
				skillPointPool--;
			}
		}

		public bool CanIncreaseInterest()
		{
			return skillPointPool > 0 || !Config.Instance.BalanceAddRemove;
		}
		public bool CanReduceInterest(SkillGroup interest)
		{
			int minimumPoints = ModAssets.MinimumPointsPerInterest(ToEditMinionStats, interest);
			foreach (var attribute in interest.relevantAttributes)
			{
				if (ToEditMinionStats.StartingLevels.ContainsKey(attribute.Id))
				{
					if (ToEditMinionStats.StartingLevels[attribute.Id] <= minimumPoints)
						return false;
				}
			}
			return true;
		}

		public void ReduceInterest(SkillGroup interest)
		{
			if (!CanReduceInterest(interest))
				return;

			int minimumPoints = ModAssets.MinimumPointsPerInterest(ToEditMinionStats, interest);
			foreach (var attribute in interest.relevantAttributes)
			{
				if (ToEditMinionStats.StartingLevels.ContainsKey(attribute.Id))
				{
					if (ToEditMinionStats.StartingLevels[attribute.Id] <= minimumPoints)
						return;

					ToEditMinionStats.StartingLevels[attribute.Id]--;
				}
			}
			skillPointPool++;
		}


		public void ReplaceInterest(SkillGroup interestOld, SkillGroup interestNew)
		{
			int oldPoints = RemoveInterest(interestOld, false);
			AddInterest(interestNew, true, oldPoints);
		}

		public int RemoveInterest(SkillGroup interest, bool rebalanceAfter = true)
		{
			if (interest == null) return 0;

			RemoveInterestUI(interest);
			int removedPoints = 0;

			SgtLogger.l(interest.Name, "Removing Interest");

			if (ToEditMinionStats.skillAptitudes.ContainsKey(interest))
				ToEditMinionStats.skillAptitudes.Remove(interest);

			var LevelsToRemove = new List<Klei.AI.Attribute>(interest.relevantAttributes);

			//SgtLogger.l(LevelsToRemove.Count().ToString());

			foreach (var aptitude in ToEditMinionStats.skillAptitudes.Keys)
			{
				var overlapping = aptitude.relevantAttributes.Intersect(LevelsToRemove);
				foreach (var lap in overlapping)
				{
					SgtLogger.l(lap.Id, "not removing skillpoints for");
					LevelsToRemove.Remove(lap);
				}
			}
			foreach (var levelToRemove in LevelsToRemove)
			{
				SgtLogger.l(levelToRemove.Name, "Removing stats for");
				if (ToEditMinionStats.StartingLevels.ContainsKey(levelToRemove.Id))
				{
					//SgtLogger.l(ToEditMinionStats.StartingLevels[levelToRemove.Id].ToString(), "old bonus");

					removedPoints += ToEditMinionStats.StartingLevels[levelToRemove.Id];
					ToEditMinionStats.StartingLevels[levelToRemove.Id] = 0;
				}
			}

			if (rebalanceAfter)
			{
				RecalculateSkillPoints();
				Beached_RecalculateLifeGoal();
			}

			if (removedPoints == 0)
				removedPoints++;

			ResetPool();

			return removedPoints;
		}
		public void AddInterest(SkillGroup interest, bool rebalanceAfter = true, int newPoints = 1)
		{
			if (interest == null) return;
			SgtLogger.l(interest.Name, "Adding Interest");
			SgtLogger.l(newPoints.ToString(), "New Points");
			var LevelsToAdd = new List<Klei.AI.Attribute>(interest.relevantAttributes);

			foreach (var aptitude in ToEditMinionStats.skillAptitudes.Keys)
			{
				var overlapping = aptitude.relevantAttributes.Intersect(LevelsToAdd);
				foreach (var lap in overlapping)
				{
					SgtLogger.l(lap.Name, "Overlapping");

					if (LevelsToAdd.Contains(lap))
					{
						SgtLogger.l(lap.Id, "not adding skillpoints for");
						LevelsToAdd.Remove(lap);
					}
				}
			}

			if (!ToEditMinionStats.skillAptitudes.ContainsKey(interest))
				ToEditMinionStats.skillAptitudes.Add(interest, 1.0f);

			foreach (var LevelToAdd in LevelsToAdd)
			{
				SgtLogger.l(LevelToAdd.Name, "adding stats for");
				if (ToEditMinionStats.StartingLevels.ContainsKey(LevelToAdd.Id))
				{
					ToEditMinionStats.StartingLevels[LevelToAdd.Id] = newPoints;
				}
			}
			if (rebalanceAfter)
			{
				RecalculateSkillPoints();
				Beached_RecalculateLifeGoal();
			}
			ResetPool();

			AddInterestUI(interest);
		}

		public void RecalculateSkillPoints()
		{
			SgtLogger.l("Recalculating Skill Points, current amount to Ship: " + AdditionalSkillPoints);
			int amountToShip = AdditionalSkillPoints;

			Dictionary<string, int> newVals = new Dictionary<string, int>();



			int minimumSkillValue = ModAssets.MinimumPointsPerInterest(ToEditMinionStats);
			SgtLogger.l(minimumSkillValue.ToString(), "minimum skill value");

			int maxNumberOfRerolls = ToEditMinionStats.StartingLevels.Count * 2;

			SgtLogger.l("Interests were changed, redistributing at random");

			foreach (var levels in ToEditMinionStats.StartingLevels)
			{
				newVals[levels.Key] = 0;
			}
			Dictionary<string, int> MinimumValuesForEachStartingLevel = new Dictionary<string, int>();
			//min. vals in each Trait;
			foreach (var skillgroup in ToEditMinionStats.skillAptitudes.Keys)
			{
				foreach (var attributeAffected in skillgroup.relevantAttributes)
				{
					if (!MinimumValuesForEachStartingLevel.ContainsKey(attributeAffected.Id))
					{
						MinimumValuesForEachStartingLevel.Add(attributeAffected.Id, minimumSkillValue);
					}
					else
					{
						MinimumValuesForEachStartingLevel[attributeAffected.Id] += minimumSkillValue;
					}
				}
			}

			//while (amountToShip > 0 && ToEditMinionStats.skillAptitudes.Count > 0)
			//{
			//    int randomPoints = UnityEngine.Random.Range(1, amountToShip + 1);
			//    amountToShip -= randomPoints;
			//    var random = MinimumValuesForEachStartingLevel.GetRandom().Key;
			//    MinimumValuesForEachStartingLevel[random] += randomPoints;
			//}


			if (MinimumValuesForEachStartingLevel.Count > 0)
			{
				List<string> keys = MinimumValuesForEachStartingLevel.Keys.ToList();
				for (int counter = amountToShip; counter > 0; --counter)
				{
					keys.Shuffle();
					MinimumValuesForEachStartingLevel[keys.First()]++;
				}
				foreach (var calculatedValue in MinimumValuesForEachStartingLevel)
				{
					newVals[calculatedValue.Key] = calculatedValue.Value;
				}
			}

			foreach (var newv in newVals)
			{
				SgtLogger.l(newv.Value + "", newv.Key);
				ToEditMinionStats.StartingLevels[newv.Key] = newv.Value;
			}
			SgtLogger.l("Skill Points recalculated");

			UpdateInterestLabels();
		}


		/// <summary>
		/// no longer in use
		/// </summary>
		/// <returns></returns>
		public List<SkillGroup> GetInterestsWithStats()
		{
			ActiveInterests.Clear();
			foreach (var skillGroup in ToEditMinionStats.skillAptitudes)
			{
				ActiveInterests.Add(skillGroup.Key);
			}

			return ActiveInterests;
		}

		/// <summary>
		/// no longer in use
		/// </summary>
		/// <returns></returns>
		public void GetNextInterest(int index, bool backwards = false)
		{
			//Debug.Log(index+ ", count "+ ActiveInterests.Count());
			if (index >= ActiveInterests.Count())
				return;
			GetNextInterest(ActiveInterests[index].Id, backwards);
		}

		/// <summary>
		/// no longer in use
		/// </summary>
		/// <returns></returns>
		public void GetNextInterest(string id, bool backwards = false)
		{
			var allSkills = Db.Get().SkillGroups.resources;
			SkillGroup OldSkill = allSkills.Find(skill => skill.Id == id);
			SkillGroup newSkill = OldSkill;
			int indexInAllSkills = allSkills.FindIndex(skill => skill == OldSkill);
			var availableSkills = allSkills.Except(ToEditMinionStats.skillAptitudes.Keys);
			var dupeStatPoints = ToEditMinionStats.StartingLevels;

			if (availableSkills.Count() == 0)
				return;
			//index += backwards ? -1 : 1; 

			///Finding the next skill in Line thats not already on the list
			for (int i = 0; i < allSkills.Count; i++)
			{
				var potentialSkill = backwards
					? allSkills[(allSkills.Count - i + indexInAllSkills) % allSkills.Count]
					: allSkills[(i + indexInAllSkills) % allSkills.Count];
				if (availableSkills.Contains(potentialSkill))
				{
					newSkill = potentialSkill;
					break;
				}
			}

			///removing old boni
			int dupeStatValue = FallBack;
			foreach (var relevantAttribute in OldSkill.relevantAttributes)
			{
				//dupeStatPoints
				string statId = relevantAttribute.Id;
				dupeStatValue = dupeStatPoints[statId] > dupeStatValue
					? dupeStatPoints[statId]
					: dupeStatValue;
				if (DoesRemoveReduceStats(statId))
				{
					dupeStatPoints[statId] = 0;
				}
				else
				{
					dupeStatValue = FallBack;
				}
			}
			ToEditMinionStats.skillAptitudes.Remove(OldSkill);


			//Debug.Log("end");
			///adding new boni
			foreach (var relevantAttribute in newSkill.relevantAttributes)
			{
				//dupeStatPoints
				string statId = relevantAttribute.Id;
				if (IsThisANewSkill(statId))
				{
					dupeStatPoints[statId] = dupeStatValue;
				}
				else
				{
					FallBack = dupeStatValue;
				}
			}

			ToEditMinionStats.skillAptitudes.Add(newSkill, 1);
			ActiveInterests[GetCurrentIndex(OldSkill.Id)] = newSkill;
		}

		public int GetCurrentIndex(string id)
		{
			return ActiveInterests.FindIndex(old => old.Id == id);
		}

		/// <summary>
		/// deprecated
		/// </summary>
		/// <param name="startingLevels"></param>
		internal void AddSkillLevels(ref Dictionary<string, int> startingLevels)
		{
			ToEditMinionStats.StartingLevels = startingLevels;
			foreach (var skillGroup in startingLevels)
			{
				//Debug.Log(skillGroup.Key + ", " + skillGroup.Value);
				if (skillGroup.Value > 0)
				{
					if (!IsThisANewSkill(skillGroup.Key))
					{
						FallBack = new KRandom().Next(1, skillGroup.Value + 1);
					}
				}
			}
		}



		public List<string> CurrentTraitsWithout(string thisTrait)
		{
			return currentTraitIds.Where(entry => entry != thisTrait).ToList();
		}

		public string GetNextTraitId(string currentId, bool backwards)
		{
			ModAssets.GetTraitListOfTrait(currentId, out var currentList);
			if (currentList == null)
				return currentId;
			int i = 0;

			i = currentList.FindIndex(t => t.id == currentId);
			if (i != -1)
			{
				do
				{
					i += (backwards ? -1 : 1);
					if (i == currentList.Count)
						i = 0;
					else if (i < 0)
						i += currentList.Count;
				}
				while (CurrentTraitsWithout(currentId).Contains(currentList[i].id));
				return currentList[i].id;
			}
			return currentId;
		}


		public bool IsThisANewSkill(string stat)
		{
			if (!USEDSTATS.ContainsKey(stat))
			{
				USEDSTATS[stat] = 1;
				return true;
			}
			else
			{
				USEDSTATS[stat]++;
				return false;
			}
		}
		public bool DoesRemoveReduceStats(string stat)
		{
			if (USEDSTATS.ContainsKey(stat))
			{
				//Debug.Log(stat + ", Count " + USEDSTATS[stat]);
				if (USEDSTATS[stat] > 1)
				{
					USEDSTATS[stat]--;
					return false;
				}
				else
				{
					USEDSTATS.Remove(stat);
					return true;
				}
			}
			else
			{
				return true;
			}
		}

		//public int GetBonusName(int index)
		//{
		//    Debug.Log("Index: " + index);
		//    var relevantAttribute = ActiveInterests[index].relevantAttributes.First().Id;
		//    Debug.Log(relevantAttribute);
		//    return dupeStatPoints[relevantAttribute];
		//}

		public int GetBonusValue(SkillGroup group)
		{
			//Debug.Log("Index: " + index);
			if (group == null || group.relevantAttributes == null || group.relevantAttributes.Count == 0)
				return 0;

			var relevantAttribute = group.relevantAttributes.First().Id;
			if (!ToEditMinionStats.StartingLevels.ContainsKey(relevantAttribute))
				return 0;

			return ToEditMinionStats.StartingLevels[relevantAttribute];
		}

	}
}
