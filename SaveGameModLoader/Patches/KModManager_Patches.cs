using HarmonyLib;
using System.Linq;

namespace SaveGameModLoader.Patches
{
	internal class KModManager_Patches
	{

        [HarmonyPatch(typeof(KMod.Manager), nameof(KMod.Manager.Report))]
        public class KMod_Manager_Report_Patch
        {
            public static void Prefix(KMod.Manager __instance)
            {
				if (__instance.events.Any())
				{
					return;
				}
				var events = __instance.events;
				for (int i = 0; i < events.Count; i++)
				{
					var @event = events[i];
					if (@event.event_type != KMod.EventType.VersionUpdate)
						continue;
					ModAssets.OnModUpdated(@event.mod);
				}
			}
        }
	}
}
