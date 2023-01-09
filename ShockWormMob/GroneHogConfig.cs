using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace ShockWormMob
{
    internal class GroneHogConfig : IEntityConfig
    {
        public const string ID = "GroneHog";
        public const string BASE_TRAIT_ID = "GroneHogBaseTrait";
        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public GameObject CreatePrefab()
        {
            string name1 = (string)"Volgus";
            string name2 = name1;
            string desc = (string)"Volgussy kinda sussy";
            EffectorValues tieR0 = TUNING.DECOR.BONUS.TIER0;
            KAnimFile anim = Assets.GetAnim((HashedString)"gronehog_kanim");
            EffectorValues decor = tieR0;
            EffectorValues noise = new EffectorValues();
            GameObject placedEntity = EntityTemplates.CreatePlacedEntity(ID, name2, desc, 25f, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise);
            Db.Get().CreateTrait(BASE_TRAIT_ID, name1, name1, (string)null, false, (ChoreGroup[])null, true, true).Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name1));
            //KPrefabID component = placedEntity.GetComponent<KPrefabID>();
            //component.AddTag(GameTags.Creatures.Walker);
            //component.AddTag(GameTags.OriginalCreature);
            placedEntity.AddOrGet<LoopingSounds>();
            placedEntity.GetComponent<LoopingSounds>().updatePosition = true;
            placedEntity.AddComponent<GroneHog>();
            return placedEntity;
        }


        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
