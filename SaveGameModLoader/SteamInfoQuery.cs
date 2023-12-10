using PeterHan.PLib.UI;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Database.MonumentPartResource;
using UtilLibs;
using static DeserializeWarnings;

namespace SaveGameModLoader
{
    internal class SteamInfoQuery
    {
        //ModId - Mod Name
        public static Dictionary<string, string> FetchedModNames = new Dictionary<string, string>();
        //ModId - Author Name
        public static Dictionary<string, string> FetchedModAuthors = new Dictionary<string, string>();


        /// <summary>
        /// Triggered when a query for mod details completes.
        /// </summary>
        private static CallResult<SteamUGCQueryCompleted_t> onMissingQueryComplete;
        static Callback<PersonaStateChange_t> m_PersonaStateChange = null;

        static List<ulong> modIdsToLookup = new List<ulong>();

        public static Dictionary<string, List<Action<string>>> PendingModAuthors = new Dictionary<string, List<Action<string>>>();
        static Dictionary<string, Action<string>> PendingLookups = new Dictionary<string, Action<string>>();

        public static void AddModIdToQuery(string modIDstring, Action<string> callback)
        {
            if (FetchedModNames.ContainsKey(modIDstring))
            {
                if(callback!=null)
                callback.Invoke(FetchedModNames[modIDstring]);
            }
            else
            {
                var isValidModId = ulong.TryParse(modIDstring, out var modId);

                if (isValidModId)
                {
                    if (callback != null)
                        PendingLookups[modIDstring] = callback;

                    if (!modIdsToLookup.Contains(modId))
                    {
                        modIdsToLookup.Add(modId);
                    }
                }
                else
                {
                    SgtLogger.warning(modIDstring + " was no valid modId");
                    callback.Invoke(modIDstring);
                }
            }

        }
        public static void InitModAuthorQuery(List<string> ids)
        {
            foreach (var id in ids)
            {

                var isValidModId = ulong.TryParse(id, out var modId);
                if (isValidModId)
                {
                    if (!modIdsToLookup.Contains(modId))
                    {
                        modIdsToLookup.Add(modId);
                    }
                }
                else
                {
                    SgtLogger.warning(id + " was no valid modId");
                }
            }
            InstantiateMissingModQuery();
        }

        public static void InstantiateMissingModQuery()
        {
            FindMissingModsQuery(modIdsToLookup);
        }
        static void FinalizeCleanup()
        {
            modIdsToLookup.Clear();
            PendingLookups.Clear();
        }

        public static void FindMissingModsQuery(List<ulong> IDs)
        {
            if (SteamManager.Initialized)
            {
                SgtLogger.log("Trying to fetch Mod Data for " + IDs.Count+" mods.");
                var list = new List<PublishedFileId_t>();
                foreach (var id in IDs)
                {
                    list.Add(new(id));
                }
                QueryUGCDetails(list.ToArray(), onMissingQueryComplete);
            }
        }

        private static void QueryUGCDetails(PublishedFileId_t[] mods, CallResult<SteamUGCQueryCompleted_t> onQueryComplete)
        {

            if (mods == null)
            {
                SgtLogger.logError("Invalid Collection ID");
                return;
            }

            SgtLogger.log(mods.Length + "<- count");
            var handle = SteamUGC.CreateQueryUGCDetailsRequest(mods, (uint)mods.Length);
            if (handle != UGCQueryHandle_t.Invalid)
            {
                SgtLogger.log("HandleValid");

                SteamUGC.SetReturnChildren(handle, true);
                SteamUGC.SetReturnLongDescription(handle, true);

                var apiCall = SteamUGC.SendQueryUGCRequest(handle);

                if (apiCall != SteamAPICall_t.Invalid)
                {
                    //SgtLogger.log("Apicall: " + apiCall);
                    onQueryComplete?.Dispose();
                    onQueryComplete = new CallResult<SteamUGCQueryCompleted_t>(OnUGCDetailsComplete);
                    onQueryComplete.Set(apiCall);
                }
                else
                {
                    SgtLogger.warning("Invalid API Call:\n" + handle);
                    SteamUGC.ReleaseQueryUGCRequest(handle);
                }
            }
        }

        private static void OnUGCDetailsComplete(SteamUGCQueryCompleted_t callback, bool ioError)
        {
            if (m_PersonaStateChange == null)
                m_PersonaStateChange = Callback<PersonaStateChange_t>.Create(OnPersonaStateChange);
            List<Tuple<ulong, string>> missingIds = new();

            var result = callback.m_eResult;
            var handle = callback.m_handle;

            List<ulong> ModList = new();

            if (ioError)
            {
                SgtLogger.error("IO ERROR");
                //ThrowErrorPopup(2);
                return;
            }

            if (!ioError && result == EResult.k_EResultOK)
            {
                for (uint i = 0U; i < callback.m_unNumResultsReturned; i++)
                {
                    if (SteamUGC.GetQueryUGCResult(handle, i, out SteamUGCDetails_t details))
                    {
                        string modID = details.m_nPublishedFileId.m_PublishedFileId.ToString();
                        if (details.m_rgchTitle == string.Empty && details.m_unNumChildren == 0)
                        {
                            SgtLogger.logwarning("could not parse mod data (mod is hidden): " + details.m_nPublishedFileId.m_PublishedFileId.ToString());

                            if (PendingLookups.ContainsKey(modID) && ModlistManager.Instance.TryGetModTitleFromStorage(modID, out var name))
                            {
                                PendingLookups[modID].Invoke(name);
                                FetchedModNames.Add(modID, name);
                                FetchedModAuthors.Add(modID, name);
                            }
                            continue;
                        }
                        if (PendingLookups.ContainsKey(modID))
                        {
                            PendingLookups[modID].Invoke(details.m_rgchTitle.ToString());
                        }

                        FetchedModNames.Add(modID, details.m_rgchTitle.ToString());
                        if (SteamFriends.RequestUserInformation(new(details.m_ulSteamIDOwner), true))
                        {
                            var authorID = details.m_ulSteamIDOwner.ToString();
                            //SgtLogger.l(authorID,"authorID for fetch");
                            if (!PendingModAuthors.ContainsKey(authorID))
                            {
                                PendingModAuthors.Add(authorID, new List<Action<string>>());
                            }
                            PendingModAuthors[authorID].Add((name) => 
                            { 
                                if (!FetchedModAuthors.ContainsKey(modID)) 
                                    FetchedModAuthors.Add(modID, name); 
                            });
                        }
                        else
                        {
                            //SgtLogger.l(SteamFriends.GetFriendPersonaName(new(details.m_ulSteamIDOwner)), "authorname");
                            if (!FetchedModAuthors.ContainsKey(modID))
                                FetchedModAuthors.Add(modID, SteamFriends.GetFriendPersonaName(new(details.m_ulSteamIDOwner)));
                        }
                    }
                }
            }


            SteamUGC.ReleaseQueryUGCRequest(handle);
            onMissingQueryComplete?.Dispose();
            onMissingQueryComplete = null;
            FinalizeCleanup();
        }
        static void OnPersonaStateChange(PersonaStateChange_t pCallback)
        {
            string id = pCallback.m_ulSteamID.ToString();
            if (PendingModAuthors.ContainsKey(id))
            {
                foreach (var action in PendingModAuthors[id])
                {
                    //SgtLogger.l(SteamFriends.GetFriendPersonaName(new(pCallback.m_ulSteamID)));
                    action(SteamFriends.GetFriendPersonaName(new(pCallback.m_ulSteamID)));
                }
                PendingModAuthors.Remove(id);
            }
        }
    }
}
