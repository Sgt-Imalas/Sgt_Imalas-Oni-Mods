using Dupery;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuperyFixed.Source.Patch
{
	internal class SpeechMonitor_Patch
    {
		/// <summary>
		/// Register custom mouth for personalities that have RoboMouthConversation marked as true.
		/// </summary>
		[HarmonyPatch(typeof(SpeechMonitor.Instance), nameof(SpeechMonitor.Instance.SetMouthId))]
        public class SpeechMonitor_SetMouthId_Patch
		{
            public static void Postfix(SpeechMonitor.Instance __instance)
            {
				if (PersonalityManager.UseRoboMouthConversation(__instance.smi.Get<MinionIdentity>()?.personalityResourceId))
				{
					__instance.smi.mouthId = "_006";
				}
			}
        }
	}
}
