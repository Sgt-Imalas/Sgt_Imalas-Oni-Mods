using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BathTub.Duck.DecorPackA_Moodlamp
{
	internal class AddDuckMoodLampPatch
	{
		[HarmonyPatch(typeof(Db), "Initialize")]
		public class Db_Initialize_Patch
		{
			public static void Postfix()
			{
				if (DecorPackA_ModAPI.TryInitialize(true))
				{
					DecorPackA_ModAPI.AddMoodLamp(
						"BT_RubberDuckie",
						STRINGS.MOODLAMPSKINS.RUBBERDUCKIE,
						"customizable",
						"moodlamp_duck_kanim",
						Color.white,
						KAnim.PlayMode.Paused,
						new HashSet<HashedString>() { "Tintable" });
				}
			}
		}
	}
}
