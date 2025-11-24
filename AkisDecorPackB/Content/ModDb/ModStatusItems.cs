using AkisDecorPackB.Content.Scripts;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Content.ModDb
{
	internal class ModStatusItems
	{
		public static StatusItem awaitingFuel;
		public static StatusItem fountainDriedOut;
		public static StatusItem fountainFlowing;

		public static void Register(BuildingStatusItems statusItems)
		{
			fountainDriedOut = new StatusItem(
				"DecorPackB_FountainDriedOut",
				"BUILDING",
				"status_item_no_liquid_to_pump",
				StatusItem.IconType.Custom,
				NotificationType.BadMinor,
				false,
				OverlayModes.None.ID,
				true);

			fountainFlowing = new StatusItem(
				"DecorPackB_FountainFlowing",
				"BUILDING",
				string.Empty,
				StatusItem.IconType.Info,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID,
				false);

			awaitingFuel = new StatusItem(
				"DecorPackB_AwaitingFuel",
				"BUILDING",
				"status_item_no_gas_to_pump",
				StatusItem.IconType.Custom,
				NotificationType.BadMinor,
				false,
				OverlayModes.None.ID,
				true);

			awaitingFuel.SetResolveStringCallback((str, obj) =>
			{
				if (obj is OilLantern lantern && lantern.TryGetComponent(out ElementConverter elementConverter))
				{
					var fuel = elementConverter.consumedElements[0].Tag.ProperName();
					var formattedMass = GameUtil.GetFormattedMass(elementConverter.consumedElements[0].MassConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");

					return string.Format(str, fuel, formattedMass);
				}

				return str;
			});

			statusItems.Add(awaitingFuel);
		}
	}
}
