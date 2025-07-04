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
    class BlinkMonitor_Patches
    {
        [HarmonyPatch(typeof(BlinkMonitor), nameof(BlinkMonitor.CreateEyes))]
        public class BlinkMonitor_CreateEyes_Patch
        {
			/// <summary>
			/// Register custom mouth for personalities that have custom mouths defined.
			/// also swap anim files for the speech monitor if the personality has a custom conversation kanim defined.
			/// </summary>
			[HarmonyPatch(typeof(FaceGraph), nameof(FaceGraph.ApplyShape), [])]
			public class BlinkMonitor_BlinkMonitor_Patch
			{
				public static void Postfix(FaceGraph __instance)
				{
					var blinkMonitor = __instance.m_blinkMonitor;
					if (blinkMonitor == null)
						return;

					var personalityResource = __instance.GetComponent<MinionIdentity>();
					if (personalityResource == null)
						return;

					var personalityResourceId = personalityResource.personalityResourceId;


					if (PersonalityManager.UseCustomBlinkMonitorKanim(personalityResourceId, out string kanim))
					{
						var eyes = DuperyPatches.PersonalityManager.FindOwnedAccessory(personalityResource.nameStringKey, Db.Get().AccessorySlots.Eyes.Id);

						var animFile = Assets.GetAnim(kanim);
						if (animFile == null)
						{
							SgtLogger.warning($"Could not find custom blink kanim {kanim} for personality {personalityResourceId}.");
							return;
						}
						SgtLogger.l("setting custom blink kanim file: " + kanim + " for " + personalityResourceId+" with anim: "+eyes);
						blinkMonitor.eyes.AnimFiles = [animFile];
						blinkMonitor.eye_anim = eyes;
					}
				}
			}
		}
    }
}
