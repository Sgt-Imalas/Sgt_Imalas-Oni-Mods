using DupeModelAccessPermissions.Content.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static UnityEngine.GraphicsBuffer;

namespace DupeModelAccessPermissions.Patches
{
	internal class AccessControlSideScreen_Patches
	{
		public static AccessControlSideScreenDoor defaultsRowBionic = null;

		//Create default row for bionics

		[HarmonyPatch(typeof(AccessControlSideScreen), nameof(AccessControlSideScreen.Refresh))]
		public class AccessControlSideScreen_Refresh_Patch
		{
			public static void Postfix(AccessControlSideScreen __instance)
			{
				InitRow(__instance);

				Rotatable component = __instance.target.GetComponent<Rotatable>();
				bool rotated = component != null && component.IsRotated;
				defaultsRowBionic.SetRotated(rotated);
				if (!__instance.target.TryGetComponent<AccessControl_Extension>(out var extension))
					return;
				defaultsRowBionic.SetContent(extension.DefaultPermissionBionics, (_, permissions) => OnDefaultPermissionBionicsChanged(__instance, extension, permissions));
			}
			static void InitRow(AccessControlSideScreen __instance)
			{
				if (defaultsRowBionic != null)
					return;
				defaultsRowBionic = Util.KInstantiateUI<AccessControlSideScreenDoor>(__instance.defaultsRow.gameObject, __instance.defaultsRow.transform.parent.gameObject, force_active: true);
				defaultsRowBionic.name = "DefaultsRowBionic";
				var transform = defaultsRowBionic.transform;
				transform.Find("LineLabel")?.GetComponent<LocText>().SetText(global::STRINGS.DUPLICANTS.MODEL.BIONIC.NAME);

				transform.SetSiblingIndex(__instance.defaultsRow.transform.GetSiblingIndex() + 1);

				//setting the "default" label to "standard duplicant"
				__instance.defaultsRow.transform.Find("LineLabel")?.GetComponent<LocText>().SetText(global::STRINGS.DUPLICANTS.MODEL.STANDARD.NAME);

				SgtLogger.l("Bionic Row initialized");
			}
			public static void OnDefaultPermissionBionicsChanged(AccessControlSideScreen __instance, AccessControl_Extension target, AccessControl.Permission permission)
			{
				target.DefaultPermissionBionics = permission;
				__instance.Refresh(__instance.identityList, rebuild: false);
				foreach (MinionAssignablesProxy identity2 in __instance.identityList)
				{
					if (target.accessControl.IsDefaultPermission(identity2))
					{
						target.accessControl.ClearPermission(identity2);
					}
				}
			}
		}

		[HarmonyPatch(typeof(AccessControlSideScreen), nameof(AccessControlSideScreen.OnPermissionDefault))]
		public class AccessControlSideScreen_OnPermissionDefault_Patch
		{
			public static void Postfix(AccessControlSideScreen __instance, MinionAssignablesProxy identity, bool isDefault)
			{
				SgtLogger.l("IsDefault: " + isDefault);

				if (__instance.target == null || !__instance.target.TryGetComponent<AccessControl_Extension>(out var extension))
				{
					return;
				}

				if (!identity.GetTargetGameObject().TryGetComponent<MinionIdentity>(out var minionIdentity) || minionIdentity.model != GameTags.Minions.Models.Bionic)
				{
					SgtLogger.l("Minion is not a bionic: " + identity.GetProperName());
					return;
				}
				if (isDefault)
					__instance.target.ClearPermission(identity);
				else
					__instance.target.SetPermission(identity, extension.DefaultPermissionBionics);
				__instance.Refresh(__instance.identityList, rebuild: false);
			}
		}
	}
}
