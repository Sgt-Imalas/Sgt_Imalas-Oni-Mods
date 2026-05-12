using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ResearchTypes;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Entities.CodexInfoDummies
{
	/// <summary>
	/// Dummy entity for fertilization codex panels
	/// </summary>
	internal class FertilizationInfo : IEntityConfig
	{
		public static readonly string ID = "AIO_CodexInfo_Fertilization";
		public GameObject CreatePrefab()
		{
			GameObject go = EntityTemplates.CreateLooseEntity(ID, global::STRINGS.UI.FormatAsLink(global::STRINGS.CREATURES.STATS.FERTILIZATION.NAME, FarmStationConfig.ID), global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.FARM_STATION_TOOLS.DESC, 1f, true, Assets.GetAnim((HashedString)"planttender_kanim"), "off", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, false);
			go.AddOrGet<CodexEntryRedirector>().CodexID = FarmStationConfig.ID; 
			return go;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
