using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Science
{
    internal class DeepSpaceSciencePatches
    {
        /// <summary>
        /// Attach DeepSpaceManager to Savegame
        /// </summary>
        [HarmonyPatch(typeof(SaveGame))]
        [HarmonyPatch("OnPrefabInit")]
        public class SaveGame_OnPrefabInit_Patch
        {
            /// <summary>
            /// Attach a component to save/reload collapse/expanded state.
            /// </summary>
            /// <param name="__instance">"this"</param>
            public static void Postfix(SaveGame __instance)
            {
                __instance.gameObject.AddOrGet<DeepSpaceScienceManager>();
            }
        } 

        [HarmonyPatch(typeof(ArtifactAnalysisStationWorkable))]
        [HarmonyPatch("ConsumeCharm")]
        public class AddDeepSpaceResearchOnSpaceArtifactResearch
        {
            public static void Prefix(ArtifactAnalysisStationWorkable __instance)
            {
                GameObject artifactToBeDefrosted = __instance.storage.FindFirst(GameTags.CharmedArtifact);
                DeepSpaceScienceManager.Instance.ArtifactResearched(artifactToBeDefrosted.HasTag(GameTags.TerrestrialArtifact));
            }
        }


        [HarmonyPatch(typeof(Assets), "OnPrefabInit")]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Prefix(Assets __instance)
            {
                InjectionMethods.AddSpriteToAssets(__instance, "research_type_deep_space_icon");
                InjectionMethods.AddSpriteToAssets(__instance, "research_type_deep_space_icon_unlock");
            }
        }

        [HarmonyPatch(typeof(ResearchTypes))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { })]
        public class ResearchTypes_Constructor_Patch
        {
            public static void Postfix(ResearchTypes __instance)
            {

                __instance.Types.Add(new ResearchType(
                    ModAssets.DeepSpaceScienceID,
                    STRINGS.DEEPSPACERESEARCH.NAME,
                    STRINGS.DEEPSPACERESEARCH.NAME,
                    Assets.GetSprite((HashedString)"research_type_deep_space_icon"),
                    new Color32(100, 100, 100, byte.MaxValue),
                    null,
                    2400f,
                    (HashedString)"research_center_kanim",
                    new string[] {},
                    STRINGS.DEEPSPACERESEARCH.RECIPEDESC
                ));
            }
        }
    }
}
