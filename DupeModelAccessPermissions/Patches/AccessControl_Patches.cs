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

		[HarmonyPatch(typeof(AccessControl), nameof(AccessControl.GetSetPermission), [typeof(KPrefabID)])]
		public class AccessControl_GetSetPermission_Patch
		{
			public static void Postfix(AccessControl __instance, KPrefabID kpid, ref AccessControl.Permission __result)
			{
				//if not a bionic, abort
				if (__instance.savedPermissions == null || !kpid.TryGetComponent<MinionAssignablesProxy>(out var proxy) || proxy.GetTargetGameObject() == null)
				{
					//SgtLogger.l("no saved permissions or go null");
					return;
				}
				var minionGO = proxy.GetTargetGameObject();
				if (!minionGO.TryGetComponent<MinionIdentity>(out var identity))
				{
					//SgtLogger.l("cant find minionIdentity on "+ minionGO.GetProperName());
					return;
				}
				if(identity.model != GameTags.Minions.Models.Bionic)
				{
					//SgtLogger.l(identity.model+" is not a bionic");
					return;
				}

				//if the bionic has specific override permissions, abort
				if (kpid != null && __instance.savedPermissions.Any(permission => permission.Key.GetId() == kpid.InstanceID))
				{
					//SgtLogger.l("bionic has specific override permissions");
					return;
				}

				if (__instance.TryGetComponent<AccessControl_Extension>(out var control_Extension))
				{
					__result = control_Extension.DefaultPermissionBionics;
					//SgtLogger.l("Setting permission value to bionic default: "+__result);
				}
			}
		}
	}
}
