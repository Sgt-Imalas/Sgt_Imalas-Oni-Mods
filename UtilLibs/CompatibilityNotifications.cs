using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine.PlayerLoop;

namespace UtilLibs
{
	public class CompatibilityNotifications
	{
		public const string CompatibilityDataKey = "Sgt_Imalas_IncompatibleModList";

		public static void CheckAndAddIncompatibles(string assemblyName, string modName, string conflictingMod)
		{
			Debug.Log("checking if incompatible mod is installed: " + assemblyName);
			initList();
			if (AppDomain.CurrentDomain.GetAssemblies().ToList().Any(ass => ass.FullName.ToLowerInvariant().Contains(assemblyName.ToLowerInvariant())))
			{
				Debug.Log("incompatible mod found: " + assemblyName);
				AddIncompatibleToList(modName, conflictingMod);
			}
			else
				Debug.Log("mod not found: " + assemblyName);
		}


		static string BrokenTimeoutFixed = "CrapManager_BrokenTimeoutFixed";
		static int ManagerFixVersion = 1;
		public static void FixBrokenTimeout(Harmony harmony)
		{
			if (PRegistry.GetData<int>(BrokenTimeoutFixed) >= ManagerFixVersion)
			{
				return;
			}
			PRegistry.PutData(BrokenTimeoutFixed, ManagerFixVersion);
			//UtilMethods.ListAllTypesWithAssemblies();

			var targetType = Type.GetType("Ony.OxygenNotIncluded.ModManager.Updater, Release_DLC1.Mod.ModManager");
			if (targetType == null)
			{
				Debug.Log("mod manager fix target type not found");
				return;
			}
            var innerClass = targetType.GetNestedTypes(AccessTools.all).FirstOrDefault(t => !t.FullName.Contains("All") && t.FullName.Contains("<Update>"));

			if(innerClass == null)
			{
				Debug.Log("mod manager update inner type not found");
				return;
			}

			var method = AccessTools.Method(innerClass, "MoveNext");
			if(method  == null)
			{
				Debug.Log("mod manager update method missing");
				return;
			}


			Debug.Log("fixing broken timeout in mod manager...");
			var methodtranspiler = AccessTools.Method(typeof(CompatibilityNotifications),nameof(BrokenTimeoutFixTranspiler));

			harmony.Patch(method, transpiler: new(methodtranspiler));

		}

		public static IEnumerable<CodeInstruction> BrokenTimeoutFixTranspiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
		{
			var codes = orig.ToList();

			var index = codes.FindIndex(c => c.LoadsConstant(5000));

			if (index == -1)
			{
				return codes;
			}
			codes[index].operand = 999999999;
			return codes;
		}



		//public static void ImproveUserExperience(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		//{
		//    var buggyMods = mods.Where(mod => mod.label.title.Contains("// by @Ony"));
		//    if (buggyMods.Count() == 0)
		//        return;

		//    HashSet<string> mistakes = new HashSet<string>(buggyMods.Select(mod => mod.staticID).ToList());

		//    foreach(var mistake in mistakes)
		//    {
		//        harmony.UnpatchAll(mistake);
		//    }
		//}

		public static void RemoveCrashingIncompatibility(Harmony harmony, IReadOnlyList<KMod.Mod> mods, string faultyId)
		{
			faultyId = faultyId.ToLowerInvariant();
			var faultyMod = mods.FirstOrDefault(mod => mod.staticID.ToLowerInvariant().Contains(faultyId));
			if (faultyMod != null)
			{
				faultyMod.SetCrashed();
				faultyMod.SetEnabledForActiveDlc(false);
				harmony.UnpatchAll(faultyMod.staticID);
			}
		}

