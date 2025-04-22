using Klei.AI;
using KSerialization;
using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Elements
{

	[SkipSaveFileSerialization]
	internal class RandomTickManager : KMonoBehaviour, ISim200ms
	{
		const int CHUNK_EDGE = 8;

		static int CHUNK_SIZE = CHUNK_EDGE * CHUNK_EDGE;
		private static int widthInChunks;
		private static int heightInChunks;
		private static int chunkCount;
		private static ushort Void_Idx;
		private static Element Void_Element;

		private static int[] RandomCellAccess;
		private static int ReshuffleIndex = 0;

		public static RandomTickManager Instance;
		[Serialize]
		public bool DoVoidMultiplication = false;
		[Serialize]
		public bool DoVoidCleanup = false;
		public MinionIdentity Target;

		public static bool VoidActive => Instance == null ? false : Instance.DoVoidMultiplication || Instance.DoVoidCleanup;

		public override void OnSpawn()
		{
			base.OnSpawn();
			SetChunks();
			Game.Instance.Subscribe((int)GameHashes.DuplicantDied, OnDuplicantDied);
		}
		void OnDuplicantDied(object data)
		{
			if (data is GameObject minionGo && minionGo.TryGetComponent<MinionIdentity>(out var identity) && identity != null)
			{
				if (identity == Target)
				{
					DeclareVoidVictory();
				}
			}
		}
		public void AdmitVoidDefeat()
		{
			if (Target == null || Target.gameObject == null)
				return;

			string name = Target.GetProperName();

			ToastManager.InstantiateToast(
				STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.TOAST,
				string.Format(STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.CONSUME_FAILURE, name));

			Target = null;
			DoVoidMultiplication = false;
			DoVoidCleanup = true;

			GameScheduler.Instance.Schedule("stopCleanup", 120, StopVoidCleanup);
		}
		public void DeclareVoidVictory()
		{
			if (Target == null || Target.gameObject == null)
				return;

			string name = Target.GetProperName();

			ToastManager.InstantiateToast(
				STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.TOAST,
				string.Format(STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.CONSUME_SUCCESS, name));
			Target = null;
			DoVoidMultiplication = false;
			DoVoidCleanup = true;

			GameScheduler.Instance.Schedule("stopCleanup", 120, StopVoidCleanup);
		}
		void StopVoidCleanup(object _)
		{
			DoVoidCleanup = false;
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
			if (DoVoidMultiplication)
				Parallel.For(0, chunkCount, (chunk) => TryVoidDuplication(GetRandomCellInChunk(chunk), dt));
			else if (DoVoidCleanup)
				Parallel.For(0, chunkCount, (chunk) => TryVoidCleanup(GetRandomCellInChunk(chunk), dt));
		}
		private static readonly CellElementEvent spawnEvent = new(
			"ChaosTwitch_DuplicateVoid",
			"Spawned by Twitch",
			true
		);

		void TryVoidCleanup(int cell, float dt)
		{
			if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
				return;

			if (Grid.Element[cell] != Void_Element)
				return;
			var mass = Grid.Mass[cell];

			SimMessages.ReplaceElement(
					cell,
					SimHashes.Vacuum,
					spawnEvent,
					0,
					Void_Element.defaultValues.temperature);
		}

		void TryVoidDuplication(int cell, float dt)
		{
			if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
				return;

			if (Grid.Element[cell] != Void_Element)
				return;
			var mass = Grid.Mass[cell];

			var massToCreate = Mathf.Clamp(mass, 100, 500);
			massToCreate *= dt;

			SimMessages.ReplaceAndDisplaceElement(
					cell,
					Void_Element.id,
					spawnEvent,
					massToCreate,
					Void_Element.defaultValues.temperature
				);
			//if (mass < 500)
			return;
			//too crashy :(
			int[] neighbors = [Grid.CellAbove(cell), Grid.CellBelow(cell), Grid.CellLeft(cell), Grid.CellRight(cell)];
			foreach (var neighborCell in neighbors)
			{
				if (Grid.IsSolidCell(neighborCell))
				{
					float amount = 0.05f * dt;
					//Grid.StrengthInfo[neighborCell]
					GameObject tile_go = Grid.Objects[cell, 9];
					if (tile_go != null && tile_go.TryGetComponent<SimCellOccupier>(out var sco) && !sco.doReplaceElement && tile_go.TryGetComponent<BuildingHP>(out var hp))
					{
						float damageAmount = amount * (float)hp.MaxHitPoints;
						hp.gameObject.Trigger(-794517298, (object)new BuildingHP.DamageSourceInfo()
						{
							damage = Mathf.RoundToInt(damageAmount),
							source = (string)global::STRINGS.BUILDINGS.DAMAGESOURCES.COMET,
							popString = (string)STRINGS.CHAOSEVENTS.STAREDINTOTHEVOID.VOID_DAMAGE
						});
					}
					else
						WorldDamage.Instance.ApplyDamage(neighborCell, amount, cell);
				}
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

			Void_Element = ElementLoader.GetElement(ModElements.VoidLiquid.id);
			Void_Idx = ElementLoader.GetElementIndex(ModElements.VoidLiquid);

			RandomCellAccess = Enumerable.Range(0, CHUNK_SIZE).ToArray();
			Shuffle();
		}
		public static void Shuffle()
		{
			System.Random rng = new System.Random();   // i.e., java.util.Random.
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

		internal void StartVoidEvent(MinionIdentity randomMinion)
		{
			Target = randomMinion;
			DoVoidCleanup = false;
			DoVoidMultiplication = true;
		}
	}
}
