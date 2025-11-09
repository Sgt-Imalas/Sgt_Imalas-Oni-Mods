using ElementUtilNamespace;
using LocalModLoader.DataClasses;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UtilLibs;
using static AnimEventHandler;
using static KleiItems;

namespace LocalModLoader
{
	internal class WebRequestHelper
	{
		static HttpClient _client = null;
		static HttpClient client
		{
			get
			{
				if(_client == null)
					_client = new HttpClient();
				return _client;
			}
		}
		internal static uint GetGameVersion()
		{
			return (uint)typeof(KleiVersion).GetField("ChangeList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
		}
		public static string GetReadableFileSize(double len)
		{
			string[] sizes = { "B", "KB", "MB", "GB", "TB" };
			int order = 0;
			while (len >= 1024 && order < sizes.Length - 1)
			{
				order++;
				len /= 1024;
			}
			return string.Format("{0:0.##} {1}", len, sizes[order]);
		}

		public static bool TryDownloadModFile(string url, string path)
		{
			try
			{
				Console.WriteLine("downloading " + url + " to " + path);
				using FileStream fs = new FileStream(path, FileMode.Create);
				var res = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
				res.Content.CopyToAsync(fs).ConfigureAwait(false).GetAwaiter().GetResult();
				Console.WriteLine("download finished");
				return File.Exists(path);
			}
			catch (Exception e)
			{
				SgtLogger.l("error while downloading:\n" + e.Message);
				return false;
			}
		}

		public static bool TryGetRemoteHashMap(string url, out HashMap map)
		{
			map = null;
			try
			{
				var json = client.GetStringAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
				if (json.IsNullOrWhiteSpace())
				{
					SgtLogger.warning("version json was empty");
					return false;
				}

				map = Newtonsoft.Json.JsonConvert.DeserializeObject<HashMap>(json);
				if (map == null)
				{
					SgtLogger.warning("could not deserialize hashmap from json");
					return false;
				}
				return true;
			}
			catch (Exception e)
			{
				SgtLogger.warning("error while fetching hashmap:\n");
				Debug.LogWarning(e.Message);
				return false;
			}
		}

		public static bool TryGetRemoteVersionInfo(string url, out RemoteModInfo remoteInfo)
		{
			remoteInfo = null;
			try
			{
				var json = client.GetStringAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
				if (json.IsNullOrWhiteSpace())
				{
					SgtLogger.warning("version json was empty");
					return false;
				}

				var info = Newtonsoft.Json.JsonConvert.DeserializeObject<RemoteVersionInfo>(json);
				remoteInfo = info.mods.FirstOrDefault(mod => mod.staticID == Mod.Info.TargetStaticID);
				if (remoteInfo == null)
				{
					SgtLogger.warning("could not find target id " + Mod.Info?.TargetStaticID + " in data");
					return false;
				}
				return true;
			}
			catch (Exception e)
			{
				SgtLogger.warning("error while fetching version:\n");
				Debug.LogWarning(e.Message);
				return false;
			}
		}
	}
}
