using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class ToxicityGrid : KMonoBehaviour, ISim200ms
	{
		static ToxicityGrid Instance;

		[Serialize]
		public float[] Toxicity = null;

		const int CHUNK_EDGE = 16;
		const float SPREAD_PERCENTAGE = 0.10f;
		const float MAX_SPREAD = 100f;
		const float OVERLAY_MAX = 1000f;

		static int CHUNK_SIZE = CHUNK_EDGE * CHUNK_EDGE;
		private static int widthInChunks;
		private static int heightInChunks;
		private static int chunkCount;

		private static int[] RandomCellAccess;
		private static int ReshuffleIndex = 0;
		HashSet<ushort> BlockedByElementIds;

		public override void OnSpawn()
		{
			Instance = this;
			base.OnSpawn();
			SgtLogger.l("ToxicityGrid.OnSpawn");
			if(Toxicity == null)
			{
				Toxicity = new float[Grid.CellCount];
				for(int i = 0; i < Toxicity.Length; i++)
				{
					Toxicity[i] = 0f;
				}
			}

			SetChunks();


			BlockedByElementIds = ElementLoader.elements.Where(e => e.IsSolid && e.hardness >= 255).Select(e => e.idx).ToHashSet();
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
			UpdateCells(dt);
		}
		public void UpdateCells(float dt)
		{
			// SgtLogger.l(RandomCellAccess[ReshuffleIndex]+" <- cell, index -> "+ ReshuffleIndex);
			Parallel.For(0, chunkCount, (chunk) => CellUpdate(GetRandomCellInChunk(chunk), dt));
		}

		bool CanExpandToCell(int targetCell, int worldId)
		{
			if(!Grid.IsValidCellInWorld(targetCell, worldId))
				return false;
			if(BlockedByElementIds.Contains(Grid.ElementIdx[targetCell]))
				return false;

			return true;
		}
		public float GetToxicityAtCell(int cell)
		{
			if (!Grid.IsValidCell(cell))
				return 0;

			if(cell < 0 || cell >= Toxicity.Length)
			{
				SgtLogger.warning("Tried to get toxicity for invalid cell " + cell);
				return 0;
			}

			return Toxicity[cell];
		}
		void ToxicitySpread(int sourceCell, int targetCell, float sourceToxicity)
		{
			float toxicityDiff = sourceToxicity - Toxicity[targetCell];
			toxicityDiff *= SPREAD_PERCENTAGE;
			toxicityDiff = Mathf.Clamp(toxicityDiff, -MAX_SPREAD, MAX_SPREAD);

			if (toxicityDiff == 0)
				return;
			Toxicity[targetCell] += toxicityDiff;
			Toxicity[sourceCell] -= toxicityDiff;
		}

		void CellUpdate(int cell, float dt)
		{
			if (!Grid.IsValidCell(cell))
				return;

			if (Toxicity[cell] <= 0)
				return;

			int worldId = Grid.WorldIdx[cell];

			var element = Grid.Element[cell];
			//todo:
			//if element is innately toxic; add some toxicity to the cell
			//Toxicity[cell] += element.GetToxicityContribution() * dt;


			int down = Grid.CellBelow(cell);
			int left = Grid.CellLeft(cell);
			int right = Grid.CellRight(cell);
			int up = Grid.CellAbove(cell);

			float currentToxicity = Toxicity[cell];
			if (CanExpandToCell(left, worldId))
			{
				ToxicitySpread(cell, left, currentToxicity);
			}
			if(CanExpandToCell(right, worldId))
			{
				ToxicitySpread(cell, right, currentToxicity);
			}
			if(CanExpandToCell(up, worldId))
			{
				ToxicitySpread(cell, up, currentToxicity);
			}
			if(CanExpandToCell(down, worldId))
			{
				ToxicitySpread(cell, down, currentToxicity);
			}
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



		private static void SetChunks()
		{
			widthInChunks = Grid.WidthInCells / CHUNK_EDGE;
			heightInChunks = Grid.HeightInCells / CHUNK_EDGE;
			chunkCount = widthInChunks * heightInChunks;

			RandomCellAccess = Enumerable.Range(0, CHUNK_SIZE).ToArray();
			Shuffle();
		}
		static System.Random rng = new System.Random();
		public static void Shuffle()
		{
			  // i.e., java.util.Random.
			int n = RandomCellAccess.Length;        // The number of items left to shuffle (loop invariant).
			while (n > 1)
			{
				int k = rng.Next(n);  // 0 <= k < n.
				n--;                     // n is now the last pertinent index;
				(RandomCellAccess[k], RandomCellAccess[n]) = (RandomCellAccess[n], RandomCellAccess[k]);     // swap array[n] with array[k] (does nothing if k == n).
			}
		}

		internal static void EmitToxicity(int cell, float amount)
		{
			if(Instance == null)
			{
				SgtLogger.warning("Tried to emit toxicity but ToxicityGrid instance is null");
				return;
			}
			SgtLogger.l("emitting " + amount + " toxicity at cell " + cell);
			Instance.Toxicity[cell] += amount;
		}

		internal static float GetToxicity(int cell)
		{
			if (Instance == null)
			{
				SgtLogger.warning("Tried to get toxicity but ToxicityGrid instance is null");
				return 0;
			}
			return Instance.GetToxicityAtCell(cell);
		}
		internal static float GetToxicityMaxPercentage(int cell)
		{
			if (Instance == null)
			{
				SgtLogger.warning("Tried to get toxicity but ToxicityGrid instance is null");
				return 0;
			}
			return Instance.GetToxicityAtCell(cell) / OVERLAY_MAX;
		}
	}
}
