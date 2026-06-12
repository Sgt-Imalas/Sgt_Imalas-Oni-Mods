using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BubbleChest.Content.Scripts
{
	internal class ChestBubbler : KMonoBehaviour, ISim200ms, ISingleSliderControl
	{
		[MyCmpReq] Storage storage;
		[MyCmpReq] Operational operational;
		[MyCmpReq] KBatchedAnimController kbac;
		[Serialize]public float bubbleRate = 200f;

		int cell = -1, cellAbove = -1;
		Vector2 emissionPos;
		

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
			if (!storage.items.Any() || !Grid.IsLiquid(cell) || !operational.IsOperational)
			{
				kbac.Play("off");
				return;
			}
			kbac.Play("on");

			var mat = storage.items[0];
			if (mat == null ||! mat.TryGetComponent<Pickupable>(out var picker))
				return;

			var bubble = picker.Take(dt * bubbleRate / 1000f);
			if (bubble == null)
				return;

			var element = bubble.GetComponent<PrimaryElement>();
			BubbleManager.Disease disease = BubbleManager.Disease.None;
			if (element.DiseaseIdx != byte.MaxValue)
				disease = new() { Idx = element.DiseaseIdx, Count = element.diseaseCount };
			BubbleManager.instance.SpawnBubble(element.ElementID, emissionPos, element.Mass, element.Temperature, disease);
			Util.KDestroyGameObject(bubble);
		}

		public string SliderTitleKey => "";
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
			return "bubble rate in g";
		}
	}
}
