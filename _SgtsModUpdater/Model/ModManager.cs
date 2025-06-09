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
using System.Windows.Media;
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
		public Dictionary<string, LocalMod> CurrentLocalInstalledMods = new();

		public void SelectRepo(ModRepoListInfo repo)
		{
			RefreshLocalModInfoList();
			CurrentRepoMods.Clear();
			foreach (var mod in repo.Mods)
			{
				mod.SetUrl(repo.RepoUrl);
				CurrentRepoMods.Add(mod);
			}
			//var fastTrack = new VersionInfoWeb("FastTrack", "0.15.10.0", "0", "Fast Track", "The ultimate optimization mod");
			//fastTrack.SetFetchUrl("https://github.com/peterhaneve/ONIMods/releases/download/FastTrackBeta/FastTrack.zip");

			//CurrentRepoMods.Add(fastTrack);

			foreach (var mod in CurrentRepoMods)
			{
				if (CurrentLocalInstalledMods.TryGetValue(mod.staticID, out var localMod))
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
			{ }
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
			if (!Directory.Exists(Paths.SteamModsFolder))
			{
				return;
			}
			foreach (var modFolder in Directory.GetDirectories(Paths.SteamModsFolder))
			{
				RefreshLocalModInfo(modFolder);
			}
		}
		LocalMod RefreshLocalModInfo(string modFolder)
		{
			try
			{
				var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();

				var RepoDataPath = Path.Combine(modFolder, "repoinfo.json");
				if (File.Exists(RepoDataPath))
				{
					try
					{
						var repoInfos = deserializer.Deserialize<List<FetchableRepoInfo>>(RepoDataPath);
						if (repoInfos != null)
							AppSettings.Instance.AddRepoIfNotExist(repoInfos);
					}
					catch { }
				}

				string modYamlFile = Path.Combine(modFolder, "mod.yaml");
				if (!File.Exists(modYamlFile))
					return null;

				string modYaml = File.ReadAllText(modYamlFile);
				ModYaml modYamlData = deserializer.Deserialize<ModYaml>(modYaml);


				string modInfoYamlFile = Path.Combine(modFolder, "mod_info.yaml");
				if (!File.Exists(modInfoYamlFile))
					return null;

				string modInfoYaml = File.ReadAllText(modInfoYamlFile);
				ModInfoYaml modInfoYamlData = deserializer.Deserialize<ModInfoYaml>(modInfoYaml);
				var localModInfo = new LocalMod(modYamlData, modInfoYamlData, modFolder);
				if (modYamlData.staticID == null)
				{
					return null;
				}

				if (CurrentLocalInstalledMods.ContainsKey(modYamlData.staticID))
					CurrentLocalInstalledMods.Remove(modYamlData.staticID);

				CurrentLocalInstalledMods.Add(modYamlData.staticID, localModInfo);
				return localModInfo;

			}
			catch (Exception e)
			{
				return null;
			}
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
			{
				var localExtractionFolder = Path.Combine(Directory.GetCurrentDirectory(), targetMod.staticID);
				ZipFile.ExtractToDirectory(targetMod.zipFileName, localExtractionFolder, true);

				//check for one layer too deep files
				if (FindModZipDirectory(localExtractionFolder, out string adjustedSource))
				{
					CopyFilesRecursively(adjustedSource, targetfolder);
				}

			}

			targetMod.SetInstalledMod(RefreshLocalModInfo(targetfolder));



			if (File.Exists(targetMod.zipFileName))
			{
				File.Delete(targetMod.zipFileName);
			}
			targetMod.Downloading = false;
		}
		private static bool FindModZipDirectory(string currentPath, out string foundModPath)
		{
			foundModPath = string.Empty;
			string modYamlFile = Path.Combine(currentPath, "mod.yaml");
			string modInfoYamlFile = Path.Combine(currentPath, "mod_info.yaml");
			if (File.Exists(modYamlFile) && File.Exists(modInfoYamlFile))
			{
				foundModPath = currentPath;
				return true;
			}

			foreach (string dirPath in Directory.GetDirectories(currentPath))
			{
				if (FindModZipDirectory(dirPath, out foundModPath))
					return true;

			}
			return false;
		}

		private static void CopyFilesRecursively(string sourcePath, string targetPath)			
		{

			Directory.CreateDirectory(targetPath);

			//Now Create all of the directories
			foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
			{
				Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
			}

			//Copy all the files & Replaces any files with the same name
			foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
			{
				File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
			}
		}
	}
}
