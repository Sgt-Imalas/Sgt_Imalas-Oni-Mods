using HarmonyLib;
using OniRetroEdition.FX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
    internal class HoverTooltipPatch
    {

        [HarmonyPatch(typeof(DigToolHoverTextCard), nameof(DigToolHoverTextCard.UpdateHoverElements))]
        public class DigToolHoverTextCard_UpdateHoverElements_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                // find injection point
                var CIs = codes.Where(ci => ci.opcode == OpCodes.Ldstr && ci.operand is string s && s == "dash").ToList();

                if(CIs.Count >0)
                {
                    CIs[0].operand = SpritePatch.DigHex;
                }
                if(CIs.Count > 1)
                {
                    CIs[1].operand = SpritePatch.DigMass;
                }
                if(CIs.Count > 2)
                {
                    CIs[2].operand = SpritePatch.DigHardness;
                }

                return codes;
            }
        }

        [HarmonyPatch(typeof(MopToolHoverTextCard), nameof(MopToolHoverTextCard.UpdateHoverElements))]
        public class MopToolHoverTextCard_UpdateHoverElements_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                // find injection point
                var CIs = codes.Where(ci => ci.opcode == OpCodes.Ldstr && ci.operand is string s && s == "dash").ToList();

                if (CIs.Count > 0)
                {
                    CIs[0].operand = SpritePatch.DigHex;
                }
                if (CIs.Count > 1)
                {
                    CIs[1].operand = SpritePatch.DigMass;
                }
                return codes;
            }
        }

        [HarmonyPatch(typeof(HoverTextScreen), nameof(HoverTextScreen.GetSprite))]
        public class HoverTextScreen_GetSprite_Patch
        {
            public static bool Prefix(string byName, HoverTextScreen __instance, ref Sprite __result)
            {
                if(__instance.HoverIcons.Any(icon => icon.name == byName))
                {
                    return true;
                }
                __result = Assets.GetSprite(byName);
                return false;
            }
        }
    }
}
