using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs;
using static ClusterTraitGenerationManager.UI.SO_StarmapEditor.HexGrid;
using static ClusterTraitGenerationManager.UI.SO_StarmapEditor.StarmapToolkit;

namespace ClusterTraitGenerationManager.UI.SO_StarmapEditor
{
	internal class DeleteDragHandler : KMonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		public HexGrid hexGrid;
		public Image highlight, secondaryHighlight = null;
		public Color normal = UIUtils.Darken(Color.red, 25f), highlighted = Color.red;
		public void OnDrop(PointerEventData eventData)
		{
			if (eventData.pointerDrag.TryGetComponent(out HexDrag hexDragger) && hexDragger.IsPOI
				//|| eventData.pointerDrag.TryGetComponent(out ToolkitDraggable _)
				)

			{
				hexGrid.RemovePOI(hexDragger);
				Destroy(hexDragger.gameObject);
			}
			highlight.color = normal;
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (highlight == null)
				TryGetComponent<Image>(out highlight);

			highlight.color = normal;
			if (secondaryHighlight != null) secondaryHighlight.color = normal;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (eventData.dragging && eventData.pointerDrag.TryGetComponent(out HexDrag hexDragger) && hexDragger.IsPOI
				|| eventData.dragging && eventData.pointerDrag.TryGetComponent(out ToolkitDraggable _)
				|| hexGrid.CurrentlySimDragged != null && hexGrid.CurrentlySimDragged.IsPOI
				|| hexGrid.CurrentlySimDraggedNew != null
				)
			{
				highlight.color = highlighted;
				if (secondaryHighlight != null) secondaryHighlight.color = highlighted;
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			highlight.color = normal;
			if (secondaryHighlight != null) secondaryHighlight.color = normal;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (eventData.clickCount == 2)
				{
					hexGrid.OnDoubleClickSimDragDeletedHandler();
					OnPointerExit(null);
				}
			}
		}
	}
}
