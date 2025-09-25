using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class ConfigurableThresholdSolidConduitDispenser : ConfigurableSolidConduitDispenser, ISingleSliderControl
	{
		[SerializeField]
		[Serialize]
		public float Threshold;

		public string SliderTitleKey => "STRINGS.UI.AIO_THRESHOLD_VALVE_SIDESCREEN.TITLE";

		public string SliderUnits => global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM;



		public bool ValidDispensable(GameObject item)
		{
			if (!solidOnly && Threshold <= 0)
				return true;

			//should never happen
			if (!item.TryGetComponent<PrimaryElement>(out var element))
				return Assets.TryGetPrefab(item.name) != null;

			if (solidOnly && !element.Element.IsSolid)
				return false;
			
			return element.Mass >= Threshold;
		}
		/// <summary>
		/// mirrored from FindSuitableItem, but only checks items at or above threshold
		/// </summary>
		/// <returns></returns>
		public Pickupable FindSuitableItemAboveThreshold()
		{
			List<GameObject> items = storage.items;
			if (!items.Any())
			{
				return null;
			}

			round_robin_index %= items.Count;
			GameObject gameObject = items[round_robin_index];
			round_robin_index++;
			if (!ValidDispensable(gameObject))
			{
				bool solidItemFound = false;
				int num = 0;
				while (!solidItemFound && num < items.Count)
				{
					gameObject = items[(round_robin_index + num) % items.Count];
					if (ValidDispensable(gameObject))
					{
						solidItemFound = true;
					}

					num++;
				}

				if (!solidItemFound)
				{
					return null;
				}
			}
			if (!gameObject)
			{
				return null;
			}
			return gameObject.GetComponent<Pickupable>();
		}


		public float GetSliderMax(int index)
		{
			return this.massDispensed;
		}

		public float GetSliderMin(int index) => 0;

		public string GetSliderTooltip(int index) => STRINGS.UI.AIO_THRESHOLD_VALVE_SIDESCREEN.TOOLTIP;

		public string GetSliderTooltipKey(int index) => null;

		public float GetSliderValue(int index) => Threshold;

		public void SetSliderValue(float percent, int index)
		{
			Threshold = percent;
		}

		public int SliderDecimalPlaces(int index) => 0;
	}
}
