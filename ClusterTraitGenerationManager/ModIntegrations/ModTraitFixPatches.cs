using HarmonyLib;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterTraitGenerationManager.ModIntegrations
{
	internal class ModTraitFixPatches 
	{ 

		[HarmonyPatch(typeof(SettingsCache))]
		[HarmonyPatch(nameof(SettingsCache.LoadWorldTraits))]
		public static class TraitInitPostfix_ExclusionFix
		{
			public static void Postfix()
			{
				string coreKey = string.Empty;
				string cryoVolcano = string.Empty;


				foreach (var trait in SettingsCache.GetCachedWorldTraitNames())
				{
					if (trait.Contains("IronCore"))
						coreKey = trait;

					if (trait.Contains(SpritePatch.missingTexture_CryoVolcanoes))
						cryoVolcano = trait;
				}

				var IronCoreTrait = SettingsCache.GetCachedWorldTrait(coreKey, false);
				if (IronCoreTrait != null)
				{
					//adjust color to have better ui visibility
					IronCoreTrait.colorHex = "B7410E"; /// BA5C3F  or B7410E


					//adding missing exclusiveWithTags to iron core to make it properly exclusive with lush core
					if (IronCoreTrait.exclusiveWithTags == null)
						IronCoreTrait.exclusiveWithTags = new List<string>();
					if (!IronCoreTrait.exclusiveWithTags.Contains("CoreTrait")) 
						IronCoreTrait.exclusiveWithTags.Add("CoreTrait");
				}

				//adjust color to have better ui visibility
				var cryoVolcanoTrait = SettingsCache.GetCachedWorldTrait(cryoVolcano, false);
				if (cryoVolcanoTrait != null)
				{
					///Light blue
					cryoVolcanoTrait.colorHex = "91D8F0";
				}
			}
		}
	}
}
