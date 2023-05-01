using Database;
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
            var minionSelectionScreen = this.GetComponent<MinionBrowserScreen>();
            MinionBrowserScreen.GridItem Selected = minionSelectionScreen.selectedGridItem;
            //EditableIdentity;
            //CurrentContainer.OnNameChanged(Selected.GetName());
            ApplySkinFromPersonality(Selected.GetPersonality(), EditableIdentity);
            EditableIdentity.personality = Selected.GetPersonality();
            CurrentContainer.characterNameTitle.OnEndEdit(Selected.GetName());
            CurrentContainer.SetAnimator();
            CurrentContainer.SetAttributes();
            CurrentContainer.SetInfoText();
            ToggleCustomScreenOff();
        }

        void ApplySkinFromPersonality(Personality personality, MinionStartingStats stats)
        {
            KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(personality);
            stats.accessories.Clear();
            foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
            {
                if (resource.accessories.Count == 0)
                {
                    continue;
                }

                Accessory accessory = null;
                if (resource == Db.Get().AccessorySlots.HeadShape)
                {
                    accessory = resource.Lookup(bodyData.headShape);
                    if (accessory == null)
                    {
                        personality.headShape = 0;
                    }
                }
                else if (resource == Db.Get().AccessorySlots.Mouth)
                {
                    accessory = resource.Lookup(bodyData.mouth);
                    if (accessory == null)
                    {
                        personality.mouth = 0;
                    }
                }
                else if (resource == Db.Get().AccessorySlots.Eyes)
                {
                    accessory = resource.Lookup(bodyData.eyes);
                    if (accessory == null)
                    {
                        personality.eyes = 0;
                    }
                }
                else if (resource == Db.Get().AccessorySlots.Hair)
                {
                    accessory = resource.Lookup(bodyData.hair);
                    if (accessory == null)
                    {
                        personality.hair = 0;
                    }
                }
                else if (resource == Db.Get().AccessorySlots.HatHair)
                {
                    accessory = resource.accessories[0];
                }
                else if (resource == Db.Get().AccessorySlots.Body)
                {
                    accessory = resource.Lookup(bodyData.body);
                    if (accessory == null)
                    {
                        personality.body = 0;
                    }
                }
                else if (resource == Db.Get().AccessorySlots.Arm)
                {
                    accessory = resource.Lookup(bodyData.arms);
                }
                else if (resource == Db.Get().AccessorySlots.ArmLower)
                {
                    accessory = resource.Lookup(bodyData.armslower);
                }
                else if (resource == Db.Get().AccessorySlots.ArmLowerSkin)
                {
                    accessory = resource.Lookup(bodyData.armLowerSkin);
                }
                else if (resource == Db.Get().AccessorySlots.ArmUpperSkin)
                {
                    accessory = resource.Lookup(bodyData.armUpperSkin);
                }
                else if (resource == Db.Get().AccessorySlots.LegSkin)
                {
                    accessory = resource.Lookup(bodyData.legSkin);
                }
                else if (resource == Db.Get().AccessorySlots.Leg)
                {
                    accessory = resource.Lookup(bodyData.legs);
                }
                else if (resource == Db.Get().AccessorySlots.Belt)
                {
                    accessory = resource.Lookup(bodyData.belt);
                    if (accessory == null)
                    {
                        accessory = resource.accessories[0];
                    }
                }
                else if (resource == Db.Get().AccessorySlots.Neck)
                {
                    accessory = resource.Lookup(bodyData.neck);
                }
                else if (resource == Db.Get().AccessorySlots.Pelvis)
                {
                    accessory = resource.Lookup(bodyData.pelvis);
                }
                else if (resource == Db.Get().AccessorySlots.Foot)
                {
                    accessory = resource.Lookup(bodyData.foot);
                    if (accessory == null)
                    {
                        accessory = resource.accessories[0];
                    }
                }
                else if (resource == Db.Get().AccessorySlots.Skirt)
                {
                    accessory = resource.Lookup(bodyData.skirt);
                }
                else if (resource == Db.Get().AccessorySlots.Necklace)
                {
                    accessory = resource.Lookup(bodyData.necklace);
                }
                else if (resource == Db.Get().AccessorySlots.Cuff)
                {
                    accessory = resource.Lookup(bodyData.cuff);
                    if (accessory == null)
                    {
                        accessory = resource.accessories[0];
                    }
                }
                else if (resource == Db.Get().AccessorySlots.Hand)
                {
                    accessory = resource.Lookup(bodyData.hand);
                    if (accessory == null)
                    {
                        accessory = resource.accessories[0];
                    }
                }

                stats.accessories.Add(accessory);
            }

            if (ModConfig.Instance.SkinsDoReactions)
            {
                if (!ModConfig.Instance.NoJoyReactions)
                {
                    stats.stressTrait = Db.Get().traits.TryGet(personality.stresstrait);
                }
                if (!ModConfig.Instance.NoStressReactions)
                {
                    stats.joyTrait = Db.Get().traits.TryGet(personality.joyTrait);
                }
            }
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
            MinionBrowserScreenConfig.Personalities().ApplyAndOpenScreen();
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
