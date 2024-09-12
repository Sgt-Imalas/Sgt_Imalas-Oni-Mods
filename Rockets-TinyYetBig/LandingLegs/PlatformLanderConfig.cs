using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.LandingLegs
{
	public class PlatformLanderConfig : IEntityConfig
	{

		public const string ID = "RTB_LanderRocketPlatform";
		public const string PREVIEW_ID = "RTB_LanderRocketPlatformScoutLander_Preview";
		public const float MASS = 750f;

		public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
		public GameObject CreatePrefab()
		{
			string name = (string)STRINGS.BUILDINGS.PREFABS.RTB_LANDERROCKETPLATFORM.NAME;
			string desc = (string)STRINGS.BUILDINGS.PREFABS.RTB_LANDERROCKETPLATFORM.DESC;
			EffectorValues tieR0_1 = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
			EffectorValues tieR0_2 = NOISE_POLLUTION.NOISY.TIER0;
			KAnimFile anim = Assets.GetAnim((HashedString)"rocket_scout_cargo_lander_kanim");
			EffectorValues decor = tieR0_1;
			EffectorValues noise = tieR0_2;
			GameObject placedEntity = EntityTemplates.CreatePlacedEntity(ID, name, desc, 400f, anim, "grounded", Grid.SceneLayer.Building, 7, 3, decor, noise);
			var thruster = placedEntity.AddOrGet<ThrusterPoweredLander>();

			thruster.previewTag = "ScoutLander_Preview".ToTag();
			thruster.cmpsToEnable = new List<string>()
			{
				nameof(RTB_LaunchPadWithoutLogic)
				,nameof(EmptyLaunchPadConditions)
				,nameof(OccupyArea)
			};

			placedEntity.AddOrGet<Prioritizable>();
			Prioritizable.AddRef(placedEntity);
			placedEntity.AddOrGet<Operational>();

			placedEntity.AddOrGet<Deconstructable>().audioSize = "large";
			placedEntity.AddOrGet<Storable>();
			Placeable placeable = placedEntity.AddOrGet<Placeable>();
			placeable.kAnimName = anim.name;
			placeable.animName = "place";
			placeable.placementRules = new List<Placeable.PlacementRules>()
			{
				Placeable.PlacementRules.OnFoundation,
				Placeable.PlacementRules.VisibleToSpace,
				Placeable.PlacementRules.RestrictToWorld
			};
			placeable.checkRootCellOnly = true;
			EntityTemplates.CreateAndRegisterPreview("ScoutLander_Preview", anim, "place", ObjectLayer.Building, 7, 3);
			return placedEntity;
		}


		public void OnPrefabInit(GameObject inst)
		{
			OccupyArea component = inst.GetComponent<OccupyArea>();
			component.ApplyToCells = true;
			component.objectLayers = new ObjectLayer[1]
			{
				ObjectLayer.Building
			};
			component.enabled = false;
			inst.AddOrGet<EmptyLaunchPadConditions>().enabled = false;

			var launchpad = inst.AddComponent<RTB_LaunchPadWithoutLogic>();
			launchpad.enabled = false;
			launchpad.baseModulePosition = new CellOffset(0, 3);
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
