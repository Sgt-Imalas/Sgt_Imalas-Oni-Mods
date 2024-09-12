using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilLibs;

namespace SaveGameModLoader
{
	class StoreModPackNameScreen : KModalScreen
	{
		KInputTextField textField;
		public ModListScreen parent;
		public bool ExportToFile = true;

		public override void OnSpawn()
		{
			base.OnSpawn();
		}
		public override void OnActivate()
		{
#if DEBUG
            //SgtLogger.log("StoreModPackScreen:");
            //UIUtils.ListAllChildren(this.transform);
#endif
			var TitleBar = transform.Find("Panel/Title_BG");

			int ModNumber = -1;
			if (ExportToFile)
			{
				ModNumber = Global.Instance.modManager.mods.FindAll(mod => mod.IsEnabledForActiveDlc()).Select(mod => mod.label).Count();
			}

			TitleBar.Find("Title").GetComponent<LocText>().text =
				ExportToFile
				? string.Format(STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.EXPORTMODLISTCONFIRMSCREEN, ModNumber)
				: STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.IMPORTMODLISTCONFIRMSCREEN;
			TitleBar.Find("CloseButton").GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);

			var ContentBar = transform.Find("Panel/Body");


			var ConfirmButtonGO = ContentBar.Find("ConfirmButton");
			var CancelButtonGO = ContentBar.Find("CancelButton");
			textField = ContentBar.Find("LocTextInputField").GetComponent<KInputTextField>();
			textField.characterLimit = 300;

			textField.placeholder.GetComponent<LocText>().text = ExportToFile ?
				STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.ENTERNAME
				: STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.ENTERCOLLECTIONLINK;
			CancelButtonGO.GetComponent<KButton>().onClick += new System.Action(((KScreen)this).Deactivate);


			var ConfirmButton = ConfirmButtonGO.GetComponent<KButton>();

			if (ExportToFile)
			{
				ConfirmButton.onClick += new System.Action(this.CreateModPack);
				ConfirmButton.onClick += new System.Action(((KScreen)this).Deactivate);
			}
			else
				ConfirmButton.onClick += () =>
				{
					HandleLink();
					//((KScreen)this).Deactivate();
				};// ImportModList(2854869130);

		}

