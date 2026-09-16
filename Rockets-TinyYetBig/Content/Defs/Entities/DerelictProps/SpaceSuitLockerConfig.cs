using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Entities.StationProps
{
	internal class SpaceSuitLockerConfig : IEntityConfig
	{
		public const string ID = "RTB_DerelictSuitLocker";
		public string[] GetDlcIds() => null;
		public GameObject CreatePrefab()
		{
			string name = global::STRINGS.BUILDINGS.PREFABS.PROPEXOSETLOCKER.NAME;
			string desc = global::STRINGS.BUILDINGS.PREFABS.PROPEXOSETLOCKER.DESC;
			EffectorValues tieR0_1 = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
			EffectorValues tieR0_2 = NOISE_POLLUTION.NOISY.TIER0;
			KAnimFile anim = Assets.GetAnim("zipper_locker_kanim");
			EffectorValues decor = tieR0_1;
			EffectorValues noise = tieR0_2;
			GameObject placedEntity = EntityTemplates.CreatePlacedEntity(ID, name, desc, 100f, anim, "on", Grid.SceneLayer.Building, 1, 2, decor, noise, additionalTags: new List<Tag>() { GameTags.Gravitas });
			PrimaryElement component = placedEntity.GetComponent<PrimaryElement>();
			component.SetElement(SimHashes.Unobtanium);
			component.Temperature = 294.15f;
			Workable workable = placedEntity.AddOrGet<Workable>();
			workable.synchronizeAnims = false;
			workable.resetProgressOnStop = true;
			SetLocker setLocker = placedEntity.AddOrGet<SetLocker>();
			setLocker.overrideAnim = "anim_interacts_zipper_locker_kanim";
			setLocker.dropOffset = new Vector2I(0, 1);
			setLocker.numDataBanks = [1, 4];
			LoreBearerUtil.AddLoreTo(placedEntity);
			placedEntity.AddOrGet<OccupyArea>().objectLayers =
			[
	  ObjectLayer.Building
			];
			placedEntity.AddOrGet<Demolishable>();
			return placedEntity;
		}

		public void OnPrefabInit(GameObject inst)
		{
			SetLocker component = inst.GetComponent<SetLocker>();
			component.possible_contents_ids =
			[[AtmoSuitConfig.ID],[JetSuitConfig.ID]];
			component.ChooseContents();
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}

}
