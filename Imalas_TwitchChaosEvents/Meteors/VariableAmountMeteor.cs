using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Meteors
{
    internal class VariableAmountMeteor: GassyMooComet
    {
        public Vector2 amountRange = new Vector2(1f, -1f);
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
                    gameObject.transform.position = new Vector3(base.gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
                    gameObject.transform.position += mooSpawnImpactOffset;
                    gameObject.GetComponent<KBatchedAnimController>().FlipX = animController.FlipX; 
                    
                    if(gameObject.TryGetComponent<PrimaryElement>(out var primaryElement))
                    {
                        primaryElement.Temperature = UnityEngine.Random.Range(temperatureRange.x,temperatureRange.y+1);
                        float amount = amountRange.y == -1f ? 1f : UnityEngine.Random.Range(amountRange.x, amountRange.y);
                        primaryElement.Units = amount;
                    }
                    gameObject.SetActive(value: true);
                }

                Util.KDestroyGameObject(base.gameObject);
            };
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
