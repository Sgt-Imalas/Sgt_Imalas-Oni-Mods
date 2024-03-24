using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Cheese.Traits
{
    internal class TraitsPatches
    {
        [HarmonyPatch(typeof(ModifierSet))]
        [HarmonyPatch("LoadTraits")]
        public static class ModifierSet_LoadTraits_Patch
        {
            public static void Prefix()
            {
                TRAITS.TRAIT_CREATORS.Add(TraitUtil.CreateNamedTrait(BracktoseIntolerant.ID, (string)STRINGS.DUPLICANTS.TRAITS.BRACKTOSEINTOLERANT.NAME, (string)STRINGS.DUPLICANTS.TRAITS.BRACKTOSEINTOLERANT.DESC));
                TRAITS.TRAIT_CREATORS.Add(TraitUtil.CreateTrait(CheeseThrower.ID, (string)STRINGS.DUPLICANTS.TRAITS.CHEESETHROWER.NAME, (string)STRINGS.DUPLICANTS.TRAITS.CHEESETHROWER.DESC, new System.Action<GameObject>(OnAddCheeseThrower)));
            }

            static void OnAddCheeseThrower(GameObject go)
            {
                go.TryGetComponent<KMonoBehaviour>(out var kb);

                new CheeseThrower.Instance(kb).StartSM();
                new JoyBehaviourMonitor.Instance(kb, "anim_loco_stickers_kanim", (string)null, Db.Get().Expressions.Sticker).StartSM();
            }
        }

        [HarmonyPatch(typeof(MinionStartingStats))]
        [HarmonyPatch("GenerateTraits")]
        public static class MinionStartingStats_GenerateTraits_Patch
        {
            private static bool TraitsAdded = false;

            public static void Prefix()
            {
                if (!TraitsAdded)
                {
                    DUPLICANTSTATS.BADTRAITS.Add(BracktoseIntolerant.GetTrait());
                    DUPLICANTSTATS.JOYTRAITS.Add(CheeseThrower.GetTrait());
                    TraitsAdded = true;
                }
            }
        }
    }
    [HarmonyPatch(typeof(DiningTableConfig))]
    [HarmonyPatch("DoPostConfigureComplete")]
    public static class DiningTableConfig_DoPostConfigureComplete_Patch
    {
        public static void Postfix(GameObject go)
        {
            go.AddOrGet<CheeseTable>();
        }
    }
}
