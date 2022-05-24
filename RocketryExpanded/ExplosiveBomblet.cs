using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RocketryExpanded
{
    public class ExplosiveBomblet : KMonoBehaviour
    {
        public int radius = -1;
        public float dmg = 5f;

        public float windowDamageMultiplier = 5f;
        public float entityDmgMultiplier = 100f;
        public float bunkerDamageMultiplier = 0f;
        public bool hasExhaustGas = true;
        public SimHashes exhaustElement = SimHashes.Fallout;
        public bool isRadioactive = true;
        byte diseaseIdx = Db.Get().Diseases.GetIndex((HashedString)Db.Get().Diseases.RadiationPoisoning.Id);
        public float smokeTemp = 600f;
        public float smokeMass = -1f;
        private float KJAtExplosion = 40000f;
        private List<Vector2I> bunkerTilesHit = new List<Vector2I>();

        protected override void OnSpawn()
        {
            base.OnSpawn();
        }


        public void Explode()
        {
            radius = radius == -1 ? GetDmgRadiusFromVal(dmg) : radius;
            smokeMass = smokeMass == -1f ? (float)(radius * 3.1416 * 2): smokeMass;
            var area = ProcGen.Util.GetCircle(transform.position, radius);

            var center = Grid.PosToXY(transform.position);

            //Debug.Log("Pos: " + this.gameObject.transform.position + ", Center: "+center.X+" X," +  center.Y + " Y");

            var areaNotProtected = GetExplosionArea(area,center);

            foreach(var cell in areaNotProtected)
            {
                DoDamage(cell, center);
            }
            foreach (var bnk in bunkerTilesHit)
            {
                   Debug.Log(bnk);
                AddHeatFromExplosion(bnk, center);
            }
            ReleaseExhaustGas(center);
            Util.KDestroyGameObject(this.gameObject);
        }
        public void AddHeatFromExplosion(Vector2I cell, Vector2I center)
        {
            float percentile = GetDmgValAtPos2(center, cell, dmg) / dmg;
            float energy = percentile;
            if (Grid.Element[Grid.PosToCell(cell)].IsVacuum)
            {
                return;
            }
            if (Grid.Element[Grid.PosToCell(cell)].IsLiquid)
            {
                energy = energy * 50 * KJAtExplosion;
            }
            else if (Grid.Element[Grid.PosToCell(cell)].IsGas)
            {
                energy = energy / 200 * KJAtExplosion;
            }
            else if (Grid.Element[Grid.PosToCell(cell)].IsSolid)
            {
                energy = energy * KJAtExplosion;
            }
            Debug.Log("Percentile = " + percentile + " , Energy = " + energy);

            SimMessages.ModifyEnergy(Grid.PosToCell(cell), energy, 9999f, SimMessages.EnergySourceID.DebugHeat);
        }

        public List<Vector2I> GetExplosionArea(List<Vector2> borderTiles, Vector2I center)
        {
            var dmgTiles = new List<Vector2I>();
            foreach(var borderTile in borderTiles)
            {
                dmgTiles.AddRange(Bresenhams(center.X, center.Y, (int)borderTile.x, (int)borderTile.y));
            }
            bunkerTilesHit = bunkerTilesHit.Distinct().ToList();
            return dmgTiles.Distinct().ToList();
        }
        public void DoDamage(Vector2I cell, Vector2I center)
        {
            AddHeatFromExplosion(cell, center);
            TileDamage(cell, center); 
            BackgroundTileDamage();
            EntityDamage(cell, center);
            //    WorldDamage.Instance.ApplyDamage(Grid.PosToCell(cell), GetDmgValAtPos2(cell, center, dmg), Grid.PosToCell(center));
            
        }

        public void BackgroundTileDamage()
        {

        }
        public void ReleaseExhaustGas(Vector2I center)
        {
            if (hasExhaustGas)
            {
                int diseaseCount = isRadioactive ? 1000000 : 0;
                Debug.Log(smokeMass);
                SimMessages.ReplaceElement(Grid.PosToCell(center), exhaustElement, null, smokeMass, smokeTemp,diseaseIdx, diseaseCount);

            }
        }
        public void TileDamage(Vector2I cellPos, Vector2I center)
        {
            int cell = Grid.PosToCell(cellPos);
            GameObject tile_go = Grid.Objects[cell, (int)ObjectLayer.FoundationTile];
                //tile_go = Grid.Objects[cell, (int)ObjectLayer.Backwall];
            float dmgMultiplier = 1f;
            bool buildingInTile = false;
            if ((UnityEngine.Object)tile_go != (UnityEngine.Object)null)
            {
                if (tile_go.GetComponent<KPrefabID>().HasTag(GameTags.Window))
                    dmgMultiplier = this.windowDamageMultiplier;

                SimCellOccupier component = tile_go.GetComponent<SimCellOccupier>();
                if ((UnityEngine.Object)component != (UnityEngine.Object)null && !component.doReplaceElement)
                    buildingInTile = true;
            }
            Element element = !buildingInTile ? Grid.Element[cell] : tile_go.GetComponent<PrimaryElement>().Element;
            if ((double)element.strength == 0.0)
                return;

            float amount = GetDmgValAtPos2(cellPos, center, dmg) * dmgMultiplier / element.strength;

            if ((double)amount == 0.0)
                return;

            if (buildingInTile)
            {
                BuildingHP component = tile_go.GetComponent<BuildingHP>();
                double a = (double)component.HitPoints / (double)component.MaxHitPoints;
                float f = amount * (float)component.MaxHitPoints;
                component.gameObject.Trigger(-794517298, (object)new BuildingHP.DamageSourceInfo()
                {
                    damage = Mathf.RoundToInt(f)
                });

                if (this.isRadioactive)
                {
                    gameObject.GetComponent<PrimaryElement>().AddDisease(diseaseIdx, (int)(100000f * amount), "nuclear explosion");
                }
            }
            else
            {
                SimMessages.ModifyDiseaseOnCell(cell, diseaseIdx, (int)(100000f * amount));
                WorldDamage.Instance.ApplyDamage(cell, amount, Grid.PosToCell(center));
            }
        }
        public void EntityDamage(Vector2I cellPos, Vector2I center)
        {
            List<GameObject> damagedEntities = new List<GameObject>();
            float damage = GetDmgValAtPos2(cellPos, center, dmg);
            int cell = Grid.PosToCell(cellPos);
            if (!Grid.IsValidCell(cell))
                return;
            GameObject building_go = Grid.Objects[cell, 1];
            if ((UnityEngine.Object)building_go != (UnityEngine.Object)null)
            {
                BuildingHP component1 = building_go.GetComponent<BuildingHP>();
                Building component2 = building_go.GetComponent<Building>();
                if ((UnityEngine.Object)component1 != (UnityEngine.Object)null && !damagedEntities.Contains(building_go))
                {
                    float f = building_go.GetComponent<KPrefabID>().HasTag(GameTags.Bunker) ? damage * this.bunkerDamageMultiplier : damage * entityDmgMultiplier;

                    component1.gameObject.Trigger(-794517298, (object)new BuildingHP.DamageSourceInfo()
                    {
                        damage = Mathf.RoundToInt(f)
                    });
                    damagedEntities.Add(building_go);
                }
            }
            ListPool<ScenePartitionerEntry, Comet>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, Comet>.Allocate();
            GameScenePartitioner.Instance.GatherEntries((int)cellPos.X, (int)cellPos.Y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>)gathered_entries);
            foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>)gathered_entries)
            {
                Pickupable pickupable = partitionerEntry.obj as Pickupable;
                Health component = pickupable.GetComponent<Health>();
                if ((UnityEngine.Object)component != (UnityEngine.Object)null && !damagedEntities.Contains(pickupable.gameObject))
                {
                    float amount = pickupable.GetComponent<KPrefabID>().HasTag(GameTags.Bunker) ? damage * this.bunkerDamageMultiplier : damage * entityDmgMultiplier;
                    component.Damage(amount);
                    damagedEntities.Add(pickupable.gameObject);
                }
            }
            gathered_entries.Recycle();

            if (this.isRadioactive)
            {
                foreach(var entity in damagedEntities)
                {
                    entity.GetComponent<PrimaryElement>().AddDisease(diseaseIdx, (int)(100000f * damage), "nuclear explosion");

                }
            }
        }

        public List<Vector2I> Bresenhams(int x0, int y0, int x1, int y1)
        {
            float vectorLength = (float)Math.Sqrt((Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2)));
            float percentile = 1f;
            float stepDecrease = 1f/vectorLength;
            var retList = new List<Vector2I>();

                int xDist = Math.Abs(x1 - x0);
                int yDist = -Math.Abs(y1 - y0);
                int xStep = (x0 < x1 ? +1 : -1);
                int yStep = (y0 < y1 ? +1 : -1);
                int error = xDist + yDist;
            while (x0 != x1 || y0 != y1)
            {
                if (2 * error - yDist > xDist - 2 * error)
                {
                    // horizontal step
                    error += yDist;
                    x0 += xStep;
                }
                else
                {
                        // vertical step
                    error += xDist;
                    y0 += yStep;
                }
                
                if (!CanDamageThisTile(x0, y0))
                {
                    return retList;
                }
                else
                {
                    retList.Add(new Vector2I(x0, y0)); 
                    percentile -= stepDecrease;
                }
            }
            return retList;
        }

        public bool CanDamageThisTile(int x, int y)
        {
            int cell = Grid.XYToCell(x,y);
            if (!Grid.IsValidCell(cell) || Grid.Element[cell].id == SimHashes.Unobtanium)
            {
                return false;
            }
            GameObject targetTile = Grid.Objects[cell, (int)ObjectLayer.FoundationTile];
            if (targetTile != null && targetTile.HasTag(GameTags.Bunker))
            {
                var vector = new Vector2I(x, y);
                ////if (!bunkerTilesHit.Contains(vector))
                //{
                //    Debug.Log(vector);
                    bunkerTilesHit.Add(vector);
               // }
                return false;
            }    
            return true;
        }

        public int GetDmgRadiusFromVal(float dmgAtPos1)
        {
            var returnVal = 1;
            while (DmgFormula(returnVal, 0, dmgAtPos1) > 0.025f)
            {
                ++returnVal;
            }
           // Debug.Log(returnVal);
            return returnVal;
        }
        public float GetDmgValAtPos2(Vector2I pos1, Vector2I pos2, float dmgAtPos1)
        {
            if (pos1 == pos2)
            {
                return dmgAtPos1;
            }
            else
            {
                float dx = Math.Abs(pos2.x - pos1.x);
                float dy = Math.Abs(pos2.y - pos1.y);

                return DmgFormula(dx,dy,dmgAtPos1);
            }
        }

        public float DmgFormula(float dx, float dy, float dmg)
        {
            //var retVal = dmg * 1f / (float)Math.Pow(2, Math.Sqrt((dx * dx) + (dy * dy)));
            double length = Math.Sqrt((dx * dx) + (dy * dy));
            float retVal = (float) (dmg * (Math.Tanh(-length / dmg)) + dmg);
            //Debug.Log(length+ "l, dmg"+retVal);
            if (retVal > 0.0f)
                return retVal;
            else return 0f;
        }
    }
}
