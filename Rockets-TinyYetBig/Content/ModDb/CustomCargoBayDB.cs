using Rockets_TinyYetBig.Buildings.CargoBays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal class CustomCargoBayDB
	{
		/// <summary>
		/// Second value is when rebalance is on, first is when off
		/// </summary>
		public static Dictionary<string, Tuple<float, float>> CargoBaySizes = new Dictionary<string, Tuple<float, float>>()
		{
			{
				SolidCargoBaySmallConfig.ID,
				new Tuple<float, float>(1200f * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.SolidCargoBayKgPerUnit * Config.Instance.SmallCargoBayUnits)
			},
			{
				SolidCargoBayClusterConfig.ID,
				new Tuple<float, float>(ROCKETRY.SOLID_CARGO_BAY_CLUSTER_CAPACITY * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.SolidCargoBayKgPerUnit * Config.Instance.MediumCargoBayUnits)
			},
			{
				SolidCargoBayClusterLargeConfig.ID,
				new Tuple<float, float>(SolidCargoBayClusterLargeConfig.CAPACITY_OFF,SolidCargoBayClusterLargeConfig.CAPACITY_ON)
			},

			{
				LiquidCargoBaySmallConfig.ID,
				new Tuple<float, float>(900f * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.LiquidCargoBayKgPerUnit * Config.Instance.SmallCargoBayUnits)
			},
			{
				LiquidCargoBayClusterConfig.ID,
				new Tuple<float, float>(ROCKETRY.LIQUID_CARGO_BAY_CLUSTER_CAPACITY * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.LiquidCargoBayKgPerUnit * Config.Instance.MediumCargoBayUnits)
			},
			{
				LiquidCargoBayClusterLargeConfig.ID,
				new Tuple<float, float>(LiquidCargoBayClusterLargeConfig.CAPACITY_OFF,LiquidCargoBayClusterLargeConfig.CAPACITY_ON)
			},

			{
				GasCargoBaySmallConfig.ID,
				new Tuple<float, float>(360f * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.GasCargoBayKgPerUnit * Config.Instance.SmallCargoBayUnits)
			},
			{
				GasCargoBayClusterConfig.ID,
				new Tuple<float, float>(ROCKETRY.GAS_CARGO_BAY_CLUSTER_CAPACITY * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.GasCargoBayKgPerUnit * Config.Instance.MediumCargoBayUnits)
			},
			{
				GasCargoBayClusterLargeConfig.ID,
				new Tuple<float, float>(GasCargoBayClusterLargeConfig.CAPACITY_OFF,GasCargoBayClusterLargeConfig.CAPACITY_ON)
			},

		};
		public static bool GetCargoBayCapacity(string id, out float cargoCapacity)
		{
			cargoCapacity = 0;
			if (!CargoBaySizes.ContainsKey(id))
			{
				return false;
			}
			else
			{
				cargoCapacity = Config.Instance.RebalancedCargoCapacity ? CargoBaySizes[id].second : CargoBaySizes[id].first;
				return true;
			}
		}

		public static void AddCargoBayLogicPorts(BuildingDef def)
		{
			int xOffset = def.WidthInCells == 3 ? 1 : 2;

			if (def.LogicOutputPorts == null)
				def.LogicOutputPorts = new List<LogicPorts.Port>();
			def.LogicOutputPorts.Add(LogicPorts.Port.OutputPort(CargoBayStatusMonitor.EMPTY_PORT_ID, new CellOffset(-xOffset, 0), STRINGS.BUILDINGS.RTB_CARGOBAY_LOGICPORTS.EMPTY.LOGIC_PORT, STRINGS.BUILDINGS.RTB_CARGOBAY_LOGICPORTS.EMPTY.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.RTB_CARGOBAY_LOGICPORTS.EMPTY.LOGIC_PORT_INACTIVE, false));
			def.LogicOutputPorts.Add(LogicPorts.Port.OutputPort(CargoBayStatusMonitor.FULL_PORT_ID, new CellOffset(xOffset, 0), STRINGS.BUILDINGS.RTB_CARGOBAY_LOGICPORTS.FULL.LOGIC_PORT, STRINGS.BUILDINGS.RTB_CARGOBAY_LOGICPORTS.FULL.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.RTB_CARGOBAY_LOGICPORTS.FULL.LOGIC_PORT_INACTIVE, false));
		}

	}
}
