using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ComplexRecipe;
using static Grid;
using static STRINGS.WORLD_TRAITS;

namespace RoboRockets.LearningBrain
{
    internal class DemolishableDroppable: Demolishable
    {
        public override void OnCompleteWork(Worker worker) => this.DropOnDestroy();

        private void DropOnDestroy()
        {
            if (this.IsNullOrDestroyed())
                return;

            Debug.Log("DROPPIN");
            Debug.Log(gameObject.transform.position + " possss");
            SpawnItemForRecipes(BrainConfig.ProductionCosts);
            Debug.Log("Done");
            UnityEngine.Object.Destroy(this.gameObject);
            Debug.Log("wat");

        }

        public void SpawnItemForRecipes(RecipeElement[] materials)
        {
            foreach (var material in materials)
            {
                Debug.Log("mat" + material.material + ", w: " + material.amount);
                SpawnItem(
                    gameObject.transform.position,
                    material.material,
                    material.amount,
                    UtilMethods.GetKelvinFromC(20f));
            }
        }
        public GameObject SpawnItem(
            Vector3 position,
            Tag src_element,
            float src_mass,
            float src_temperature 
            )
        {
            GameObject gameObject = (GameObject)null;
            int cell1 = Grid.PosToCell(position);
            Element element = ElementLoader.GetElement(src_element);
            if (element != null)
            {
                float num = src_mass;
                for (int index1 = 0; (double)index1 < (double)src_mass / 400.0; ++index1)
                {
                    float mass = num;
                    if ((double)num > 400.0)
                    {
                        mass = 400f;
                        num -= 400f;
                    }
                    gameObject = element.substance.SpawnResource(Grid.CellToPosCBC(cell1, Grid.SceneLayer.Ore), mass, src_temperature, byte.MaxValue, 0);
                }
            }
            else
            {
                for (int index3 = 0; (double)index3 < (double)src_mass; ++index3)
                {
                    gameObject = GameUtil.KInstantiate(Assets.GetPrefab(src_element), Grid.CellToPosCBC(cell1, Grid.SceneLayer.Ore), Grid.SceneLayer.Ore);
                    gameObject.SetActive(true);
                }
            }
            return gameObject;
        }


    }
}
