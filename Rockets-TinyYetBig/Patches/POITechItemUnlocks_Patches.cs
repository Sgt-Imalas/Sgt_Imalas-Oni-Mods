using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches
{
    class POITechItemUnlocks_Patches
    {

        [HarmonyPatch(typeof(POITechItemUnlocks), nameof(POITechItemUnlocks.GetMessageBody))]
        public class POITechItemUnlocks_GetMessageBody_Patch
        {
            public static void Postfix(POITechItemUnlocks.Instance smi, ref string __result)
            {
                if(smi.def.POITechUnlockIDs.Any() &&
				   smi.def.POITechUnlockIDs.Contains(ModAssets.DeepSpaceScienceID))
				{
                    __result = STRINGS.UI.RTB_RESEACH_UNLOCK.TEXT; //Fat TODO!

                    if(ModAssets.Techs.SpaceStationTech != null)
                        Research.Instance.Get(ModAssets.Techs.SpaceStationTech).Purchased();
				}
			}
        }
    }
}
