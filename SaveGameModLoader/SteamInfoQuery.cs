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

    public class FetchedModData
    {
        public FetchedModData(ulong id, string name) {
            modId = id;
            modName = name;
        }

        public ulong modId;
        public string modName;
        public string authorName =string.Empty;
    }
    internal class SteamInfoQuery
    {

        public static Dictionary<string, FetchedModData> FetchedModData = new Dictionary<string, FetchedModData>();


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
            modIDstring = modIDstring.Replace(".Steam", string.Empty);

            if (FetchedModData.ContainsKey(modIDstring))
            {
                if(callback!=null)
                callback.Invoke(FetchedModData[modIDstring].modName);
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

                var purgedId = id.Replace(".Steam", string.Empty);
                var isValidModId = ulong.TryParse(purgedId, out var modId);
                if (isValidModId)
                {
                    if (!modIdsToLookup.Contains(modId))
                    {
                        modIdsToLookup.Add(modId);
                    }
                }
                else
                {
                    SgtLogger.warning(purgedId + " was no valid modId");
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
            if (SteamManager.Initialized && IDs.Count > 0)
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
                                if (!FetchedModData.ContainsKey(modID))
                                    FetchedModData[modID] = new FetchedModData(details.m_nPublishedFileId.m_PublishedFileId, name);
                            }
                            continue;
                        }
                        if (PendingLookups.ContainsKey(modID))
                        {
                            PendingLookups[modID].Invoke(details.m_rgchTitle.ToString());
                        }

                        if (!FetchedModData.ContainsKey(modID))
                            FetchedModData[modID] = new FetchedModData(details.m_nPublishedFileId.m_PublishedFileId, details.m_rgchTitle.ToString());

                        MPM_Config.Instance.AddAutoFetchedSteamTags(modID, details.m_rgchTags);

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
                                if (FetchedModData.ContainsKey(modID))
                                    FetchedModData[modID].authorName = (name); 
                            });
                        }
                        else
                        {
                            //SgtLogger.l(SteamFriends.GetFriendPersonaName(new(details.m_ulSteamIDOwner)), "authorname");

                            if (FetchedModData.ContainsKey(modID))
                                FetchedModData[modID].authorName = SteamFriends.GetFriendPersonaName(new(details.m_ulSteamIDOwner));
                        }
                    }
                }
                //MPM_Config.SaveInstanceToFile();
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
