using HarmonyLib;
using PeterHan.PLib.Core;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Database.MonumentPartResource;
using static STRINGS.UI.FRONTEND;

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

        static void initList()
        {
            if (PRegistry.GetData<Dictionary<string, string>>(CompatibilityDataKey) != null )
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
            Dictionary<string,string> current = PRegistry.GetData<Dictionary<string, string>>(CompatibilityDataKey);
            if (current == null)
            {
                current = new Dictionary<string, string>();
            }

            if (conflictingModName.Count() > 40)
            {
                conflictingModName = conflictingModName.Remove(40);
                conflictingModName += "...";
            }
            if (!current.ContainsKey(modName))
                current.Add(modName, "");

            current[modName] += "\n• " + conflictingModName;

            PRegistry.PutData(CompatibilityDataKey, current);

        }
        public static void DumpIncompatibilityMessage(MainMenu parent)
        {
            Dictionary<string, string> current = PRegistry.GetData<Dictionary<string, string>>(CompatibilityDataKey);
            if (current == null || current.Count==0)
                return;



            foreach (var item in current)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine($"{item.Key} has declared the following mods as conflicting:");
                message.AppendLine(item.Value);

                KMod.Manager.Dialog(parent.gameObject, "Conflicting Mods found!", message.ToString(),
                            UI.CONFIRMDIALOG.OK);
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
