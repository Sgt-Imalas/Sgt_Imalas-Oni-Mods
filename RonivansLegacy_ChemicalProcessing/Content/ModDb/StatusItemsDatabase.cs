using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.BUILDING.STATUSITEMS;

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
		public static StatusItem CG_RotatableSolarPanelWattage;

		public static void CreateStatusItems()
		{
			var bsi = Db.Get().BuildingStatusItems;

			HPA_NeedGasIn = bsi.CreateStatusItem("HPA_NeedGasIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID);
			HPA_NeedLiquidIn = bsi.CreateStatusItem("HPA_NeedLiquidIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID);
			HPA_NeedSolidIn = bsi.CreateStatusItem("HPA_NeedSolidIn", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.SolidConveyor.ID);
			
			HPA_NeedGasOut = bsi.CreateStatusItem("HPA_NeedGasOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID);
			HPA_NeedLiquidOut = bsi.CreateStatusItem("HPA_NeedLiquidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID);
			HPA_NeedSolidOut = bsi.CreateStatusItem("HPA_NeedSolidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.SolidConveyor.ID);

			CG_RotatableSolarPanelWattage = bsi.CreateStatusItem("CG_RotatableSolarPanelWattage", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Power.ID);
			CG_RotatableSolarPanelWattage.resolveStringCallback = delegate (string str, object data)
			{
				RotatableSmallSolarPanel solarPanel = (RotatableSmallSolarPanel)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(solarPanel.CurrentWattage));
				return str;
			};
		}

		internal static void RegisterClonedStatusStrings()
		{
			Strings.Add("STRINGS.BUILDING.STATUSITEMS.CG_ROTATABLESOLARPANELWATTAGE.NAME", global::STRINGS.BUILDING.STATUSITEMS.SOLARPANELWATTAGE.NAME);
			Strings.Add("STRINGS.BUILDING.STATUSITEMS.CG_ROTATABLESOLARPANELWATTAGE.TOOLTIP", global::STRINGS.BUILDING.STATUSITEMS.SOLARPANELWATTAGE.TOOLTIP);
		}
	}
}
