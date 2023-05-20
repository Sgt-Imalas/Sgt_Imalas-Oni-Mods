using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static BuildingToken.STRINGS;

namespace BuildingToken.Tokens
{
    internal class BuildingTokenConfig : IMultiEntityConfig
    {
        public List<GameObject> CreatePrefabs()
        {
            var list = new List<GameObject>();
            foreach (var buildingTokenID in ModAssets.BuildingTokenTags)
            {
                list.Add(CreateToken(buildingTokenID.Key,buildingTokenID.Value));
            }
            return list;
        }

        public static GameObject CreateToken(
            string referencedBuilding,
            Tag tag
            )
        {
            string id = "buildingToken_" + tag.ToString().ToLower();
            GameObject looseEntity =
                EntityTemplates.CreateLooseEntity
                (id,
                tag.ProperName(),
                string.Format(BT_BUILDINGTOKEN.DESC, tag.ProperName().Replace("-Token","")),
                1f,
                true,
                Assets.GetAnim((HashedString)"building_coin_kanim"),
                "object",
                Grid.SceneLayer.Ore,
                EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.6f,
                height: 0.6f,
                isPickupable: true,
                sortOrder: SORTORDER.BUILDINGELEMENTS,
                element: SimHashes.Creature,
                additionalTags: new List<Tag>()
                  {
                    tag,                   
                    GameTags.IndustrialIngredient
                  });

            looseEntity.AddOrGet<EntitySplitter>().maxStackSize = 50f;
            looseEntity.GetComponent<KPrefabID>().AddTag(GameTags.PedestalDisplayable);
            var SeededRandom = new SeededRandom(tag.hash);
            looseEntity.AddOrGet<coinPainter>().Tint = Color.HSVToRGB(SeededRandom.RandomRange(0f, 1f), SeededRandom.RandomRange(0.4f, 1f), SeededRandom.RandomRange(0.6f, 1f));

            ModAssets.BuildingTokens[referencedBuilding] = id;

            return looseEntity;
        }

        public void OnPrefabInit(GameObject inst)
        {

        }

        public void OnSpawn(GameObject inst)
        {

        }
    }
}
