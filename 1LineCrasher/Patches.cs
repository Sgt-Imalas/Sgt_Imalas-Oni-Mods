using HarmonyLib;
using System;
using System.Collections.Generic;

namespace _1LineCrasher
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(DateTime))]
        [HarmonyPatch(nameof(DateTime.Update))]
        public static class CommaToNumber
        {
            /// <summary>
            /// "CLEAN" this project once to generate publicizer files
            /// </summary>
            public static void Postfix(DateTime __instance)
            {
                __instance.text.text = (GameClock.Instance.GetTimeInCycles() +1).ToString("n3");
            }
        }

        [HarmonyPatch(typeof(CodexEntryGenerator))]
        [HarmonyPatch(nameof(CodexEntryGenerator.PopulateCategoryEntries))]
        [HarmonyPatch(new Type[] {typeof( List <CategoryEntry>), typeof(Comparison<CodexEntry>) })]
        public static class FixMoped
        {
            public static void Prefix(ref Comparison<CodexEntry> comparison)
            {
                comparison = new Comparison<CodexEntry>((entryA, entryB) => entryA.id.CompareTo(entryB.id));
            }
        }
        //[HarmonyPatch(typeof(Localization), nameof(Localization.OverloadStrings))]
        //[HarmonyPatch(new Type[] { typeof(Dictionary<string,string>) })]
        //public static class Localization_Initialize_Patch
        //{
        //    [HarmonyPriority(Priority.VeryLow)]
        //    public static void Prefix(ref Dictionary<string, string> translated_strings)
        //    {
        //        var keys = new List<string>(translated_strings.Keys);
        //        foreach (var key in keys)
        //        {
        //            translated_strings[key] = "Moped";
        //        }
        //    }
        //}
        //[HarmonyPatch(typeof(RetiredColonyInfoScreen), nameof(RetiredColonyInfoScreen.DisplayWorlds))]
        ////[HarmonyPatch(new Type[] { typeof(Dictionary<string, string>) })]
        //public static class MopedPatch2
        //{
        //    [HarmonyPriority(Priority.VeryLow)]
        //    public static void Prefix(ref Dictionary<string, string> translated_strings)
        //    {
        //        var keys = new List<string>(translated_strings.Keys);
        //        foreach (var key in keys)
        //        {
        //            translated_strings[key] = "Moped";
        //        }
        //    }
        //}
    }
}
