using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	class Reactor_Patches
	{     
		/// <summary>		
		/// Fix the reactor meter by removing that obsolete frame scale hack thing from an earlier reactor implementation
		/// /// </summary>
		/// 
		[HarmonyPatch(typeof(Reactor), nameof(Reactor.OnSpawn))]
		public static class Reactor_OnSpawn_Patch
		{
			public static void Prefix()
			{
				Reactor.meterFrameScaleHack = 1;
			}
		}
	}
}
