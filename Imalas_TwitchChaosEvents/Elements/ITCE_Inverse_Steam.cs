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
