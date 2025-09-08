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
		public static StatusItem HPA_NeedGasOut;
		public static StatusItem HPA_ProhibitGas;

		public static StatusItem HPA_NeedLiquidIn;
		public static StatusItem HPA_NeedLiquidOut;
		public static StatusItem HPA_ProhibitLiquid;

		public static StatusItem HPA_NeedSolidIn;
		public static StatusItem HPA_NeedSolidOut;
		public static StatusItem HPA_ProhibitSolid;

		public static StatusItem LOGISTIC_NeedSolidIn;
		public static StatusItem LOGISTIC_NeedSolidOut;  
		
		public static StatusItem CG_RotatableSolarPanelWattage;
		public static StatusItem AlgaeGrower_LightEfficiency;

		public static StatusItem Converter_StorageFull;

		public static void CreateStatusItems()
		{
			var bsi = Db.Get().BuildingStatusItems;

			HPA_NeedGasIn = bsi.CreateStatusItem("HPA_NeedGasIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID);
			HPA_NeedLiquidIn = bsi.CreateStatusItem("HPA_NeedLiquidIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID);
			HPA_NeedSolidIn = bsi.CreateStatusItem("HPA_NeedSolidIn", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.SolidConveyor.ID);
			LOGISTIC_NeedSolidIn = bsi.CreateStatusItem("LOGISTIC_NeedSolidIn", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.SolidConveyor.ID);

			HPA_NeedGasOut = bsi.CreateStatusItem("HPA_NeedGasOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID);
			HPA_NeedLiquidOut = bsi.CreateStatusItem("HPA_NeedLiquidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID);
			HPA_NeedSolidOut = bsi.CreateStatusItem("HPA_NeedSolidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.SolidConveyor.ID);
			LOGISTIC_NeedSolidOut = bsi.CreateStatusItem("LOGISTIC_NeedSolidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.SolidConveyor.ID);

			HPA_ProhibitGas = bsi.CreateStatusItem("HPA_ProhibitGas", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID);
			HPA_ProhibitSolid = bsi.CreateStatusItem("HPA_ProhibitSolid", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.SolidConveyor.ID);
			HPA_ProhibitLiquid = bsi.CreateStatusItem("HPA_ProhibitLiquid", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID);


			CG_RotatableSolarPanelWattage = bsi.CreateStatusItem("CG_RotatableSolarPanelWattage", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Power.ID);
			CG_RotatableSolarPanelWattage.resolveStringCallback = delegate (string str, object data)
			{
				RotatableSmallSolarPanel solarPanel = (RotatableSmallSolarPanel)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(solarPanel.CurrentWattage));
				return str;
			};

			AlgaeGrower_LightEfficiency = bsi.CreateStatusItem("AlgaeGrower_LightEfficiency", STRINGS.BUILDING.STATUSITEMS.ALGAEGROWER_LIGHTEFFICIENCY.NAME, STRINGS.BUILDING.STATUSITEMS.ALGAEGROWER_LIGHTEFFICIENCY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			AlgaeGrower_LightEfficiency.resolveStringCallback = delegate (string str, object obj)
			 {
				 LightEfficiencyConverter converter = obj as LightEfficiencyConverter;
				 return string.Format(str, GameUtil.GetFormattedPercent(converter.LightEfficiency * 100f), GameUtil.GetFormattedLux((int)converter.MiniumLightRequirement));
			 };

			Converter_StorageFull = bsi.CreateStatusItem("Converter_StorageFull", STRINGS.BUILDING.STATUSITEMS.CONVERTER_STORAGEFULL.NAME, STRINGS.BUILDING.STATUSITEMS.CONVERTER_STORAGEFULL.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: true, OverlayModes.None.ID);
			Converter_StorageFull.resolveStringCallback = delegate (string str, object obj)
			{
				ElementThresholdOperational converter = obj as ElementThresholdOperational;
				return string.Format(str, converter.ThresholdTag.Name);
			}; 
			Converter_StorageFull.resolveTooltipCallback = delegate (string str, object obj)
			{
				ElementThresholdOperational converter = obj as ElementThresholdOperational;
				return string.Format(str, converter.ThresholdTag.Name);
			};
		}

		internal static void RegisterClonedStatusStrings()
		{
			Strings.Add("STRINGS.BUILDING.STATUSITEMS.CG_ROTATABLESOLARPANELWATTAGE.NAME", global::STRINGS.BUILDING.STATUSITEMS.SOLARPANELWATTAGE.NAME);
			Strings.Add("STRINGS.BUILDING.STATUSITEMS.CG_ROTATABLESOLARPANELWATTAGE.TOOLTIP", global::STRINGS.BUILDING.STATUSITEMS.SOLARPANELWATTAGE.TOOLTIP);
		}
	}
}
