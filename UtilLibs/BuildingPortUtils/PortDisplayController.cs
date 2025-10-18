using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UtilLibs.BuildingPortUtils.SharedConduitUtils;

namespace UtilLibs.BuildingPortUtils
{

	[SkipSaveFileSerialization]
	public class PortDisplayController : KMonoBehaviour
	{
		[SerializeField]
		private HashedString lastMode = OverlayModes.None.ID;

		[SerializeField]
		private List<PortDisplay2> gasOverlay = new List<PortDisplay2>();

		[SerializeField]
		private List<PortDisplay2> liquidOverlay = new List<PortDisplay2>();

		[SerializeField]
		private List<PortDisplay2> solidOverlay = new List<PortDisplay2>();

		static Dictionary<PortDisplay2, int> activePortCells = [];
		static Dictionary<int, ActivePortInfo> activePortInfo = [];
		static Dictionary<int, VanillaPortInfo> vanillaPortInfo = [];

		public struct VanillaPortInfo
		{
			public string portDesc;
			public Sprite sprite;
			public Color color;
			public VanillaPortInfo(string portDesc, Sprite sprite, Color color)
			{
				this.portDesc = portDesc;
				this.sprite = sprite;
				this.color = color;
			}
		}

		public struct ActivePortInfo
		{
			public string portDesc;
			public PortDisplay2 port;
			public int cell;
			public ActivePortInfo(int cell, string portDesc, PortDisplay2 port)
			{
				this.portDesc = portDesc;
				this.port = port;
				this.cell = cell;
			}
		}

		static int lastCell = -1;
		public static bool TryGetActivePortDesc(int utilityCell, out string portDesc, out Sprite sprite, out Color color)
		{
			portDesc = null;
			sprite = null;
			color = Color.white;

			if(utilityCell != lastCell)
			{
				lastCell = utilityCell;
				SgtLogger.l("Trying to get active port info for cell " + utilityCell);
			}

			if (activePortInfo.TryGetValue(utilityCell, out var portInfo))
			{
				portDesc = portInfo.portDesc;
				sprite = portInfo.port.sprite;
				color = portInfo.port.color;
				return true;
			}
			if(vanillaPortInfo.TryGetValue(utilityCell, out var vanillaInfo))
			{
				portDesc = vanillaInfo.portDesc;
				sprite = vanillaInfo.sprite;
				color = vanillaInfo.color;
				return true;
			}
			return false;
		}
		public void AssignPort(GameObject go, DisplayConduitPortInfo port)
		{
			PortDisplay2 portDisplay = go.AddComponent<PortDisplay2>();
			portDisplay.AssignPort(port);

			switch (port.type)
			{
				case ConduitType.Gas:
					this.gasOverlay.Add(portDisplay);
					break;
				case ConduitType.Liquid:
					this.liquidOverlay.Add(portDisplay);
					break;
				case ConduitType.Solid:
					this.solidOverlay.Add(portDisplay);
					break;
			}
		}

		public List<PortDisplay2> GetAllPorts()
		{
			return gasOverlay.Concat(liquidOverlay).Concat(solidOverlay).ToList();
		}

		public void Init(GameObject go)
		{
			string ID = go.GetComponent<KPrefabID>().PrefabTag.Name;

			// criteria for drawing port icons on buildings
			// vanilla will only attempt to draw icons on buildings with BuildingCellVisualizer
			go.AddOrGet<BuildingCellVisualizer>();

			// when vanilla tries to draw, call this controller if the building is in the DrawPorts list
			ConduitDisplayPortPatching.AddBuilding(ID);
		}


		public void Draw(BuildingCellVisualizer __instance, HashedString mode, GameObject go)
		{
			bool isNewMode = mode != this.lastMode;

			if (isNewMode)
			{
				this.ClearPorts();
				this.lastMode = mode;
			}

			foreach (PortDisplay2 port in this.GetPorts(mode))
			{
				int utilityCell = port.GetUtilityCell(__instance.building);

				activePortInfo[utilityCell] = new(utilityCell, GetPortFilterDesc(port), port);
				activePortCells[port] = utilityCell;

				port.Draw(go, __instance, isNewMode);
			}
		}


		private string GetPortFilterDesc(PortDisplay2 port)
		{
			if (port == null)
				return null;


			if (port.input)
			{
				foreach (var portConsumer in GetComponents<PortConduitConsumer>())
				{

					if (portConsumer.conduitType == port.type
					&& portConsumer.conduitOffset == port.offset
					&& portConsumer.conduitOffsetFlipped == port.offsetFlipped)
					{
						return GetFilteredPortTooltip(port.type, port.input, [portConsumer.capacityTag]);
					}
				}
			}
			else
			{
				foreach (var portDispenser in GetComponents<PortConduitDispenserBase>())
				{

					if (portDispenser.conduitType == port.type
					&& portDispenser.conduitOffset == port.offset
					&& portDispenser.conduitOffsetFlipped == port.offsetFlipped)
					{
						return GetFilteredPortTooltip(port.type, port.input, portDispenser.tagFilter, portDispenser.elementFilter, portDispenser.invertElementFilter);
					}
				}
			}
			return null;
		}


		private void ClearPorts()
		{
			foreach (PortDisplay2 port in this.GetPorts(this.lastMode))
			{
				port.DisableIcons();
				activePortInfo.Remove(activePortCells[port]);
				activePortCells.Remove(port);
			}
		}

