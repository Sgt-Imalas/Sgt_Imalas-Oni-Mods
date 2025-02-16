using BioluminescentDupes.Content.Scripts;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UtilLibs;
using static BioluminescentDupes.STRINGS.DUPLICANTS;

namespace BioluminescentDupes.Patches
{
	internal class LocalizationPatch
	{
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
				RegisterTrait();
			}

			static void RegisterTrait()
			{
				TUNING.TRAITS.TRAIT_CREATORS.Add(TraitUtil.CreateComponentTrait<BD_Bioluminescence>(BD_Bioluminescence.ID, STRINGS.DUPLICANTS.TRAITS.BD_BIOLUMINESCENCE.NAME, STRINGS.DUPLICANTS.TRAITS.BD_BIOLUMINESCENCE.DESC, true));
				if (!DUPLICANTSTATS.GOODTRAITS.Contains(BD_Bioluminescence.GetTrait()))
				{
					DUPLICANTSTATS.GOODTRAITS.Add(BD_Bioluminescence.GetTrait());
					if (DUPLICANTSTATS.GOODTRAITS.Any(traitval => traitval.id == nameof(GlowStick)))
					{
						var glowstick = DUPLICANTSTATS.GOODTRAITS.First(traitval => traitval.id == nameof(GlowStick));
						if (glowstick.mutuallyExclusiveTraits == null)
						{
							glowstick.mutuallyExclusiveTraits = [BD_Bioluminescence.ID];
						}
						else
						{
							glowstick.mutuallyExclusiveTraits.Add(BD_Bioluminescence.ID);
						}
					}
				}
			}
		}
	}
}
