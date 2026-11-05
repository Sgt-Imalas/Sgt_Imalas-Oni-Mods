using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;

namespace ShapeProjector.Content.Scripts.UI
{
	internal class ShapeProjectorSideScreen : SideScreenContent
	{
		public override bool IsValidForTarget(GameObject target) => target.TryGetComponent<ShapeProjectorLogic>(out _);

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.CheckShouldShowTopTitle = () => false;
			//titleKey = "STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.TITLE";
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			ConnectReference();
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
		}

		protected ShapeProjectorLogic Target;

		protected GameObject rowPrefab;
		protected GameObject listContainer;
		protected Transform AddNewButtonTransform;
		private readonly List<ProjectionShapeUIEntry> RowEntries = new();
		private FColorPickerArray ColorPicker;
		//private Dictionary<DockingSpacecraftHandler, DockingHandlerEntry> DockingTargets = new();
		//private Dictionary<DockingSpacecraftHandler, RectTransform> Rotatings = new Dictionary<DockingSpacecraftHandler, RectTransform>();

		public override void ClearTarget()
		{
			Target = null;
			base.ClearTarget();
		}

		public override void SetTarget(GameObject target)
		{
			base.SetTarget(target);
			if (!target.TryGetComponent(out Target))
			{
				SgtLogger.l("Target is not a ShapeProjector");
				return;
			}
			ConnectReference();
			Refresh();
		}


		bool refsConnected = false;
		void ConnectReference()
		{
			if (refsConnected)
				return;
			refsConnected = true;
			rowPrefab = transform.Find("ContentContainer/ScrollRectContainer/ItemPrefab").gameObject;
			listContainer = transform.Find("ContentContainer/ScrollRectContainer").gameObject;
			//var layout = transform.Find("Content/ContentScrollRect/Scrollbar").GetComponent<LayoutElement>();
			//SgtLogger.debuglog(String.Format("{0}, {1}, {2}", layout.minHeight, layout.preferredHeight, layout.flexibleHeight));
			//layout.minHeight = 150f;
			rowPrefab.SetActive(false);
			rowPrefab.AddComponent<ProjectionShapeUIEntry>();


			var addNewButton = transform.Find("AddNewButton").gameObject.AddOrGet<FButton>();
			addNewButton.OnClick += AddNewShapeTarget;
			AddNewButtonTransform = addNewButton.transform;
			//noChannelRow = transform.Find("Content/ContentScrollRect/RowContainer/Rows/NoChannelRow").gameObject;
			ColorPicker = transform.Find("ColorPicker").gameObject.AddOrGet<FColorPickerArray>();
			ColorPicker.OnColorChange += SetCurrentProjectorTint;


			//TryChangeText(noChannelRow.transform, "Labels/Label", "Nothing");
			//TryChangeText(noChannelRow.transform, "Labels/DescLabel", "Nothing to dock to.");

		}
		void SetCurrentProjectorTint(Color tint)
		{
			Target?.SetHighlightColor(tint);
		}
		void AddNewShapeTarget()
		{
			var shape = Target?.AddNewShape();
			if (shape != null)
			{
				MakeNewShapeEntry(shape);
			}
		}

		private void Refresh()
		{
			foreach (var currentEntry in RowEntries)
			{
				currentEntry.gameObject.SetActive(false);
			}
			int count = Target.CurrentShapeCount();
			int currentCount = RowEntries.Count;
			for (int index = 0; index < count; ++index)
			{
				var shape = Target.GetShapeData(index);
				ProjectionShapeUIEntry entry;
				if (index < currentCount)
				{
					entry = RowEntries[index];
					entry.gameObject.SetActive(true);
					entry.UpdateDisplayData(Target, shape);
				}
				else
				{
					MakeNewShapeEntry(shape);
				}
			}
			ColorPicker.SetSelected(Target.HighlightColor);
		}
		ProjectionShapeUIEntry MakeNewShapeEntry(ShapeProjectorLogic.ShapeData shapeData)
		{
			GameObject RowEntry = Util.KInstantiateUI(rowPrefab, listContainer);
			var entry = RowEntry.AddOrGet<ProjectionShapeUIEntry>();
			entry.gameObject.SetActive(true);
			entry.Init();
			entry.UpdateDisplayData(Target, shapeData);
			RowEntries.Add(entry);
			return entry;
		}
	}
}
