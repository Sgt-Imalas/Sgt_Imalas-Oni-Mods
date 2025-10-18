using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs.BuildingPortUtils
{
	internal class SharedConduitUtils
	{
		static StringBuilder sb = new StringBuilder();
		public static string GetFilteredPortTooltip(ConduitType type, bool isInput, Tag[] filterTags = null, SimHashes[] elementFilterTags = null, bool invertedFilter = false)
		{
			if (elementFilterTags == null)
				elementFilterTags = [];
			if(filterTags == null)
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
				ConduitType.Solid => !input ? STRINGS.UI.OVERLAYS.CONVEYOR.INPUT : STRINGS.UI.OVERLAYS.CONVEYOR.OUTPUT,
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
