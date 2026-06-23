using AquaticMinnowMinion.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_StatusItems
	{
		public static StatusItem BreathingInAquatic;

		static string o2 = null;
		public static void InitStatusitems(Db db)
		{
			var bsi = Db.Get().BuildingStatusItems;

			BreathingInAquatic = bsi.CreateStatusItem("BreathingInAquatic", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, status_overlays: 130);
			BreathingInAquatic.resolveStringCallback = (Func<string, object, string>)((str, data) =>
			{
				OxygenBreather oxygenBreather = (OxygenBreather)data;

				float num = oxygenBreather.O2Accumulator == HandleVector<int>.InvalidHandle ? 0.0f : Game.Instance.accumulators.GetAverageRate(oxygenBreather.O2Accumulator);

				if (o2 == null)
					o2 = ElementLoader.FindElementByHash(SimHashes.Oxygen).name;

				string consumedElement = o2;
				if(oxygenBreather.GetCurrentGasProvider() is GasOrWaterBreatherFromWorldProvider aquaConsumer)
				{
					consumedElement = ElementLoader.FindElementByHash(aquaConsumer.LastConsumedElement).name;
				}


				return str.Replace("{Element}",consumedElement).Replace("{ConsumptionRate}", GameUtil.GetFormattedMass(-num, GameUtil.TimeSlice.PerSecond));
			});
		}
	}
}
