﻿using FMOD.Studio;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace SetStartDupes
{
	internal class DupeSkinScreenAddon : KModalScreen
	{
		public static bool IsCustomActive = false;
		List<Transform> StuffToDeactivate = new List<Transform>();
		List<Transform> StuffToActivate = new List<Transform>();
		Dictionary<string,Transform> Personalities = new ();
		[MyCmpGet]
		MinionBrowserScreen minionSelectionScreen;
		
		public override void OnPrefabInit()
		{			
			base.OnPrefabInit();

			StuffToActivate.Clear();
			StuffToActivate.Clear();

			StuffToDeactivate.Add(transform.Find("PreviewColumn/LayoutBreaker/Content/SelectedItemInfo"));
			StuffToDeactivate.Add(transform.Find("PreviewColumn/LayoutBreaker/Content/Cycler"));
			StuffToDeactivate.Add(transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer/Buttons/EditOutfitButton"));
			StuffToDeactivate.Add(transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer/Buttons/ChangeOutfitButton"));

			var ConfirmButton = Util.KInstantiateUI(transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer/Buttons/EditOutfitButton").gameObject, transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer/Buttons").gameObject, true);
			UIUtils.TryChangeText(ConfirmButton.transform, "Label", STRINGS.UI.BUTTONS.APPLYSKIN);
			UIUtils.AddActionToButton(ConfirmButton.transform, "", () => SetSelectedDupe());
			StuffToActivate.Add(ConfirmButton.transform);			
		}
		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
			{
				ToggleCustomScreenOff();
			}

			base.OnKeyDown(e);
		}
		void ToggleCustomScreenOff()
		{
			IsCustomActive = false;
			ToggleUICmps();
			LockerNavigator.Instance.PopScreen();
		}


		public void SetSelectedDupe()
		{
			MinionBrowserScreen.GridItem Selected = minionSelectionScreen.selectedGridItem;

			bool OverrideName = true;

			SgtLogger.l(Selected.GetName(), "Personality selected");
			//CurrentContainer.OnNameChanged(Selected.GetName());

			///if changing minionstartingstats
			if (EditableIdentity != null)
			{
				SgtLogger.l("Editing Starting Stat Dupe");
				if (EditableIdentity.Name != EditableIdentity.personality.Name)
				{
					OverrideName = false;
				}
				ModAssets.ApplySkinFromPersonality(Selected.GetPersonality(), EditableIdentity);
			}
			else
			{
				SgtLogger.l("minionStartingStats was null!");
			}

			///if changing minionstartingstats
			if (CurrentContainer != null)
			{
				if (CurrentContainer.animController != null)
				{
					UnityEngine.Object.Destroy(CurrentContainer.animController.gameObject);
					CurrentContainer.animController = null;
				}

				if (OverrideName)
					CurrentContainer.characterNameTitle.OnEndEdit(Selected.GetName());
				CurrentContainer.SetAnimator();
				CurrentContainer.SetAttributes();
				CurrentContainer.SetInfoText();

				ModAssets.UpdatePersonalityLockButton(CurrentContainer);
			}
			else
			{
				SgtLogger.l("current container was null!");
			}

			///if changing live dupe
			if (EditingSkinOnExistingDupeGO != null)
			{
				ModAssets.ApplySkinToExistingDuplicant(Selected.GetPersonality(), EditingSkinOnExistingDupeGO);
			}


			ToggleCustomScreenOff();
		}

		void ToggleUICmps()
		{
			foreach (Transform t in StuffToDeactivate)
			{
				t.gameObject.SetActive(!IsCustomActive);
			}
			foreach (Transform t in StuffToActivate)
			{
				t.gameObject.SetActive(IsCustomActive);
			}
			//minionSelectionScreen.RefreshGalleryFn += () =>
			//{
			//	SgtLogger.l("OnRefreshGallery");
			//	Personality personality = null;
			//	if (EditableIdentity != null)
			//		personality = EditableIdentity.personality;
			//	else if (EditingSkinOnExistingDupeGO != null)
			//	{
			//		if (EditingSkinOnExistingDupeGO.TryGetComponent<MinionIdentity>(out var IdentityHolder))
			//		{
			//			personality = Db.Get().Personalities.GetPersonalityFromNameStringKey(IdentityHolder.nameStringKey);
			//		}
			//	}
			//	bool sameModel = personality == null ? true : (minionSelectionScreen.selectedGridItem.GetPersonality().model == personality.model);

			//	ConfirmButton.GetComponent<KButton>().interactable = !IsCustomActive || sameModel;
			//};
		}

		public void InitUI(CharacterContainer container, MinionStartingStats identity)
		{
			EditableIdentity = identity;
			CurrentContainer = container;
			ToggleUICmps();
		}

		GameObject EditingSkinOnExistingDupeGO = null;
		public static Personality StartPersonality;
		MinionStartingStats EditableIdentity;
		CharacterContainer CurrentContainer;

		internal static void ShowSkinScreen(CharacterContainer container, GameObject LiveDupeGO = null)
		{
			var instance = LockerNavigator.Instance.duplicantCatalogueScreen.AddOrGet<DupeSkinScreenAddon>();
			IsCustomActive = true;
			instance.EditingSkinOnExistingDupeGO = LiveDupeGO;

			Personality targetPersonality = null;
			if (container != null)
			{
				targetPersonality = container.stats.personality;
				StartPersonality = targetPersonality;
				var personalityGridItems = MinionBrowserScreenConfig.Personalities(targetPersonality);
				personalityGridItems.ApplyAndOpenScreen();

				instance.InitUI(container, container.stats);
			}
			else if (LiveDupeGO != null && LiveDupeGO.TryGetComponent<MinionIdentity>(out var IdentityHolder))
			{
				targetPersonality = Db.Get().Personalities.Get(IdentityHolder.personalityResourceId);
				StartPersonality = targetPersonality;

				var personalityGridItems = MinionBrowserScreenConfig.Personalities(targetPersonality);
				personalityGridItems.ApplyAndOpenScreen();

				instance.InitUI(null, null);
			}

		}
		public override void OnShow(bool show)
		{
			if (!show)
			{
				IsCustomActive = false;
				StartPersonality = null;
			}
			base.OnShow(show);
		}
	}
}
