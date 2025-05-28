using HarmonyLib;
using System;
using UnityEngine;

namespace CritterTraitsReborn.Patches
{
	[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToBasicCreature), [
	  typeof(bool),
	  typeof(GameObject),
	  typeof(FactionManager.FactionID),
	  typeof(string) ,
	  typeof(string) ,
	  typeof(NavType) ,
	  typeof(int) ,
	  typeof(float ),
	  typeof(string) ,
	  typeof(int),
	  typeof(bool ) ,
	  typeof(bool) ,
	  typeof(float ),
	  typeof(float ),
	  typeof(float ),
	  typeof(float )
])]
	class EntityTemplates_ExtendEntityToBasicCreature
	{
		static void Postfix(ref GameObject __result)
		{
			__result.AddOrGet<Components.CritterTraits>();
		}
	}
}
