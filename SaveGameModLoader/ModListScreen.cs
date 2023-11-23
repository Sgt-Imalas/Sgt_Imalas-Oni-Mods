using SaveGameModLoader.ModFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace SaveGameModLoader
{
    class ModListScreen : KModalScreen
    {
        private GameObject ButtonPrefab;
        private GameObject ContentParentStandalone;
        private GameObject ContentParentSaveGame;

        private KButton _clearSearchButton;
        private TMP_InputField _search;

        Dictionary<GameObject,string> ListEntries = new Dictionary<GameObject,string>();


        public override void OnSpawn()
        {
            base.OnSpawn();
        }
        public void ConfigureButtons()
        {
            _search.text = "";
            _search.onValueChanged.AddListener(
                _ =>
                {
                    foreach(var entry in ListEntries)
                    {
                        bool enabled = entry.Value.ToLowerInvariant().Contains(_.ToLowerInvariant());
                        entry.Key.SetActive(enabled);
                    }
                }
            );

            _clearSearchButton.onClick += () => _search.text = "";
            var tt = _clearSearchButton.GetComponent<ToolTip>();
            if (tt != null)
            {
                tt.toolTip = "Clear Search";
            }
        }


        public override void OnActivate()
        {
#if DEBUG
            //UIUtils.ListAllChildren(this.transform);
#endif
            var TitleBar = transform.Find("Content/BG/TitleBar");

            TitleBar.Find("Title").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTWINDOWTITLE;
            TitleBar.Find("CloseButton").GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);


            var ButtonBar = transform.Find("Content/BG/Buttons");

            var rmButtonBcTooltip = ButtonBar.Find("UninstallButton");
            UnityEngine.Object.Destroy(rmButtonBcTooltip.gameObject);

            var OpenModPackFolderButtonGO = ButtonBar.Find("DoneButton");
            var CreateMPButtonGO = ButtonBar.Find("WorkshopButton");

            ButtonPrefab = CreateMPButtonGO.gameObject;

            ContentParentStandalone = transform.Find("Content/ScrollWindow/Viewport/Content/PreinstalledContent").gameObject;

            var SpacerParent = ContentParentStandalone.transform.parent.gameObject;
            var csf = UIUtils.TryFindComponent<VerticalLayoutGroup>(SpacerParent.transform);


            var local = Util.KInstantiateUI(FilterPatches._prefab, SpacerParent,true);

            var trans = local.transform;
            //trans.SetParent(SpacerParent.transform, false);
            //local.SetActive(true);
            _search = trans.Find("LocTextInputField").GetComponent<TMP_InputField>();
            _clearSearchButton = trans.Find("ClearButton").GetComponent<KButton>();
            ConfigureButtons();



            var spacer = Util.KInstantiateUI(ButtonPrefab, SpacerParent, true);
            UIUtils.TryChangeText(spacer.transform, "Label", STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTSTANDALONEHEADER);
            spacer.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 25);
            //UIUtils.ListAllChildrenWithComponents(spacer.transform.parent);
            var LE = spacer.AddComponent<LayoutElement>();
            LE.minHeight = 30;
            //ele.minHeight = 25;
            //fitter.enabled = false;vlg
            spacer.rectTransform().SetAsFirstSibling();
            trans.SetAsFirstSibling();

            //UIUtils.FindAndDisable(spacer.transform, "Label");
            var bt1 = UIUtils.TryFindComponent<KButton>(spacer.transform);
            bt1.isInteractable = false;


            var spacer2 = Util.KInstantiateUI(ButtonPrefab, SpacerParent, true);
            var LE2 = spacer2.AddComponent<LayoutElement>();
            LE2.minHeight = 30;
            UIUtils.TryChangeText(spacer2.transform, "Label", STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTSAVEGAMEHEADER);
            //UIUtils.FindAndDisable(spacer2.transform, "Label");
            UIUtils.TryChangeText(spacer2.transform, "Label", STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTSAVEGAMEHEADER);
            var bt2 = UIUtils.TryFindComponent<KButton>(spacer2.transform);
            bt2.isInteractable = false;

            ContentParentSaveGame = Util.KInstantiateUI(ContentParentStandalone, ContentParentStandalone.transform.parent.gameObject, true);


            var ImportCollectionGO = Util.KInstantiateUI(ButtonPrefab, ButtonBar.gameObject, true);
            var DoneButtonGO = Util.KInstantiateUI(ButtonPrefab, ButtonBar.gameObject, true);


            var ImportCollection = ImportCollectionGO.transform;
            UIUtils.TryChangeText(ImportCollection, "Label", STRINGS.UI.FRONTEND.MODLISTVIEW.IMPORTCOLLECTIONLIST);
            //UIUtils.TryFindComponent<KButton>(ImportCollection).isInteractable = false;///Until I have figured it out;
            UIUtils.AddActionToButton(ImportCollection, "", () => { this.OnClickNewModPack(false); }, true);

            UIUtils.AddSimpleTooltipToObject(ImportCollection, STRINGS.UI.FRONTEND.MODLISTVIEW.IMPORTCOLLECTIONLISTTOOLTIP);

            var DoneButton = DoneButtonGO.transform;
            DoneButton.Find("Label").GetComponent<LocText>().text = global::STRINGS.UI.FRONTEND.DONE_BUTTON;
            DoneButton.GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);

            CreateMPButtonGO.Find("Label").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODLISTVIEW.EXPORTMODLISTBUTTON;
            UIUtils.AddSimpleTooltipToObject(CreateMPButtonGO, STRINGS.UI.FRONTEND.MODLISTVIEW.EXPORTMODLISTBUTTONINFO);
            CreateMPButtonGO.GetComponent<KButton>().onClick += () => { this.OnClickNewModPack(true); };

            OpenModPackFolderButtonGO.Find("Label").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODLISTVIEW.OPENMODLISTFOLDERBUTTON;
            OpenModPackFolderButtonGO.FindOrAddUnityComponent<ToolTip>().SetSimpleTooltip(STRINGS.UI.FRONTEND.MODLISTVIEW.OPENMODLISTFOLDERBUTTONINFO);
            OpenModPackFolderButtonGO.GetComponent<KButton>().onClick += () => System.Diagnostics.Process.Start(System.IO.Path.Combine("file://", ModAssets.ModPacksPath));

            RefreshModlistView();
        }


        public static void InstantiateSingleModlistView(GameObject parent, KeyValuePair<string, SaveGameModList> exportedList, KModalScreen sis)
        {
            var window = Util.KInstantiateUI(ScreenPrefabs.Instance.modsMenu.gameObject, parent);
            //window.SetActive(false);
            
            window.name = "SingleList";
#if DEBUG
           // SgtLogger.log("SINGLE LIST:");
           // UIUtils.ListAllChildren(window.transform);
#endif
            var oldComp = window.GetComponent<ModsScreen>();
            UnityEngine.Object.Destroy(oldComp);
            var mlv = (SingleModListView)window.AddComponent(typeof(SingleModListView));
            mlv.InstantiateParams(exportedList, sis);
        }
        public static void InstantiateMissingModsView(GameObject parent, List<string> missingModIds, System.Action onClose = null)
        {
            var window = Util.KInstantiateUI(ScreenPrefabs.Instance.modsMenu.gameObject, parent);
            //window.SetActive(false);

            window.name = "SingleList";
#if DEBUG
            // SgtLogger.log("SINGLE LIST:");
            // UIUtils.ListAllChildren(window.transform);
#endif
            var oldComp = window.GetComponent<ModsScreen>();
            UnityEngine.Object.Destroy(oldComp);
            var mlv = (SingleModListView)window.AddComponent(typeof(SingleModListView));
            mlv.InstantiateMissing(missingModIds, onClose);
        }


        public void RefreshModlistView()
        {
            ModlistManager.Instance.GetAllStoredModlists();
            ModlistManager.Instance.GetAllModPacks();

            foreach (var btnToRemove in ListEntries)
            {
                UnityEngine.Object.Destroy(btnToRemove.Key);
            }
            ListEntries.Clear();
#if DEBUG 
            //SgtLogger.log("LAYOUT:");
            //foreach (var v in ContentParentStandalone.GetComponents(typeof (Component)))
            //{
            //    SgtLogger.log(v.GetType() + " <- Type, name ->" + v.name);
            //}
            //SgtLogger.log("Exported Lists:");
#endif

            ///Layout Exported

            var Layout = UIUtils.TryFindComponent<GridLayoutGroup>(ContentParentStandalone.transform);

            Layout.constraintCount = 2;
            Layout.startAxis = GridLayoutGroup.Axis.Vertical;
            Layout.cellSize = new Vector2(320, 100);
            Layout.padding = new RectOffset(5, 5, 5, 5);
            ///Layout Savegames

            var Layout2 = UIUtils.TryFindComponent<GridLayoutGroup>(ContentParentSaveGame.transform);

            Layout2.constraintCount = 2;
            Layout2.startAxis = GridLayoutGroup.Axis.Vertical;
            Layout2.cellSize = new Vector2(320, 100);
            Layout2.padding = new RectOffset(5, 5, 5, 5);



            foreach (var exportedList in ModlistManager.Instance.ModPacks)
            {
#if DEBUG
               // SgtLogger.log(exportedList.Key);
#endif
                var contentbutton = Util.KInstantiateUI(ButtonPrefab, ContentParentStandalone, true);


                //contentbutton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150f);
                contentbutton.transform.Find("Label").GetComponent<LocText>().text = ModListInfoBuilder(exportedList.Key, exportedList.Value.SavePoints);
                ListEntries.Add(contentbutton, exportedList.Value.ReferencedColonySaveName);
                contentbutton.GetComponent<KButton>().onClick += () => InstantiateSingleModlistView(gameObject, exportedList, this);//ModlistManager.Instance.InstantiateSyncViewWithoutRestart(exportedList.Value.SavePoints.Last().Value, this.gameObject);
                contentbutton.GetComponent<KButton>().isInteractable = DlcManager.IsExpansion1Active() ? exportedList.Value.Type != SaveGameModList.DLCType.baseGame : exportedList.Value.Type != SaveGameModList.DLCType.spacedOut;
            }
#if DEBUG
            //SgtLogger.log("Savegames:");
#endif
            foreach (var saveGameList in ModlistManager.Instance.Modlists)
            {
#if DEBUG
                //SgtLogger.log(saveGameList.Key);
#endif
                var contentbutton = Util.KInstantiateUI(ButtonPrefab, ContentParentSaveGame, true);
                contentbutton.transform.Find("Label").GetComponent<LocText>().text = ModListInfoBuilder(saveGameList.Key, saveGameList.Value.SavePoints);
                ListEntries.Add(contentbutton, saveGameList.Value.ReferencedColonySaveName);
                contentbutton.GetComponent<KButton>().onClick += () => InstantiateSingleModlistView(gameObject, saveGameList, this);//ModlistManager.Instance.InstantiateSyncViewWithoutRestart(saveGameList.Value.SavePoints.Last().Value, this.gameObject);
                contentbutton.GetComponent<KButton>().isInteractable = DlcManager.IsExpansion1Active() ? saveGameList.Value.Type != SaveGameModList.DLCType.baseGame : saveGameList.Value.Type != SaveGameModList.DLCType.spacedOut;
            }
        }
        public string ModListInfoBuilder(string name, Dictionary<string, List<KMod.Label>> mods)
        {
            var sb = new StringBuilder();
            sb.AppendLine(name);
            sb.Append(""); sb.Append(mods.Count); sb.AppendLine(mods.Count > 1 ? STRINGS.UI.FRONTEND.MODLISTVIEW.SINGLEENTRY.MORESTOREDLISTS : STRINGS.UI.FRONTEND.MODLISTVIEW.SINGLEENTRY.ONESTOREDLIST);
            sb.Append(string.Format(STRINGS.UI.FRONTEND.MODLISTVIEW.SINGLEENTRY.LATESTCOUNT, mods.Last().Value.Count));
            return sb.ToString();
        }

        public void OnClickNewModPack(bool export)
        {

            var Prefab = Util.KInstantiateUI(ScreenPrefabs.Instance.FileNameDialog.gameObject);
            Prefab.SetActive(false);
            var copy = Prefab.transform;
            UnityEngine.Object.Destroy(Prefab);

            var newScreen = Util.KInstantiateUI(copy.gameObject, this.gameObject, true);
            var ImportOrExport = newScreen.AddComponent<StoreModPackNameScreen>();
            ImportOrExport.parent = this;
            ImportOrExport.ExportToFile = export;
        }



        public override void OnDeactivate()
        {

        }
    }
}
