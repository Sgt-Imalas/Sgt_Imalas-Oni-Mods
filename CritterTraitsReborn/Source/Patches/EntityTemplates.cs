using HarmonyLib;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using static FactionManager;

namespace CritterTraitsReborn.Patches
{
	[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToBasicCreature),
		[
		typeof(bool),
		typeof(GameObject),
		typeof(string),
		typeof(string),
		typeof(string),
		typeof(FactionManager.FactionID),
		typeof(string),
		typeof(string),
		typeof(NavType),
		typeof(int),
		typeof(float),
		typeof(string),
		typeof(float),
		typeof(bool),
		typeof(bool),
		typeof(float),
		typeof(float),
		typeof(float),
		typeof(float)
		])]


	class EntityTemplates_ExtendEntityToBasicCreature
	{
		static void Postfix(ref GameObject __result)
		{
			__result.AddOrGet<Components.CritterTraits>();
		}
	}
}
