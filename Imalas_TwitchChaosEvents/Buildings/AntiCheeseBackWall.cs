using ONITwitchLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.ELEMENTS;
using static STRINGS.SUBWORLDS;

namespace Imalas_TwitchChaosEvents.Buildings
{
	internal class AntiCheeseBackWall:KMonoBehaviour
	{
		public override void OnCleanUp()
		{
			EngageAntiCheese();
			base.OnCleanUp();
		}
		void EngageAntiCheese()
		{
			var outerCircle = ProcGen.Util.GetCircle(transform.position, 3);
			var center = Grid.PosToXY(transform.position);
			int centerCell = Grid.PosToCell(transform.position);
			CalculateExplosionArea(outerCircle, center);

			int i = 0;
			while (i < damageTilesHit.Count)
			{				
				DoDamage(damageTilesHit[i], centerCell);
				i++;
			}
			ToastManager.InstantiateToast(STRINGS.CHAOSEVENTS.CHEESEBACKWALLTRIGGERED.TOAST,STRINGS.CHAOSEVENTS.CHEESEBACKWALLTRIGGERED.TOASTTEXT);
		}
		public void DoDamage(Tuple<int, float> cellAndEnergy, int centerCell)
		{
			TileDamage(cellAndEnergy, centerCell);
			BackgroundTileDamage(cellAndEnergy);
		}

		public void BackgroundTileDamage(Tuple<int, float> cellAndEnergy)
		{
			int cell = cellAndEnergy.first;
			GameObject tile_go = Grid.Objects[cell, (int)ObjectLayer.Backwall];

			if ((double)cellAndEnergy.second == 0.0 || tile_go == null)
				return;
			tile_go.TryGetComponent<BuildingHP>(out var hP);
			hP.gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo()
			{
				damage = 2000
			});
		}
		public void TileDamage(Tuple<int, float> cellAndEnergy, int centerCell)
		{
			int cell = cellAndEnergy.first;

			GameObject tile_go = Grid.Objects[cell, (int)ObjectLayer.FoundationTile];
			//tile_go = Grid.Objects[cell, (int)ObjectLayer.Backwall];
			
			bool buildingInTile = false;
			if (tile_go != null)
			{
				SimCellOccupier component = tile_go.GetComponent<SimCellOccupier>();
				if (component != null && !component.doReplaceElement)
					buildingInTile = true;
			}
			Element element = !buildingInTile ? Grid.Element[cell] : tile_go.GetComponent<PrimaryElement>().Element;
			if (element.strength == 0.0)
				return;

			float amount = 2000;

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
			}
			else
			{
				WorldDamage.Instance.ApplyDamage(cell, amount, centerCell);
			}
		}
		private List<Tuple<int, float>> damageTilesHit = new List<Tuple<int, float>>();
		public bool CanDamageThisTile(int x, int y)
		{
			int cell = Grid.XYToCell(x, y);
			if (!Grid.IsValidCell(cell) || Grid.Element[cell].id == SimHashes.Unobtanium)
			{
				return false;
			}
			return true;
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
						concurrentDamageCells.Add(new Tuple<int, float>(pos, 2000));
				}
				);
			damageTilesHit.Clear();
			damageTilesHit.AddRange(concurrentDamageCells);

		}
	}
}
