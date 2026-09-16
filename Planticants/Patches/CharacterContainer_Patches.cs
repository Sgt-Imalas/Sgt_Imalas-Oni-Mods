using HarmonyLib;
using Planticants.Content.ModDb;

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

				CharacterContainer.portraitBGAnimsByModel[ModTags.PlantMinion] = CharacterContainer.portraitBGAnimsByModel[GameTags.Minions.Models.Bionic];
			}

        }
    }
}
