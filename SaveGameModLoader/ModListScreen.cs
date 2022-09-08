using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        List<GameObject> buttonRefs = new();

        protected override void OnSpawn()
        {
            base.OnSpawn();
        }
        protected override void OnActivate()
        {
#if DEBUG
            UIUtils.ListAllChildren(this.transform);
#endif
            var TitleBar = transform.Find("Content/BG/TitleBar");

            TitleBar.Find("Title").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTWINDOWTITLE;
            TitleBar.Find("CloseButton").GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);


            var ButtonBar = transform.Find("Content/BG/Buttons");

            var rmButtonBcTooltip = ButtonBar.Find("UninstallButton");
            UnityEngine.Object.Destroy(rmButtonBcTooltip.gameObject);

            var OpenModPackFolderButtonGO  = ButtonBar.Find("DoneButton") ;
            var CreateMPButtonGO = ButtonBar.Find("WorkshopButton");

            ButtonPrefab = CreateMPButtonGO.gameObject;

            ContentParentStandalone = transform.Find("Content/ScrollWindow/Viewport/Content/PreinstalledContent").gameObject;
#if DEBUG
            foreach (var v in ContentParentStandalone.transform.parent.gameObject.GetComponents(typeof(Component)))
            {
                Debug.Log(v.GetType() + " <- Type, name ->" + v.name);
            }
#endif
            var SpacerParent = ContentParentStandalone.transform.parent.gameObject;
            var csf = UIUtils.TryFindComponent<VerticalLayoutGroup>(SpacerParent.transform);

            var spacer = Util.KInstantiateUI(ButtonPrefab, SpacerParent, true);
            UIUtils.TryChangeText(spacer.transform, "Label", STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTSTANDALONEHEADER);
            //UIUtils.FindAndDisable(spacer.transform, "Label");
            var bt1 = UIUtils.TryFindComponent<KButton>(spacer.transform);
            spacer.rectTransform().SetAsFirstSibling();
            bt1.isInteractable = false;


            var spacer2 = Util.KInstantiateUI(ButtonPrefab, SpacerParent, true);
            UIUtils.TryChangeText(spacer2.transform, "Label", STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTSAVEGAMEHEADER);
            //UIUtils.FindAndDisable(spacer2.transform, "Label");
            UIUtils.TryChangeText(spacer2.transform, "Label", STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTSAVEGAMEHEADER);
            var bt2 = UIUtils.TryFindComponent<KButton>(spacer2.transform);
            bt2.isInteractable = false;

            ContentParentSaveGame = Util.KInstantiateUI(ContentParentStandalone, ContentParentStandalone.transform.parent.gameObject, true);


            var ImportCollectionGO = Util.KInstantiateUI(ButtonPrefab, ButtonBar.gameObject, true);
            var DoneButtonGO = Util.KInstantiateUI(ButtonPrefab, ButtonBar.gameObject,true);

            
            var ImportCollection = ImportCollectionGO.transform;
            UIUtils.TryChangeText(ImportCollection,"Label",STRINGS.UI.FRONTEND.MODLISTVIEW.IMPORTCOLLECTIONLIST);
            UIUtils.AddActionToButton(ImportCollection, "", () => { this.OnClickNewModPack(false); }, true);

            UIUtils.AddSimpleTooltipToObject(ImportCollection, STRINGS.UI.FRONTEND.MODLISTVIEW.IMPORTCOLLECTIONLISTTOOLTIP);

           var DoneButton = DoneButtonGO.transform;
            DoneButton.Find("Label").GetComponent<LocText>().text = global::STRINGS.UI.FRONTEND.DONE_BUTTON;
            DoneButton.GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);

            CreateMPButtonGO.Find("Label").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODLISTVIEW.EXPORTMODLISTBUTTON;
            UIUtils.AddSimpleTooltipToObject(CreateMPButtonGO,STRINGS.UI.FRONTEND.MODLISTVIEW.EXPORTMODLISTBUTTONINFO);
            CreateMPButtonGO.GetComponent<KButton>().onClick += ()=> { this.OnClickNewModPack(true); };

            OpenModPackFolderButtonGO.Find("Label").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODLISTVIEW.OPENMODLISTFOLDERBUTTON;
            OpenModPackFolderButtonGO.FindOrAddUnityComponent<ToolTip>().SetSimpleTooltip(STRINGS.UI.FRONTEND.MODLISTVIEW.OPENMODLISTFOLDERBUTTONINFO);
            OpenModPackFolderButtonGO.GetComponent<KButton>().onClick += () => App.OpenWebURL("file://" + ModAssets.ModPacksPath);

            RefreshModlistView();
        }


        public static void InstantiateSingleModlistView(GameObject parent, KeyValuePair<string, SaveGameModList> exportedList, KModalScreen sis)
        {
            var window = Util.KInstantiateUI(ScreenPrefabs.Instance.modsMenu.gameObject,parent);
            //window.SetActive(false);

            window.name = "SingleList";
#if DEBUG
           // Debug.Log("SINGLE LIST:");
           // UIUtils.ListAllChildren(window.transform);
#endif
            var oldComp = window.GetComponent<ModsScreen>();
            UnityEngine.Object.Destroy(oldComp);
            var mlv = (SingleModListView)window.AddComponent(typeof(SingleModListView));
            mlv.InstantiateParams(exportedList, sis);
        }
        public static void InstantiateMissingModsView(GameObject parent, List<KMod.Label> mossing)
        {
            var window = Util.KInstantiateUI(ScreenPrefabs.Instance.modsMenu.gameObject, parent);
            //window.SetActive(false);

            window.name = "SingleList";
#if DEBUG
            // Debug.Log("SINGLE LIST:");
            // UIUtils.ListAllChildren(window.transform);
#endif
            var oldComp = window.GetComponent<ModsScreen>();
            UnityEngine.Object.Destroy(oldComp);
            var mlv = (SingleModListView)window.AddComponent(typeof(SingleModListView));
            mlv.InstantiateMissing(mossing);
        }


        public void RefreshModlistView()
        {
            ModlistManager.Instance.GetAllStoredModlists();
            ModlistManager.Instance.GetAllModPacks();
            foreach (var btnToRemove in buttonRefs)
            {
                UnityEngine.Object.Destroy(btnToRemove);
            }
#if DEBUG 
            Debug.Log("LAYOUT:");
            foreach (var v in ContentParentStandalone.GetComponents(typeof (Component)))
            {
                Debug.Log(v.GetType() + " <- Type, name ->" + v.name);
            }
            Debug.Log("Exported Lists:");
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
                Debug.Log(exportedList.Key);
#endif
                var contentbutton = Util.KInstantiateUI(ButtonPrefab, ContentParentStandalone, true);
                

                //contentbutton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150f);
                contentbutton.transform.Find("Label").GetComponent<LocText>().text = ModListInfoBuilder(exportedList.Key, exportedList.Value.SavePoints);
                buttonRefs.Add(contentbutton);
                contentbutton.GetComponent<KButton>().onClick += () => InstantiateSingleModlistView(gameObject, exportedList,this);//ModlistManager.Instance.InstantiateSyncViewWithoutRestart(exportedList.Value.SavePoints.Last().Value, this.gameObject);
                contentbutton.GetComponent<KButton>().isInteractable = DlcManager.IsExpansion1Active() ? exportedList.Value.Type != SaveGameModList.DLCType.baseGame : exportedList.Value.Type != SaveGameModList.DLCType.spacedOut;
            }
