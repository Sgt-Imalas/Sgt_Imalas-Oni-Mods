using Dupery;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

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
				var personalityResourceId = __instance.smi.Get<MinionIdentity>().personalityResourceId;
				Personality personality = Db.Get().Personalities.Get(personalityResourceId);
				if (personality.speech_mouth > 0)
				{
					__instance.mouthId = $"_{personality.speech_mouth:000}";
				}


				if (PersonalityManager.UseCustomSpeechMonitorKanim(personalityResourceId, out string kanim))
				{
					var animFile = Assets.GetAnim(kanim);
					if (animFile == null)
					{
						SgtLogger.warning($"Could not find custom conversation kanim {kanim} for personality {personalityResourceId}.");
						return;
					}
					SgtLogger.l("setting custom conversation kanim: " + kanim + " for "+ personalityResourceId);
					__instance.mouth.AnimFiles = [animFile];
				}
			}
        }
	}
}
