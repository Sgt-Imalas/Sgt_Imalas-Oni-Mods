using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
	internal class SpaceConstructionSiteConfig : IEntityConfig, IHasDlcRestrictions
	{
		public const string ID = "RTB_StationConstructionSite";

		public string[] GetDlcIds() =>null;

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
			site.buildPartStorage = enity.AddComponent<Storage>();
			site.AssignProject(ConstructionProjects.SpaceStationInit);


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
