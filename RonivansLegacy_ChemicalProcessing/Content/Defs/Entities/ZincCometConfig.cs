using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RonivansLegacy_ChemicalProcessing.STRINGS;

namespace Dupes_Industrial_Overhaul.Chemical_Processing.Space
{
	class ZincCometConfig : IEntityConfig
	{
		public static readonly string ID = "ZincComet";
		public string[] GetDlcIds() => null;
		public GameObject CreatePrefab()
		{
			var gameObject = 
				BaseCometConfig.BaseComet(ID,
				UI.SPACEDESTINATIONS.COMETS.ZINCCOMET.NAME,
				"meteor_gold_kanim",
				ModElements.Aurichalcite_Solid,
				new Vector2(3f, 20f),
				new Vector2(323.15f, 423.15f),
				"Meteor_Medium_Impact", 1, 
				SimHashes.CarbonDioxide,
				SpawnFXHashes.MeteorImpactMetal, 0.6f);

			Comet comet = gameObject.AddOrGet<Comet>();
			comet.explosionOreCount = new Vector2I(2, 4);
			comet.entityDamage = 15;
			comet.totalTileDamage = 0.5f;
			comet.splashRadius = 1;

			return gameObject;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