		private List<PortDisplay2> GetPorts(HashedString mode)
		{
			if (mode == OverlayModes.GasConduits.ID) return this.gasOverlay;
			if (mode == OverlayModes.LiquidConduits.ID) return this.liquidOverlay;
			if (mode == OverlayModes.SolidConveyor.ID) return this.solidOverlay;

			return new List<PortDisplay2>();
		}

		static HashSet<EntityCellVisualizer.Ports> ValidPortTypes = new()
		{
			EntityCellVisualizer.Ports.GasIn,
			EntityCellVisualizer.Ports.GasOut,
			EntityCellVisualizer.Ports.LiquidIn,
			EntityCellVisualizer.Ports.LiquidOut,
			EntityCellVisualizer.Ports.SolidIn,
			EntityCellVisualizer.Ports.SolidOut
		};

		public static bool? VanillaPortsHandled = null;
		static readonly string PLib_Registry_VanillaPorts = "UtilLibs_BuildingPortUtils_VanillaPortsHandled";
		//static bool PortValidForMode(EntityCellVisualizer.PortEntry port, HashedString mode)
		//{
		//	return (mode == OverlayModes.GasConduits.ID && (port.type == EntityCellVisualizer.Ports.GasIn || port.type == EntityCellVisualizer.Ports.GasOut))
		//		|| (mode == OverlayModes.LiquidConduits.ID && (port.type == EntityCellVisualizer.Ports.LiquidIn || port.type == EntityCellVisualizer.Ports.LiquidOut))
		//		|| (mode == OverlayModes.SolidConveyor.ID && (port.type == EntityCellVisualizer.Ports.SolidIn || port.type == EntityCellVisualizer.Ports.SolidOut));
		//}
		internal static void HandleVanillaPortInfo(BuildingCellVisualizer instance, HashedString mode)
		{
			//only fetch and draw them from one mod
			if (VanillaPortsHandled == null)
			{
				bool alreadyHandled = PRegistry.GetData<bool>(PLib_Registry_VanillaPorts);
				if (!alreadyHandled)
				{
					VanillaPortsHandled = false;
					PRegistry.PutData(PLib_Registry_VanillaPorts, true);
				}
				else
				{
					VanillaPortsHandled = true;
				}
			}
			if (VanillaPortsHandled.Value)
				return;

			foreach (var port in instance.ports)
			{
				//bool validForMode = PortValidForMode(port, mode);
				///electic, logic ports need to be skipped to not remove ports from the dict. if in the same place
				if (!ValidPortTypes.Contains(port.type))
					continue;

				int cell = instance.ComputeCell(port.cellOffset);
				//port disabled
				if (port.visualizer == null || port.visualizer != null && !port.visualizer.activeInHierarchy)
				{
					vanillaPortInfo.Remove(cell);
				}
				//port enabled
				else if (port.visualizer != null)
				{
					Sprite portSprite = Assets.GetSprite("unknown");
					var color = port.connectedTint;
					bool input = false;
					ConduitType conduitType = ConduitType.None;

					switch (port.type)
					{
						case EntityCellVisualizer.Ports.GasIn:
							conduitType = ConduitType.Gas;
							input = true;
							break;
						case EntityCellVisualizer.Ports.GasOut:
							conduitType = ConduitType.Gas;
							break;
						case EntityCellVisualizer.Ports.LiquidIn:
							conduitType = ConduitType.Liquid;
							input = true;
							break;
						case EntityCellVisualizer.Ports.LiquidOut:
							conduitType = ConduitType.Liquid;
							break;
						case EntityCellVisualizer.Ports.SolidIn:
							input = true;
							conduitType = ConduitType.Solid;
							break;
						case EntityCellVisualizer.Ports.SolidOut:
							conduitType = ConduitType.Solid;
							break;
					}
					portSprite = GetSprite(input, conduitType);

					if (input)
					{
						if (conduitType != ConduitType.Solid)
						{
							foreach (var consumer in instance.GetComponents<ConduitConsumer>())
							{
								if (consumer.conduitType == conduitType
								&& consumer.GetInputCell(consumer.GetConduitManager().conduitType) == cell)
								{
									vanillaPortInfo[cell] = new(GetFilteredPortTooltip(conduitType, input, [consumer.capacityTag]), portSprite, color);
									break;
								}
							}

						}
						else
						{
							foreach (var consumer in instance.GetComponents<SolidConduitConsumer>())
							{
								if (consumer.GetInputCell() == cell)
								{
									vanillaPortInfo[cell] = new(GetFilteredPortTooltip(conduitType, input, [consumer.capacityTag]), portSprite, color);
									break;
								}
							}
						}
					}
					else
					{
						if (conduitType != ConduitType.Solid)
						{
							foreach (var dispenser in instance.GetComponents<ConduitDispenser>())
							{
								if (dispenser.conduitType == conduitType
								&& dispenser.GetOutputCell(dispenser.conduitType) == cell)
								{
									vanillaPortInfo[cell] = new(GetFilteredPortTooltip(conduitType, input, null, dispenser.elementFilter, dispenser.invertElementFilter), portSprite, color);
									break;
								}
							}
						}
						else
						{
							foreach (var dispenser in instance.GetComponents<SolidConduitDispenser>())
							{
								if (dispenser.GetOutputCell() == cell)
								{
									vanillaPortInfo[cell] = new(GetFilteredPortTooltip(conduitType, input, null,dispenser.elementFilter, dispenser.invertElementFilter), portSprite, color);
									break;
								}
							}
						}
					}
				}
			}
		}
	}
}
