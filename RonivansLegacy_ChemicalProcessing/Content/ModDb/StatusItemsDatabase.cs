using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
    class StatusItemsDatabase
	{
		public static StatusItem HPA_NeedGasIn;
		public static StatusItem HPA_NeedLiquidIn;
		public static StatusItem HPA_NeedGasOut;
		public static StatusItem HPA_NeedLiquidOut;
		public static StatusItem HPA_NeedSolidIn;
		public static StatusItem HPA_NeedSolidOut;

		public static void CreateStatusItems()
		{
			HPA_NeedGasIn = new StatusItem("HPA_NeedGasIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID);
			HPA_NeedLiquidIn = new StatusItem("HPA_NeedLiquidIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID);
			HPA_NeedSolidIn = new StatusItem("HPA_NeedSolidIn", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.SolidConveyor.ID);
			
			HPA_NeedGasOut = new StatusItem("HPA_NeedGasOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID);
			HPA_NeedLiquidOut = new StatusItem("HPA_NeedLiquidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID);
			HPA_NeedSolidOut = new StatusItem("HPA_NeedSolidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.SolidConveyor.ID);
		}
	}
}
