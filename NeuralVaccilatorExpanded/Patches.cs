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
            GeniusShare = "NVE_SharingGenius",
            DaftPunk = "NVE_DaftPunk";


        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public static class RegisterNewTraits
        {
            public static void CreateDaftPunkTrait(Db db)
            {
                string traitName = (string)STRINGS.DUPLICANTS.TRAITS.NVE_DAFTPUNK.NAME;
                Trait trait = db.CreateTrait(DaftPunk,traitName, (string)STRINGS.DUPLICANTS.TRAITS.NVE_DAFTPUNK.DESC, null, should_save: true, null, true, is_valid_starter_trait: false);
                trait.Add(new AttributeModifier(db.Attributes.Learning.Id, -3, traitName));

                trait.Add(new AttributeModifier(db.Attributes.GermResistance.Id, 2f, traitName));
                trait.Add(new AttributeModifier(db.Attributes.RadiationResistance.Id, 0.2f, traitName));

                trait.Add(new AttributeModifier(db.Attributes.Machinery.Id, 5, traitName));
                trait.Add(new AttributeModifier(db.Attributes.Athletics.Id, 5, traitName));
                trait.Add(new AttributeModifier(db.Attributes.CarryAmount.Id, 500, traitName));

                //trait.Add(new AttributeModifier(db.Amounts.HitPoints.maxAttribute.Id, 150, traitName));

            }
            public static void Postfix(Db __instance)
            {
                TraitUtil.CreateAttributeEffectTrait(ExpertMechanic, (string)STRINGS.DUPLICANTS.TRAITS.NVE_EXPERTMECHANIC.NAME, (string)STRINGS.DUPLICANTS.TRAITS.NVE_EXPERTMECHANIC.DESC, __instance.Attributes.Machinery.Id, 10f,true).Invoke();
                TraitUtil.CreateAttributeEffectTrait(SuperBrains, (string)STRINGS.DUPLICANTS.TRAITS.NVE_SUPERBRAINS.NAME, (string)STRINGS.DUPLICANTS.TRAITS.NVE_SUPERBRAINS.DESC, __instance.Attributes.Learning.Id, 10f, true).Invoke();
                TraitUtil.CreateComponentTrait<NVE_SharingGenius>(GeniusShare, (string)STRINGS.DUPLICANTS.TRAITS.NVE_SHARINGGENIUS.NAME, (string)STRINGS.DUPLICANTS.TRAITS.NVE_SHARINGGENIUS.DESC, true).Invoke();
                CreateDaftPunkTrait(__instance);

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
                    },
                    new DUPLICANTSTATS.TraitVal
                    {
                        id = DaftPunk,
                        dlcId = ""
                    }
                };


                DUPLICANTSTATS.GENESHUFFLERTRAITS.AddRange(AdditionalTraits);

                SgtLogger.l("registered additional traits to list");

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
