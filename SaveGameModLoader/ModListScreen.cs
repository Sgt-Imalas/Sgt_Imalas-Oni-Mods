using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace SaveGameModLoader
{
    class ModListScreen : KModalScreen
    {
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

            TitleBar.Find("Title").GetComponent<LocText>().text = "Modpacks";
            TitleBar.Find("CloseButton").GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);

            var ButtonBar = transform.Find("Content/BG/Buttons");

            var rmButtonBcTooltip = ButtonBar.Find("UninstallButton");
            UnityEngine.Object.Destroy(rmButtonBcTooltip.gameObject);

            var OpenModPackFolderButtonGO  = ButtonBar.Find("DoneButton") ;
            var CreateMPButtonGO = ButtonBar.Find("WorkshopButton");
            var DoneButtonGO = Util.KInstantiateUI(CreateMPButtonGO.gameObject,ButtonBar.gameObject,true);

            var DoneButton = DoneButtonGO.transform;
            DoneButton.Find("Label").GetComponent<LocText>().text = "Done";
            DoneButton.GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);

            CreateMPButtonGO.Find("Label").GetComponent<LocText>().text = "Create Modpack";
            CreateMPButtonGO.FindOrAddUnityComponent<ToolTip>().SetSimpleTooltip("Create a new Modpack from your current mod config");
            CreateMPButtonGO.GetComponent<KButton>().onClick += new System.Action(this.OnClickNewModPack);

            OpenModPackFolderButtonGO.Find("Label").GetComponent<LocText>().text = "Open Modpack Folder";
            OpenModPackFolderButtonGO.FindOrAddUnityComponent<ToolTip>().SetSimpleTooltip("Open the Modpack Folder to see all installed your modpacks");
            OpenModPackFolderButtonGO.GetComponent<KButton>().onClick += () => App.OpenWebURL("file://" + ModAssets.ModPacksPath);

        }
        public void OnClickNewModPack()
        {

            var Prefab = Util.KInstantiateUI(ScreenPrefabs.Instance.FileNameDialog.gameObject);
            Prefab.SetActive(false);
            var copy = Prefab.transform;
            UnityEngine.Object.Destroy(Prefab);

            var newScreen = Util.KInstantiateUI(copy.gameObject, this.gameObject, true);
            newScreen.AddComponent(typeof(StoreModPackNameScreen));

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
