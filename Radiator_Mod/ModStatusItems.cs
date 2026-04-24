using Database;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static STRINGS.BUILDING.STATUSITEMS;
using static STRINGS.BUILDINGS.PREFABS;

namespace Radiator_Mod
{
	internal class ModStatusItems
	{
		public static StatusItem _radiating_status;
		public static StatusItem _no_space_status;
		public static StatusItem _protected_from_impacts_status;
		public const string Category = "BUILDING", InSpaceRadiating = "RM_INSPACERADIATING", NotInSpace = "RM_NOTINSPACE", BunkerDown = "RM_BUNKERDOWN";
		public static void Register(BuildingStatusItems statusItems)
		{
			_radiating_status = new StatusItem(InSpaceRadiating, Category, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.HeatFlow.ID);
			_radiating_status.resolveTooltipCallback = _FormatStatusCallback;
			_radiating_status.resolveStringCallback = _FormatStatusCallback;
			_no_space_status = new StatusItem(NotInSpace, Category, string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.TileMode.ID);
			_protected_from_impacts_status = new StatusItem(BunkerDown, Category, string.Empty, StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.TileMode.ID);

			statusItems.Add(_radiating_status);
			statusItems.Add(_no_space_status);
			statusItems.Add(_protected_from_impacts_status);
		}
		/// <summary>
		/// formatting for dtu/s status msg
		/// </summary>
		/// <param name="formatstr"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		private static string _FormatStatusCallback(string formatstr, object data)
		{
			var radiator = (RadiatorBase)data;
			var radiation_rate = GameUtil.GetFormattedHeatEnergyRate(radiator.CurrentCoolingRadiation);
			formatstr = formatstr.Replace("{0}", radiation_rate);
			formatstr = formatstr.Replace("{AREAPERCENTAGE}", Mathf.RoundToInt(100f * radiator.AreaPercentage()).ToString());
			return formatstr;
		}
	}
}
