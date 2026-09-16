using PeterHan.PLib.Core;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UtilLibs.BuildingPortUtils
{
	public static class SharedConduitUtils
	{


		/// <summary>
		/// ModIntegration:
		/// if your mod modifies the base capacity of conduits,
		/// please put these modified values into PRegistry under the respective key,
		/// that way this mod can adjust its conduit patches for high capacity piping:
		/// 
		/// example:
		/// PRegistry.PutData("ConduitCapacity_Gas", yourGasCapacity);
		/// </summary>
		public static readonly string
			PRegistryCache_ConduitCapacity_Solid = "ConduitCapacity_Solid",
			PRegistryCache_ConduitCapacity_Liquid = "ConduitCapacity_Liquid",
			PRegistryCache_ConduitCapacity_Gas = "ConduitCapacity_Gas";

		/// <summary>
		/// Integration with CustomizeBuildings custom pipe & rail capacities
		/// </summary>
		public static float GetDefaultConduitCapacity(ConduitType type)
		{
			float capacity = 0;
			switch (type)
			{
				case ConduitType.Gas:
					capacity = ConduitFlow.MAX_GAS_MASS;
					float gas = PRegistry.GetData<float>(PRegistryCache_ConduitCapacity_Gas);
					if (gas != default)
						capacity = gas;
					break;
				case ConduitType.Liquid:
					capacity = ConduitFlow.MAX_LIQUID_MASS;
					float liquid = PRegistry.GetData<float>(PRegistryCache_ConduitCapacity_Liquid);
					if (liquid != default)
						capacity = liquid;
					break;
				case ConduitType.Solid:
					capacity = SolidConduitFlow.MAX_SOLID_MASS;
					float solid = PRegistry.GetData<float>(PRegistryCache_ConduitCapacity_Solid); if (solid != default)
						capacity = solid;
					break;
			}
			return capacity;
		}


		static StringBuilder sb = new StringBuilder();
		public static string GetFilteredPortTooltip(ConduitType type, bool isInput, Tag[] filterTags = null, SimHashes[] elementFilterTags = null, bool invertedFilter = false)
		{
			if (elementFilterTags == null)
				elementFilterTags = [];
			if (filterTags == null)
				filterTags = [];

			sb.Clear();

			sb.Append(GetPortDescription(type, isInput));
			if (invertedFilter)
			{
				sb.Append(", ");
				sb.Append(STRINGS.DUPLICANTS.CHORES.PRECONDITIONS.IS_PERMITTED);
			}
			sb.Append(": ");

			if (!filterTags.Any() && !elementFilterTags.Any())
			{
				sb.Append(STRINGS.MISC.TAGS.ANY);
				return sb.ToString();
			}
			bool anyWritten = false;
			for (int i = 0; i < elementFilterTags.Length; i++)
			{
				if (anyWritten)
				{
					sb.Append(", ");
				}
				anyWritten = true;

				SimHashes elementId = elementFilterTags[i];
				var element = ElementLoader.GetElement(elementId.CreateTag());
				if (element != null)
					sb.Append(element.name);
			}
			for (int i = 0; i < filterTags.Length; i++)
			{
				if (anyWritten)
					sb.Append(", ");
				anyWritten = true;
				var tag = filterTags[i];
				var prefab = Assets.TryGetPrefab(tag);
				if (Strings.TryGet("STRINGS.MISC.TAGS." + tag.ToString().ToUpperInvariant(), out var stringEntry))
					sb.Append(stringEntry.String);
				else if (prefab != null)
					sb.Append(prefab.GetProperName());
			}

			return sb.ToString();
		}

		public static Color GetIOColor(bool input, ConduitType type)
		{
			var resources = BuildingCellVisualizerResources.Instance();
			var ioColors = type == ConduitType.Gas ? resources.gasIOColours : resources.liquidIOColours;
			var colorSet = input ? ioColors.input : ioColors.output;
			return colorSet.connected;
		}

		public static string GetPortDescription(ConduitType type, bool input)
		{
			return type switch
			{
				ConduitType.Gas => !input ? STRINGS.UI.OVERLAYS.GASPLUMBING.CONSUMER : STRINGS.UI.OVERLAYS.GASPLUMBING.PRODUCER,
				ConduitType.Liquid => !input ? STRINGS.UI.OVERLAYS.LIQUIDPLUMBING.CONSUMER : STRINGS.UI.OVERLAYS.LIQUIDPLUMBING.PRODUCER,
				ConduitType.Solid => !input ? STRINGS.UI.OVERLAYS.CONVEYOR.OUTPUT : STRINGS.UI.OVERLAYS.CONVEYOR.INPUT,
				_ => "",
			};
		}
		public static int GetConduitLayer(ConduitType conduitType)
		{
			switch (conduitType)
			{
				case ConduitType.Gas:
					return (int)ObjectLayer.GasConduit;
				case ConduitType.Liquid:
					return (int)ObjectLayer.LiquidConduit;
				case ConduitType.Solid:
					return (int)ObjectLayer.SolidConduit;
			}
			return -1;
		}

		public static IConduitFlow GetConduitFlow(ConduitType conduitType)
		{
			switch (conduitType)
			{
				case ConduitType.Gas:
					return Game.Instance.gasConduitFlow;
				case ConduitType.Liquid:
					return Game.Instance.liquidConduitFlow;
				case ConduitType.Solid:
					return Game.Instance.solidConduitFlow;
			}
			return null;
		}

		public static IUtilityNetworkMgr GetConduitMng(ConduitType conduitType)
		{
			switch (conduitType)
			{
				case ConduitType.Gas:
					return Game.Instance.gasConduitSystem;
				case ConduitType.Liquid:
					return Game.Instance.liquidConduitSystem;
				case ConduitType.Solid:
					return Game.Instance.solidConduitSystem;
			}
			return null;
		}

		public static Sprite GetSprite(bool input, ConduitType type)
		{
			var resources = BuildingCellVisualizerResources.Instance();
			switch (type)
			{
				case ConduitType.Gas:
					if (input)
						return resources.gasInputIcon;
					else
						return resources.gasOutputIcon;
				case ConduitType.Liquid:
					if (input)
						return resources.liquidInputIcon;
					else
						return resources.liquidOutputIcon;
				case ConduitType.Solid:
					if (input)
						return resources.liquidInputIcon;
					else
						return resources.liquidOutputIcon;
				default:
					return null;
			}
		}
	}
}
