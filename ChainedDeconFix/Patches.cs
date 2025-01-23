using Database;
using FMOD.Studio;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ChainedDeconFix.ModAssets;

namespace ChainedDeconFix
{
	internal class Patches
	{
		[HarmonyPatch(typeof(Deconstructable))]
		[HarmonyPatch(
	nameof(Deconstructable.TriggerDestroy),
	[
			typeof(float),
			typeof(byte),
			typeof(int),
			typeof(WorkerBase),
			]
		)]
		public static class ChainedDeconPatch
		{
			private static int GetLayerForDeconstructable(Deconstructable deconstructable)
			{
				var cell = deconstructable.GetCell();

				for (int layer = 1; layer <= 27; layer++)
				{
					var obj = Grid.Objects[cell, layer];

					if (obj != null && obj.GetComponent<Deconstructable>() == deconstructable)
					{
						return layer;
					}
				}
				return (int)ObjectLayer.Building;
			}

			private static IEnumerable<int> GetAdjacentCells(int cell)
			{
				yield return Grid.CellAbove(cell);
				yield return Grid.CellBelow(cell);
				yield return Grid.CellLeft(cell);
				yield return Grid.CellRight(cell);
			}
			private static void DeconstructAdjacent(Deconstructable rootDeconstructable, string name, int layer)
			{
				GetAdjacentCells(rootDeconstructable.GetCell())
					.Select(cell => Grid.Objects[cell, layer])
					.Where(gameObject => gameObject != null)
					.Select(gameObject => gameObject.GetComponent<Deconstructable>())
					.Where(deconstructable =>
						deconstructable != null
						&& deconstructable.IsMarkedForDeconstruction()
						&& deconstructable.name == name
						&& deconstructable.destroyed == false
					)
					.Do(deconstructable =>
					{
						var cell = deconstructable.GetCell();

						if (CellsDeconstructed.Contains(cell))
						{
							return;
						}

						deconstructable.OnCompleteWork(null);
						CellsDeconstructed.Add(cell);
						DeconstructAdjacent(deconstructable, name, layer);
					});
			}

			[HarmonyPrefix]
			[HarmonyPriority(Priority.HigherThanNormal)]
			public static bool StopExecutionIfInstanceIsNull(Deconstructable __instance)
			{
				var shouldContinue = __instance != null;

				return shouldContinue;
			}

			private static bool Busy = false;
			private static readonly List<int> CellsDeconstructed = new List<int>();

			public static void Postfix(Deconstructable __instance)
			{
				if (Busy)
				{
					return;
				}

				Busy = true;

				try
				{
					if (__instance == null)
					{
						SgtLogger.l("__instance is null at Deconstructable.TriggerDestroy.Postfix");

						return;
					}

					var name = __instance.name;

					if (__instance.destroyed == false)
					{
						SgtLogger.l("DestroyedGetter is null:" + name);

						return;
					}

					if (State.ChainAll == false)
					{
						if (State.Chainables.Contains(name) == false)
						{
							return;
						}
					}

					var layer = GetLayerForDeconstructable(__instance);

					CellsDeconstructed.Clear();

					DeconstructAdjacent(__instance, name, layer);
				}
				catch (Exception e)
				{
					SgtLogger.l("Deconstructable.TriggerDestroy.Postfix Error;\n" + e);
				}
				finally
				{
					Busy = false;
				}
			}
		}
	}
}
