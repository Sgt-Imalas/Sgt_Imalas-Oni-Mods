﻿using Imalas_TwitchChaosEvents.Elements;
using UnityEngine;
using static Imalas_TwitchChaosEvents.Elements.ELEMENTpatches;

namespace Imalas_TwitchChaosEvents.Creeper
{
	internal class ITCE_Liquid_Poop : IOreConfig
	{
		public SimHashes ElementID => ModElements.LiquidPoop.SimHash;

		public string[] GetDlcIds() =>	null;

		public GameObject CreatePrefab()
		{
			GameObject liquidOreEntity = EntityTemplates.CreateLiquidOreEntity(this.ElementID);
			Sublimates sublimates = liquidOreEntity.AddOrGet<Sublimates>();
			sublimates.spawnFXHash = Game_InitializeFXSpawners_Patch.ITCE_PoopyLiquidFX;// SpawnFXHashes.ContaminatedOxygenBubbleWater;
			sublimates.info = new Sublimates.Info(4E-05f, 0.025f, max_destination_mass: 1.8f, 1f, SimHashes.Methane);
			return liquidOreEntity;
		}
	}
}

