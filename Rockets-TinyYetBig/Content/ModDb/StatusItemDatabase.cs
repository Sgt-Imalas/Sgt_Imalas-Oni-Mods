using Database;
using Epic.OnlineServices.Stats;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.NonRocketBuildings;
using System;
using System.Collections.Generic;
using UtilLibs;
using static Rockets_TinyYetBig.STRINGS.BUILDING.STATUSITEMS;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal static class StatusItemDatabase
	{
		public static StatusItem RTB_ModuleGeneratorNotPowered;
		public static StatusItem RTB_ModuleGeneratorPowered;
		public static StatusItem RTB_ModuleGeneratorFuelStatus;
		public static StatusItem RTB_ModuleGeneratorLandedEnabled;
		public static StatusItem RTB_RocketBatteryStatus;
		public static StatusItem RTB_AlwaysActiveOn;
		public static StatusItem RTB_AlwaysActiveOff;
		public static StatusItem RTB_CritterModuleContent;
		public static StatusItem RTB_AccessHatchStorage;
		public static StatusItem RTB_MiningInformationBoons;


		public static StatusItem RTB_SpaceStationConstruction_Status;

		public static StatusItem RTB_SpaceStation_DeploymentState;
		public static StatusItem RTB_SpaceStation_FreshlyDeployed;
		public static StatusItem RTB_SpaceStation_OrbitHealth;
		public static StatusItem RTB_DockingActive;
		public static StatusItem RTB_PendingStationPartDeconstruction;
		public static StatusItem RTB_PendingUpgradeRemoval;
		public static StatusItem RTB_NoSunLight;
		public static StatusItem RTB_MissingElement;
		public static StatusItem RTB_MissingRocketModule;

		public static void Register()
		{
			var bsi = Db.Get().BuildingStatusItems;
			var noneOverlay = OverlayModes.None.ID;


			RTB_PendingUpgradeRemoval = bsi.CreateStatusItem("RTB_PENDINGUPGRADEREMOVAL", "BUILDING", "status_item_pending_deconstruction", StatusItem.IconType.Custom, NotificationType.Neutral, false, noneOverlay)
				.TooltipCallback((string str, object obj) =>
				  {
					  if (obj is RocketModuleUpgrade upgrade)
					  {
						  return str.Replace("{0}", upgrade.GetName());
					  }
					  return str;
				  });
			RTB_PendingStationPartDeconstruction = bsi.CreateStatusItem("RTB_PENDINGSTATIONPARTDECONSTRUCTION", "BUILDING", "status_item_pending_deconstruction", StatusItem.IconType.Custom, NotificationType.Neutral, false, noneOverlay);

			RTB_RocketBatteryStatus = bsi.CreateStatusItem("RTB_ROCKETBATTERYSTATUS", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID)
				.StringCallback((str, data) =>
				{
					var stats = (Tuple<float, float>)data;
					//var stats = generator.GetConsumptionStatusStats();
					//str = str.Replace("{GeneratorType}", generator.GetProperName());
					str = str.Replace("{CurrentCharge}", GameUtil.GetFormattedJoules(stats.first));
					str = str.Replace("{MaxCharge}", GameUtil.GetFormattedJoules(stats.second));
					return str;
				});

			RTB_ModuleGeneratorNotPowered = bsi.CreateStatusItem("RTB_MODULEGENERATORNOTPOWERED", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID)
				.StringCallback((str, data) =>
				{
					Generator generator = (RTB_ModuleGenerator)data;
					str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(0.0f));
					str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
					return str;
				});

			RTB_ModuleGeneratorPowered = bsi.CreateStatusItem("RTB_MODULEGENERATORPOWERED", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID)
				.StringCallback((str, data) =>
				{
					Generator generator = (RTB_ModuleGenerator)data;
					str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
					str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
					return str;
				});

			RTB_AlwaysActiveOn = bsi.CreateStatusItem("RTB_MODULEGENERATORALWAYSACTIVEPOWERED", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID)
				.StringCallback((str, data) =>
				{
					Generator generator = (RTB_ModuleGenerator)data;
					str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
					str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
					return str;
				});

			RTB_AlwaysActiveOff = bsi.CreateStatusItem("RTB_MODULEGENERATORALWAYSACTIVENOTPOWERED", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID)
				.StringCallback((str, data) =>
				{
					Generator generator = (RTB_ModuleGenerator)data;
					str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(0.0f));
					str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
					return str;
				});

			RTB_ModuleGeneratorFuelStatus = bsi.CreateStatusItem("RTB_MODULEGENERATORFUELSTATUS", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID)
				.StringCallback((str, data) =>
				{
					var stats = (Tuple<float, float>)data;
					//var stats = generator.GetConsumptionStatusStats();
					//str = str.Replace("{GeneratorType}", generator.GetProperName());
					str = str.Replace("{CurrentFuelStorage}", GameUtil.GetFormattedMass(stats.first));
					str = str.Replace("{MaxFuelStorage}", GameUtil.GetFormattedMass(stats.second));
					return str;
				});

			RTB_ModuleGeneratorLandedEnabled = bsi.CreateStatusItem("RTB_ROCKETGENERATORLANDEDACTIVE", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID);

			RTB_MiningInformationBoons = bsi.CreateStatusItem("RTB_MININGINFORMATIONBOONS", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, noneOverlay)
				.StringCallback((str, data) =>
				{
					if (data is Tuple<float, string> tooltipData)
					{
						str = str.Replace("{RATEPERCENTAGE}", tooltipData.first.ToString("0%"));
						str = str.Replace("{TOOLTIP}", tooltipData.second);
					}
					return str;
				});

			RTB_CritterModuleContent = bsi.CreateStatusItem("RTB_CRITTERMODULECONTENT", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, noneOverlay)
				.StringCallback((str, data) =>
				{
					var CritterStorage = (CritterStasisChamberModule)data;
					//var stats = generator.GetConsumptionStatusStats();
					//str = str.Replace("{GeneratorType}", generator.GetProperName());

					string newValue1 = Util.FormatWholeNumber(CritterStorage.CurrentCapacity);
					string newValue2 = Util.FormatWholeNumber(CritterStorage.CurrentMaxCapacity);
					string CritterData = CritterStorage.GetStatusItem();

					str = str.Replace("{0}", newValue1);
					str = str.Replace("{1}", newValue2);
					str = str.Replace("{CritterContentStatus}", CritterData);
					return str;
				});

			RTB_AccessHatchStorage = bsi.CreateStatusItem("RTB_FOODSTORAGESTATUS", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, noneOverlay)
				.StringCallback((str, data) =>
				{
					FridgeModuleHatchGrabber hatch = (FridgeModuleHatchGrabber)data;
					str = str.Replace("{FOODLIST}", hatch.GetAllMassDesc());
					str = str.Replace("{REMAININGMASS}", hatch.TotalKCAL.ToString());
					return str;
				});

			RTB_SpaceStationConstruction_Status = bsi.CreateStatusItem("RTB_STATIONCONSTRUCTORSTATUS", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, noneOverlay)
				.StringCallback((str, data) =>
				{
					//var StationConstructior = (SpaceStationBuilder)data;

					//if (StationConstructior.ConstructionTimes(out bool isConstructing, out var remainingTime))
					//{
					//	if (isConstructing)
					//	{
					//		str = str.Replace("{STATUS}", RTB_STATIONCONSTRUCTORSTATUS.CONSTRUCTING);
					//		str = str.Replace("{TIME}", GameUtil.GetFormattedTime(remainingTime));
					//	}
					//	else
					//	{
					//		str = str.Replace("{STATUS}", RTB_STATIONCONSTRUCTORSTATUS.DECONSTRUCTING);
					//		str = str.Replace("{TIME}", GameUtil.GetFormattedTime(remainingTime));
					//	}
					//	str = str.Replace("{TOOLTIP}", RTB_STATIONCONSTRUCTORSTATUS.TIMEREMAINING);
					//}
					//else
					//{
					//	str = str.Replace("{TOOLTIP}", RTB_STATIONCONSTRUCTORSTATUS.NONEQUEUED);
					//	str = str.Replace("{STATUS}", RTB_STATIONCONSTRUCTORSTATUS.IDLE);
					//}
					return str;
				});

			RTB_DockingActive = bsi.CreateStatusItem("RTB_DOCKEDSTATUS", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, noneOverlay)
				.StringCallback((str, data) =>
				{
					var worldIDs = (List<int>)data;

					string worldsList = string.Empty;

					foreach (var id in worldIDs)
					{
						var world = ClusterManager.Instance.GetWorld(id);
						if (world != null)
						{
							worldsList += string.Format(RTB_DOCKEDSTATUS.DOCKEDINFO, world.GetProperName());
						}
						if (worldIDs.Count == 1)
						{
							str = str.Replace("{SINGLEDOCKED}", world.GetProperName());
						}
						else
						{
							str = str.Replace("{SINGLEDOCKED}", RTB_DOCKEDSTATUS.MULTIPLES);
						}
					}
					str = str.Replace("{DOCKINGLIST}", worldsList);
					return str;
				});

			RTB_NoSunLight = bsi.CreateStatusItem("RTB_NoSunLight", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.Bad, allow_multiples: false, noneOverlay);
			RTB_MissingElement = bsi.CreateStatusItem("RTB_MissingElement", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: true, noneOverlay)
				.StringCallback((str, data) => 
				{
					if (data is not string tag)
						return str;

					return string.Format(str, tag);
				});
			RTB_MissingRocketModule = bsi.CreateStatusItem("RTB_MissingRocketModule", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: true, noneOverlay)
				.StringCallback((str, data) =>
				{
					if (data is not Tag tag)
						return str;

					var tagName = Strings.Get($"STRINGS.MISC.TAGS.{tag.ToString().ToUpperInvariant()}").ToString();
					if (tag.Name.Contains("MISSING"))
					{
						var prefab = Assets.GetPrefab(str);
						if (prefab != null)
							return tagName = prefab.GetProperName();
					}
					return str.Replace("{MODULE_TYPE}",tagName);
				});

			FixDrillingCallback();
			SgtLogger.debuglog("[Rocketry Expanded] Status items initialized");

		}

		public static void FixDrillingCallback()
		{
			var statusItems = Db.Get().BuildingStatusItems;

			statusItems.SpacePOIHarvesting.resolveStringCallback = delegate (string str, object data)
			{
				var extract = (Tuple<Clustercraft, float>)data;

				ClusterGridEntity pOIAtCurrentLocation = extract.first.GetPOIAtCurrentLocation();
				string text3 = ((pOIAtCurrentLocation == null) ? "Unknown POI" : pOIAtCurrentLocation.GetProperName());// <- that error in loc. ("unknown poi") is klei fault
				return GameUtil.SafeStringFormat(
					global::STRINGS.BUILDING.STATUSITEMS.SPACEPOIHARVESTING.NAME, text3,
					GameUtil.GetFormattedMass(extract.second, GameUtil.TimeSlice.PerSecond));
			};
		}
	}
}

