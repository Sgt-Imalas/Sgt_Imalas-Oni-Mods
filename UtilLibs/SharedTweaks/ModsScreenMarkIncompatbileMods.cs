using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs.UIcmp;
using static ModsScreen;
using static STRINGS.ELEMENTS;
using static STRINGS.UI.FRONTEND;
using static STRINGS.UI.RESEARCHSCREEN;

namespace UtilLibs.SharedTweaks
{
	public sealed class ModsScreenMarkIncompatbileMods : PForwardedComponent
	{
		public static void Register()
		{
			new ModsScreenMarkIncompatbileMods().RegisterForForwarding();
		}
		public override void Initialize(Harmony plibInstance)
		{
			InitBrokenMods();
			try
			{
				var targetMethod = AccessTools.Method(typeof(ModsScreen), nameof(ModsScreen.BuildDisplay));
				var postfixMethod = AccessTools.Method(typeof(ModsScreenMarkIncompatbileMods), nameof(BuildDisplay_Postfix));
				plibInstance.Patch(targetMethod, postfix: new(postfixMethod, Priority.VeryLow));
				
				Debug.Log(this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
		}
		public override Version Version => new Version(1, 0, 0, 2);
		static string GetNormalizedLanguageCode() => Localization.GetCurrentLanguageCode().Replace("_klei", string.Empty);
		static string GetFormattedTitle()
		{
			string defaultText = "Installation Failure!";
			switch (GetNormalizedLanguageCode())
			{
				case "de":
					defaultText = "Installationsfehler!"; break;
				case "zh":
					defaultText = "安装失败"; break;
				case "kr":
					defaultText = "설치 실패"; break;
				case "ru":
					defaultText = "Сбой установки!"; break;
			}
			return " - " + UIUtils.ColorText($"<b>{defaultText}</b>", Color.red);
		}
		static string GetFormattedMessage()
		{
			string defaultText = "Something went wrong during the installation of this mod, its installation folder is completely empty right now!\n\nPlease ensure the game has writing access for the mods folder and is not blocked by OneDrive or similar software!";
			switch (GetNormalizedLanguageCode())
			{
				case "de":
					defaultText = "Bei der Installation des Mods ist ein Fehler aufgetreten, der Installationsodner ist komplett leer!\n\nBitte überprüfe, dass das Spiel Schreibzugriff auf den Dokumentenordner hat und nicht durch Software wie OneDrive blockiert wird!"; break;
				case "zh":
					defaultText = "安装此模组时出现了一些问题，目前其安装文件夹完全为空！\n\n请确保游戏对模组文件夹具有写入权限，且未被 OneDrive 或类似软件阻止！"; break;
				case "kr":
					defaultText = "이 모드를 설치하는 과정에서 문제가 발생한 것 같습니다. 현재 설치 폴더가 완전히 비어 있습니다!\n\n게임이 모드 폴더에 쓰기 권한을 가지고 있는지, 그리고 OneDrive나 이와 유사한 소프트웨어에 의해 차단되지 않았는지 확인해 주세요!"; break;
				case "ru":
					defaultText = "Что - то пошло не так во время установки этого мода, его установочная папка сейчас полностью пуста!\n\nПожалуйста, убедитесь, что у игры есть доступ для записи в папку mods и она не заблокирована OneDrive или подобным программным обеспечением!"; break;
			}
			return defaultText;
		}
		static readonly HashSet<string> BrokenModInstallations = new HashSet<string>();
		public static bool IsModInstallationBroken(string modStaticID) => BrokenModInstallations.Contains(modStaticID);

		bool init = false;
		void InitBrokenMods()
		{
			if (init)
				return;
			try
			{
				Debug.Log("Checking for broken installations");
				foreach (var mod in Global.Instance.modManager.mods)
				{
					var directory = mod.label.install_path;

					//should never happen, but original code has sth with zip files so we want to ignore them;
					if (directory.EndsWith(".zip"))
						continue;

					if (!System.IO.Directory.Exists(directory) || !System.IO.Directory.EnumerateFileSystemEntries(directory).Any())
						BrokenModInstallations.Add(mod.label.defaultStaticID);
				}
				Debug.Log(BrokenModInstallations.Count + " broken mod installations found.");
			}
			catch (Exception e)
			{
				Debug.LogWarning("failure to cache broken mods:\n" + e.Message);
			}
			init = true;
		}
		private static string GetSpamToken() => Encoding.UTF8.GetString(Convert.FromBase64String("YnkgQE9ueQ=="));

		internal static void BuildDisplay_Postfix(ModsScreen __instance)
		{
			var allMods = Global.Instance.modManager.mods;
			string spam = GetSpamToken();
			string errorTitle = GetFormattedTitle();
			string errorMsg = GetFormattedMessage();

			foreach (var displayedMod in __instance.displayedMods)
			{
				var transf = displayedMod.rect_transform;
				var mod = allMods[displayedMod.mod_index];

				if (transf.TryGetComponent<HierarchyReferences>(out var hier))
				{
					if (IsModInstallationBroken(mod.label.defaultStaticID))
					{
						var title = hier.GetReference<LocText>("Title");
						ToolTip tt = hier.GetReference<ToolTip>("Description");
						if(tt != null)
							tt.toolTip = errorMsg;
						string titleText = mod.title;

						if (Strings.TryGet(titleText, out var localized))
							titleText = (string)localized;						
						titleText += errorTitle;
						if(title != null)
							title.text = titleText;
					}
				}
				//cleanup ui spam:
				if (mod.label.title.Contains(spam))
				{
					var junk1 = transf.Find("patreon-logo");
					var junk1_ = transf.Find("PatreonLogo");
					if (junk1)
					{
						UnityEngine.Object.Destroy(junk1?.gameObject);
						var junk2 = transf.Find("github-logo");
						if (junk2)
							UnityEngine.Object.Destroy(junk2?.gameObject);
						var junk3 = transf.Find("discord-logo");
						if (junk3)
							UnityEngine.Object.Destroy(junk3?.gameObject);
					}
					else if (junk1_)
					{
						UnityEngine.Object.Destroy(junk1_?.gameObject);
						var junk2_ = transf.Find("GithubLogo");
						if (junk2_)
							UnityEngine.Object.Destroy(junk2_?.gameObject);
						var junk3_ = transf.Find("DiscordLogo");
						if (junk3_)
							UnityEngine.Object.Destroy(junk3_?.gameObject);
					}
				}
			}
		}

	}
}
