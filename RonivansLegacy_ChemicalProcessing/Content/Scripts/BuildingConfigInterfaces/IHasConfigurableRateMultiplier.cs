using RonivansLegacy_ChemicalProcessing.Content.ModDb.BuildingConfigurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.BuildingConfigInterfaces
{
	internal interface IHasConfigurableRateMultiplier
	{
		float GetDefaultMultiplier() => 1;
		void SetMultiplier(float multiplier);

		Func<BuildingConfigurationEntry, string> GetCurrentRateDescription();
		string GetRateLabel();
	}
}