		public static void DisableLoggingPrevention(IReadOnlyList<KMod.Mod> _mods)
		{

			//AddIncompatibleToList("Oxygen Not Included", "Ony Debug Console");

			List<KMod.Mod> mods = _mods.ToList();

			Mod faultyMod = mods.Find(mod => mod.staticID.ToUpperInvariant().Contains("DEBUGCONSOLE"));

			if (faultyMod != null && faultyMod.IsEnabledForActiveDlc())
			{
				Debug.Log("DebugConsole detected, disabling");
				faultyMod.SetCrashed();
				faultyMod.SetEnabledForDlc("", false);
				faultyMod.SetEnabledForDlc("EXPANSION1_ID", false);

				//App.instance.Restart();
			}
		}
		public static void FlagLoggingPrevention(IReadOnlyList<KMod.Mod> _mods)
		{

			//AddIncompatibleToList("Oxygen Not Included", "Ony Debug Console");

			List<KMod.Mod> mods = _mods.ToList();

			Mod faultyMod = mods.Find(mod => mod.staticID.ToUpperInvariant().Contains("DEBUGCONSOLE"));

			if (faultyMod != null && faultyMod.IsEnabledForActiveDlc())
			{
				faultyMod.SetCrashed();
				AddIncompatibleToList(GameName, faultyMod.title);
			}
		}
		static readonly string GameName = "OxygenNotIncluded_DebugConsole";


		static void initList()
		{
			if (PRegistry.GetData<Dictionary<string, string>>(CompatibilityDataKey) != null)
				return;

			var current = new List<Tuple<string, string>>();
			//foreach(var ass in AppDomain.CurrentDomain.GetAssemblies().ToList())
			//{
			//    SgtLogger.l(ass.FullName);
			//}
			PRegistry.PutData(CompatibilityDataKey, current);
		}


		static void AddIncompatibleToList(string modName, string conflictingModName)

		{
			Dictionary<string, string> current = PRegistry.GetData<Dictionary<string, string>>(CompatibilityDataKey);
			if (current == null)
			{
				current = new Dictionary<string, string>();
			}

			if (conflictingModName.Count() > 40)
			{
				conflictingModName = conflictingModName.Remove(40);
				conflictingModName += "...";
			}

			if (modName == GameName)
			{
				if (!current.ContainsKey(modName))
				{
					current.Add(modName, "");
					current[modName] = conflictingModName;
				}
			}
			else
			{
				if (!current.ContainsKey(modName))
					current.Add(modName, "");
				current[modName] += "\n• " + conflictingModName;
			}

			PRegistry.PutData(CompatibilityDataKey, current);

		}
		public static void DumpIncompatibilityMessage(MainMenu parent)
		{
			Dictionary<string, string> current = PRegistry.GetData<Dictionary<string, string>>(CompatibilityDataKey);
			if (current == null || current.Count == 0)
				return;

			foreach (var item in current)
			{
				if (item.Key == GameName)
				{
					StringBuilder message = new StringBuilder();
					message.AppendLine($"The mod \"{item.Value}\" has been detected, please disable it as it prevents other mods from proper logging.");
					KMod.Manager.Dialog(parent.gameObject, "Debug Console detected", message.ToString(),
								STRINGS.UI.CONFIRMDIALOG.OK);
				}
				else
				{
					StringBuilder message = new StringBuilder();
					message.AppendLine($"{item.Key} has declared the following mods as conflicting:");
					message.AppendLine(item.Value);
					KMod.Manager.Dialog(parent.gameObject, "Conflicting Mods found!", message.ToString(),
								STRINGS.UI.CONFIRMDIALOG.OK);
				}
			}


			PRegistry.PutData(CompatibilityDataKey, null);


		}

		[HarmonyPatch(typeof(MainMenu), "OnSpawn")]
		public static class MainMenu_OnSpawn_Patch
		{
			/// <summary>
			/// Applied after Update runs.
			/// </summary>
			internal static void Postfix(MainMenu __instance)
			{
				DumpIncompatibilityMessage(__instance);
			}
		}

	}
}
