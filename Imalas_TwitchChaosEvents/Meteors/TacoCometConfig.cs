using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Meteors
{
	internal class TacoCometConfig : IEntityConfig
	{
		public const string ID = "ITC_TacoComet";
		public GameObject CreatePrefab()
		{
			GameObject entity = EntityTemplates.CreateEntity(ID, STRINGS.COMETS.ITC_TACOCOMET.NAME);
			entity.AddOrGet<SaveLoadRoot>();
			entity.AddOrGet<LoopingSounds>();
			VariableAmountMeteor gassyMooComet = entity.AddOrGet<VariableAmountMeteor>();
			gassyMooComet.massRange = new Vector2(0.3f, 0.8f);
			gassyMooComet.EXHAUST_ELEMENT = SimHashes.Void;
			gassyMooComet.temperatureRange = new Vector2(UtilMethods.GetKelvinFromC(45f), UtilMethods.GetKelvinFromC(90f));
			gassyMooComet.entityDamage = 0;
			gassyMooComet.explosionOreCount = new Vector2I(0, 0);
			gassyMooComet.totalTileDamage = 0.0f;
			gassyMooComet.splashRadius = 1;
			//gassyMooComet.impactSound = "Meteor_GassyMoo_Impact";
			gassyMooComet.flyingSoundID = 0;
			gassyMooComet.explosionEffectHash = SpawnFXHashes.MeteorImpactSlime;
			gassyMooComet.addTiles = 0;
			gassyMooComet.affectedByDifficulty = false;
			gassyMooComet.destroyOnExplode = false;
			gassyMooComet.craterPrefabs = new string[1] { TacoNotSpoilingConfig.ID };
			gassyMooComet.amountRange = new Vector2(0.015f, 0.1f);
			gassyMooComet.mooSpawnImpactOffset = Vector2.zero;
			//gassyMooComet.spawnOffset = new Vector2(0.0f, 0.38f);
			//gassyMooComet.spawnWithOffset = true;
			//gassyMooComet.offsetPosition = new Vector3(0.5f, 0.4f, 0f);   
			PrimaryElement primaryElement = entity.AddOrGet<PrimaryElement>();
			primaryElement.SetElement(SimHashes.Creature);
			primaryElement.Temperature = (float)(((double)gassyMooComet.temperatureRange.x + (double)gassyMooComet.temperatureRange.y) / 2.0);
			KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
			kbatchedAnimController.AnimFiles = new KAnimFile[1]
			{
				Assets.GetAnim((HashedString) "meteor_taco_kanim")
			};
			kbatchedAnimController.isMovable = true;
			kbatchedAnimController.initialAnim = "fall_loop";
			kbatchedAnimController.initialMode = KAnim.PlayMode.Loop;
			kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
			entity.AddOrGet<KCircleCollider2D>().radius = 1.1f;
			entity.AddTag(GameTags.Comet);
			return entity;
		}

		public string[] GetDlcIds() => null;

		public void OnPrefabInit(GameObject inst) { }

		public void OnSpawn(GameObject inst)
		{

		}
	}
}
