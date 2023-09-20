using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace SaveGameModLoader
{
    class SyncViewScreen : KModalScreen
    {
        public System.Action RefreshAction;
        public bool LoadOnClose = false;
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            UnityEngine.Object.Destroy(this);
        }

        public override void OnActivate()
        {
            var modScreen = this.transform;
            ///Set Title of Mod Sync Screen.
            modScreen.Find("Panel/Title/Title").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODSYNCING.MODDIFFS;

            var DetailsView = modScreen.Find("Panel/DetailsView").gameObject;
            var workShopButton = modScreen.Find("Panel/DetailsView/WorkshopButton");
            if (workShopButton == null)
            {
                SgtLogger.logError("Couldnt add buttons to Sync Menu");
                return;
            }
            ModlistManager manager = ModlistManager.Instance;


            int DiffCount = ModlistManager.ModListDifferencesPublic;
            int MissingCount = ModlistManager.MissingModsPublic.Count;

            ///Disable toggle all button if no mods are in the list
            var ToggleAll = modScreen.Find("Panel/DetailsView/ToggleAllButton");
            var ToggleAllButton = ToggleAll.GetComponent<KButton>();
            ToggleAllButton.isInteractable = DiffCount > 0;
            ToggleAll.gameObject.SetActive(DiffCount > 0);

            ///Make Close button to "SyncSelected"-button
            var closeBtObj = modScreen.Find("Panel/DetailsView/CloseButton");
            var closeBt = closeBtObj.GetComponent<KButton>();
            closeBt.isInteractable = DiffCount > 0 && DiffCount > MissingCount; ///higher than missing count so it only enabled if diffs>missing; aka atleast 1 mod in the list to change
            if(LoadOnClose)
                closeBt.onClick += () => { manager.AutoRestart(); };
            closeBtObj.Find("Text").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODSYNCING.SYNCSELECTED;
            closeBtObj.name = "SyncSelectedButton";

            ///Sync all mods button
            var SyncAllButtonObject = Util.KInstantiateUI<RectTransform>(workShopButton.gameObject, DetailsView, true);
            SyncAllButtonObject.name = "SyncAllModsButton";
            SyncAllButtonObject.Find("Text").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODSYNCING.SYNCALL;
            var SyncAllButton = SyncAllButtonObject.GetComponentInChildren<KButton>(true);
            SyncAllButton.ClearOnClick();
            SyncAllButton.isInteractable = DiffCount > 0;
            SyncAllButton.onClick += () => { manager.SyncAllMods(null,LoadOnClose); };



            ///new Close button
            var NewCloseButtonObject = Util.KInstantiateUI<RectTransform>(workShopButton.gameObject, DetailsView, true);
            NewCloseButtonObject.name = "newCloseButton";
            NewCloseButtonObject.Find("Text").GetComponent<LocText>().text = global::STRINGS.UI.CREDITSSCREEN.CLOSEBUTTON;
            NewCloseButtonObject.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 10, 100);
            var NewCloseButton = NewCloseButtonObject.GetComponentInChildren<KButton>(true);
            NewCloseButton.ClearOnClick();

            var CloseModScreenMethod = typeof(ModsScreen).GetMethod("Exit", BindingFlags.NonPublic | BindingFlags.Instance);
            if (CloseModScreenMethod != null)
                NewCloseButton.onClick += () => CloseModScreenMethod.Invoke(modScreen.GetComponent<ModsScreen>(), null);


            var EntryPos2 = modScreen.Find("Panel").gameObject;

            missingModListEntry = Util.KInstantiateUI<RectTransform>(workShopButton.gameObject, EntryPos2, true);
            missingModListEntry.name = "infoButton";


            RefreshAction += () => ReevaluateInfoState();
            workShopButton.gameObject.SetActive(false);
            ReevaluateInfoState();

        }

        RectTransform missingModListEntry;
        public void ReevaluateInfoState()
        {
            ModlistManager manager = ModlistManager.Instance;
            int DiffCount = ModlistManager.ModListDifferencesPublic;
            int MissingCount = ModlistManager.MissingModsPublic.Count;
            var BtnText = missingModListEntry.Find("Text").GetComponent<LocText>();
            var bgColorImage = missingModListEntry.GetComponent<KImage>();
            var Btn = missingModListEntry.GetComponent<KButton>();

            var CloseModScreenMethod = typeof(ModsScreen).GetMethod("Exit", BindingFlags.NonPublic | BindingFlags.Instance);

            if (MissingCount == 0 && DiffCount == 0)
            {
                BtnText.text = STRINGS.UI.FRONTEND.MODSYNCING.ALLSYNCED;
                var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
                ColorStyle.inactiveColor = new Color(0.25f, 0.8f, 0.25f);
                ColorStyle.hoverColor = new Color(0.35f, 0.8f, 0.35f);
                bgColorImage.colorStyleSetting = ColorStyle;
                bgColorImage.ApplyColorStyleSetting();
                Btn.ClearOnClick();
                Btn.onClick += () =>
                {
                    CloseModScreenMethod.Invoke(this.GetComponent<ModsScreen>(), null);
                };
            }
            else if (MissingCount > 0)
            {
                var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
                ColorStyle.inactiveColor = new Color(1f, 0.25f, 0.25f);
                ColorStyle.hoverColor = new Color(1f, 0.35f, 0.35f);
                bgColorImage.colorStyleSetting = ColorStyle;
                bgColorImage.ApplyColorStyleSetting();
                BtnText.text = STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMOD;
                Btn.ClearOnClick();
                Btn.onClick += () =>
                {
                    ModListScreen.InstantiateMissingModsView(this.gameObject, ModlistManager.MissingModsPublic.ToList(), RefreshAction);
                    ReevaluateInfoState();
                };
            }
            else
                UnityEngine.Object.Destroy(missingModListEntry.gameObject);
        }

    }
}
