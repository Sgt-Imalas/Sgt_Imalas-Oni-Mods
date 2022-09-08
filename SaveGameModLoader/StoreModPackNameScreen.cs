
using Steamworks;
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
                ConfirmButton.onClick += () => InitHandles();// ImportModList(2854869130);

            ConfirmButton.onClick += new System.Action(((KScreen)this).Deactivate);
            InitHandles();
        }

        public void CreateModPack()
        {
            var fileName = textField.text;
            SaveModPack(fileName);
            ModlistManager.Instance.GetAllModPacks();
            parent.RefreshModlistView();
        }



        /// <summary>
        /// Triggered when a query for mod details completes.
        /// </summary>
        private CallResult<SteamUGCQueryCompleted_t> onQueryComplete;

        private void QueryUGCDetails(PublishedFileId_t[] mods)
        {

            if (mods == null)
                throw new ArgumentNullException(nameof(mods));
            var handle = SteamUGC.CreateQueryUGCDetailsRequest(mods, (uint)mods.Length);
            if (handle != UGCQueryHandle_t.Invalid)
            {
                SteamUGC.SetReturnChildren(handle, true);
                SteamUGC.SetReturnLongDescription(handle, true);
                var apiCall = SteamUGC.SendQueryUGCRequest(handle);
                if (apiCall != SteamAPICall_t.Invalid)
                {
                    onQueryComplete?.Dispose();
                    onQueryComplete = new CallResult<SteamUGCQueryCompleted_t>(
                        OnUGCDetailsComplete);
                    onQueryComplete.Set(apiCall);
                }
                else
                    SteamUGC.ReleaseQueryUGCRequest(handle);
            }
        }

        public void InitHandles()
        {
            if (SteamManager.Initialized)
            {
                var list = new List<PublishedFileId_t>() { new(1830656006) };
                QueryUGCDetails(list.ToArray());
            }
        }
        private void OnUGCDetailsComplete(SteamUGCQueryCompleted_t callback, bool ioError)
        {
            var result = callback.m_eResult;
            var handle = callback.m_handle;

            if (!ioError && result == EResult.k_EResultOK)
            {
                for (uint i = 0U; i < callback.m_unNumResultsReturned; i++)
                {
                    if (SteamUGC.GetQueryUGCResult(handle, i, out SteamUGCDetails_t details))
                    {
                        Debug.Log("DATA RECIEVED:");
                        Debug.Log("Title: " + details.m_rgchTitle);
                        Debug.Log("Description: " + details.m_rgchDescription);
                        Debug.Log("ChildrenCount: " + details.m_unNumChildren);
                        if(details.m_eFileType== EWorkshopFileType.k_EWorkshopFileTypeCollection)
                        {
                            var returnArray = new PublishedFileId_t[details.m_unNumChildren];
                            SteamUGC.GetQueryUGCChildren(handle, 0, returnArray, details.m_unNumChildren);
                             foreach(var v in returnArray)
                            {
                                Debug.Log("ITEM: " + v.m_PublishedFileId);
                            }
                        }
                    }
                }
            }
            
            SteamUGC.ReleaseQueryUGCRequest(handle);
            onQueryComplete?.Dispose();
            onQueryComplete = null;
        }

        //void InitQuery()
        //{
        //    var fileId = new PublishedFileId_t(2857827854);
        //    var fileArray = new PublishedFileId_t[] { fileId };
        //    var handle = SteamUGC.CreateQueryUGCDetailsRequest(fileArray, 1);

        //    steamQueryCompleted.Set(handle);
        //    Debug.Log("Called GetNumberOfCurrentPlayers()");
        //}

        public void ImportModList(UInt32 id = 2857827854)
        {
            if (!SteamManager.Initialized)
            {
                Debug.Log("NOMANAGERFOUND");
                return;
            }
            string name = SteamFriends.GetPersonaName();
            Debug.Log("NAME: " + name);
            //var call = SteamUGC.SendQueryUGCRequest(handle);

            //SteamUGC.GetQueryUGCResult(handle, 0, out SteamUGCDetails_t itemDetails);
            //for (int i = 0; i < itemDetails.m_unNumChildren; i++)
            //{
            //    PublishedFileId_t[] individualItem = new PublishedFileId_t[1];
            //    SteamUGC.GetQueryUGCChildren(handle, (uint)i, individualItem, 1);
            //}
            //ulong collectionString = 2854869130; //long.Parse(textField.text);
            //Debug.Log(collectionString);

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
