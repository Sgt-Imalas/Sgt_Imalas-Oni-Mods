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

		public override void OnSpawn()
		{
			base.OnSpawn();
			HighPressureConduitRegistration.RegisterHighPressureConduit(this);
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			HighPressureConduitRegistration.UnregisterHighPressureConduit(this);
		}
	}
}
