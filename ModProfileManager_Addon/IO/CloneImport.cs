using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilLibs;

namespace ModProfileManager_Addon.IO
{
	internal class CloneImport
	{
		static string[] clonePresetFolders = new string[]{
			Path.Combine(KMod.Manager.GetDirectory(),"Steam","3284635963","saved_presets")
		};

		public static void ImportFromClone()
		{
			if (ModAssets.ClonePresets.Count > 0)
			{
				return;
			}
			Dictionary<string, ClonePreset> clones = new();
			foreach (var folder in clonePresetFolders)
			{
				if (Directory.Exists(folder))
				{
					var files = new DirectoryInfo(folder).GetFiles();

					foreach (FileInfo cloneFile in files)
					{
						try
						{
							if (TryReadFromClone(cloneFile, out ClonePreset clone) && clone.modIDs.Count > 0)
							{
								if (!clones.ContainsKey(clone.Name))
									clones.Add(clone.Name, clone);
							}
						}
						catch (Exception e)
						{
							SgtLogger.warning("Couln't import Mod preset from: " + cloneFile.FullName + ", Error: " + e);
						}
					}
				}
			}
			foreach (var clone in clones.Values)
			{
				if (!ModAssets.ClonePresets.ContainsKey(clone.Name))
				{
					var import = new SaveGameModList(clone.Name, true);
					import.SetClone(true);
					ModAssets.ClonePresets.Add(clone.Name, import);
					import.SavePoints[clone.Name] = new();
				}
			}

			var mm = Global.Instance.modManager;
			foreach (var mod in mm.mods)
			{
				foreach (var clone in clones.Values)
				{
					if (clone.modIDs.Contains(mod.label.id))
					{
						ModAssets.ClonePresets[clone.Name].SavePoints[clone.Name].Add(mod.label);
					}
				}
			}
			List<string> ToRemove = new();
			foreach (var entry in ModAssets.ClonePresets)
			{
				if (entry.Value.SavePoints.Count == 0 || entry.Value.SavePoints.Last().Value.Count == 0)
				{
					ToRemove.Add(entry.Key);
				}
			}
			foreach (var toRemoveEntry in ToRemove)
			{
				ModAssets.ClonePresets.Remove(toRemoveEntry);
			}
		}

		class ClonePreset
		{
			public ClonePreset(string name)
			{
				Name = name;
			}
			public string Name;
			public HashSet<string> modIDs = new HashSet<string>();
		}

		private static bool TryReadFromClone(FileInfo cloneFile, out ClonePreset preset)
		{
			preset = null;
			if (cloneFile.Exists)
			{
				try
				{
					var stream = cloneFile.OpenRead();
					using StreamReader reader = new(stream);
					using JsonTextReader jsonReader = new JsonTextReader(reader);

					JObject rootObject = (JObject)JToken.ReadFrom(jsonReader).Root;


					JToken NameToken = rootObject.SelectToken("Name");
					if (NameToken == null || NameToken.Type != JTokenType.String)
						return false;
					string PresetName = NameToken.Value<string>();

					JToken modsToken = rootObject.SelectToken("Mods");

					if (modsToken == null || modsToken.Type != JTokenType.Array)
						return false;


					JArray modsArray = modsToken.Value<JArray>();

					if (modsArray == null)
						return false;

					HashSet<string> modIDs = new();
					foreach (JToken mod in modsArray)
					{
						JToken idToken = mod.SelectToken("Id");
						if (NameToken == null || NameToken.Type != JTokenType.String)
							continue;
						string id = idToken.Value<string>();
						modIDs.Add(id);
					}

					if (modIDs.Count <= 0)
						return false;

					preset = new(PresetName);
					preset.modIDs = modIDs;
					return true;
				}

				catch (Exception exception)
				{
					Debug.Log("Error when importing preset from clone: " + cloneFile.Name + ",\n" + nameof(exception) + ": " + exception.Message);
				}
			}
			return false;
		}
	}
}
