using Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static MinionBrowserScreen;
using static STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE;

namespace SetStartDupes
{
    internal class DupePresetScreenAddon : KModalScreen
    {




        public static bool IsCustomActive = false;
        List<Transform> StuffToDeactivate = new List<Transform>();
        List<Transform> StuffToActivate = new List<Transform>();
        Dictionary<MinionStatConfig, MultiToggle> Presets = new Dictionary<MinionStatConfig, MultiToggle>();


        GameObject AddNewButton;
        LocText TopBarLabel;

        MinionStartingStats EditableIdentity;
        CharacterContainer CurrentContainer;
        MinionStatConfig CurrentlySelected;

        KButton Select;
        KButton Rename;
        KButton Delete;


        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Init();
        }

        private bool init =false;
        private void Init()
        {
            if (init)
                return;
            init = true;

            StuffToActivate.Clear();
            StuffToActivate.Clear();
            UIUtils.ListAllChildrenPath(this.transform);

            var addNewButton = transform.Find("GalleryColumn/LayoutBreaker/Content/ScrollArea/ScrollRect/Content/AddOutfitButton");
            //StuffToDeactivate.Add(addNewButton);

            StuffToDeactivate.Add(transform.Find("PreviewColumn/LayoutBreaker/Content/Dupe Vis"));
            StuffToDeactivate.Add(transform.Find("PreviewColumn/LayoutBreaker/Content/SelectedItemInfo/ScrollArea/ScrollRect/Content/OutfitDescriptionContainer"));
            StuffToDeactivate.Add(transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer/PickOutfitButton"));
            StuffToDeactivate.Add(transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer/EditOutfitButton"));
            StuffToDeactivate.Add(transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer/RenameOutfitButton"));
            StuffToDeactivate.Add(transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer/DeleteOutfitButton"));

            AddNewButton = Util.KInstantiateUI(addNewButton.gameObject, transform.Find("GalleryColumn/LayoutBreaker/Content/ScrollArea/ScrollRect/Content").gameObject, true);
            UIUtils.TryChangeText(AddNewButton.transform, "Padding/Label", "Add new Preset"); ///STRINGSLOC!
            AddNewButton.GetComponent<MultiToggle>().onClick += () => CreateNewFromCurrent();
            StuffToActivate.Add(AddNewButton.transform);

            TopBarLabel = transform.Find("PreviewColumn/LayoutBreaker/Header/Label").GetComponent<LocText>();

            var SelectPresetButton = Util.KInstantiateUI(transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer/PickOutfitButton").gameObject, transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer").gameObject, true);
            UIUtils.TryChangeText(SelectPresetButton.transform, "Label", "Load selected preset"); ///STRINGSLOC!
            SelectPresetButton.name = "SelectPreset";
            SelectPresetButton.transform.SetAsFirstSibling();
            StuffToActivate.Add(SelectPresetButton.transform);
            Select = SelectPresetButton.GetComponent<KButton>();

            var RenamePresetButton = Util.KInstantiateUI(transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer/RenameOutfitButton").gameObject, transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer").gameObject, true);
            RenamePresetButton.name = "RenamePreset";
            StuffToActivate.Add(RenamePresetButton.transform);
            Rename = RenamePresetButton.GetComponent<KButton>();

            var DeletePresetButton = Util.KInstantiateUI(transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer/DeleteOutfitButton").gameObject, transform.Find("PreviewColumn/LayoutBreaker/Content/ButtonsContainer").gameObject, true);
            DeletePresetButton.name = "DeletePreset";
            //UIUtils.AddActionToButton(ConfirmButton.transform, "", () => SetSelectedDupe());
            StuffToActivate.Add(DeletePresetButton.transform);
            Delete = DeletePresetButton.GetComponent<KButton>();
            Delete.onClick += () => DeleteCurrentlySelected();

            Select.interactable = false;
            Rename.interactable = false;
            Delete.interactable = false;
            ReloadPresets();
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

        void CreateNewFromCurrent()
        {
            MinionStatConfig.CreateFromStartingStats(EditableIdentity, ModAssets.DupeTemplateName + Presets.Count);
            //ToggleCustomScreenOff();
            ReloadPresets();
        }

        void DeleteCurrentlySelected()
        {
            if(CurrentlySelected != null)
            {
                DeletePreset(CurrentlySelected);
                CurrentlySelected= null; 
                ReloadPresets();
            }
        }
        void DeletePreset(MinionStatConfig config)
        {
            var files = new DirectoryInfo(ModAssets.DupeTemplatePath).GetFiles();
            for (int i = 0; i < files.Count(); i++)
            {
                var File = files[i];
                if (!File.Name.Contains(config.FileName))
                    continue;

                try
                {
                    File.Delete();
                }
                catch (Exception e)
                {
                    SgtLogger.logError("Couln't load minion preset from: " + File.FullName + ", Error: " + e);
                }
                break;
            }
        }

        List<MinionStatConfig> LoadPresets()
        {
            List<MinionStatConfig> minionStatConfigs= new List<MinionStatConfig>();
            var files = new DirectoryInfo(ModAssets.DupeTemplatePath).GetFiles();


            for (int i = 0; i< files.Count(); i++)
            {
                var File = files[i];
                try
                {
                    var preset = MinionStatConfig.ReadFromFile(File);
                    if (preset != null)
                    {
                        minionStatConfigs.Add(preset);
                    }
                }
                catch (Exception e)
                {
                    SgtLogger.logError("Couln't load minion preset from: " + File.FullName + ", Error: " + e);
                }
            }
            return minionStatConfigs;
        }


        void SetAsCurrent (MinionStatConfig config)
        {
            CurrentlySelected= config;
            TopBarLabel.text = config.ConfigName;
        }


        public void SetSelectedDupe()
        {
            //var minionSelectionScreen = this.GetComponent<MinionBrowserScreen>();
            //MinionBrowserScreen.GridItem Selected = minionSelectionScreen.selectedGridItem;
            ////EditableIdentity;
            ////CurrentContainer.OnNameChanged(Selected.GetName());
            //ApplySkinFromPersonality(Selected.GetPersonality(), EditableIdentity);
            //EditableIdentity.personality = Selected.GetPersonality();
            //CurrentContainer.characterNameTitle.OnEndEdit(Selected.GetName());
            //CurrentContainer.SetAnimator();
            //ToggleCustomScreenOff();
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
        }

        void ToggleUICmps()
        {
            foreach (Transform child in transform.Find("GalleryColumn/LayoutBreaker/Content/ScrollArea/ScrollRect/Content").transform)
            {
                //var go = child.gameObject;
                //if (Presets.Values.Contains(go))
                //{
                //    go.SetActive(IsCustomActive);
                //}
                //else
                {
                    child.gameObject.SetActive(!IsCustomActive);
                }
            }
            foreach (Transform t in StuffToDeactivate)
            {
                SgtLogger.Assert(t.name, t);
                t.gameObject.SetActive(!IsCustomActive);
            }
            foreach (Transform t in StuffToActivate)
            {
                SgtLogger.Assert(t.name, t);
                t.gameObject.SetActive(IsCustomActive);
            }
            foreach (var t in Presets.Values)
            {
                SgtLogger.Assert(t.name, t);
                t.gameObject.SetActive(IsCustomActive);
            }
        }

        public void InitUI(CharacterContainer container, MinionStartingStats identity)
        {
            EditableIdentity = identity;
            CurrentContainer = container;
            ToggleUICmps();
        }

        internal static void ShowPresetScreen(CharacterContainer container,MinionStartingStats startingStats)
        {
            var instance = LockerNavigator.Instance.outfitBrowserScreen.AddOrGet<DupePresetScreenAddon>();
            IsCustomActive = true;
            LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.outfitBrowserScreen);

            instance.InitUI(container, startingStats);
            //instance.ReloadPresets();
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
