using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static Cheese.STRINGS.DUPLICANTS.TRAITS;
using static HarmonyLib.Code;

namespace Cheese.Traits
{
    internal class BracktoseIntolerant
    {
        public static string ID = nameof(BracktoseIntolerant);

        public static DUPLICANTSTATS.TraitVal GetTrait()
        {
            return new DUPLICANTSTATS.TraitVal()
            {
                id = ID,
                statBonus = DUPLICANTSTATS.SMALL_STATPOINT_BONUS,
                rarity = DUPLICANTSTATS.RARITY_UNCOMMON,
                dlcId = DlcManager.VANILLA_ID
            };
        }

        public static bool HasAffectingTrait(GameObject duplicant)
        {
            if (duplicant == null || !duplicant.TryGetComponent<Klei.AI.Traits>(out var traits))
                return false;

            bool hasTrait = traits.HasTrait(ID);
            return hasTrait;
        }

        internal static void HandleDupeEffect(Worker worker)
        {
            if(worker.gameObject != null 
                && HasAffectingTrait(worker.gameObject)
                && worker.TryGetComponent<MinionModifiers> (out var dupeModifiers))
            {
                SicknessExposureInfo foodPoisoning = new SicknessExposureInfo(FoodSickness.ID, BRACKTOSEINTOLERANT.NAME);
                dupeModifiers.sicknesses.Infect(foodPoisoning);
            }
        }
    }
}
