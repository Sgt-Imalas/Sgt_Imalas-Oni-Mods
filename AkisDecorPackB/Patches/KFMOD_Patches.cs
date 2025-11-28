using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Patches
{
	internal class KFMOD_Patches
	{
		public static void DoMuted(System.Action a)
		{
			SkipSounds = true;
			a();
			SkipSounds = false;
		}

		public static bool SkipSounds { get; set; } = false;

        [HarmonyPatch(typeof(KFMOD), nameof(KFMOD.PlayUISound),[typeof(string)])]
        public class KFMOD_PlayUISound_Patch
        {
            public static bool Prefix() => !SkipSounds;

		}
	}
}
