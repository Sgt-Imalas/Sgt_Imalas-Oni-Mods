using AkisDecorPackB.Content.ModDb;
using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Patches
{
	internal class ArtableStages_Patches
	{
		[HarmonyPatch(typeof(ArtableStages), MethodType.Constructor, typeof(ResourceSet))]
		public class ArtableStages_Ctor_Patch
		{
			public static void Postfix(ArtableStages __instance)
			{
				ModSkins.RegisterArtables(__instance);
			}
		}
	}
}
