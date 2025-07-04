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
		/// Register custom mouth for personalities that have custom mouths defined.
		/// also swap anim files for the speech monitor if the personality has a custom conversation kanim defined.
		/// </summary>
		[HarmonyPatch(typeof(SpeechMonitor), nameof(SpeechMonitor.CreateMouth))]
        public class SpeechMonitor_SetMouthId_Patch
		{
            public static void Postfix(SpeechMonitor.Instance smi)
            {
				var personalityResourceId = smi.Get<MinionIdentity>().personalityResourceId;
				Personality personality = Db.Get().Personalities.Get(personalityResourceId);
				if (personality.speech_mouth > 0)
				{
					///this is buggy in vanilla, it assigns the mouth, not the speech_mouth
					smi.mouthId = $"_{personality.speech_mouth:000}";
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
					smi.mouth.AnimFiles = [animFile];
				}
			}
        }
	}
}
