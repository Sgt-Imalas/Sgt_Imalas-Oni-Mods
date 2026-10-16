using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace UtilLibs
{
	public static class IO_Utils
	{
		public static string ModID => Assembly.GetExecutingAssembly().GetName().Name;
		public static string ModPath => _modPath;
		private static string _modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		public static void OverrideModPath(string newPath) => _modPath = newPath;
		public static string ModsFolder => KMod.Manager.GetDirectory();
		public static string ConfigsFolder => Path.Combine(ModsFolder, "config");
		public static string ModConfigFolder => Path.Combine(ModsFolder, "config", ModID);

		public static void PutToClipboard(string toPut)
		{
			GUIUtility.systemCopyBuffer = toPut;
		}

		public static bool TryGetStringFromClipboard(out string clipboardText)
		{
			clipboardText = GUIUtility.systemCopyBuffer;
			return !clipboardText.IsNullOrWhiteSpace();
		}

		public static bool ReadFromFile<T>(FileInfo filePath, out T output, string forceExtensionTo = "", JsonSerializerSettings converterSettings = null)
		{
			try
			{
				if (!filePath.Exists || (forceExtensionTo != string.Empty && filePath.Extension != forceExtensionTo) || filePath.MacFile())
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

		public static void DumpToFile(object data, string path, bool useCustomConverter = true)
		{
			var converterSettings = new JsonSerializerSettings()
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				TypeNameHandling = TypeNameHandling.Objects,
				Formatting = Formatting.Indented,
				ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
				ContractResolver = new InjectionMethods.IncludePrivateContractResolver(),
				Converters = [new Vector2IConverter()]
			};
			WriteToFile(data, path, useCustomConverter ? converterSettings : null);
		}

		public static bool NotAMacFile(this FileInfo fileInfo)
		{
			return !MacFile(fileInfo);
		}
		public static bool MacFile(this FileInfo fileInfo)
		{
			return fileInfo.Name.StartsWith("._");
		}

		public sealed class Vector2IConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(Vector2I);
			}

			public override void WriteJson(
				JsonWriter writer,
				object value,
				JsonSerializer serializer)
			{
				var v = (Vector2I)value;

				writer.WriteStartObject();
				writer.WritePropertyName("x");
				writer.WriteValue(v.X);
				writer.WritePropertyName("y");
				writer.WriteValue(v.Y);
				writer.WriteEndObject();
			}

			public override object ReadJson(
				JsonReader reader,
				Type objectType,
				object existingValue,
				JsonSerializer serializer)
			{
				var obj = JObject.Load(reader);

				return new Vector2I(
					obj["x"]!.Value<int>(),
					obj["y"]!.Value<int>()
				);
			}
		}


		public static T DeepClone<T>(this T obj)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;

				return (T)formatter.Deserialize(ms);
			}
		}

	}
}
