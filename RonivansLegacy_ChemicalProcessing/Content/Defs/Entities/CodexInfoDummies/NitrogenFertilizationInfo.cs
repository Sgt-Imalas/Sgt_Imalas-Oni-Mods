using Klei.AI;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ResearchTypes;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Entities
{
	/// <summary>
	/// Dummy entity for atmospheric fertilization codex panels
	/// </summary>
	internal class NitrogenFertilizationInfo : IEntityConfig
	{
		public static readonly string ID = "AIO_CodexInfo_NitrogenFertilizationInfo";
		public GameObject CreatePrefab()
		{
			GameObject go = EntityTemplates.CreateLooseEntity(ID, global::STRINGS.UI.FormatAsLink(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.NAME, ModElements.Nitrogen_Gas.Tag.ToString()), string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.TOOLTIP, GameUtil.GetFormattedMass(PlantNitrogenConsumer.NitrogenConsumedPerSecond, GameUtil.TimeSlice.PerSecond)), 1f, true, Assets.GetAnim((HashedString)"kit_planttender_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, false);
			go.AddOrGet<CodexEntryRedirector>().CodexID = ModElements.Nitrogen_Gas.Tag.ToString();

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
