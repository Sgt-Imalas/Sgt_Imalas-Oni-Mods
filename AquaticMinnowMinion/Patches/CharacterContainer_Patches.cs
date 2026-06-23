using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AquaticMinnowMinion.ModAssets;
using static CharacterContainer;

namespace AquaticMinnowMinion.Patches
{
    class CharacterContainer_Patches
    {

        [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.OnSpawn))]
        public class CharacterContainer_OnSpawn_Patch
        {
            public static void Prefix(CharacterContainer __instance)
            {
				if (!__instance.allMinionModels.Contains(ModTags.AquaticMinion))
                    __instance.allMinionModels.Add(ModTags.AquaticMinion);

                CharacterContainer.portraitBGAnimsByModel[ModTags.AquaticMinion] = new()
                {
                    animFileName = "crewselect_backdrop_swim_kanim",
                    hasPreAnim = true,
                    foregroundAnimFileName = "crewselect_backdrop_swim_fg_kanim",
                };
			}

        }
    }
}
