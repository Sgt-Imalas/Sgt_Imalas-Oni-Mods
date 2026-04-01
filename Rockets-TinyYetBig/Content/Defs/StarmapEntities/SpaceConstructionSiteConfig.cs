using Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Rockets_TinyYetBig.STRINGS.UI_MOD.UISIDESCREENS;

namespace Rockets_TinyYetBig.Content.Defs.StarmapEntities
{
	internal class SpaceConstructionSiteConfig : IEntityConfig, IHasDlcRestrictions
	{
		public const string ID = "RTB_StationConstructionSite";
		public string[] GetAnyRequiredDlcIds()
		{
			return null;
		}

		public string[] GetDlcIds() => null;

		public GameObject CreatePrefab()
		{
			var enity = EntityTemplates.CreateEntity(
				   id: ID,
				   name: "Space Station construction site",
				   true);
			enity.AddOrGet<CharacterOverlay>().shouldShowName = true;


			enity.AddOrGet<Storage>();
			enity.AddOrGet<SpaceConstructionSite>();
			var site = enity.AddOrGet<SpaceConstructable>();
			//site.buildPartStorage = enity.AddComponent<Storage>();
			//site.AssignProject(ConstructionProjects.SpaceStationInit);


			return enity;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}

		public string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];

		public string[] GetForbiddenDlcIds() => null;
	}
}
