using HarmonyLib;
using Klei;
using KMod;
using LocalModLoader.DataClasses;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UtilLibs;

namespace LocalModLoader
{
	public class Mod : UserMod2
	{
		public static Mod Instance;
		public static string CustomTargetModFolderPath => Path.Combine(CustomModsPath(), Info.TargetStaticID);
		public string ModZipDownloadPath => Path.Combine(CustomDownloadsPath(), Info.TargetStaticID + ".zip");
		public static TargetModInfo Info = new();
		static LoadedModData Sideload;
		static Content SideloadContent;
		static KMod.Directory sideload_file_source;

		public override void OnLoad(Harmony harmony)
		{
			string configPath = Path.Combine(IO_Utils.ModPath, "target.json");
			if (IO_Utils.ReadFromFile<TargetModInfo>(configPath, out var parsedInfo))
			{
				Info = parsedInfo;
			}
			else
			{
				IO_Utils.WriteToFile(Info, configPath);
			}
			Instance = this;
			base.OnLoad(harmony);

			if (Info.TargetStaticID.IsNullOrWhiteSpace())
			{
				SgtLogger.error("Mod Loader mod was not configured to load a specific mod. aborting");
				return;
			}
			LoadModCustom();
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			if (Sideload == null) 
				return;

			foreach (var userModInstance in Sideload.userMod2Instances)
			{
				SgtLogger.l("Passing OnAllModsLoaded to Sideload: " + userModInstance.Key.GetName());
				userModInstance.Value.OnAllModsLoaded(harmony, mods);
			}
		}


		public static string CustomModsPath()
		{
			string playerPrefsDefinedFolder = KPlayerPrefs.GetString("CustomModsFolder");
			if (string.IsNullOrEmpty(playerPrefsDefinedFolder))
				return Path.Combine(Application.persistentDataPath, "mods","SideLoaded");
			return playerPrefsDefinedFolder;
		}
		public static string CustomDownloadsPath()
		{
			string playerPrefsDefinedFolder = KPlayerPrefs.GetString("CustomDownloadsFolder");

			if (string.IsNullOrEmpty(playerPrefsDefinedFolder))
				return Path.Combine(Application.persistentDataPath, "mod_downloads");
			return playerPrefsDefinedFolder;
		}

		public void LoadModCustom()
		{
			System.IO.Directory.CreateDirectory(CustomTargetModFolderPath);
			SgtLogger.l("Custom Mod Path: " + CustomTargetModFolderPath);
			LoadChainModVersion();

			if (VersionCheck.UpdateAvailable(out var info))
			{
				SgtLogger.l("Update Available");
				UpdateModFrom(info);
				LoadChainModVersion();
			}
			LoadModFiles();
		}

