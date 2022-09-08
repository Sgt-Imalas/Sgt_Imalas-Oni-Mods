
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
            textField.characterLimit = 300;

            CancelButtonGO.GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);
            

            var ConfirmButton = ConfirmButtonGO.GetComponent<KButton>();

            if (ExportToFile)
                ConfirmButton.onClick += new System.Action(this.CreateModPack);
            else
                ConfirmButton.onClick += () =>
                {
                    StartModCollectionQuery();
                   // ((KScreen)this).Deactivate();
                };// ImportModList(2854869130);

            ConfirmButton.onClick += new System.Action(((KScreen)this).Deactivate);
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
            {
                Debug.LogError("Invalid Collection ID");
            }

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

        public void StartModCollectionQuery()
        {
            if (SteamManager.Initialized)
            {
                string cut = textField.text;

                if (!cut.Contains("https://steamcommunity.com/sharedfiles/filedetails/?id="))
                    return;

                cut = cut.Replace("https://steamcommunity.com/sharedfiles/filedetails/?id=", string.Empty);

                var CollectionID = ulong.Parse(cut);
                Debug.Log("TRY Parse ID: " + CollectionID);

                var list = new List<PublishedFileId_t>() { new(CollectionID) };
                QueryUGCDetails(list.ToArray());
            }
        }
        private void OnUGCDetailsComplete(SteamUGCQueryCompleted_t callback, bool ioError)
        {
            ModListScreen reference = parent;
               var result = callback.m_eResult;
            var handle = callback.m_handle;
            Debug.Log("QUERY CALL DONE");

            string ModListTitle=string.Empty;
            List<ulong> ModList = new();

            if (!ioError && result == EResult.k_EResultOK)
            {
                for (uint i = 0U; i < callback.m_unNumResultsReturned; i++)
                {
                    if (SteamUGC.GetQueryUGCResult(handle, i, out SteamUGCDetails_t details))
                    {
                        Debug.Log("DATA RECIEVED:");
                        Debug.Log("Title: " + details.m_rgchTitle);
                        Debug.Log("ChildrenCount: " + details.m_unNumChildren);
                        ModListTitle = details.m_rgchTitle;

                        if (details.m_eFileType== EWorkshopFileType.k_EWorkshopFileTypeCollection)
                        {
                            var returnArray = new PublishedFileId_t[details.m_unNumChildren];
                            SteamUGC.GetQueryUGCChildren(handle, i, returnArray, details.m_unNumChildren);
                            foreach(var v in returnArray)
                            {
                                ModList.Add(v.m_PublishedFileId);
                                //Debug.Log("ITEM: " + v.m_PublishedFileId);
                            }
                        }
                    }
                }

                
            }
            
            SteamUGC.ReleaseQueryUGCRequest(handle);
            onQueryComplete?.Dispose();
            onQueryComplete = null;
#if DEBUG
            Debug.Log(ModListTitle + " -> count: " + ModList.Count);
#endif

            if (ModListTitle != string.Empty && ModList.Count > 0)
            {
                var allMods = Global.Instance.modManager.mods.Select(mod=>mod.label).ToList();
                List<KMod.Label> ModLabels = new();
                foreach (var id in ModList)
                {
                    //var mod = allMods.Except(id, new ModlistManager.ModDifferencesByIdComparer());
                    KMod.Label mod = new();
                    mod.id = id.ToString();
                    mod.distribution_platform = KMod.Label.DistributionPlatform.Steam;

                    var RefMod = allMods.Find(refm => refm.id == mod.id);

                    if(RefMod.id != null && RefMod.id != "")
                    {
                        mod.title = RefMod.title;
                        mod.version = RefMod.version;
                    }
                    else
                    {

                        mod.title =  "Missing Info; ID: "+id;
                        mod.version = 404;
                    }


                    ModLabels.Add(mod);
                    Debug.Log(mod.title + "; "+mod.id);
                }
                Debug.Log("STILL EXISTING");
                ModlistManager.Instance.CreateOrAddToModPacks(ModListTitle, ModLabels);
                reference.RefreshModlistView(); 
                //((KScreen)this).Deactivate();
            }
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
