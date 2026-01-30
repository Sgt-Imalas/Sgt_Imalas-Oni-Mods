using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	internal class CustomSodaFountain
	{
		public static readonly PortDisplayInput GasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(1, 1));
		public static void AttachPorts(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, GasInputPort);
		}
		public static void ConfigureBuilding(GameObject go)
		{
			///===> Soda Gas Input <==============================================================
			PortConduitConsumer GasConsumer = go.AddComponent<PortConduitConsumer>();
			GasConsumer.capacityTag = ModAssets.Tags.SodaFountainGas;
			GasConsumer.alwaysConsume = true;
			GasConsumer.capacityKG = 4;
			GasConsumer.AssignPort(GasInputPort);
			GasConsumer.SkipSetOperational = true;

			go.AddOrGet<ManualDeliveryKG>().RequestedItemTag = ModAssets.Tags.SodaFountainGas;
			go.GetComponent<SodaFountain>().ingredientTag = ModAssets.Tags.SodaFountainGas;
		}
	}
}
