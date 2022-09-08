
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace SaveGameModLoader
{
    class StoreModPackNameScreen : KModalScreen
    {
        KInputTextField textField;
        public ModListScreen parent;
        public bool ExportToFile = true;

        protected override void OnSpawn()
        {
            base.OnSpawn();
        }
        protected override void OnActivate()
        {
#if DEBUG
            //Debug.Log("StoreModPackScreen:");
            //UIUtils.ListAllChildren(this.transform);
#endif
            var TitleBar = transform.Find("Panel/Title_BG");

            TitleBar.Find("Title").GetComponent<LocText>().text = ExportToFile?STRINGS.UI.FRONTEND.MODSYNCING.EXPORTMODLISTCONFIRMSCREEN: STRINGS.UI.FRONTEND.MODSYNCING.IMPORTMODLISTCONFIRMSCREEN;
            TitleBar.Find("CloseButton").GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);

            var ContentBar = transform.Find("Panel/Body");


            var ConfirmButtonGO = ContentBar.Find("ConfirmButton");
            var CancelButtonGO = ContentBar.Find("CancelButton");
            textField = ContentBar.Find("LocTextInputField").GetComponent<KInputTextField>();
            
            CancelButtonGO.GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);
            

            var ConfirmButton = ConfirmButtonGO.GetComponent<KButton>();

            if (ExportToFile)
                ConfirmButton.onClick += new System.Action(this.CreateModPack);
            else
                ConfirmButton.onClick += () => FindWorkShopObjectAsync(2854869130);

            ConfirmButton.onClick += new System.Action(((KScreen)this).Deactivate);
        }
        public void CreateModPack()
        {
            var fileName = textField.text;
            SaveModPack(fileName);
            ModlistManager.Instance.GetAllModPacks();
            parent.RefreshModlistView();
        }

        public void ImportModList()
        {
            ulong collectionString = 2854869130; //long.Parse(textField.text);
            Debug.Log(collectionString);

            //var itemInfo = FindWorkShopObject(collectionString);
            //Debug.Log(itemInfo);

            //Console.WriteLine($"Title: {itemInfo?.Title}");
            //Console.WriteLine($"IsInstalled: {itemInfo?.IsInstalled}");
            //Console.WriteLine($"IsDownloading: {itemInfo?.IsDownloading}");
            //Console.WriteLine($"IsDownloadPending: {itemInfo?.IsDownloadPending}");
            //Console.WriteLine($"IsSubscribed: {itemInfo?.IsSubscribed}");
            //Console.WriteLine($"NeedsUpdate: {itemInfo?.NeedsUpdate}");
            //Console.WriteLine($"Description: {itemInfo?.Description}");
            
        }

        public void FindWorkShopObjectAsync(ulong id)
        {

        }
        public void SaveModPack(string fileName)
        {
            if (fileName == string.Empty)
                return;
            fileName = fileName.Replace(".sav", ".json");
            var enabledModLabels = Global.Instance.modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label).ToList();
            ModlistManager.Instance.CreateOrAddToModPacks(fileName, enabledModLabels);
            ModlistManager.Instance.GetAllModPacks();
        }
    }
}
