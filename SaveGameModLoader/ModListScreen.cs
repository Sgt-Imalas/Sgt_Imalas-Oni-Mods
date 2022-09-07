using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace SaveGameModLoader
{
    class ModListScreen : KModalScreen
    {
        private GameObject ButtonPrefab;
        private GameObject ContentParentStandalone;
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

            ContentParentStandalone = transform.Find("Content/ScrollWindow/Viewport/Content/PreinstalledContent").gameObject;
            

            var ButtonBar = transform.Find("Content/BG/Buttons");

            var rmButtonBcTooltip = ButtonBar.Find("UninstallButton");
            UnityEngine.Object.Destroy(rmButtonBcTooltip.gameObject);

            var OpenModPackFolderButtonGO  = ButtonBar.Find("DoneButton") ;
            var CreateMPButtonGO = ButtonBar.Find("WorkshopButton");

            ButtonPrefab = CreateMPButtonGO.gameObject;
            var DoneButtonGO = Util.KInstantiateUI(ButtonPrefab, ButtonBar.gameObject,true);

            var DoneButton = DoneButtonGO.transform;
            DoneButton.Find("Label").GetComponent<LocText>().text = global::STRINGS.UI.FRONTEND.DONE_BUTTON;
            DoneButton.GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);

            CreateMPButtonGO.Find("Label").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODLISTVIEW.EXPORTMODLISTBUTTON;
            CreateMPButtonGO.FindOrAddUnityComponent<ToolTip>().SetSimpleTooltip(STRINGS.UI.FRONTEND.MODLISTVIEW.EXPORTMODLISTBUTTONINFO);
            CreateMPButtonGO.GetComponent<KButton>().onClick += new System.Action(this.OnClickNewModPack);

            OpenModPackFolderButtonGO.Find("Label").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODLISTVIEW.OPENMODLISTFOLDERBUTTON;
            OpenModPackFolderButtonGO.FindOrAddUnityComponent<ToolTip>().SetSimpleTooltip(STRINGS.UI.FRONTEND.MODLISTVIEW.OPENMODLISTFOLDERBUTTONINFO);
            OpenModPackFolderButtonGO.GetComponent<KButton>().onClick += () => App.OpenWebURL("file://" + ModAssets.ModPacksPath);
            RefreshModlistView();
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
            Debug.Log("Exported Lists:");
#endif
            foreach (var exportedList in ModlistManager.Instance.ModPacks)
            {
#if DEBUG
                Debug.Log(exportedList.Key);
#endif
                var contentbutton = Util.KInstantiateUI(ButtonPrefab, ContentParentStandalone, true);
                contentbutton.transform.Find("Label").GetComponent<LocText>().text = exportedList.Key;
                buttonRefs.Add(contentbutton);
                contentbutton.GetComponent<KButton>().onClick += () => ModlistManager.Instance.InstantiateModViewFromGridView(exportedList.Value.SavePoints.Last().Value, this.gameObject);
            }
#if DEBUG
            Debug.Log("Savegames:");
#endif
            foreach (var saveGameList in ModlistManager.Instance.Modlists)
            {
#if DEBUG
                Debug.Log(saveGameList.Key);
#endif
                var contentbutton = Util.KInstantiateUI(ButtonPrefab, ContentParentStandalone, true);
                contentbutton.transform.Find("Label").GetComponent<LocText>().text = saveGameList.Key;
                buttonRefs.Add(contentbutton);
                contentbutton.GetComponent<KButton>().onClick += () => ModlistManager.Instance.InstantiateModViewFromGridView(saveGameList.Value.SavePoints.Last().Value, this.gameObject);
            }
        }
        public void OnClickNewModPack()
        {

            var Prefab = Util.KInstantiateUI(ScreenPrefabs.Instance.FileNameDialog.gameObject);
            Prefab.SetActive(false);
            var copy = Prefab.transform;
            UnityEngine.Object.Destroy(Prefab);

            var newScreen = Util.KInstantiateUI(copy.gameObject, this.gameObject, true);
            newScreen.AddComponent<StoreModPackNameScreen>().parent = this;
            //var fileNameDialog = (FileNameDialog)KScreenManager.Instance.StartScreen(fileNameDialogGO, this.transform.parent.gameObject);

            //fileNameDialog.onConfirm = (System.Action<string>)(filename =>
            //{
            //    filename = System.IO.Path.Combine(SaveLoader.GetActiveSaveColonyFolder(), filename);
            //    this.SaveModPack(filename);
            //});
        }



        protected override void OnDeactivate()
        {
            
        }
    }
}
