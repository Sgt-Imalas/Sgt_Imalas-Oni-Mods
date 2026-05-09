using Dupery;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace DuperyFixed.Source.Patch
{
	internal class MinionBrowserScreen_Patches
	{
		[HarmonyPatch]
		public class MinionBrowserScreen_PopulateGallery_Patch
		{

			[HarmonyPrepare]
			public static bool Prepare() => FindMethod_PopulateGallery() != null;
			[HarmonyTargetMethod]
			public static MethodBase FindMethod_PopulateGallery()
			{
				var targetType = typeof(MinionBrowserScreen);
				foreach (var method in targetType.GetMethods(AccessTools.all))
				{
					if (method.Name.Contains("AddGridIcon"))
					{
						SgtLogger.l("MinionBrowserScreen_AddGridIcon found as:" + method.Name);
						return method;
					}
				}
				SgtLogger.warning("COULD NOT FIND ADDGRIDICON!!!");
				foreach (var method in targetType.GetMethods(AccessTools.all))
				{
					SgtLogger.l(method.Name);
				}
				return null;
			}

			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();

				int injectionIndex = codes.Count - 2;
				codes.InsertRange(injectionIndex, [
					new CodeInstruction(OpCodes.Ldarg_1),//griditem
					new CodeInstruction(OpCodes.Ldloc_1),//assigned GO
					new CodeInstruction(OpCodes.Call,AccessTools.Method(typeof(MinionBrowserScreen_PopulateGallery_Patch),nameof(AddDuperyTooltip)))]
					);
				return codes;
			}
			static void AddDuperyTooltip(MinionBrowserScreen.GridItem dupe, GameObject item)
			{
				if(!PersonalityManager.TryGetModId(dupe.GetPersonality().Id, out var modId))
					return;
				string modName = GetModName(modId);
				item.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(global::STRINGS.UI.MINION_BROWSER_SCREEN.TOOLTIP_FROM_DLC.Replace("DLC",string.Empty), modName));
			}
			static Dictionary<string,string> cachedModNames= new Dictionary<string,string>();
			static string GetModName(string id)
			{
				
				if(!cachedModNames.TryGetValue(id, out string modName))
				{
					KMod.Manager modManager = Global.Instance.modManager;
					foreach(var mod in modManager.mods)
					{
						if (!mod.IsEnabledForActiveDlc())
							continue;
						string name = global::STRINGS.UI.PRE_KEYWORD + mod.title+ global::STRINGS.UI.PST_KEYWORD;
						cachedModNames[mod.staticID] = name;
						if (mod.staticID == id)
						{
							modName = name;
							break;
						}
					}
				}
				if (modName.IsNullOrWhiteSpace())
					modName = id;
				return modName;
			}
		}
	}
}
