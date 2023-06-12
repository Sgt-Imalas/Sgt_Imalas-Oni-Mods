using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static MinionBrowserScreen;
using static STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE;

namespace SetStartDupes
{
    internal class DupeSkinScreenAddon : KModalScreen
    {
        public static bool IsCustomActive = false;
        List<Transform> StuffToDeactivate = new List<Transform>();
        List<Transform> StuffToActivate = new List<Transform>();
        [MyCmpGet]
        MinionBrowserScreen minionSelectionScreen;
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();

            StuffToActivate.Clear();
            StuffToActivate.Clear();

            //UIUtils.ListAllChildren(this.transform);
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
            //EditableIdentity;
            //CurrentContainer.OnNameChanged(Selected.GetName());
            ModAssets.ApplySkinFromPersonality(Selected.GetPersonality(), EditableIdentity);
            CurrentContainer.characterNameTitle.OnEndEdit(Selected.GetName());
            CurrentContainer.SetAnimator();
            CurrentContainer.SetAttributes();
            CurrentContainer.SetInfoText();
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
        }

        public void InitUI(CharacterContainer container, MinionStartingStats identity)
        {
            EditableIdentity = identity;
            CurrentContainer = container;
            ToggleUICmps();
        }
        MinionStartingStats EditableIdentity;
        CharacterContainer CurrentContainer;

        internal static void ShowSkinScreen(CharacterContainer container,MinionStartingStats startingStats)
        {
           


            var instance = LockerNavigator.Instance.duplicantCatalogueScreen.AddOrGet<DupeSkinScreenAddon>();
            IsCustomActive = true;

            MinionBrowserScreenConfig.Personalities(startingStats.personality).ApplyAndOpenScreen();
            instance.InitUI(container, startingStats);
        }
        public override void OnShow(bool show)
        {
            if (!show)
            {
                IsCustomActive = false;
            }           
            base.OnShow(show);
        }
    }
}
