using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Imalas_TwitchChaosEvents.Elements.ELEMENTpatches;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Elements
{
	namespace Imalas_TwitchChaosEvents.Elements
	{
		class ITCE_Inverse_Steam : IOreConfig
		{
			public SimHashes ElementID => ModElements.InverseSteam.SimHash;

			public string[] GetDlcIds() => null;

			public GameObject CreatePrefab()
			{
				GameObject gasOreEntity = EntityTemplates.CreateGasOreEntity(this.ElementID);
				gasOreEntity.AddOrGet<BottleFlipper>();
				return gasOreEntity;
			}
		}
	}
}
