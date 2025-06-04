using HarmonyLib;
using Planticants.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planticants.Patches
{
    class CharacterContainer_Patches
    {

        [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.OnSpawn))]
        public class CharacterContainer_OnSpawn_Patch
        {
            public static void Prefix(CharacterContainer __instance)
            {
                if(!__instance.allMinionModels.Contains(ModTags.PlantMinion))
                    __instance.allMinionModels.Add(ModTags.PlantMinion);

				CharacterContainer.portraitBGAnims[ModTags.PlantMinion] = "updated_crewSelect_bionic_backdrop_kanim";
			}

        }
    }
}
