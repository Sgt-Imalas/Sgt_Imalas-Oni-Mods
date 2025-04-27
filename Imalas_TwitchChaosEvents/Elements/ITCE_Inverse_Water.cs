using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Imalas_TwitchChaosEvents.Elements.ELEMENTpatches;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Elements
{
	class ITCE_Inverse_Water : IOreConfig
	{
		public SimHashes ElementID => ModElements.InverseWater.SimHash;

		public string[] GetDlcIds() => null;

		public GameObject CreatePrefab()
		{
			GameObject liquidOreEntity = EntityTemplates.CreateLiquidOreEntity(this.ElementID);
			liquidOreEntity.AddOrGet<BottleFlipper>();
			return liquidOreEntity;
		}
	}
}
