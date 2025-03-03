using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal class CarbonField_AnimationFix
	{
		public static void ExecutePatch()
		{
			System.Reflection.MethodInfo patched = AccessTools.Method(typeof(HarvestablePOIConfig), "CreatePrefabs");
			System.Reflection.MethodInfo postfix = AccessTools.Method(typeof(CarbonField_AnimationFix), "Postfix");
			Mod.harmonyInstance.Patch(patched, null, new HarmonyMethod(postfix));
		}
		public static void Postfix(ref List<GameObject> __result)
		{
			foreach (var obj in __result)
			{
				//SgtLogger.l(obj.ToString(),"PATCHSS");
				if (obj.TryGetComponent<HarvestablePOIClusterGridEntity>(out var poi))
				{
					if (poi.PrefabID().ToString().Contains(HarvestablePOIConfig.CarbonAsteroidField))
					{
						poi.m_Anim = "carbon_asteroid_field";
						SgtLogger.l("Fixed Carbon POI sprite");
						break;
					}
				}
			}
		}
	}
}
