using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
    internal class NeedTraitPatches
    {
        //from Akis Bio Inks; https://github.com/aki-art/ONI-Mods/blob/master/PrintingPodRecharge/Patches/CharacterSelectionControllerPatch.cs
        [HarmonyPatch(typeof(CharacterContainer), "SetInfoText")]
        public class CharacterContainer_SetInfoText_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                var m_SetSimpleTooltip = AccessTools.Method(typeof(ToolTip), "SetSimpleTooltip");
                var index = codes.FindIndex(c => c.Calls(m_SetSimpleTooltip));

                if (index == -1)
                    return codes;

                var m_SetColorForTrait = AccessTools.Method(typeof(CharacterContainer_SetInfoText_Patch), "SetColorForTrait", new[] { typeof(LocText), typeof(Trait) });

                codes.InsertRange(index + 1, new[]
                {
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call, m_SetColorForTrait)
                });

                return codes;
            }

            private static void SetColorForTrait(LocText locTest, Trait trait)
            {
                if (TUNING.DUPLICANTSTATS.NEEDTRAITS.Any(traitVal => traitVal.id == trait.Id))
                {
                    locTest.color = Util.ColorFromHex("ffdb6e"); 
                }
            }
        }
        [HarmonyPatch(typeof(MinionStartingStats), "GenerateTraits")]
        public class MinionStartingStats_GenerateTraits_Patch
        {
            static HashSet<string> needTraits;

            private static System.Random random = new System.Random();
            public static void Postfix(MinionStartingStats __instance)
            {
                if(needTraits == null)
                {
                    needTraits = new HashSet<string>(TUNING.DUPLICANTSTATS.NEEDTRAITS.Select(item => item.id)); 
                }


                Trait needTrait = Db.Get().traits.TryGet(GetRandomNeedTrait().id);

                //skip if need trait already added (ie. bio inks)
                if (__instance.Traits.Any(trait => trait != null && needTraits.Contains(trait.Id)))
                    return;

                if(__instance.Traits.Count == 0)
                {
                    __instance.Traits.Add(needTrait);
                    return;

                } 

                for(int i = 0; i<__instance.Traits.Count; i++)
                {
                    var trait = __instance.Traits[i];
                    if (!trait.PositiveTrait)
                    {
                        __instance.Traits.Insert(i, needTrait);
                        return;
                    }
                }
                __instance.Traits.Add(needTrait);

            }
            public static DUPLICANTSTATS.TraitVal GetRandomNeedTrait()
            {
                return TUNING.DUPLICANTSTATS.NEEDTRAITS[random.Next(0, TUNING.DUPLICANTSTATS.NEEDTRAITS.Count)];
            }

        }
    }
}
