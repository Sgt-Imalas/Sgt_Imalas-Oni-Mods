using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Twitch_ExplosiveMaterials
{
	public class ExplosiveBomblet : KMonoBehaviour, ISim200ms
	{
		public static GameHashes ImpactTriggered = (GameHashes)Hash.SDBMLower("FallingBomblet_ImpactTriggered");
		private bool isTriggered = false;
		private float timeToDetonate = -1f;
		private bool HasDetonated = false;
		public bool GravityTriggered = false;

		public int radius = -1;
		public float dmg = 5f;

		public float windowDamageMultiplier = 5f;
		public float buildingyDmgMultiplier = 150f;
		public float entityDmgMultiplier = 100f;
		public float dupeDmgMultiplier = 400f;
		public float bunkerDamageMultiplier = 0f;

		public bool hasExhaustGas = true;
		public SimHashes exhaustElement = SimHashes.Fallout;
		public bool isRadioactive = true;
		byte diseaseIdx = Db.Get().Diseases.GetIndex((HashedString)Db.Get().Diseases.RadiationPoisoning.Id);
		public float smokeTemp = 600f;
		public float smokeMass = -1f;
		public float smokeMassMultiplier = 5f;
		public float KJAtExplosion = 50000f;
		public float GermBaseMultiplier = 150000f;

		private List<Tuple<int, float>> bunkerTilesHit = new List<Tuple<int, float>>();
		private List<Tuple<int, float>> damageTilesHit = new List<Tuple<int, float>>();

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (GravityTriggered)
				this.Subscribe((int)ImpactTriggered, (obj) => Detonate());
		}
		public void Sim200ms(float dt)
		{
			if (isTriggered)
			{
				if (timeToDetonate > 0)
				{
					timeToDetonate -= dt;
				}
				else if (!HasDetonated)
				{
					HasDetonated = true;
					StartCoroutine(Explode());
				}
			}
		}

		public void Detonate(float timer = -1f)
		{
			isTriggered = true;
			timeToDetonate = timer;
		}

		public void SetNoGravity()
		{
			Debug.Log(GameComps.Fallers.Has(gameObject));
			if (GameComps.Fallers.Has(gameObject))
				GameComps.Fallers.Remove(gameObject);
		}

		public void ApplyParams()
		{
			radius = radius == -1 ? GetDmgRadiusFromVal(dmg) : radius;
			smokeMass = smokeMass == -1f ? (float)(radius * 3.1416 * 2) * smokeMassMultiplier : smokeMass;
		}

		private IEnumerator Explode()
		{
			ApplyParams();
			var outerCircle = ProcGen.Util.GetCircle(transform.position, radius);
			var center = Grid.PosToXY(transform.position);
			int centerCell = Grid.PosToCell(transform.position);
			CalculateExplosionArea(outerCircle, center);
			//Debug.Log("Pos: " + this.gameObject.transform.position + ", Center: "+center.X+" X," +  center.Y + " Y");

			//Parallel.For(0, Grid.CellCount, (i) => ProcessPixel(pixelsPtr, i, rByte, gByte, bByte));

			//Parallel.For(0, damageTilesHit.Count, (i) => DoDamage(damageTilesHit[i], centerCell));
			//Parallel.For(0, bunkerTilesHit.Count, (i) => AddHeatFromExplosion(bunkerTilesHit[i]));
			damageTilesHit = damageTilesHit.OrderByDescending(t => t.second).ToList();
			bunkerTilesHit = bunkerTilesHit.OrderByDescending(t => t.second).ToList();

			int i = 0, j = 0;
			while (i < damageTilesHit.Count)
			{
				DoDamage(damageTilesHit[i], centerCell);
				i++;
				//yield return null;
			}
			while (j < bunkerTilesHit.Count)
			{
				AddHeatFromExplosion(bunkerTilesHit[j]);
				j++;
				//yield return null;
			}
			//for (int i = 0; i < damageTilesHit.Count; ++i)
			//{
			//    DoDamage(damageTilesHit[i], centerCell);
			//    yield return new WaitForSeconds(0.05f);
			//}
			//for (int i = 0; i < bunkerTilesHit.Count; ++i)
			//{
			//    AddHeatFromExplosion(bunkerTilesHit[i]);
			//    yield return new WaitForSeconds(0.05f);
			//}


			ReleaseExhaustGas(center);
			Util.KDestroyGameObject(this.gameObject);
			yield return null;
		}
		public void AddHeatFromExplosion(Tuple<int, float> cellAndEnergy)
		{
			int cell = cellAndEnergy.first;
			float energy = cellAndEnergy.second;
			if (Grid.Element[cell].IsVacuum)
			{
				return;
			}
			if (Grid.Element[cell].IsLiquid)
			{
				energy = energy * 50 * KJAtExplosion;
			}
			else if (Grid.Element[cell].IsGas)
			{
				energy = energy / 200 * KJAtExplosion;
			}
			else if (Grid.Element[cell].IsSolid)
			{
				energy = energy * KJAtExplosion;
			}

			SimMessages.ModifyEnergy(cell, energy, 9800f, SimMessages.EnergySourceID.DebugHeat);
		}

		public void CalculateExplosionArea(List<Vector2> borderTiles, Vector2I center)
		{
			var dmgTiles = new List<Vector2I>();
			foreach (var borderTile in borderTiles)
			{
				dmgTiles.AddRange(Bresenhams(center.X, center.Y, (int)borderTile.x, (int)borderTile.y));
			}
			dmgTiles = dmgTiles.Distinct().ToList();

			var concurrentBunkerTiles = new ConcurrentBag<Tuple<int, float>>();
			var concurrentDamageCells = new ConcurrentBag<Tuple<int, float>>();

			Parallel.For(0, dmgTiles.Count,
				(i) =>
				{
					var pos = Grid.PosToCell(dmgTiles[i]);
					if (Grid.IsValidCell(pos))
						concurrentDamageCells.Add(new Tuple<int, float>(pos, GetDmgValAtPos2(center, dmgTiles[i], dmg) / dmg));
				}
				);
			Parallel.For(0, bunkerTilesHit.Count,
				(i) =>
				{
					var pos = Grid.CellToXY(bunkerTilesHit[i].first);
					concurrentBunkerTiles.Add(new Tuple<int, float>(bunkerTilesHit[i].first, GetDmgValAtPos2(center, pos, dmg) / dmg));
				});
			damageTilesHit.Clear();
			bunkerTilesHit.Clear();
			damageTilesHit.AddRange(concurrentDamageCells);
			bunkerTilesHit.AddRange(concurrentBunkerTiles);

		}

		public void DoDamage(Tuple<int, float> cellAndEnergy, int centerCell)
		{
			AddHeatFromExplosion(cellAndEnergy);
			TileDamage(cellAndEnergy, centerCell);
			BackgroundTileDamage(cellAndEnergy);
			EntityDamage(cellAndEnergy);
			//    WorldDamage.Instance.ApplyDamage(Grid.PosToCell(cell), GetDmgValAtPos2(cell, center, dmg), Grid.PosToCell(center));

		}

		public void BackgroundTileDamage(Tuple<int, float> cellAndEnergy)
		{
			int cell = cellAndEnergy.first;
			GameObject tile_go = Grid.Objects[cell, (int)ObjectLayer.Backwall];

			if ((double)cellAndEnergy.second == 0.0 || tile_go == null)
				return;
			tile_go.TryGetComponent<BuildingHP>(out var hP);
			double a = hP.HitPoints / (double)hP.MaxHitPoints;
			float f = cellAndEnergy.second * hP.MaxHitPoints * windowDamageMultiplier;
			hP.gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo()
			{
				damage = Mathf.RoundToInt(f)
			});

			if (this.isRadioactive && gameObject.TryGetComponent<PrimaryElement>(out var primaryElement))
			{
				primaryElement.GetComponent<PrimaryElement>().AddDisease(diseaseIdx, (int)(GermBaseMultiplier * cellAndEnergy.second), "nuclear explosion");
			}
		}
		public void ReleaseExhaustGas(Vector2I center)
		{
			if (hasExhaustGas)
			{
				int diseaseCount = isRadioactive ? (int)GermBaseMultiplier * 10 : 0;
				Debug.Log(smokeMass);
				SimMessages.ReplaceElement(Grid.PosToCell(center), exhaustElement, null, smokeMass, smokeTemp, diseaseIdx, diseaseCount);

			}
		}
		public void TileDamage(Tuple<int, float> cellAndEnergy, int centerCell)
		{
			int cell = cellAndEnergy.first;

			GameObject tile_go = Grid.Objects[cell, (int)ObjectLayer.FoundationTile];
			//tile_go = Grid.Objects[cell, (int)ObjectLayer.Backwall];
			float dmgMultiplier = 1f;
			bool buildingInTile = false;
			if (tile_go != null)
			{
				if (tile_go.GetComponent<KPrefabID>().HasTag(GameTags.Window))
					dmgMultiplier = this.windowDamageMultiplier;

				SimCellOccupier component = tile_go.GetComponent<SimCellOccupier>();
				if (component != null && !component.doReplaceElement)
					buildingInTile = true;
			}
			Element element = !buildingInTile ? Grid.Element[cell] : tile_go.GetComponent<PrimaryElement>().Element;
			if (element.strength == 0.0)
				return;

			float amount = cellAndEnergy.second * dmgMultiplier / element.strength;

			if ((double)amount == 0.0)
				return;

			if (buildingInTile)
			{
				BuildingHP component = tile_go.GetComponent<BuildingHP>();
				double a = component.HitPoints / (double)component.MaxHitPoints;
				float f = amount * component.MaxHitPoints;
				component.gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo()
				{
					damage = Mathf.RoundToInt(f)
				});

				if (this.isRadioactive)
				{
					gameObject.GetComponent<PrimaryElement>().AddDisease(diseaseIdx, (int)(GermBaseMultiplier * amount), "nuclear explosion");
				}
			}
			else
			{
				SimMessages.ModifyDiseaseOnCell(cell, diseaseIdx, (int)(GermBaseMultiplier * amount));
				WorldDamage.Instance.ApplyDamage(cell, amount, centerCell);
			}
		}
		public void EntityDamage(Tuple<int, float> cellAndEnergy)
		{
			List<GameObject> damagedEntities = new List<GameObject>();
			float damage = cellAndEnergy.second;
			int cell = cellAndEnergy.first;
			var cellPos = Grid.CellToXY(cell);

			if (!Grid.IsValidCell(cell))
				return;
			GameObject building_go = Grid.Objects[cell, (int)ObjectLayer.Building];
			if (building_go != null && building_go.TryGetComponent<BuildingHP>(out var buildingHP))
			{
				if (!damagedEntities.Contains(building_go))
				{
					float f = building_go.GetComponent<KPrefabID>().HasTag(GameTags.Bunker) ? damage * this.bunkerDamageMultiplier : damage * buildingyDmgMultiplier;

					buildingHP.gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo()
					{
						damage = Mathf.RoundToInt(f)
					});
					damagedEntities.Add(building_go);
				}
			}
			ListPool<ScenePartitionerEntry, Comet>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, Comet>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(cellPos.X, cellPos.Y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, gathered_entries);
			foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>)gathered_entries)
			{
				Pickupable pickupable = partitionerEntry.obj as Pickupable;
				if (pickupable.TryGetComponent<Health>(out var component) && !damagedEntities.Contains(pickupable.gameObject))
				{
					float amount = pickupable.GetComponent<KPrefabID>().HasTag(GameTags.DupeBrain) ? damage * this.dupeDmgMultiplier : damage * entityDmgMultiplier;
					component.Damage(amount);
					damagedEntities.Add(pickupable.gameObject);
				}
			}
			gathered_entries.Recycle();

			if (this.isRadioactive)
			{
				foreach (var entity in damagedEntities)
				{
					entity.GetComponent<PrimaryElement>().AddDisease(diseaseIdx, (int)(GermBaseMultiplier * damage), "nuclear explosion");

				}
			}
		}

		public List<Vector2I> Bresenhams(int x0, int y0, int x1, int y1)
		{
			float vectorLength = (float)Math.Sqrt((Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2)));
			float percentile = 1f;
			float stepDecrease = 1f / vectorLength;
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
			int cell = Grid.XYToCell(x, y);
			if (!Grid.IsValidCell(cell) || Grid.Element[cell].id == SimHashes.Unobtanium)
			{
				return false;
			}
			GameObject targetTile = Grid.Objects[cell, (int)ObjectLayer.FoundationTile];
			if (targetTile != null && targetTile.HasTag(GameTags.Bunker))
			{
				var vector = new Vector2I(x, y);
				bunkerTilesHit.Add(new Tuple<int, float>(cell, -1f));
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

				return DmgFormula(dx, dy, dmgAtPos1);
			}
		}

		public float DmgFormula(float dx, float dy, float dmg)
		{
			//var retVal = dmg * 1f / (float)Math.Pow(2, Math.Sqrt((dx * dx) + (dy * dy)));
			double length = Math.Sqrt((dx * dx) + (dy * dy));
			float retVal = (float)(dmg * (Math.Tanh(-length / dmg)) + dmg);
			//Debug.Log(length+ "l, dmg"+retVal);
			if (retVal > 0.0f)
				return retVal;
			else return 0f;
		}
	}
}
