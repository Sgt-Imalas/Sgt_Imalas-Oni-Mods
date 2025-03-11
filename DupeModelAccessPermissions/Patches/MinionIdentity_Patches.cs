using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace DupeModelAccessPermissions.Patches
{
	internal class MinionIdentity_Patches
	{

        [HarmonyPatch(typeof(MinionIdentity), nameof(MinionIdentity.ValidateProxy))]
        public class MinionIdentity_ValidateProxy_Patch
		{
            public static void Postfix(MinionIdentity __instance)
            {
                if(__instance.model == GameTags.Minions.Models.Bionic)
                {
                    //Grid permissions use the instance id of the assignable proxy, not the minion instance id itself
                    int idx = __instance.assignableProxy.Get().GetComponent<KPrefabID>().InstanceID;

                    SgtLogger.l($"Registering bionic minion for door access: {__instance.GetProperName()} with id : {idx}");
					Mod.BionicMinionInstanceIds.Add(idx);
				}
            }
        }
	}
}