#if DEBUG
            Debug.Log("Savegames:");
#endif
            foreach (var saveGameList in ModlistManager.Instance.Modlists)
            {
#if DEBUG
                Debug.Log(saveGameList.Key);
#endif
                var contentbutton = Util.KInstantiateUI(ButtonPrefab, ContentParentSaveGame, true);
                contentbutton.transform.Find("Label").GetComponent<LocText>().text = ModListInfoBuilder(saveGameList.Key, saveGameList.Value.SavePoints);
                buttonRefs.Add(contentbutton);
                contentbutton.GetComponent<KButton>().onClick += () => InstantiateSingleModlistView(gameObject, saveGameList,this);//ModlistManager.Instance.InstantiateSyncViewWithoutRestart(saveGameList.Value.SavePoints.Last().Value, this.gameObject);
                contentbutton.GetComponent<KButton>().isInteractable = DlcManager.IsExpansion1Active() ? saveGameList.Value.Type != SaveGameModList.DLCType.baseGame : saveGameList.Value.Type != SaveGameModList.DLCType.spacedOut;
            }
        }
        public string ModListInfoBuilder(string name,Dictionary<string,List<KMod.Label>> mods)
        {
            var sb = new StringBuilder();
            sb.Append("\""); sb.Append(name); sb.AppendLine("\"");
            sb.Append(""); sb.Append(mods.Count); sb.AppendLine(mods.Count>1? STRINGS.UI.FRONTEND.MODLISTVIEW.SINGLEENTRY.MORESTOREDLISTS : STRINGS.UI.FRONTEND.MODLISTVIEW.SINGLEENTRY.ONESTOREDLIST);
            sb.Append(string.Format(STRINGS.UI.FRONTEND.MODLISTVIEW.SINGLEENTRY.LATESTCOUNT,mods.Last().Value.Count));
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



        protected override void OnDeactivate()
        {
            
        }
    }
}
