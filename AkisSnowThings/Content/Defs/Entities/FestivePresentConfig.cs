using AkisSnowThings.Content.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static AkisSnowThings.STRINGS.UI;
using static STRINGS.UI.SPACEARTIFACTS;

namespace AkisSnowThings.Content.Defs.Entities
{
	internal class FestivePresentConfig : IEntityConfig
	{
		public const string ID = "SnowSculptures_Present";
		private const string Anim = "festive_present_kanim";
		public static EffectorValues DECORBONUS = new EffectorValues(5, 2);

		public GameObject CreatePrefab()
		{
			var go = EntityTemplates.CreateLooseEntity(
				ID,
				STRINGS.ENTITIES.PREFABS.SNOWSCULPTURES_FESTIVEPRESENT.NAME,
				STRINGS.ENTITIES.PREFABS.SNOWSCULPTURES_FESTIVEPRESENT.DESC,
				1f,
				true,
				Assets.GetAnim(Anim),
				"closed",
				// Want it to be below ore, but in front of the pod or nearby buildings.
				Grid.SceneLayer.BuildingFront,
				EntityTemplates.CollisionShape.RECTANGLE,
				1f,
				1.1f,
				true
			);

			var openPresentWorkable = go.AddOrGet<OpenFestivePresentWorkable>();
			openPresentWorkable.SetWorkTime( 5f);
			openPresentWorkable.ConfigureMultitoolContext((HashedString)"build", (Tag)EffectConfigs.BuildSplashId);

			var ownable = go.AddOrGet<Ownable>();
			ownable.tintWhenUnassigned = false;
			ownable.slotID = ModAssets.PresentSlotId;
			go.AddOrGet<Prioritizable>();

			go.AddOrGet<OccupyArea>();
			DecorProvider decorProvider = go.AddOrGet<DecorProvider>();
			decorProvider.SetValues(DECORBONUS);
			return go;
		}

		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

		public void OnPrefabInit(GameObject inst)
		{
			
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}