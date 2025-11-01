using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DupePodRailgun.Buildings
{
    internal class DupeRailgunBulletConfig : IEntityConfig,IHasDlcRestrictions
    {
        public const string ID = "DPR_DupeRailGunPayload";
        public const float MASS = 300f;
        public const int LANDING_EDGE_PADDING = 3;
		public string[] GetAnyRequiredDlcIds()
		{
			return null;
		}

		public string[] GetDlcIds() => null;

        public GameObject CreatePrefab()
        {
            GameObject looseEntity =
                EntityTemplates.CreateLooseEntity(ID,
                (string)STRINGS.ITEMS.DPR_DUPERAILGUNPAYLOAD.NAME,
                (string)STRINGS.ITEMS.DPR_DUPERAILGUNPAYLOAD.DESC,
                MASS, true,
                Assets.GetAnim((HashedString)"railgun_capsule_kanim"),
                "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 1.5f, 2.5f,
                isPickupable: false, additionalTags: new List<Tag>()
                {
                    GameTags.IgnoreMaterialCategory,
                    GameTags.Experimental
                });
            looseEntity.AddOrGetDef<RailGunPayload.Def>().attractToBeacons = false;
            looseEntity.AddComponent<LoopingSounds>();

            looseEntity.AddOrGet<MinionStorage>();
            ClusterDestinationSelector destinationSelector = looseEntity.AddOrGet<ClusterDestinationSelector>();
            destinationSelector.assignable = false;
            destinationSelector.shouldPointTowardsPath = true;
            destinationSelector.requireAsteroidDestination = true;
            BallisticClusterGridEntity clusterGridEntity = looseEntity.AddOrGet<BallisticClusterGridEntity>();
            clusterGridEntity.clusterAnimName = "payload01_kanim";
            clusterGridEntity.isWorldEntity = true;
            clusterGridEntity.nameKey = new StringKey("STRINGS.ITEMS.RAILGUNPAYLOAD.NAME");
            looseEntity.AddOrGet<ClusterTraveler>();
            return looseEntity;
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
