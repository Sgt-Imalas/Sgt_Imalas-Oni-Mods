using _3GuBsVisualFixesNTweaks.Defs.Entities;
using Epic.OnlineServices.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
	[SkipSaveFileSerialization]
	internal class RandomTickManager : KMonoBehaviour, ISim200ms
	{
		const int CHUNK_EDGE = 8;

		static int CHUNK_SIZE = CHUNK_EDGE * CHUNK_EDGE;
		private static int widthInChunks;
		private static int heightInChunks;
		private static int chunkCount;
		private static ushort magmaIdx;
		private static float maxMagmaMass;
		private static GameObject MagmaBubblePrefab;

		private static int[] RandomCellAccess;
		private static int ReshuffleIndex = 0;
		HashSet<Vector3> PendingMagmaBubbles = new HashSet<Vector3>();


		System.Random rng = new System.Random();
		Color MagmaColor = UIUtils.rgb(253, 171, 87);

		public override void OnSpawn()
		{
			base.OnSpawn();

			SetChunks();
		}
		public void Sim200ms(float dt)
		{
			ReshuffleIndex++;
			if (ReshuffleIndex == CHUNK_SIZE)
			{
				Shuffle();
				ReshuffleIndex = 0;
				//SgtLogger.l("resetting and shuffling index");
			}

			UpdateCells();
		}
		public void UpdateCells()
		{
			// SgtLogger.l(RandomCellAccess[ReshuffleIndex]+" <- cell, index -> "+ ReshuffleIndex);
			Parallel.For(0, chunkCount, (chunk) => MarkMagmaBubbleCells(GetRandomCellInChunk(chunk)));

			foreach (var magmaBubblePos in PendingMagmaBubbles)
			{
				var bubble = GameUtil.KInstantiate(MagmaBubblePrefab, magmaBubblePos, Grid.SceneLayer.FXFront2);
				bubble.SetActive(true);
				if (bubble.TryGetComponent<KBatchedAnimController>(out var kbac))
				{
					kbac.TintColour = MagmaColor;
					kbac.animScale *= ((float)rng.NextDouble()) * 0.4f + 0.6f;
				}

			}
			PendingMagmaBubbles.Clear();
		}
		void MarkMagmaBubbleCells(int cell)
		{
			if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
				return;

			if (Grid.ElementIdx[cell] != magmaIdx)
				return;
			int above = Grid.CellAbove(cell);
			if (!Grid.IsValidCell(above) || !Grid.IsVisible(above) || Grid.IsSolidCell(above) || Grid.IsLiquid(above))
				return;

			//SgtLogger.l("Spawning magma bubble at " + cell);

			var tilePercentageFull = Grid.Mass[cell] / maxMagmaMass;
			float calculatedHeight = (0.64f * Mathf.Log10(tilePercentageFull)) + 1f;

			//SgtLogger.l(cell+": perc: "+tilePercentageFull+", calc: "+calculatedHeight); 
			calculatedHeight = Mathf.Clamp(calculatedHeight, 0.20f, 0.90f);
			var pos = Grid.CellToPosCBC(cell, Grid.SceneLayer.FXFront);
			pos += new Vector3(0, calculatedHeight);

			PendingMagmaBubbles.Add(pos);
			//KBatchedAnimController component2 = hitEffect.GetComponent<KBatchedAnimController>();
			//hitEffect.SetActive(value: true);
			//component2.sceneLayer = Grid.SceneLayer.FXFront2;
			//component2.enabled = false;
			//component2.enabled = true;
		}

		private int GetRandomCellInChunk(int chunk)
		{
			ChunkOffset(chunk, out var x, out var y);

			int currentRandomIndex = RandomCellAccess[ReshuffleIndex] + chunk % (CHUNK_SIZE - 1);

			x += currentRandomIndex % CHUNK_EDGE;
			y += currentRandomIndex / CHUNK_EDGE;
			return Grid.XYToCell(x, y);
		}
		public static int XYToChunk(int x, int y)
		{
			return x + y * widthInChunks;
		}

		public static void ChunkToXY(int chunk, out int x, out int y)
		{
			x = chunk % widthInChunks;
			y = chunk / widthInChunks;
		}

		public static void ChunkOffset(int chunk, out int x, out int y)
		{
			x = chunk % widthInChunks * CHUNK_EDGE;
			y = chunk / widthInChunks * CHUNK_EDGE;
		}



		private void SetChunks()
		{
			widthInChunks = Grid.WidthInCells / CHUNK_EDGE;
			heightInChunks = Grid.HeightInCells / CHUNK_EDGE;
			chunkCount = widthInChunks * heightInChunks;

			magmaIdx = ElementLoader.GetElementIndex(SimHashes.Magma);
			maxMagmaMass = ElementLoader.FindElementByHash(SimHashes.Magma).maxMass; //1840kg
			RandomCellAccess = Enumerable.Range(0, CHUNK_SIZE).ToArray();
			MagmaBubblePrefab = Assets.GetPrefab(MagmaBubbleFXConfig.ID);
			Shuffle();
		}
		public void Shuffle()
		{
			int n = RandomCellAccess.Length;        // The number of items left to shuffle (loop invariant).
			while (n > 1)
			{
				int k = rng.Next(n);  // 0 <= k < n.
				n--;                     // n is now the last pertinent index;
				int temp = RandomCellAccess[n];     // swap array[n] with array[k] (does nothing if k == n).
				RandomCellAccess[n] = RandomCellAccess[k];
				RandomCellAccess[k] = temp;
			}
		}
	}
}
