using Dupery;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace DuperyFixed.Source.Patch
{
	internal class CharacterContainer_Patches
	{
        /// <summary>
        /// Force custom outfits to be the "none selected" outfit 
        /// </summary>
        [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.RefreshOutfitSelector))]
        public class CharacterContainer_RefreshOutfitSelector_Patch
        {
            public static void Prefix(CharacterContainer __instance)
            {
                if (!ActivelyGenerating.Contains(__instance))
                    return;

				string customDuperyBody = DuperyPatches.PersonalityManager.FindOwnedAccessory(__instance.stats.personality.nameStringKey, Db.Get().AccessorySlots.Body.Id);
                if (customDuperyBody == null)
                    return;

				Option<ClothingOutfitTarget> selectedOutfit = ClothingOutfitTarget.TryFromTemplateId(__instance.stats.personality.GetSelectedTemplateOutfitId(ClothingOutfitUtility.OutfitType.Clothing));
                if (selectedOutfit.IsNone())
                    __instance.outfitSelectorIndex = -1;
			}
        }

        static HashSet<CharacterContainer> ActivelyGenerating = [];
        [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.GenerateCharacter))]
        public class CharacterContainer_GenerateCharacter_Patch
        {
            public static void Prefix(CharacterContainer __instance)
            {
                ActivelyGenerating.Add(__instance);
			}
			public static void Postfix(CharacterContainer __instance)
			{
				ActivelyGenerating.Remove(__instance);
			}
		}
	}
}
