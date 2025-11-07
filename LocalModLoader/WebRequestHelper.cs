using ElementUtilNamespace;
using LocalModLoader.DataClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UtilLibs;
using static AnimEventHandler;
using static KleiItems;

namespace LocalModLoader
{
	internal class WebRequestHelper
	{
		public static IEnumerator TryGetRequest(string url, System.Action<string> OnComplete, System.Action<string> OnFail)
		{
			//Debug.Log("Calling MNI API: " + url);
			using (UnityWebRequest request = new UnityWebRequest(url, "GET"))
			{
				request.downloadHandler = new DownloadHandlerBuffer();
				request.timeout = 20;
				Debug.Log("Trying to send GET Request ...");
				yield return request.SendWebRequest();

				while (!request.isDone)
					yield return true;

				if (request.result != UnityWebRequest.Result.Success)
				{
					Debug.LogWarning(request.error);
					//ModAssets.ConnectionError();
					OnFail(request.downloadHandler.text);
				}
				else
				{
					Debug.Log("GET Request complete!");
					//ModAssets.ConnectionSuccessful();
					OnComplete(request.downloadHandler.text);
				}
			}
		}

		static IEnumerator DownloadModZip(string url, string path, Action<bool> OnComplete)
		{
			var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
			uwr.downloadHandler = new DownloadHandlerFile(path);
			yield return uwr.SendWebRequest();


			while (!uwr.isDone)
				yield return true;

			bool success = uwr.result == UnityWebRequest.Result.Success;

			if (!success)
				Debug.LogError(uwr.error);
			else
				Debug.Log("File successfully downloaded and saved to " + path);
			OnComplete(success);
		}

		public static bool TryDownloadModFile(string url, string path)
		{
			bool downloadSuccessful = false ;
			Action<bool> onComplete = (bool success) => downloadSuccessful = success;

			var downloadRequest = DownloadModZip(url, path, onComplete);
			//block thread to prevent race condition.
			while (downloadRequest.MoveNext());

			return downloadSuccessful;
		}

		public static bool GetRemoteVersionInfo(string versionInfoUrl, out RemoteModInfo remoteInfo)
		{
			remoteInfo = null;
			var writeError = (string error) => SgtLogger.l("Could not fetch version info for mod " + Mod.Info.TargetStaticID + ", error:\n" + error);
			RemoteVersionInfo info = null;
			var versionParse = (string json) =>
			{
				info = Newtonsoft.Json.JsonConvert.DeserializeObject<RemoteVersionInfo>(json);
			};

			var getRequest = TryGetRequest(versionInfoUrl, versionParse, writeError);
			//block thread to prevent race condition.
			while (getRequest.MoveNext());

			if (info == null)
			{
				SgtLogger.warning("failed to fetch mod info data");
				return false;
			}

			remoteInfo = info.mods.FirstOrDefault(mod => mod.staticID == Mod.Info.TargetStaticID);
			if (remoteInfo == null)
			{
				SgtLogger.warning("could not find target id " + Mod.Info.TargetStaticID + " in data");
				return false;
			}
			return true;

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
				len = len / 1024;
			}
			return String.Format("{0:0.##} {1}", len, sizes[order]);
		}
	}
}
