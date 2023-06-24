using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static NeuralVaccilatorExpanded.ModAssets;
using static ResearchTypes;

namespace NeuralVaccilatorExpanded
{
    internal class Patches
    {
        public static readonly string
            SuperBrains = "NVE_SuperBrains",
            ExpertMechanic = "NVE_ExpertMechanic",
            GeniusShare = "NVE_SharingGenius";


        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public static class RegisterNewTraits
        {
            public static void Postfix(Db __instance)
            {
                TraitUtil.CreateAttributeEffectTrait(ExpertMechanic, (string)STRINGS.DUPLICANTS.TRAITS.NVE_EXPERTMECHANIC.NAME, (string)STRINGS.DUPLICANTS.TRAITS.NVE_EXPERTMECHANIC.DESC, __instance.Attributes.Machinery.Id, 10f,true).Invoke();
                TraitUtil.CreateAttributeEffectTrait(SuperBrains, (string)STRINGS.DUPLICANTS.TRAITS.NVE_SUPERBRAINS.NAME, (string)STRINGS.DUPLICANTS.TRAITS.NVE_SUPERBRAINS.DESC, __instance.Attributes.Learning.Id, 10f, true).Invoke();
                TraitUtil.CreateComponentTrait<NVE_SharingGenius>(GeniusShare, (string)STRINGS.DUPLICANTS.TRAITS.NVE_SHARINGGENIUS.NAME, (string)STRINGS.DUPLICANTS.TRAITS.NVE_SHARINGGENIUS.DESC, true).Invoke();

                var AdditionalTraits = new List<DUPLICANTSTATS.TraitVal>
                {
                    new DUPLICANTSTATS.TraitVal
                    {
                        id = SuperBrains,
                        dlcId = ""
                    },
                    new DUPLICANTSTATS.TraitVal
                    {
                        id = ExpertMechanic,
                        dlcId = ""
                    },
                    new DUPLICANTSTATS.TraitVal
                    {
                        id = GeniusShare,
                        dlcId = ""
                    }
                };


                DUPLICANTSTATS.GENESHUFFLERTRAITS.AddRange(AdditionalTraits);

                SgtLogger.l("registered additional traits to list");

            }
        }

        [HarmonyPatch(typeof(DUPLICANTSTATS), MethodType.Constructor)]
        //[HarmonyPatch(new Type[] { typeof(int), typeof(List<string>) })]
        public static class RegisterNewTraitsToGeneshufflerList
        {

            public static void Prefix()
            {
                //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
            }
        }

        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }
    }
}