		void HandleLink()
		{

			string cut = textField.text;
			if (cut.Contains("https://steamcommunity.com/sharedfiles/filedetails/?id="))
			{
				StartModCollectionQuery();
			}
			else
			{


				if (ModlistManager.Instance.TryParseRML(cut, out var list) || ModlistManager.Instance.TryParseLog(cut, out list))
				{
					var doYouWantToImport = string.Format(STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.IMPORTYESNO_LOCAL, ModAssets.GetSanitizedNamePath(cut), list.Count);

					KMod.Manager.Dialog(Global.Instance.globalCanvas,
					STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.VALIDCOLLECTIONHEADER_FILE, doYouWantToImport, null, () =>
					{
						ModlistManager.Instance.CreateOrAddToModPacks(ModAssets.GetSanitizedNamePath(cut), list);
						parent.RefreshModlistView();
						((KScreen)this).Deactivate();
						CreatePopup(STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.SUCCESSTITLE, string.Format(STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.ADDEDNEW, ModAssets.GetSanitizedNamePath(cut), list.Count()));
					}, null, () =>
					{
						((KScreen)this).Deactivate();
					});
				}
				else
					ThrowErrorPopup();
			}

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
		private CallResult<SteamUGCQueryCompleted_t> onInitialQueryComplete;
		private CallResult<SteamUGCQueryCompleted_t> onMissingQueryComplete;

		private Callback<PersonaStateChange_t> personaState;
		Constructable constructable;


		class Constructable
		{
			public int GetProgress() => Progress;
			public void ResetProgress() => Progress = 0;
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
				if (Progress == 0)
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
					SgtLogger.log("adding Author: " + name);
					authorName = name;
					++Progress;
					InitModStats();
				}
			}
			public void InitModStats()
			{
				if (Progress == 2)
				{
					var allMods = Global.Instance.modManager.mods.Select(mod => mod.label).ToList();
					SgtLogger.log("adding known mods");
					foreach (var id in modIDs)
					{
						KMod.Label mod = new();
						mod.id = id.ToString();
						mod.distribution_platform = KMod.Label.DistributionPlatform.Steam;
						var RefMod = allMods.Find(refm => refm.defaultStaticID == mod.defaultStaticID);
						if (RefMod.defaultStaticID != null && RefMod.defaultStaticID != "")
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
						SgtLogger.log(mod.title + "; " + mod.defaultStaticID);
					}
					SgtLogger.log("all known mods added");
					++Progress;
					SgtLogger.log("MissingCOunt: " + missingMods.Count);
					if (missingMods.Count > 0)
						parentTwo.FindMissingModsQuery(missingMods);
					else
					{
						++Progress;
						FinalizeConstructedList();
					}
				}
			}
			public void InsertMissingIDs(List<Tuple<ulong, string>> missingMods)
			{
				if (Progress == 3)
				{
					SgtLogger.log("adding unknown mods");
					foreach (var id in missingMods)
					{
						KMod.Label mod = new();
						mod.id = id.first.ToString();
						mod.distribution_platform = KMod.Label.DistributionPlatform.Steam;

						mod.title = id.second.ToString();
						mod.version = 404;
						mods.Add(mod);
						SgtLogger.log(mod.title + ": " + mod.defaultStaticID);
					}
					SgtLogger.log("all unknown mods added");
					++Progress;
					FinalizeConstructedList();
				}
			}
			public void FinalizeConstructedList()
			{

				var ModListTitle = string.Format(STRINGS.UI.FRONTEND.MODLISTVIEW.IMPORTEDTITLEANDAUTHOR, ModAssets.GetSanitizedNamePath(this.Title), this.authorName);
				var doYouWantToImport = string.Format(STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.IMPORTYESNO, ModAssets.GetSanitizedNamePath(this.Title), mods.Count);


				KMod.Manager.Dialog(Global.Instance.globalCanvas,
				STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.VALIDCOLLECTIONHEADER, doYouWantToImport, null, () =>
				{
					ModlistManager.Instance.CreateOrAddToModPacks(ModListTitle, mods);
					parentTwo.parent.RefreshModlistView();
					Progress = 0;
					((KScreen)parentTwo).Deactivate();
					CreatePopup(STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.SUCCESSTITLE, string.Format(STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.ADDEDNEW, ModListTitle, mods.Count()));
				}, null, () =>
				{
					Progress = 0;
					((KScreen)parentTwo).Deactivate();
				});



			}

		}

		public void FindMissingModsQuery(List<ulong> IDs)
		{
			if (SteamManager.Initialized)
			{
				SgtLogger.log("TryFetchingMissingMods, " + IDs.Count);
				var list = new List<PublishedFileId_t>();
				foreach (var id in IDs)
				{
					list.Add(new(id));
				}
				QueryUGCDetails(list.ToArray(), onMissingQueryComplete);
			}
		}

		void ThrowErrorPopup(int errornr = 0)
		{
			string errormsg = string.Empty;
			switch (errornr)
			{
				case 0:
					errormsg = STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.WRONGFORMAT;
					break;
				case 1:
					errormsg = STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.PARSINGERROR;
					break;
				case 2:
					errormsg = STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.STEAMINFOERROR;
					break;

			}
			CreatePopup(STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.ERRORTITLE, errormsg);
			textField.text = string.Empty;
		}

		public void StartModCollectionQuery()
		{
			constructable = new Constructable(this);
			if (SteamManager.Initialized)
			{
				constructable.ResetProgress();
				string cut = textField.text;
				cut = cut.Replace("https://steamcommunity.com/sharedfiles/filedetails/?id=", string.Empty);

				SgtLogger.log("Try Parse ID: " + cut);

				if (cut.Length < 10 || !cut.All(Char.IsDigit))
				{
					ThrowErrorPopup(0);
					return;
				}

				cut = cut.Substring(cut.Length - 10);

				ulong CollectionID = 0;
				CollectionID = ulong.Parse(cut);

				SgtLogger.log("Try Parse ID: " + CollectionID);

				if (CollectionID == 0)
				{
					ThrowErrorPopup(1);
					return;
				}

				var list = new List<PublishedFileId_t>() { new(CollectionID) };
				QueryUGCDetails(list.ToArray(), onInitialQueryComplete);
			}
		}

		static void CreatePopup(string title, string content)
		{
			KMod.Manager.Dialog(Global.Instance.globalCanvas,
				title, content);
		}

		private void QueryUGCDetails(PublishedFileId_t[] mods, CallResult<SteamUGCQueryCompleted_t> onQueryComplete)
		{

			if (mods == null)
			{
				SgtLogger.logError("Invalid Collection ID");
				return;
			}

			SgtLogger.log(mods.Length + "< - count");
			var handle = SteamUGC.CreateQueryUGCDetailsRequest(mods, (uint)mods.Length);
			if (handle != UGCQueryHandle_t.Invalid)
			{
				SgtLogger.log("HandleValid");
				if (constructable.GetProgress() < 2) { }
				SteamUGC.SetReturnChildren(handle, true);
				SteamUGC.SetReturnLongDescription(handle, true);

				var apiCall = SteamUGC.SendQueryUGCRequest(handle);

				if (apiCall != SteamAPICall_t.Invalid)
				{
					//SgtLogger.log("Apicall: " + apiCall);
					onQueryComplete?.Dispose();
					onQueryComplete = new CallResult<SteamUGCQueryCompleted_t>(
						OnUGCDetailsComplete);
					onQueryComplete.Set(apiCall);
				}
				else
				{
					SgtLogger.warning("Invalid API Call " + handle);
					SteamUGC.ReleaseQueryUGCRequest(handle);

				}
			}
		}

		void LoadName(CSteamID id)
		{
			string CollectionAuthor = SteamFriends.GetFriendPersonaName(id);
			SgtLogger.log(CollectionAuthor + " AUTOR");
			if (CollectionAuthor == "" || CollectionAuthor == "[unknown]")
				personaState = Callback<PersonaStateChange_t>.Create((cb) =>
				{
					if (id == (CSteamID)cb.m_ulSteamID)
					{
						string CollectionAuthor = SteamFriends.GetFriendPersonaName(id);
#if DEBUG
                        SgtLogger.log(CollectionAuthor + " AUTOR");
#endif
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

			List<ulong> ModList = new();

#if DEBUG
            SgtLogger.log("QUERY CALL " + constructable.GetProgress() + " DONE");
            SgtLogger.log(ioError + " <- Error?");
            SgtLogger.log(EResult.k_EResultOK + " <- Result?");
#endif
			if (ioError)
			{
				ThrowErrorPopup(2);
				return;
			}

			if (!ioError && result == EResult.k_EResultOK)
			{
				for (uint i = 0U; i < callback.m_unNumResultsReturned; i++)
				{
					if (SteamUGC.GetQueryUGCResult(handle, i, out SteamUGCDetails_t details))
					{
						if (details.m_rgchTitle == string.Empty && details.m_unNumChildren == 0)
						{
							SgtLogger.logwarning("could not parse mod data (mod is hidden).");
							continue;
						}

#if DEBUG
                        SgtLogger.log("Title: " + details.m_rgchTitle);
                        if(details.m_unNumChildren>0)
                            SgtLogger.log("ChildrenCount: " + details.m_unNumChildren);
#endif

						if (details.m_eFileType == EWorkshopFileType.k_EWorkshopFileTypeCollection && constructable.GetProgress() == 0)
						{
							var returnArray = new PublishedFileId_t[details.m_unNumChildren];
							SteamUGC.GetQueryUGCChildren(handle, i, returnArray, details.m_unNumChildren);
							foreach (var v in returnArray)
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
			onInitialQueryComplete?.Dispose();
			onMissingQueryComplete?.Dispose();
			onInitialQueryComplete = null;
			onMissingQueryComplete = null;
#if DEBUG
            SgtLogger.log("PRog: " + constructable.GetProgress());
#endif
			if (missingIds.Count > 0 && constructable.GetProgress() == 3)
			{
#if DEBUG
                SgtLogger.log("Inserting missing IDs");
#endif
				constructable.InsertMissingIDs(missingIds);

			}
		}

		public void SaveModPack(string fileName)
		{
			if (fileName == string.Empty)
				return;
			fileName = fileName.Replace(".sav", ".json");
			var enabledModLabels = Global.Instance.modManager.mods.FindAll(mod => mod.IsEnabledForActiveDlc()).Select(mod => mod.label).ToList();
			ModlistManager.Instance.CreateOrAddToModPacks(fileName, enabledModLabels);
			ModlistManager.Instance.GetAllModPacks();
			CreatePopup(STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.EXPORTCONFIRMATION, string.Format(STRINGS.UI.FRONTEND.MODLISTVIEW.POPUP.EXPORTCONFIRMATIONTOOLTIP, fileName, enabledModLabels.Count()));
		}
	}
}
