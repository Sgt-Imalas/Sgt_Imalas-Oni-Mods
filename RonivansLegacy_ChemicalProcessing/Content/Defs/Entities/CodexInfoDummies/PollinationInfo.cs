using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ResearchTypes;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Entities
{
	/// <summary>
	/// Dummy entity for pollination codex panels
	/// </summary>
	internal class PollinationInfo : IEntityConfig
	{
		public static readonly string ID = "AIO_CodexInfo_Pollination";
		public GameObject CreatePrefab()
		{
			GameObject go = EntityTemplates.CreateLooseEntity(ID, global::STRINGS.UI.FormatAsLink(global::STRINGS.CREATURES.STATUSITEMS.POLLINATING.INTERACTING.NAME, "POLLINATORS"), global::STRINGS.CREATURES.STATUSITEMS.POLLINATING.INTERACTING.TOOLTIP, 1f, true, Assets.GetAnim((HashedString)"kit_planttender_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, false);
			go.AddOrGet<CodexEntryRedirector>().CodexID = "POLLINATORS";
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
