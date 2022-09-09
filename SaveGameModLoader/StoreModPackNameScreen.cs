
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
                   //((KScreen)this).Deactivate();
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

        private Callback<PersonaStateChange_t> personaState;
        Constructable constructable ;

        
        class Constructable
        {
            public int GetProgress() => Progress;
            public Constructable(StoreModPackNameScreen parent)
            {
                parentTwo = parent;
            }

         /// <summary>
         /// 0 == not started
         /// 1 == mod ids & title fetched
         /// 2 == author fetched
         /// 3 == all missing mods noted;
         /// 4 == all missing mod titles fetched
         /// </summary>
            int Progress = 0;
            StoreModPackNameScreen parentTwo;

            List<ulong> modIDs = new();
            List<KMod.Label> mods = new();
            string Title;
            string authorName;

            List<ulong> missingMods = new();


            public void SetModIDsAndTitle(List<ulong> _mods, string title)
            {
                if(Progress == 0) 
                { 
                    mods.Clear();
                    modIDs.AddRange(_mods); 
                    Title = title;
                     ++Progress;
                }
            }
            public void SetAuthorName(string name)
            {
                if (Progress == 1)
                {
                    Debug.Log("adding Author: " +name);
                    authorName = name;
                    ++Progress;
                    InitModStats();
                }
            }
            public void InitModStats()
            {
                if(Progress == 2) { 
                    var allMods = Global.Instance.modManager.mods.Select(mod => mod.label).ToList();
                    Debug.Log("adding known mods");
                    foreach (var id in modIDs)
                    {
                        KMod.Label mod = new();
                        mod.id = id.ToString();
                        mod.distribution_platform = KMod.Label.DistributionPlatform.Steam;
                        var RefMod = allMods.Find(refm => refm.id == mod.id);
                        if (RefMod.id != null && RefMod.id != "")
                        {
                            mod.title = RefMod.title;
                            mod.version = RefMod.version;
                        }
                        else
                        {
                            missingMods.Add(id);
                            continue;
                        }
                        mods.Add(mod);
                        Debug.Log(mod.title + "; " + mod.id);
                    }
                    Debug.Log("all known mods added");
                    ++Progress;
                    Debug.Log("MissingCOunt: " + missingMods.Count);
                    if(missingMods.Count>0)
                        parentTwo.FindMissingModsQuery(missingMods);
                    else
                    {
                        ++Progress;
                        FinalizeConstructedList();
                    }
                }
                //ModlistManager.Instance.CreateOrAddToModPacks(ModListTitle, ModLabels);
                //reference.RefreshModlistView();
                //((KScreen)this).Deactivate();
            }
            public void InsertMissingIDs(List<Tuple<ulong, string>> missingMods)
            {
                if (Progress == 3)
                {
                    Debug.Log("adding unknown mods");
                    foreach (var id in missingMods)
                    {
                        KMod.Label mod = new();
                        mod.id = id.first.ToString();
                        mod.distribution_platform = KMod.Label.DistributionPlatform.Steam;
                        
                        mod.title = id.second.ToString();
                        mod.version = 404;
                        mods.Add(mod);
                        Debug.Log(mod.title + ": " + mod.id);
                    }
                    Debug.Log("all unknown mods added");
                    ++Progress; 
                    FinalizeConstructedList();
                }
            }
            public void FinalizeConstructedList()
            {
                var ModListTitle = string.Format(STRINGS.UI.FRONTEND.MODLISTVIEW.IMPORTEDTITLEANDAUTHOR, this.Title, this.authorName);
                ModlistManager.Instance.CreateOrAddToModPacks(ModListTitle, mods);
                parentTwo.parent.RefreshModlistView();
                
                Progress = 0;
            }

        }

        public void FindMissingModsQuery(List<ulong> IDs)
        {
            if (SteamManager.Initialized)
            {
                Debug.Log("TryFetchingMissingMods, " + IDs.Count);
                var list = new List<PublishedFileId_t>();
                foreach(var id in IDs)
                {
                    list.Add(new(id));
                }
                QueryUGCDetails(list.ToArray());
            }
        }


        public void StartModCollectionQuery()
        {
            constructable = new Constructable(this);
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

        private void QueryUGCDetails(PublishedFileId_t[] mods)
        {

            if (mods == null)
            {
                Debug.LogError("Invalid Collection ID");
            }

            Debug.Log(mods.Length + "< - count");
            var handle = SteamUGC.CreateQueryUGCDetailsRequest(mods, (uint)mods.Length);
            if (handle != UGCQueryHandle_t.Invalid)
            {
                Debug.Log("HandleValid");
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
                {
                    Debug.LogWarning("Invalid API Call "+handle);
                    SteamUGC.ReleaseQueryUGCRequest(handle);

                }
            }
        }



        void LoadName(CSteamID id)
        {
            string CollectionAuthor = SteamFriends.GetFriendPersonaName(id);
            Debug.Log(CollectionAuthor + " AUTOR");
            if (CollectionAuthor == "" || CollectionAuthor == "[unknown]")
                personaState = Callback<PersonaStateChange_t>.Create((cb) =>
                {
                    if (id == (CSteamID)cb.m_ulSteamID)
                    {
                        string CollectionAuthor = SteamFriends.GetFriendPersonaName(id);
                        Debug.Log(CollectionAuthor + " AUTOR");
                        if (CollectionAuthor == "" || CollectionAuthor == "[unknown]")
                            LoadName(id);
                        else
                        {
                            constructable.SetAuthorName(CollectionAuthor);
                        }
                    }
                });
            else
            {
                constructable.SetAuthorName(CollectionAuthor);

            }
        }
        private void OnUGCDetailsComplete(SteamUGCQueryCompleted_t callback, bool ioError)
        {
            List<Tuple<ulong, string>> missingIds = new();

            ModListScreen reference = parent;
               var result = callback.m_eResult;
            var handle = callback.m_handle;
#if DEBUG
            Debug.Log("QUERY CALL 1 DONE");
#endif
            List<ulong> ModList = new();

            if (!ioError && result == EResult.k_EResultOK)
            {
                for (uint i = 0U; i < callback.m_unNumResultsReturned; i++)
                {
                    if (SteamUGC.GetQueryUGCResult(handle, i, out SteamUGCDetails_t details))
                    {
#if DEBUG
                        Debug.Log("DATA RECIEVED:");
                        Debug.Log("Title: " + details.m_rgchTitle);
                        Debug.Log("ChildrenCount: " + details.m_unNumChildren);
#endif

                        if (details.m_eFileType== EWorkshopFileType.k_EWorkshopFileTypeCollection && constructable.GetProgress()==0)
                        {
                            var returnArray = new PublishedFileId_t[details.m_unNumChildren];
                            SteamUGC.GetQueryUGCChildren(handle, i, returnArray, details.m_unNumChildren);
                            foreach(var v in returnArray)
                            {
                                ModList.Add(v.m_PublishedFileId);
                            }
                            constructable.SetModIDsAndTitle(ModList, details.m_rgchTitle);

                            var steamID = new CSteamID(details.m_ulSteamIDOwner);

                            LoadName(steamID);
                            SteamFriends.RequestUserInformation(steamID, true);
                        }
                        else
                        {
                            var tuple = new Tuple<ulong, string>(details.m_nPublishedFileId.m_PublishedFileId, details.m_rgchTitle.ToString());
                            missingIds.Add(tuple);
                        }
                    }
                }
            }
            
            SteamUGC.ReleaseQueryUGCRequest(handle);
            onQueryComplete?.Dispose();
            onQueryComplete = null;

            if (missingIds.Count > 0)
            {
                Debug.Log("Inserting missing IDs");
                constructable.InsertMissingIDs(missingIds);

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
