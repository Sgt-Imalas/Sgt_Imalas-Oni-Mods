using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.NonRocketBuildings;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static Rockets_TinyYetBig.STRINGS.BUILDING.STATUSITEMS;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal static class ModStatusItems
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

		public static void Register()
		{
			RTB_RocketBatteryStatus = new StatusItem(
				  "RTB_ROCKETBATTERYSTATUS",
				  "BUILDING",
				  string.Empty,
				  StatusItem.IconType.Info,
				  NotificationType.Neutral,
				  false,
				  OverlayModes.Power.ID
				  );

			RTB_ModuleGeneratorNotPowered = new StatusItem(
				  "RTB_MODULEGENERATORNOTPOWERED",
				  "BUILDING",
				  string.Empty,
				  StatusItem.IconType.Info,
				  NotificationType.Neutral,
				  false,
				  OverlayModes.Power.ID
				  );

			RTB_ModuleGeneratorPowered = new StatusItem(
			   "RTB_MODULEGENERATORPOWERED",
			   "BUILDING",
			   string.Empty,
			   StatusItem.IconType.Info,
			   NotificationType.Neutral,
			   false,
			   OverlayModes.Power.ID);

			RTB_AlwaysActiveOn = new StatusItem(
				"RTB_MODULEGENERATORALWAYSACTIVEPOWERED",
				"BUILDING",
				string.Empty,
				StatusItem.IconType.Info,
				NotificationType.Neutral,
				false,
				OverlayModes.Power.ID);

			RTB_AlwaysActiveOff = new StatusItem(
				 "RTB_MODULEGENERATORALWAYSACTIVENOTPOWERED",
				 "BUILDING",
				 string.Empty,
				 StatusItem.IconType.Info,
				 NotificationType.Neutral,
				 false,
				 OverlayModes.Power.ID);

			RTB_ModuleGeneratorFuelStatus = new StatusItem(
				 "RTB_MODULEGENERATORFUELSTATUS",
				 "BUILDING",
				 string.Empty,
				 StatusItem.IconType.Info,
				 NotificationType.Neutral,
				 false,
				 OverlayModes.Power.ID);

			RTB_ModuleGeneratorLandedEnabled = new StatusItem(
				 "RTB_ROCKETGENERATORLANDEDACTIVE",
				 "BUILDING",
				 string.Empty,
				 StatusItem.IconType.Info,
				 NotificationType.Neutral,
				 false,
				 OverlayModes.Power.ID);

			RTB_MiningInformationBoons = new StatusItem(
				 "RTB_MININGINFORMATIONBOONS",
				 "BUILDING",
				 string.Empty,
				 StatusItem.IconType.Info,
				 NotificationType.Neutral,
			false,
				 OverlayModes.None.ID);

			RTB_CritterModuleContent = new StatusItem(
				 "RTB_CRITTERMODULECONTENT",
				 "BUILDING",
				 string.Empty,
				 StatusItem.IconType.Info,
				 NotificationType.Neutral,
			false,
				 OverlayModes.None.ID);

			RTB_AccessHatchStorage = new StatusItem(
				 "RTB_FOODSTORAGESTATUS",
				 "BUILDING",
				 string.Empty,
				 StatusItem.IconType.Info,
				 NotificationType.Neutral,
			false,
				 OverlayModes.None.ID);

			RTB_SpaceStationConstruction_Status = new StatusItem(
				 "RTB_STATIONCONSTRUCTORSTATUS",
				 "BUILDING",
				 string.Empty,
				 StatusItem.IconType.Info,
				 NotificationType.Neutral,
			false,
				 OverlayModes.None.ID);

			RTB_DockingActive = new StatusItem(
				 "RTB_DOCKEDSTATUS",
				 "BUILDING",
				 string.Empty,
				 StatusItem.IconType.Info,
				 NotificationType.Neutral,
			false,
				 OverlayModes.None.ID);



			RTB_SpaceStationConstruction_Status.resolveStringCallback = ((str, data) =>
			{
				var StationConstructior = (SpaceStationBuilder)data;

				if (StationConstructior.ConstructionTimes(out bool isConstructing, out var remainingTime))
				{
					if (isConstructing)
					{
						str = str.Replace("{STATUS}", RTB_STATIONCONSTRUCTORSTATUS.CONSTRUCTING);
						str = str.Replace("{TIME}", GameUtil.GetFormattedTime(remainingTime));
					}
					else
					{
						str = str.Replace("{STATUS}", RTB_STATIONCONSTRUCTORSTATUS.DECONSTRUCTING);
						str = str.Replace("{TIME}", GameUtil.GetFormattedTime(remainingTime));
					}
					str = str.Replace("{TOOLTIP}", RTB_STATIONCONSTRUCTORSTATUS.TIMEREMAINING);
				}
				else
				{
					str = str.Replace("{TOOLTIP}", RTB_STATIONCONSTRUCTORSTATUS.NONEQUEUED);
					str = str.Replace("{STATUS}", RTB_STATIONCONSTRUCTORSTATUS.IDLE);
				}
				return str;
			});


			RTB_CritterModuleContent.resolveStringCallback = (Func<string, object, string>)((str, data) =>
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
			RTB_MiningInformationBoons.resolveStringCallback = (Func<string, object, string>)((str, data) =>
			{
				if (data is Tuple<float, string> tooltipData)
				{
					str = str.Replace("{RATEPERCENTAGE}", tooltipData.first.ToString("0%"));
					str = str.Replace("{TOOLTIP}", tooltipData.second);
				}
				return str;
			});

			RTB_RocketBatteryStatus.resolveStringCallback = (Func<string, object, string>)((str, data) =>
			{
				var stats = (Tuple<float, float>)data;
				//var stats = generator.GetConsumptionStatusStats();
				//str = str.Replace("{GeneratorType}", generator.GetProperName());
				str = str.Replace("{CurrentCharge}", GameUtil.GetFormattedJoules(stats.first));
				str = str.Replace("{MaxCharge}", GameUtil.GetFormattedJoules(stats.second));
				return str;
			});

			RTB_DockingActive.resolveStringCallback = (Func<string, object, string>)((str, data) =>
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

			RTB_ModuleGeneratorFuelStatus.resolveStringCallback = (Func<string, object, string>)((str, data) =>
			{
				var stats = (Tuple<float, float>)data;
				//var stats = generator.GetConsumptionStatusStats();
				//str = str.Replace("{GeneratorType}", generator.GetProperName());
				str = str.Replace("{CurrentFuelStorage}", GameUtil.GetFormattedMass(stats.first));
				str = str.Replace("{MaxFuelStorage}", GameUtil.GetFormattedMass(stats.second));
				return str;
			});

			RTB_ModuleGeneratorNotPowered.resolveStringCallback = (Func<string, object, string>)((str, data) =>
			{
				Generator generator = (RTB_ModuleGenerator)data;
				str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(0.0f));
				str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
				return str;
			});
			RTB_ModuleGeneratorPowered.resolveStringCallback = (Func<string, object, string>)((str, data) =>
			{
				Generator generator = (RTB_ModuleGenerator)data;
				str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
				str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
				return str;
			});
			RTB_AlwaysActiveOff.resolveStringCallback = (Func<string, object, string>)((str, data) =>
			{
				Generator generator = (RTB_ModuleGenerator)data;
				str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(0.0f));
				str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
				return str;
			});
			RTB_AlwaysActiveOn.resolveStringCallback = (Func<string, object, string>)((str, data) =>
			{
				Generator generator = (RTB_ModuleGenerator)data;
				str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
				str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
				return str;
			});
			RTB_AccessHatchStorage.resolveStringCallback = (Func<string, object, string>)((str, data) =>
			{
				FridgeModuleHatchGrabber hatch = (FridgeModuleHatchGrabber)data;
				str = str.Replace("{FOODLIST}", hatch.GetAllMassDesc());
				str = str.Replace("{REMAININGMASS}", hatch.TotalKCAL.ToString());
				return str;
			});

			FixDrillingCallback();
			SgtLogger.debuglog("[Rocketry Expanded] Status items initialized");

		}

		public static void FixDrillingCallback()
		{
			var statusItems = Db.Get().BuildingStatusItems;

			statusItems.SpacePOIHarvesting.resolveStringCallback = delegate (string str, object data)
			{
				var extract = (Tuple<Clustercraft,float>)data;

				ClusterGridEntity pOIAtCurrentLocation = extract.first.GetPOIAtCurrentLocation();
				string text3 = ((pOIAtCurrentLocation == null) ? "Unknown POI" : pOIAtCurrentLocation.GetProperName());// <- that error in loc. is klei fault
				return GameUtil.SafeStringFormat(
					global::STRINGS.BUILDING.STATUSITEMS.SPACEPOIHARVESTING.NAME, text3,
					GameUtil.GetFormattedMass(extract.second, GameUtil.TimeSlice.PerSecond));
			};
		}
	}
}

