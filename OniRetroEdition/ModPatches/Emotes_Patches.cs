using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OniRetroEdition.ModPatches
{
	public class Emotes_Patches
	{
		public static Emote High_Noise_React = null;

		[HarmonyPatch(typeof(Emotes.MinionEmotes), nameof(Emotes.MinionEmotes.InitializePhysicalStatus))]
		public class Emotes_InitializePhysicalStatus_Patch
		{
			public static void Postfix(Emotes.MinionEmotes __instance)
			{
				High_Noise_React = new Emote(__instance, "High Noise React", new EmoteStep[1]
				{
					new EmoteStep()
					{
						anim = (HashedString) "loudnoise"
					}
				}, "anim_loud_noise_kanim");
			}
		}
	}
}
