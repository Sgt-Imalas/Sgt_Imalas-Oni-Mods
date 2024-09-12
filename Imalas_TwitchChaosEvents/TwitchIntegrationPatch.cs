using HarmonyLib;

namespace Imalas_TwitchChaosEvents
{
	internal class TwitchIntegrationPatch
	{
		[HarmonyPatch(typeof(Db), "Initialize")]
		public static class Db_Initialize_Patch
		{
			public static void Postfix()
			{
				Util_TwitchIntegrationLib.EventRegistration.InitializeTwitchEventsInNameSpace("Imalas_TwitchChaosEvents.Events");
			}
		}
	}
}
