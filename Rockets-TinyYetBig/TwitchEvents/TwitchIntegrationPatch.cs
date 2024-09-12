using HarmonyLib;

namespace Rockets_TinyYetBig.TwitchEvents
{
	public class TwitchIntegrationPatch
	{
		[HarmonyPatch(typeof(Db), "Initialize")]
		public static class Db_Initialize_Patch
		{
			public static void Postfix(Db __instance)
			{
				Util_TwitchIntegrationLib.EventRegistration.InitializeTwitchEventsInNameSpace("Rockets_TinyYetBig.TwitchEvents.Events");
			}
		}
	}
}
