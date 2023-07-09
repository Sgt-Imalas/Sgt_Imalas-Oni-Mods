using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.SPACEDESTINATIONS.COMETS;

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
            gassyMooComet.impactSound = "Meteor_dust_light_Impact";
            gassyMooComet.flyingSoundID = 0;
            gassyMooComet.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
            gassyMooComet.addTiles = 0;
            gassyMooComet.affectedByDifficulty = false;
            gassyMooComet.destroyOnExplode = false;
            gassyMooComet.craterPrefabs = new string[1] { TacoConfig.ID };
            gassyMooComet.amountRange = new Vector2(0.015f, 0.1f);
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
            entity.AddOrGet<KCircleCollider2D>().radius = 0.9f;
            entity.AddTag(GameTags.Comet);
            return entity;
        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public void OnPrefabInit(GameObject inst) { }

        public void OnSpawn(GameObject inst) 
        {
            SgtLogger.l($"spawned Taco Meteor at {inst.transform.position.x},{inst.transform.position.y}");
        
        }
    }
}
