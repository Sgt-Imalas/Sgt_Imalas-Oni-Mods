using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs.BuildingPortUtils
{
    public static class PortConduitExtensions
	{       
		// Get the index of the layer with the pipes for the conduit type in question 
		internal static int GetConduitObjectLayer(this ConduitType conduitType)
		{
			switch (conduitType)
			{
				case ConduitType.Gas: return (int)ObjectLayer.GasConduit;
				case ConduitType.Liquid: return (int)ObjectLayer.LiquidConduit;
				case ConduitType.Solid: return (int)ObjectLayer.SolidConduit;
			}
			return 0;
		}
		internal static int GetPortObjectLayer(this ConduitType conduitType)
		{
			switch (conduitType)
			{
				case ConduitType.Gas: return (int)ObjectLayer.GasConduitConnection;
				case ConduitType.Liquid: return (int)ObjectLayer.LiquidConduitConnection;
				case ConduitType.Solid: return (int)ObjectLayer.SolidConduitConnection;
			}
			return 0;
		}

		// Get the index of the layer with the connectors (ports) for the conduit type in question 
		internal static bool IsConnected(this ConduitType conduitType, int cell)
		{
			GameObject building = Grid.Objects[cell, conduitType.GetConduitObjectLayer()];

			return building != null && building.TryGetComponent<BuildingComplete>(out _);
		}

		// Get a cell of a building. Takes rotation into account
		internal static int GetCellWithOffset(this Building building, CellOffset offset)
		{
			int bottomLeftCell = Grid.PosToCell(building);

			CellOffset rotatedOffset = building.GetRotatedOffset(offset);
			return Grid.OffsetCell(bottomLeftCell, rotatedOffset);
		}
	}
}
