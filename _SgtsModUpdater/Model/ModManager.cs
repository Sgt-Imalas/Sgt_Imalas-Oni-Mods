using _SgtsModUpdater.Extensions;
using _SgtsModUpdater.Model.LocalMods;
using _SgtsModUpdater.Model.Update;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
		public ObservableCollection<RemoteMod> CurrentRepoMods = new();
		public Dictionary<string, LocalMod> CurrentLocalInstalledMods = new();

		public ObservableCollection<LocalMod> LocalInstalledMods = new();

		public void SelectRepo(ModRepoListInfo repo)
		{
			RefreshLocalModInfoList();
			CurrentRepoMods.Clear();
			foreach (var mod in repo.Mods)
			{
				mod.InferDownloadUrlIfMissing(repo.RepoUrl);
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
		internal async void FetchRepos()
		{
			Console.WriteLine("Fetching mod repos..");
			RefreshLocalModInfoList(true);

			foreach (var repo in AppSettings.Instance.ReposToFetch)
			{
				await FetchRepo(repo);
			}
		}
		public async Task<bool> FetchRepo(FetchableRepoInfo repo)
		{
			try
			{
				using (var wc = new HttpClient())
				{
					var json = await wc.GetStringAsync(repo.UpdateIndexURL);

					var data = Newtonsoft.Json.JsonConvert.DeserializeObject<VersionInfoWebCollection>(json);
					if (data != null && data.mods.Any())
					{
						var repoInfo = new ModRepoListInfo(repo.UpdateIndexName, repo.RepoUrl);
						foreach (var mod in data.mods)
							repoInfo.Mods.Add(mod);
						Console.WriteLine("Found " + data.mods.Count + " mods in repo " + repo.UpdateIndexName);
						Repos.Add(repoInfo);
						return true;
					}

				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to fetch repo info of " + repo.UpdateIndexName + " from url " + repo.UpdateIndexURL);
			}
			return false;
		}
		public void RefreshLocalModInfoList(bool collectRepos = false)
		{
			CurrentLocalInstalledMods.Clear();

			Console.WriteLine("Refreshing info on local mods..");
			if (Directory.Exists(Paths.LocalModsFolder))
			{
				foreach (var modFolder in Directory.GetDirectories(Paths.LocalModsFolder))
				{
					RefreshLocalModInfo(modFolder, collectRepos);
				}
			}
			if (Directory.Exists(Paths.SteamModsFolder))
			{
				foreach (var modFolder in Directory.GetDirectories(Paths.SteamModsFolder))
				{
					RefreshLocalModInfo(modFolder, collectRepos);
				}
			}
			LocalInstalledMods = new(CurrentLocalInstalledMods.Values);
			Console.WriteLine(CurrentLocalInstalledMods.Count + " installed mods found");
		}
		LocalMod RefreshLocalModInfo(string modFolder, bool collectRepos = false)
		{
			try
			{
				var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();

				if (collectRepos)
				{
					var RepoDataPath = Path.Combine(modFolder, "LauncherMetadata.json");
					if (File.Exists(RepoDataPath))
					{
						try
						{
							string repoData = File.ReadAllText(RepoDataPath);
							var repoInfos = deserializer.Deserialize<FetchableRepoInfo>(repoData);
							if (repoInfos != null)
							{
								repoInfos.InferMissing();
								AppSettings.Instance.AddRepoIfNotExist(repoInfos);
							}
						}
						catch (Exception e)
						{
						}
					}
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
				foreach (var file in Directory.GetFiles(modFolder))
				{
					var lower = file.ToLowerInvariant();
					if (lower.Contains("plib"))
						continue;
					var info = new FileInfo(file);
					if (info.Extension != "dll")
						continue;
					string dllVersion = FileVersionInfo.GetVersionInfo(file).FileVersion;
					if (dllVersion != null)
					{
						localModInfo.DllVersion = dllVersion;
					}
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

		internal async Task TryDeleteLocalMod(RemoteMod targetMod)
		{
			if (!targetMod.CanDeleteLocal)
				return;
			Console.WriteLine("Deleting local installation of " + targetMod.modName + "...");

			string targetfolder = targetMod.LocalMod != null ? targetMod.LocalMod.FolderPath : Path.Combine(Paths.LocalModsFolder, targetMod.staticID);

			Console.WriteLine("Deleting folder " + targetfolder + "...");
			Directory.Delete(targetfolder, true);
			targetMod.SetInstalledMod(RefreshLocalModInfo(targetfolder));
		}

		public Progress<float> UpdateProgressbar = null;
		public System.Action<long> GetDownloadSize = null;

		internal async Task TryInstallUpdate(RemoteMod targetMod)
		{
			//if (!targetMod.IsNewVersionAvailable())
			//	return;
			if (File.Exists(targetMod.zipFileName))
			{
				File.Delete(targetMod.zipFileName);
			}
			Console.WriteLine("Starting installation of " + targetMod.modName + "...");

			targetMod.Downloading = true;
			Console.WriteLine("Fetching from " + targetMod.downloadURL);
			try
			{
				using (HttpClient client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromMinutes(5);

					using (var fs = new FileStream(targetMod.zipFileName, FileMode.CreateNew))
						await client.DownloadAsync(targetMod.downloadURL, fs, UpdateProgressbar, default, GetDownloadSize);

				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Mod download failed! Exception: " + e.Message);
				if (File.Exists(targetMod.zipFileName))
					File.Delete(targetMod.zipFileName);

				targetMod.Downloading = false;
				return;
			}
			if (!File.Exists(targetMod.zipFileName))
			{
				targetMod.Downloading = false;
				Console.WriteLine("We downloaded the mod, but the zip file " + targetMod.zipFileName + " does not exist! how did this happen??");
				return;
			}

			Console.WriteLine("Mod zip download successful. File size: " + Paths.GetReadableFileSize(new FileInfo(targetMod.zipFileName).Length));

			string targetfolder = targetMod.LocalMod != null ? targetMod.LocalMod.FolderPath : Path.Combine(Paths.LocalModsFolder, targetMod.staticID);

			Console.WriteLine("Starting mod extraction. Target folder: " + targetfolder);

			if (File.Exists(targetMod.zipFileName))
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					Mouse.OverrideCursor = Cursors.Wait;
				});
				var localExtractionFolder = Path.Combine(Directory.GetCurrentDirectory(), targetMod.staticID);
				ZipFile.ExtractToDirectory(targetMod.zipFileName, localExtractionFolder, true);

				//check for one layer too deep files
				if (FindModZipDirectory(localExtractionFolder, out string adjustedSource))
				{
					CopyFilesRecursively(adjustedSource, targetfolder);
				}
				Directory.Delete(localExtractionFolder, true);

				Application.Current.Dispatcher.Invoke(() =>
				{
					Mouse.OverrideCursor = null;
				});
			}

			targetMod.SetInstalledMod(RefreshLocalModInfo(targetfolder));

			Console.WriteLine(targetMod.ModName + " Version " + targetMod.version + " has been installed successfully");

			if (File.Exists(targetMod.zipFileName))
			{
				Console.WriteLine("Removing downloaded zip file.");
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