		const string ModYaml = "mod.yaml", ModInfoYaml = "mod_info.yaml";
		void LoadChainModVersion()
		{
			var modyamlFile = Path.Combine(CustomTargetModFolderPath, ModYaml);
			var modinfoyamlFile = Path.Combine(CustomTargetModFolderPath, ModInfoYaml);
			if (!File.Exists(modyamlFile) || !File.Exists(modinfoyamlFile))
			{
				SgtLogger.l("target mod not installed yet");
				mod.packagedModInfo.version = "not installed";
				return;
			}
			sideload_file_source = new KMod.Directory(CustomTargetModFolderPath);
			KModHeader header = KModUtil.GetHeader(sideload_file_source, this.mod.label.defaultStaticID, this.mod.label.title, this.mod.description, this.mod.IsDev);
			mod.label.title = header.title;
			mod.staticID = header.staticID;
			mod.description = header.description;


			string readText = sideload_file_source.Read(ModInfoYaml);
			if (!readText.IsNullOrWhiteSpace())
			{
				YamlIO.ErrorHandler handle_error = ((e, force_warning) => YamlIO.LogError(e, true));
				var modInfoForFolder = YamlIO.Parse<KMod.Mod.PackagedModInfo>(readText, new FileHandle(), handle_error);
				if (modInfoForFolder.minimumSupportedBuild > VersionCheck.GetGameVersion())
					mod.packagedModInfo.version = "Incompatible Version!";
				else
					mod.packagedModInfo.version = modInfoForFolder.version;
			}
		}
		void UpdateModFrom(RemoteModInfo remoteModInfo)
		{
			System.IO.Directory.Delete(CustomTargetModFolderPath, true);
			System.IO.Directory.CreateDirectory(CustomTargetModFolderPath);
			if (File.Exists(ModZipDownloadPath))
			{
				File.Delete(ModZipDownloadPath);
			}

			SgtLogger.l("Starting installation of " + remoteModInfo.staticID + "...");

			if (!WebRequestHelper.TryDownloadModFile(remoteModInfo.downloadURL, ModZipDownloadPath))
			{
				SgtLogger.warning("download failed!");
				return;
			}
			SgtLogger.l("Mod zip download successful. File size: " + WebRequestHelper.GetReadableFileSize(new FileInfo(ModZipDownloadPath).Length));
			try
			{
				System.IO.Compression.ZipFile.ExtractToDirectory(ModZipDownloadPath, CustomTargetModFolderPath);
				SgtLogger.l("Mod installed successfully");
			}
			catch (Exception ex)
			{
				SgtLogger.error("Error during zip extraction:\n" + ex.Message);
			}
			if (File.Exists(ModZipDownloadPath))
			{
				SgtLogger.l("Removing downloaded zip file.");
				File.Delete(ModZipDownloadPath);
			}
		}
		void LoadModFiles()
		{
			if (!System.IO.Directory.GetFiles(CustomTargetModFolderPath).Any())
			{
				SgtLogger.l("could not load custom mod from target folder, its empty!");
				return;
			}
			LoadModLayerableFiles();
			SgtLogger.l("loading custom mod dll");
			Sideload = DLLLoader.LoadDLLs(this.mod, this.mod.staticID, CustomTargetModFolderPath, false);
			var modData = this.mod.loaded_mod_data;
			mod.ScanContentFromSource(CustomTargetModFolderPath, out var content);
			SgtLogger.l("loaded mod content available: " + content);
			SideloadContent = content;
		}


		[HarmonyPatch(typeof(Manager), nameof(Manager.Load))]
		public class Manager_Load_Patch
		{
			public static void Postfix(Manager __instance, Content content)
			{
				if(content == Content.Animation && (SideloadContent & Content.Animation) != 0 && LoadModAnimations())
				{
					SgtLogger.l("loading mod animations..");
				}
			}
		}

		static void LoadModLayerableFiles()
		{
			SgtLogger.l("FileSystem: " + sideload_file_source.GetFileSystem().GetRoot());
			FileSystem.file_sources.Insert(0, sideload_file_source.GetFileSystem());
		}

		static bool LoadModAnimations()
		{
			string path = FileSystem.Normalize(System.IO.Path.Combine(CustomTargetModFolderPath, "anim"));
			if (!System.IO.Directory.Exists(path))
				return false;
			int num = 0;
			foreach (DirectoryInfo directory1 in new DirectoryInfo(path).GetDirectories())
			{
				foreach (DirectoryInfo directory2 in directory1.GetDirectories())
				{
					KAnimFile.Mod anim_mod = new KAnimFile.Mod();
					foreach (FileInfo file in directory2.GetFiles())
					{
						if (!file.Name.StartsWith("._"))
						{
							if (file.Extension == ".png")
							{
								byte[] data = File.ReadAllBytes(file.FullName);
								Texture2D tex = new Texture2D(2, 2);
								tex.LoadImage(data);
								anim_mod.textures.Add(tex);
							}
							else if (file.Extension == ".bytes")
							{
								string withoutExtension = System.IO.Path.GetFileNameWithoutExtension(file.Name);
								byte[] numArray = File.ReadAllBytes(file.FullName);
								if (withoutExtension.EndsWith("_anim"))
									anim_mod.anim = numArray;
								else if (withoutExtension.EndsWith("_build"))
									anim_mod.build = numArray;
								else
									DebugUtil.LogWarningArgs((object)$"Unhandled TextAsset ({file.FullName})...ignoring");
							}
							else
								DebugUtil.LogWarningArgs((object)$"Unhandled asset ({file.FullName})...ignoring");
						}
					}
					string name = directory2.Name + "_kanim";
					if (anim_mod.IsValid() && ModUtil.AddKAnimMod(name, anim_mod))
						++num;
				}
			}
			return num > 0;
		}
	}
}
