using ElementUtilNamespace;
using System.Collections.Generic;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Elements
{
	public class ModElements
	{
		public static ElementInfo
			InverseIce,
			InverseWater,
			InverseWaterFlakingCrashPrevention,
			InverseSteam,
			VoidLiquid,
			Creeper,
			CreeperGas,
			LiquidPoop;

		public static void RegisterSubstances(List<Substance> list)
		{
			//var gem = list.Find(e => e.elementID == SimHashes.Diamond).material;
			var water = list.Find(e => e.elementID == SimHashes.Water);
			var ice = list.Find(e => e.elementID == SimHashes.Ice);
			var steam = list.Find(e => e.elementID == SimHashes.Steam);
			Color.RGBToHSV(steam.colour, out var steamH, out var steamS, out var steamV);
			Color.RGBToHSV(ice.colour, out var iceH, out var iceS, out var iceV);
			Color.RGBToHSV(water.colour, out var waterH, out var waterS, out var waterV);

			steamH = steamH + 0.5f % 1f;
			waterH = waterH + 0.5f % 1f;
			iceH = iceH + 0.5f % 1f;

			var voidColor = new Color(0,0,0,4f);


			InverseIce = ElementInfo.Solid("ITCE_Inverse_Ice", Color.HSVToRGB(iceH, iceS, iceV));
			InverseWater = ElementInfo.Liquid("ITCE_Inverse_Water", Color.HSVToRGB(waterH, waterS, waterV));
			InverseWaterFlakingCrashPrevention = ElementInfo.Liquid("ITCE_Inverse_Water_Placeholder", Color.HSVToRGB(waterH, waterS, waterV));

			InverseSteam = ElementInfo.Gas("ITCE_Inverse_Steam", Color.HSVToRGB(steamH, steamS, steamV));
			CreeperGas = ElementInfo.Gas("ITCE_CreepyLiquidGas", new Color(121f / 255f, 74f / 255f, 230f / 255f));
			Creeper = ElementInfo.Liquid("ITCE_CreepyLiquid", new Color(100f / 255f, 62f / 255f, 191f / 255f));
			LiquidPoop = new ElementInfo("ITCE_Liquid_Poop", "itce_liquid_poop_kanim", Element.State.Liquid, new Color(128f / 255f, 61f / 255f, 43f / 255f));
			VoidLiquid = ElementInfo.Liquid("ITCE_VoidLiquid", voidColor);


			var newElements = new HashSet<Substance>()
			{
				InverseIce.CreateSubstance(),
				InverseWater.CreateSubstance(),
				InverseWaterFlakingCrashPrevention.CreateSubstance(),
				InverseSteam.CreateSubstance(),
				Creeper.CreateSubstance(),
				CreeperGas.CreateSubstance(),
				LiquidPoop.CreateSubstance()
				,VoidLiquid.CreateSubstance()
			};
			list.AddRange(newElements);

			Rottable.AtmosphereModifier.Add((int)Creeper.SimHash, Rottable.RotAtmosphereQuality.Contaminating);
			Rottable.AtmosphereModifier.Add((int)LiquidPoop.SimHash, Rottable.RotAtmosphereQuality.Contaminating);
		}
	}
}
