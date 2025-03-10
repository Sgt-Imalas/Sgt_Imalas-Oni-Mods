using ElementUtilNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace AkisSnowThings.Content.Scripts.Elements
{
	internal class SnowModElements
	{
		public static ElementInfo
			EvergreenTreeSapFrozen = ElementInfo.Solid("EvergreenTreeSapFrozen", UIUtils.rgb(255, 215, 79)),
			EvergreenTreeSap = ElementInfo.Liquid("EvergreenTreeSap", UIUtils.rgb(255, 178, 74))
			;

		public static void RegisterSubstances(List<Substance> list)
		{
			var newElements = new HashSet<Substance>()
			{
				EvergreenTreeSapFrozen.CreateSubstance(),
				EvergreenTreeSap.CreateSubstance()				
			};

			list.AddRange(newElements);

			Rottable.AtmosphereModifier.Add((int)EvergreenTreeSap.SimHash, Rottable.RotAtmosphereQuality.Sterilizing);
		}
	}
}
