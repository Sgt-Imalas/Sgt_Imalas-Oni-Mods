using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace SaveGameModLoader.Patches
{
    internal class UnpatchCrashing
    {
        internal static void SecureLog(Harmony harmony)
        {
            foreach (MethodBase original in Harmony.GetAllPatchedMethods().ToList())
            {
                bool num = original.HasMethodBody();
                var patchInfo2 = Harmony.GetPatchInfo(original);
                if (num)
                {
                    patchInfo2.Postfixes.DoIf(IDCheck, delegate (Patch patchInfo)
                    {
                        harmony.Unpatch(original, patchInfo.PatchMethod);
                    });
                    patchInfo2.Prefixes.DoIf(IDCheck, delegate (Patch patchInfo)
                    {
                        harmony.Unpatch(original, patchInfo.PatchMethod);
                    });
                }

                patchInfo2.Transpilers.DoIf(IDCheck, delegate (Patch patchInfo)
                {
                    harmony.Unpatch(original, patchInfo.PatchMethod);
                });
                if (num)
                {
                    patchInfo2.Finalizers.DoIf(IDCheck, delegate (Patch patchInfo)
                    {
                        harmony.Unpatch(original, patchInfo.PatchMethod);
                    });
                }
            }

            bool IDCheck(Patch patchInfo)
            {
                SgtLogger.l(patchInfo.owner);
                return patchInfo.owner.ToLowerInvariant().Contains("debugconsole") || patchInfo.owner.ToLowerInvariant() == ("modmanager");
            }
        }
    }
}
