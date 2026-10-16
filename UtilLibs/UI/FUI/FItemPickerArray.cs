using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UtilLibs.UI.FUI
{
	public class FItemPickerArray : KMonoBehaviour
	{
		class FItemPickerEntry : KMonoBehaviour
		{
			Image SymbolDisplay;
			FToggleButton button;
			string TargetSelection;
			Action<string> onIconSelect = null;

			Sprite toDisplay = null;

			public override void OnPrefabInit()
			{
				base.OnPrefabInit();
				SymbolDisplay = transform.Find("Image").gameObject.GetComponent<Image>();
				button = gameObject.AddOrGet<FToggleButton>();
				button.OnClick += ColorEntryClicked;
				if (toDisplay != null)
					SymbolDisplay.sprite = toDisplay;
				Refresh(string.Empty);
			}
			public void Init(string id, Sprite icon, Action<string> onColorSelect)
			{
				TargetSelection = id;
				onIconSelect = onColorSelect;
				SymbolDisplay?.sprite = icon;
				toDisplay = icon;
			}

			public void Refresh(string selected)
			{
				if (button == null)
					return;

				button.SetIsSelected(selected == TargetSelection);
			}
			void ColorEntryClicked()
			{
				onIconSelect?.Invoke(TargetSelection);
			}
		}

		FItemPickerEntry Prefab;
		Dictionary<string, FItemPickerEntry> PalletteEntries = [];
		public string SelectedEntry = string.Empty;
		public event Action<string> OnSelectionChanged;
		Dictionary<string, Sprite> _values = [];
		Comparison<string> _sortFunction = null;

		public void Init(Dictionary<string, Sprite> vals, Comparison<string> sorter)
		{
			_values = vals;
			_sortFunction = sorter;
			InitPallette();
		}

		public void SetSelected(string selected)
		{
			OnSelectItem(selected);
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			InitPallette();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			RefreshPallette();
		}

		void InitPallette()
		{
			Prefab = transform.Find("Item").gameObject.AddComponent<FItemPickerEntry>();
			Prefab.gameObject.SetActive(false);
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				var child = transform.GetChild(i);
				if (child.gameObject != Prefab.gameObject)
					Destroy(child.gameObject);
			}
			PalletteEntries.Clear();

			List<string> keysOrdered = _values.Keys.ToList();
			if (_sortFunction != null)
			{
				keysOrdered.Sort(_sortFunction);
				//SgtLogger.l("Sorted items");
			}
			//else
				//SgtLogger.l("no sorting...");

			foreach (var key in keysOrdered)
			{
				//SgtLogger.l("Adding icon: " + key);
				var entry = Util.KInstantiateUI<FItemPickerEntry>(Prefab.gameObject, gameObject);
				entry.Init(key, _values[key], OnSelectItem);
				entry.gameObject.SetActive(true);
				PalletteEntries[key] = entry;
			}
			if (_values != null && _values.Keys.Any())
				SelectedEntry = keysOrdered.First();
		}
		void RefreshPallette()
		{
			foreach (var entry in PalletteEntries)
				entry.Value.Refresh(SelectedEntry);
		}
		void OnSelectItem(string item)
		{
			if (item != SelectedEntry)
			{
				SelectedEntry = item;
				OnSelectionChanged?.Invoke(SelectedEntry);
				RefreshPallette();
			}
		}
	}
}
