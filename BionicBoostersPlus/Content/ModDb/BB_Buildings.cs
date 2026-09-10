using BionicBoostersPlus.Content.Defs.Buildings;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace BionicBoostersPlus.Content.ModDb
{
	internal class BB_Buildings
	{
		public static void Register()
		{
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.BruteForceRefinement, BoosterRecyclerConfig.ID);
			if (DlcManager.IsExpansion1Active())
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.RadiationTechnologies.RadiationRefinement, Mk3BoosterMakerConfig.ID);
			else
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.SoundAmplifiers, Mk3BoosterMakerConfig.ID);
		}
	}
}
