using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
	internal class Personality_Patches
	{

		[HarmonyPatch(typeof(Personality), MethodType.Constructor,
			[typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(string), typeof(bool), typeof(string), typeof(Tag), typeof(int)])]
		public class Personality_Constructor_Patch
		{

			public static void Postfix(Personality __instance, string name)
			{
				SgtLogger.l("Initializing Minion: " + name);
				switch (name)
				{
					case "Stinky":
						__instance.congenitaltrait = "Stinky";
						break;
					case "Ellie":
						__instance.congenitaltrait = "Ellie";
						break;
					case "Joshua":
						__instance.congenitaltrait = "Joshua";
						break;
					case "Liam":
						__instance.congenitaltrait = "Liam";
						break;
					default: break;
				}


				if (__instance.model != GameTags.Minions.Models.Standard || !Config.Instance.oldDupeSuits)
					return;
				//Red shirt
				__instance.body = 1;
			}
		}
	}
}
