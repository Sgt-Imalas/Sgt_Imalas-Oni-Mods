/*
 * Copyright 2023 Peter Han
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using UnityEngine;

namespace MassMoveTo.Tools.SweepByType
{
	/// <summary>
	/// A version of ClearTool (sweep) that allows filtering.
	/// </summary>
	public sealed class FilteredMoveSelectTool : DragTool
	{
		protected FilteredClearHover HoverCard;

		/// <summary>
		/// The singleton instance of this tool.
		/// </summary>
		public static FilteredMoveSelectTool Instance { get; private set; }

		// Detours for private fields in InterfaceTool
		private static readonly IDetouredField<DragTool, GameObject> AREA_VISUALIZER =
			PDetours.DetourField<DragTool, GameObject>("areaVisualizer");
		private static readonly IDetouredField<DragTool, GameObject> AREA_VISUALIZER_TEXT_PREFAB =
			PDetours.DetourField<DragTool, GameObject>("areaVisualizerTextPrefab");
		private static readonly IDetouredField<DragTool, Texture2D> BOX_CURSOR =
			PDetours.DetourField<DragTool, Texture2D>("boxCursor");
		private static readonly IDetouredField<InterfaceTool, Texture2D> CURSOR =
			PDetours.DetourField<InterfaceTool, Texture2D>(nameof(cursor));
		private static readonly IDetouredField<InterfaceTool, GameObject> VISUALIZER =
			PDetours.DetourField<InterfaceTool, GameObject>(nameof(visualizer));

		/// <summary>
		/// Destroys the singleton instance.
		/// </summary>
		internal static void DestroyInstance()
		{
			var inst = Instance;
			if (inst != null)
			{
				Destroy(inst);
				Instance = null;
			}
		}

		/// <summary>
		/// Whether "all" was selected.
		/// </summary>
		private bool cachedAll;

		/// <summary>
		/// The types to sweep.
		/// </summary>
		private HashSetPool<Tag, FilteredMoveSelectTool>.PooledHashSet cachedTypes;

		/// <summary>
		/// Allows selection of the type to sweep.
		/// </summary>
		internal TypeSelectControl TypeSelect { get; private set; }

		internal FilteredMoveSelectTool()
		{
			cachedAll = false;
			cachedTypes = null;
			TypeSelect = null;
		}

		/// <summary>
		/// Destroys the cached list after a drag completes.
		/// </summary>
		private void DoneDrag()
		{
			if (cachedTypes != null)
			{
				cachedAll = false;
				cachedTypes.Recycle();
				cachedTypes = null;
			}
		}

		/// <summary>
		/// Marks a debris item to be swept if it matches the filters.
		/// </summary>
		/// <param name="content">The item to sweep.</param>
		/// <param name="priority">The priority to set the sweep errand.</param>
		private void MarkForMove(GameObject content, PrioritySetting priority)
		{
			if (content.TryGetComponent<Movable>(out var movable)
				&& !movable.IsMarkedForMove  //check if not already marked
				&& (cachedAll || cachedTypes.Contains(content.PrefabID()))) //check if the filter type is selected
			{
				ModAssets.MarkForMove(movable, priority);
			}
		}

		public override void OnActivateTool()
		{
			ModAssets.ClearStashed();
			var menu = ToolMenu.Instance;
			base.OnActivateTool();
			// Update only on tool activation to improve performance
			if (TypeSelect != null)
			{
				var root = TypeSelect.RootPanel;
				bool firstTime = TypeSelect.CategoryCount < 1;
				TypeSelect.Update();
				if (firstTime)
				{
					var savedTypes = SaveGame.Instance?.GetComponent<SavedTypeSelections>()?.
						GetSavedTypes();
					// First time, load the user's old settings if available
					// If nothing at all is selected, then this is probably the first ever load
					if (savedTypes != null && savedTypes.Count > 0)
						TypeSelect.SetSelections(savedTypes);
				}
				root.SetParent(menu.gameObject);
				root.transform.SetAsFirstSibling();
				root.SetActive(true);
			}
			menu.PriorityScreen.Show(true);
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			// Clean up everything needed
			if (TypeSelect != null)
			{
				Destroy(TypeSelect.RootPanel);
				TypeSelect = null;
			}
			DoneDrag();
		}

		public override void OnDeactivateTool(InterfaceTool newTool)
		{
			var menu = ToolMenu.Instance;
			// Unparent but do not dispose
			if (TypeSelect != null)
				TypeSelect.RootPanel.SetActive(false);
			menu.PriorityScreen.Show(false);
			base.OnDeactivateTool(newTool);
		}

		public override void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp)
		{
			DoneDrag();
			if (ModAssets.HasStashed)
			{
				this.DeactivateTool();
				TargetSelectTool.Instance.Activate();
			}
		}

		public override void OnDragTool(int cell, int distFromOrigin)
		{
			//ignore entombed items
			if (Grid.Solid[cell])
				return;

			var gameObject = Grid.Objects[cell, (int)ObjectLayer.Pickupables];
			if (gameObject != null && TypeSelect != null)
			{
				// item in a linked list of all debris in layer 3/ObjectLayer.Pickupables
				var objectListNode = gameObject.GetComponent<Pickupable>().objectLayerListItem;
				var priority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
				if (cachedTypes == null)
				{
					// Build the list
					cachedTypes = HashSetPool<Tag, FilteredMoveSelectTool>.Allocate();
					TypeSelect.AddTypesToSweep(cachedTypes);
					cachedAll = TypeSelect.IsAllSelected;
				}
				while (objectListNode != null)
				{
					var content = objectListNode.gameObject;
					objectListNode = objectListNode.nextItem;
					// Ignore Duplicants
					if (content != null && content.GetComponent<MinionIdentity>() == null)
						MarkForMove(content, priority);
				}
			}
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			var inst = ClearTool.Instance;
			var movetool = MoveToLocationTool.Instance;
			Instance = this;
			interceptNumberKeysForPriority = true;
			populateHitsList = true;
			// Get the cursor from the existing sweep tool
			if (inst != null)
			{
				HoverCard = gameObject.AddComponent<FilteredClearHover>();
				cursor = CURSOR.Get(inst);
				BOX_CURSOR.Set(this, cursor);
				// Get the area visualizer from the sweep tool
				var avTemplate = AREA_VISUALIZER.Get(inst);
				if (avTemplate != null)
				{
					var areaVisualizer = Util.KInstantiate(avTemplate, gameObject,
						"FilteredMassMoveToolAreaVisualizer");
					areaVisualizer.SetActive(false);
					areaVisualizerSpriteRenderer = areaVisualizer.GetComponent<
						SpriteRenderer>();
					// The visualizer is private so we need to set it with reflection
					AREA_VISUALIZER.Set(this, areaVisualizer);
					AREA_VISUALIZER_TEXT_PREFAB.Set(this, AREA_VISUALIZER_TEXT_PREFAB.Get(
						inst));
				}
				visualizer = Util.KInstantiate(VISUALIZER.Get(inst), gameObject,
					"FilteredMassMoveToolSprite");
				visualizer.SetActive(false);
			}

			TypeSelect = new TypeSelectControl(false);
		}
	}
}
