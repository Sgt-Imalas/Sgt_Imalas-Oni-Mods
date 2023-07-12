using Database;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static STRINGS.UI.CLUSTERMAP;
using static UnityEngine.UI.Image;

namespace Imalas_TwitchChaosEvents.Meteors
{
    internal class VariableAmountMeteor: GassyMooComet
    {
        [Serialize] public Vector2 amountRange = new Vector2(1f, -1f);
        [Serialize] public Vector3 spawnOffset = new Vector2(0,0);
        [Serialize] public bool IgnoredByBlaster = true;

        public override void OnSpawn()
        {
            base.OnSpawn();
            if(IgnoredByBlaster)
                Components.Meteors.Remove(this.gameObject.GetMyWorldId(), this);
        }
        public override void SpawnCraterPrefabs()
        {
            KBatchedAnimController animController = GetComponent<KBatchedAnimController>();
            animController.Play("landing");
            animController.onAnimComplete += delegate
            {
                if (craterPrefabs != null && craterPrefabs.Length != 0)
                {
                    int cell = Grid.PosToCell(this);
                    if (Grid.IsValidCell(Grid.CellAbove(cell)))
                    {
                        cell = Grid.CellAbove(cell);
                    }

                    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(craterPrefabs[UnityEngine.Random.Range(0, craterPrefabs.Length)]), Grid.CellToPos(cell));
                    gameObject.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(base.gameObject), Grid.SceneLayer.Ore));
                    //gameObject.transform.position += mooSpawnImpactOffset;
                    gameObject.transform.position += spawnOffset;
                    gameObject.GetComponent<KBatchedAnimController>().FlipX = animController.FlipX; 
                    
                    if(gameObject.TryGetComponent<PrimaryElement>(out var primaryElement))
                    {
                        primaryElement.Temperature = UnityEngine.Random.Range(temperatureRange.x,temperatureRange.y+1);
                        float amount = amountRange.y == -1f ? 1f : UnityEngine.Random.Range(amountRange.x, amountRange.y);
                        primaryElement.Units = amount;
                    }
                    gameObject.SetActive(value: true);
                    //YeetTheDrop(gameObject);
                }

                Util.KDestroyGameObject(base.gameObject);
            };
        }
        void YeetTheDrop(GameObject drop)
        {
            Vector2 normalized = Vector2.zero;
            normalized += new Vector2(0f, 0.55f);
            normalized *= 0.5f * UnityEngine.Random.Range(explosionSpeedRange.x, explosionSpeedRange.y);
            if (GameComps.Fallers.Has(drop))
            {
                GameComps.Fallers.Remove(drop);
            }

            if (GameComps.Gravities.Has(drop))
            {
                GameComps.Gravities.Remove(drop);
            }

            GameComps.Fallers.Add(drop, normalized);
        }


        public override void RandomizeVelocity()
        {
            float num = UnityEngine.Random.Range(spawnAngle.x, spawnAngle.y);
            float f = num * (float)Math.PI / 180f;
            float num2 = UnityEngine.Random.Range(spawnVelocity.x, spawnVelocity.y);
            velocity = new Vector2((0f - Mathf.Cos(f)) * num2, Mathf.Sin(f) * num2);
            GetComponent<KBatchedAnimController>().Rotation = 0f - num - 90f;
        }
    }
}
