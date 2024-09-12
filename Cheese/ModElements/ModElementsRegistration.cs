using ElementUtilNamespace;
using System.Collections.Generic;
using UtilLibs;

namespace Cheese.ModElements
{
	public class ModElementRegistration
	{
		public static ElementInfo
			Cheese = ElementInfo.Solid("Cheese", UIUtils.rgb(225, 241, 99)),
			SaltyMilkFat = ElementInfo.Solid("SaltyMilkFat", UIUtils.rgb(123, 108, 56)),
			CheeseMolten = ElementInfo.Liquid("CheeseMolten", UIUtils.rgb(240, 180, 0))
			;

		public static void RegisterSubstances(List<Substance> list)
		{
			var newElements = new HashSet<Substance>()
			{
				Cheese.CreateSubstance(),
				SaltyMilkFat.CreateSubstance(),
				CheeseMolten.CreateSubstance(),
			};
			list.AddRange(newElements);

			Rottable.AtmosphereModifier.Add((int)CheeseMolten.SimHash, Rottable.RotAtmosphereQuality.Sterilizing);
		}
		public static ElementsAudio.ElementAudioConfig[] CreateAudioConfigs(ElementsAudio instance)
		{
			return new[]
			{
				SgtElementUtil.CopyElementAudioConfig(SimHashes.SlimeMold, Cheese),
			};
		}

	}
}
