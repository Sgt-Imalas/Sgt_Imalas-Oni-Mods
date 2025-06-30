using _SgtsModUpdater.Model.Update;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _SgtsModUpdater
{
	class AppSettings
	{
		private static AppSettings s_instance;

		public static AppSettings Instance
		{
			get
			{
				if (s_instance == null)
					Init();
				return s_instance;
			}
		}

		[JsonProperty]
		private List<FetchableRepoInfo> _reposToFetch = null;

		[JsonIgnore]
		public List<FetchableRepoInfo> ReposToFetch
		{
			get
			{
				if (_reposToFetch == null || !_reposToFetch.Any())
				{
					_reposToFetch = [
						new("Sgts Mods Nightly","https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases/download/AllMods_Automated_Build_Nightly/_ReleaseVersionInfoData.json"),
						new ("Sgts Mods Full",  "https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases/download/AllMods_Automated_Build_FullRelease/_ReleaseVersionInfoData.json"),
						new ("MapsNotIncluded",  "https://github.com/barratt/mapsnotincluded.org/releases/download/MNI_Mod_Automatic_Release/_ReleaseVersionInfoData.json"),
					];
				}
				return _reposToFetch;
			}
		}

		private static string GetSettingsFile()
		{
			string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string dataFolder = Path.Combine(appDataFolder, "ONIModUpdater");

			Directory.CreateDirectory(dataFolder);

			return Path.Combine(dataFolder, "settings.json");
		}

		public static void Init()
		{
			string settingsFile = GetSettingsFile();

			if (File.Exists(settingsFile))
			{
				try
				{
					string json = File.ReadAllText(settingsFile);
					s_instance = JsonConvert.DeserializeObject<AppSettings>(json);
				}
				catch (Exception ex)
				{ }
			}

			if (s_instance == null)
			{
				s_instance = new AppSettings();
				Save();
			}
		}

		public static void Save()
		{
			string settingsFile = GetSettingsFile();

			try
			{
				string json = JsonConvert.SerializeObject(s_instance, Formatting.Indented);
				File.WriteAllText(settingsFile, json);
			}
			catch (Exception ex)
			{

			}
		}

		internal void AddRepoIfNotExist(List<FetchableRepoInfo> repoInfos)
		{
			foreach (var repo in repoInfos)
			{
				if (_reposToFetch.Any(existing => existing.UpdateIndexUrl == repo.UpdateIndexUrl))
					continue;
				Console.WriteLine("Adding repo " + repo.Name + " with url " + repo.UpdateIndexUrl);
				_reposToFetch.Add(repo);
				Save();
			}

		}

		internal void DeleteRepo(ModRepoListInfo? rowItem)
		{
			if (rowItem != null)
				ReposToFetch.RemoveAll(item => item.ReleaseInfo == rowItem.RepoUrl);
		}
	}
}
