using BubbleChest.Content.Defs.Buildings;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static STRINGS.ELEMENTS;

namespace BubbleChest.Content.Scripts
{
	internal class ChestBubbler : KMonoBehaviour, ISim200ms, ISingleSliderControl, ISim1000ms
	{
		[MyCmpReq] Storage storage;
		[MyCmpReq] Operational operational;
		[MyCmpReq] KBatchedAnimController kbac;
		[Serialize] public float bubbleRate = 200f;

		int cell = -1, cellAbove = -1, cellColumnHeight = 0;
		Vector2 emissionPos;

		SimHashes lastGas = SimHashes.Oxygen;

		public override void OnSpawn()
		{
			cell = Grid.PosToCell(this);
			cellAbove = Grid.CellAbove(cell);

			emissionPos = Grid.CellToPosCTC(cell, Grid.SceneLayer.Gas);
			//emissionPos += new Vector2(0, 0.3f);
			base.OnSpawn();
		}

		public void Sim200ms(float dt)
		{
			if (!storage.items.Any() || !Grid.IsLiquid(cell) || !Grid.IsLiquid(cellAbove) || !operational.IsOperational)
			{
				kbac.Play("off");
				return;
			}
			kbac.Play("on");

			var mat = storage.items[0];
			if (mat == null || !mat.TryGetComponent<Pickupable>(out var picker))
				return;

			var bubble = picker.Take(dt * bubbleRate / 1000f);
			if (bubble == null)
				return;

			var element = bubble.GetComponent<PrimaryElement>();
			lastGas = element.ElementID;
			BubbleManager.Disease disease = BubbleManager.Disease.None;
			if (element.DiseaseIdx != byte.MaxValue)
				disease = new() { Idx = element.DiseaseIdx, Count = element.diseaseCount };
			BubbleManager.instance.SpawnBubble(element.ElementID, emissionPos, element.Mass, element.Temperature, disease);
			Util.KDestroyGameObject(bubble);
		}

		public void Sim1000ms(float dt)
		{
			RefreshBubbleCells();
			MakeFishiesHappy();
		}

		void RefreshBubbleCells()
		{
			cellColumnHeight = 0;
			int cell = cellAbove;


			while (!BubbleManager.ShouldPop(Grid.CellToPos(cell), lastGas, out _))
			{
				if (Grid.IsValidCell(cell))
				{
					cellColumnHeight++;
					cell = Grid.CellAbove(cell);
				}
				else
					break;
			}
		}
		void MakeFishiesHappy()
		{
			ListPool<ScenePartitionerEntry, ChestBubbler>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ChestBubbler>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(cellAbove).x - 1, Grid.CellToXY(cellAbove).y, 3, cellColumnHeight, GameScenePartitioner.Instance.pickupablesLayer, gathered_entries);
			for (int index = 0; index < gathered_entries.Count; ++index)
			{
				if (gathered_entries[index].obj is Pickupable pickupable && !pickupable.wasAbsorbed)
				{
					if (pickupable.TryGetComponent<KPrefabID>(out var component))
					{
						if (!component.HasAllTags([GameTags.SwimmingCreature, GameTags.Creatures.Swimmer, GameTags.CreatureBrain]) || component.gameObject.GetDef<BabyMonitor.Def>() != null || !component.TryGetComponent<Effects>(out var effects))
							continue;

						//if (!effects.HasEffect(BubbleChestConfig.EFFECT_ID) && component.TryGetComponent<KBatchedAnimController>(out var critter_kbac))
						//	critter_kbac.Play("grooming_pst");

						effects.Add(BubbleChestConfig.EFFECT_ID, true);
					}

				}
			}
			gathered_entries.Recycle();
		}
		#region slider
		public string SliderTitleKey => "STRINGS.BUILDINGS.PREFABS.BC_BUBBLECHEST.SLIDER_TITLE";
		public string SliderUnits => global::STRINGS.UI.UNITSUFFIXES.MASS.GRAM;
		public int SliderDecimalPlaces(int index) => 0;

		public float GetSliderMin(int index) => 50;

		public float GetSliderMax(int index) => 1000;

		public float GetSliderValue(int index) => bubbleRate;

		public void SetSliderValue(float percent, int index) => bubbleRate = percent;

		public string GetSliderTooltipKey(int index)
		{
			return "";
		}

		public string GetSliderTooltip(int index)
		{
			return STRINGS.BUILDINGS.PREFABS.BC_BUBBLECHEST.SLIDER_LABEL;
		}

		#endregion
	}
}
