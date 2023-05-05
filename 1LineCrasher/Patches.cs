using HarmonyLib;
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
    }
}
