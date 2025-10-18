using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace UtilLibs
{
	public static class IO_Utils
	{
		public static string ModID => Assembly.GetExecutingAssembly().GetName().Name;
		public static string ModPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		public static string ModsFolder => KMod.Manager.GetDirectory();
		public static string ConfigsFolder => Path.Combine(ModsFolder, "config");

		public static void PutToClipboard(string toPut)
		{
			var TextEditorType = Type.GetType("UnityEngine.TextEditor, UnityEngine");
			if (TextEditorType != null)
			{
				var editor = Activator.CreateInstance(TextEditorType);
				var tr = Traverse.Create(editor);
				tr.Property("text").SetValue(toPut);
				tr.Method("SelectAll").GetValue();
				tr.Method("Copy").GetValue();
			}
		}
		public static bool ReadFromFile<T>(FileInfo filePath, out T output, string forceExtensionTo = "", JsonSerializerSettings converterSettings = null)
		{
			try
			{
				if (!filePath.Exists || (forceExtensionTo != string.Empty && filePath.Extension != forceExtensionTo) 
					&& !filePath.Name.StartsWith("._")//macOS hidden files
					)
				{
					SgtLogger.logwarning(filePath.FullName, "File does not exist!");
					output = default(T);
					return false;
				}
				else
				{
					FileStream filestream = filePath.OpenRead();
					using (var sr = new StreamReader(filestream))
					{
						string jsonString = sr.ReadToEnd();
						output = JsonConvert.DeserializeObject<T>(jsonString, converterSettings);
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				SgtLogger.warning("failed reading " + filePath.FullName + ":\n\n" + ex.Message);
				output = default(T);
				return false;
			}
		}
		public static bool ReadFromFile<T>(string FileOrigin, out T output, string forceExtensionTo = "", JsonSerializerSettings converterSettings = null) => ReadFromFile(new FileInfo(FileOrigin), out output, forceExtensionTo, converterSettings);
		
		public static bool WriteToFile<T>(T DataObject, string filePath, JsonSerializerSettings converterSettings = null)
		{
			try
			{
				var fileInfo = new FileInfo(filePath);
				FileStream fcreate = fileInfo.Open(FileMode.Create);

				var JsonString = JsonConvert.SerializeObject(DataObject, Formatting.Indented, converterSettings);
				using (var streamWriter = new StreamWriter(fcreate))
				{
					streamWriter.Write(JsonString);
				}
				return true;
			}
			catch (Exception e)
			{
				SgtLogger.logError("Could not write file, Exception: " + e);
				return false;
			}
		}
		public static bool DeleteFile(string filePath)
		{
			try
			{
				var fileInfo = new FileInfo(filePath);
				fileInfo.Delete();
				return true;
			}
			catch (Exception e)
			{
				SgtLogger.logError("Could not delete file, Exception: " + e);
				return false;
			}
		}
	}
}
