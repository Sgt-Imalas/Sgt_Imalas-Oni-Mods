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
		public enum CargobaySizeCategory
		{
			Small,
			Medium,
			Large,
		}

		public struct CargoBayInfo
		{
			public float Units;
			public float KGperUnit;
			public CargobaySizeCategory Category;
			public float Capacity => Units * KGperUnit;
			public float UnbalancedCapacity;
			public CargoBayInfo(float unbalanced, float kGperUnit, float units, CargobaySizeCategory cat)
			{
				UnbalancedCapacity = unbalanced;
				Units = units;
				KGperUnit = kGperUnit;
				Category = cat;
			}

		}

		/// <summary>
		/// Second value is when rebalance is on, first is when off
		/// </summary>
		public static Dictionary<string, CargoBayInfo> CargoBaySizes = new()
		{
			{
				SolidCargoBaySmallConfig.ID,
				new (1200f * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.SolidCargoBayKgPerUnit , Config.Instance.SmallCargoBayUnits, CargobaySizeCategory.Small)
			},
			{
				SolidCargoBayClusterConfig.ID,
				new (ROCKETRY.SOLID_CARGO_BAY_CLUSTER_CAPACITY * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.SolidCargoBayKgPerUnit, Config.Instance.MediumCargoBayUnits, CargobaySizeCategory.Medium)
			},
			{
				SolidCargoBayClusterLargeConfig.ID,
				new (SolidCargoBayClusterLargeConfig.CAPACITY_OFF,Config.Instance.SolidCargoBayKgPerUnit, Config.Instance.CollossalCargoBayUnits, CargobaySizeCategory.Large)
			},

			{
				LiquidCargoBaySmallConfig.ID,
				new(900f * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.LiquidCargoBayKgPerUnit , Config.Instance.SmallCargoBayUnits,CargobaySizeCategory.Small)
			},
			{
				LiquidCargoBayClusterConfig.ID,
				new (ROCKETRY.LIQUID_CARGO_BAY_CLUSTER_CAPACITY * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.LiquidCargoBayKgPerUnit ,Config.Instance.MediumCargoBayUnits,CargobaySizeCategory.Medium)
			},
			{
				LiquidCargoBayClusterLargeConfig.ID,
				new (LiquidCargoBayClusterLargeConfig.CAPACITY_OFF,Config.Instance.LiquidCargoBayKgPerUnit,Config.Instance.CollossalCargoBayUnits, CargobaySizeCategory.Large)
			},

			{
				GasCargoBaySmallConfig.ID,
				new (360f * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.GasCargoBayKgPerUnit ,Config.Instance.SmallCargoBayUnits,CargobaySizeCategory.Small)
			},
			{
				GasCargoBayClusterConfig.ID,
				new (ROCKETRY.GAS_CARGO_BAY_CLUSTER_CAPACITY * ROCKETRY.CARGO_CAPACITY_SCALE,Config.Instance.GasCargoBayKgPerUnit, Config.Instance.MediumCargoBayUnits,CargobaySizeCategory.Medium)
			},
			{
				GasCargoBayClusterLargeConfig.ID,
				new (GasCargoBayClusterLargeConfig.CAPACITY_OFF, Config.Instance.GasCargoBayKgPerUnit,Config.Instance.CollossalCargoBayUnits, CargobaySizeCategory.Large)
			},

		};
		public static bool TryGetCargoBayCapacity(string id, out float cargoCapacity)
		{
			cargoCapacity = 0;
			if (!CargoBaySizes.ContainsKey(id))
			{
				return false;
			}
			else
			{
				cargoCapacity = Config.Instance.RebalancedCargoCapacity ? CargoBaySizes[id].Capacity : CargoBaySizes[id].UnbalancedCapacity;
				return true;
			}
		}

		public static bool TryGetCargobayCollectionSpeed(string id, out float cargoCollectionSpeed)
		{
			cargoCollectionSpeed = 0;
			float divider = 3600f;
			if (!CargoBaySizes.ContainsKey(id))
			{
				return false;
			}
			else
			{
				var data = CargoBaySizes[id];
				float rebalancedCollectionSpeed =
					data.Category switch
					{
						CargobaySizeCategory.Small => data.Capacity / divider, //vanilla nr, keep as baseline for small; 6 cycles till full
						CargobaySizeCategory.Medium => data.Capacity / divider / 1.25f, //still faster than vanilla numbers, 7.5 cycles till full
						CargobaySizeCategory.Large => data.Capacity / divider / 1.6f, //9.6 cycles till full
						_ => -1

					};
				cargoCollectionSpeed = Config.Instance.RebalancedCargoCapacity
					? rebalancedCollectionSpeed
					: data.Capacity / 3600f;
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
