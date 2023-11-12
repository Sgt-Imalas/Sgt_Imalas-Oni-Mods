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
                __instance.text.text = (GameClock.Instance.GetTimeInCycles() + 1).ToString("n3");
            }
        }

        [HarmonyPatch(typeof(CodexEntryGenerator))]
        [HarmonyPatch(nameof(CodexEntryGenerator.PopulateCategoryEntries))]
        [HarmonyPatch(new Type[] { typeof(List<CategoryEntry>), typeof(Comparison<CodexEntry>) })]
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

        //[HarmonyPatch(typeof(RailGun.States))]
        //[HarmonyPatch(nameof(RailGun.States.InitializeStates))]
        //public static class Modify_RailgunCooldown
        //{
        //    public static bool Prepare() => false;
        //    static float NewCooldownTimer = 20f;
        //    public static void Postfix(RailGun.States __instance)
        //    {
        //        __instance.on.cooldown.pre.Enter((smi) => smi.sm.cooldownTimer.Set(NewCooldownTimer, smi));
        //    }
        //}

        /// <summary>
        /// custom meteor example code
        /// </summary>
        //[HarmonyPatch(typeof(Db), "Initialize")]
        //public static class Db_addSeason
        //{
        //    public static void Postfix(Db __instance)
        //    {
        //        __instance.GameplayEvents.Add(
        //            new MeteorShowerSeason(
        //                "AllShowersInOnceID",
        //                GameplaySeason.Type.World,
        //                "EXPANSION1_ID",
        //                20f,
        //                false,
        //                startActive: true,
        //                clusterTravelDuration: 6000f)
        //            .AddEvent(Db.Get().GameplayEvents.MeteorShowerDustEvent)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterCopperShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterGoldShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterIronShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterIceShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterBiologicalShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterBleachStoneShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterUraniumShower));
        //        ///obv. not all events
        //    }
        //}
    }
}
