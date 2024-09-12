using Imalas_TwitchChaosEvents.Elements;
using ONITwitchLib;
using System.Collections.Generic;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Creeper
{
	internal class ITCE_CreepyLiquidGas : IOreConfig
	{
		public SimHashes ElementID => ModElements.CreeperGas.SimHash;

		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

		public GameObject CreatePrefab()
		{
			GameObject gasOreEntity = EntityTemplates.CreateGasOreEntity(this.ElementID, new List<Tag>() { ExtraTags.OniTwitchSurpriseBoxForceDisabled });

			return gasOreEntity;
		}
	}
}
