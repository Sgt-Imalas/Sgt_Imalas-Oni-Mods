using _SgtsModUpdater.Model.LocalMods;
using _SgtsModUpdater.Model.Update;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace _SgtsModUpdater.Model
{
	class ModManager
	{
		private static ModManager _instance;
		public static ModManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ModManager();
				}
				return _instance;
			}
		}
		public ObservableCollection<ModRepoListInfo> Repos = new();
		public ObservableCollection<VersionInfoWeb> CurrentRepoMods = new();
		public Dictionary<string,LocalMod> CurrentLocalInstalledMods = new();

		public void SelectRepo(ModRepoListInfo repo)
		{
			RefreshLocalModInfoList();
			CurrentRepoMods.Clear();
			foreach (var mod in repo.Mods)
			{
				mod.SetUrl(repo.RepoUrl);
				CurrentRepoMods.Add(mod);
			}

			foreach(var mod in CurrentRepoMods)
			{
				if(CurrentLocalInstalledMods.TryGetValue(mod.staticID, out var localMod))
				{
					mod.SetInstalledMod(localMod);
				}
			}
		}
		internal void FetchRepos()
		{
			foreach (var repo in AppSettings.Instance.ReposToFetch)
			{
				FetchRepo(repo);
			}
		}
		async void FetchRepo(FetchableRepoInfo repo)
		{
			try
			{
				using (var wc = new HttpClient())
				{
					var json = await wc.GetStringAsync(repo.ReleaseInfo);

					var data = Newtonsoft.Json.JsonConvert.DeserializeObject<VersionInfoWebCollection>(json);
					if (data != null && data.mods.Any())
					{
						var repoInfo = new ModRepoListInfo(repo.Name, repo.Url);
						foreach (var mod in data.mods)
							repoInfo.Mods.Add(mod);
						Repos.Add(repoInfo);
					}

				}
			}
			catch (Exception e)
			{}
		}
		public void RefreshLocalModInfoList()
		{
			CurrentLocalInstalledMods.Clear();

			if (!Directory.Exists(Paths.LocalModsFolder))
			{
				return;
			}
			foreach (var modFolder in Directory.GetDirectories(Paths.LocalModsFolder))
			{
				RefreshLocalModInfo(modFolder);
			}
		}
		LocalMod RefreshLocalModInfo(string modFolder)
		{
			var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();

			string modYamlFile = Path.Combine(modFolder, "mod.yaml");
			string modYaml = File.ReadAllText(modYamlFile);
			ModYaml modYamlData = deserializer.Deserialize<ModYaml>(modYaml);


			string modInfoYamlFile = Path.Combine(modFolder, "mod_info.yaml");
			string modInfoYaml = File.ReadAllText(modInfoYamlFile);
			ModInfoYaml modInfoYamlData = deserializer.Deserialize<ModInfoYaml>(modInfoYaml);
			var localModInfo = new LocalMod (modYamlData, modInfoYamlData, modFolder);

			CurrentLocalInstalledMods.Add(modYamlData.staticID, localModInfo);
			return localModInfo;
		}

		internal async Task TryInstallUpdate(VersionInfoWeb targetMod)
		{
			if (!targetMod.IsNewVersionAvailable())
				return;
			if (File.Exists(targetMod.zipFileName))
			{
				File.Delete(targetMod.zipFileName);
			}
			targetMod.Downloading = true;

			try
			{
				using (HttpClient client = new HttpClient())
				{
					using (var s = await client.GetStreamAsync(targetMod.FetchUrl))
					{
						using (var fs = new FileStream(targetMod.zipFileName, FileMode.CreateNew))
						{
							await s.CopyToAsync(fs);

						}
					}
				}
			}
			catch
			{
			}

			string targetfolder = targetMod.LocalMod != null ? targetMod.LocalMod.FolderPath : Path.Combine(Paths.LocalModsFolder, targetMod.staticID);
			if (File.Exists(targetMod.zipFileName))
				ZipFile.ExtractToDirectory(targetMod.zipFileName, targetfolder, true);

			targetMod.SetInstalledMod(RefreshLocalModInfo(targetfolder));

			if (File.Exists(targetMod.zipFileName))
			{
				File.Delete(targetMod.zipFileName);
			}
			targetMod.Downloading = false;
		}
	}
}
