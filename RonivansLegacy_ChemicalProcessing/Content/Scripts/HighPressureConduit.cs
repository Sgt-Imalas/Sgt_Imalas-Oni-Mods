using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static FlowUtilityNetwork;
using static UnityEngine.GraphicsBuffer;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	public class HighPressureConduit : KMonoBehaviour
	{	
		[MyCmpReq]
		public BuildingComplete buildingComplete;
		[SerializeField]
		public bool InsulateSolidContents = false;
		[MyCmpGet]
		public SolidConduit solidConduit;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			// Register the conduit with the high pressure conduit system
			HighPressureConduitRegistration.RegisterHighPressureConduit(this);
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			SetInsulationOnSpawn();
		}

		void SetInsulationOnSpawn()
		{
			if (!InsulateSolidContents || solidConduit == null)
				return;
			var itemHandle = SolidConduit.GetFlowManager().GetContents(Grid.PosToCell(this));
			if (itemHandle.pickupableHandle.IsValid())
			{
				var item = SolidConduit.GetFlowManager().GetPickupable(itemHandle.pickupableHandle);
				HighPressureConduitRegistration.SetInsulatedState(item, true);
			}
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			HighPressureConduitRegistration.UnregisterHighPressureConduit(this);
		}
	}
}
