using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.Entities
{
    internal class BonesConfig : IEntityConfig
    {
        public static string ID = "OniRetro_MinionBones";
        public static ComplexRecipe recipe;

        public GameObject CreatePrefab()
        {
            return EntityTemplates.CreateLooseEntity(ID, STRINGS.ITEMS.RETROONI_BONES.NAME, STRINGS.ITEMS.RETROONI_BONES.DESC, 10f, true, Assets.GetAnim((HashedString)"retro_bones_kanim"), "working_pre", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.8f, true);

        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
            var kbac = inst.GetComponent<KBatchedAnimController>();
            kbac.Offset = new Vector3(0, -0.4f);

            kbac.Play("working_pre");
            kbac.Queue("working_loop");
            kbac.Queue("working_pst");
        }
    }
}
