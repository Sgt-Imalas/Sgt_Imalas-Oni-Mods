using DupeModelAccessPermissions.Content.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static AccessControl;

namespace DupeModelAccessPermissions.Patches
{
	internal class AccessControl_Patches
	{
		[HarmonyPatch(typeof(AccessControl), nameof(AccessControl.OnPrefabInit))]
		public class AccessControl_OnPrefabInit_Patch
		{
			public static void Postfix(AccessControl __instance)
			{
				__instance.gameObject.AddOrGet<AccessControl_Extension>();
			}
		}
	}
}
