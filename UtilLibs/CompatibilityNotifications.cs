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

        public static void CheckAndAddIncompatibles(string assemblyName, string modName, string reason="")
        {
            Debug.Log("checking if incompatible mod is installed: " + assemblyName);
            initList();
            if (AppDomain.CurrentDomain.GetAssemblies().ToList().Any(ass => ass.FullName.ToLowerInvariant().Contains(assemblyName.ToLowerInvariant())))
            {
                Debug.Log("incompatible mod found: " + assemblyName);
                AddIncompatibleToList(modName, reason);
            }
            else
                Debug.Log("mod not found: " + assemblyName);
        }

        static void initList()
        {
            if (PRegistry.GetData<List<Tuple<string, string>>>(CompatibilityDataKey) != null )
                return;

            var current = new List<Tuple<string, string>>();
            foreach(var ass in AppDomain.CurrentDomain.GetAssemblies().ToList())
            {
                SgtLogger.l(ass.FullName);
            }
            PRegistry.PutData(CompatibilityDataKey, current);


            CheckAndAddIncompatibles(".Mod.DebugConsole", "Debug Console");

        }


        static void AddIncompatibleToList(string modName, string reason ="")

        {
            List<Tuple<string,string>> current = PRegistry.GetData<List<Tuple<string, string>>>(CompatibilityDataKey);
            if (current == null)
            {
                current = new List<Tuple<string, string>>();
            }
            current.Add(new Tuple<string, string>(modName,reason));
            PRegistry.PutData(CompatibilityDataKey, current);

        }
        public static void DumpIncompatibilityMessage(MainMenu parent)
        {
            List<Tuple<string, string>> current = PRegistry.GetData<List<Tuple<string, string>>>(CompatibilityDataKey);
            if (current == null || current.Count==0)
                return;
            

            StringBuilder message = new StringBuilder();
            message.AppendLine("The following conflicting mods have been found, disable them to mitigate errors:");
            
            foreach(Tuple<string, string> item in current)
            {
                message.Append("• ");
                string modName = item.first;
                if (modName.Count() > 35)
                {
                    modName = modName.Remove(33);
                    modName += "...";
                }

                message.Append(modName);
                if(item.second!=string.Empty)
                {
                    message.Append(": ");
                    message.Append(item.second);
                }
                message.AppendLine();
            }

            Debug.Log(message.ToString());

            PRegistry.PutData(CompatibilityDataKey, null);

            KMod.Manager.Dialog(parent.gameObject, "Conflicting Mods found!", message.ToString(),
                        UI.CONFIRMDIALOG.OK);

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
