using UnityEngine.EventSystems;

namespace ClusterTraitGenerationManager.UI.SO_StarmapEditor
{
	internal class StarmapCursorHandler : KMonoBehaviour, IPointerExitHandler, IPointerEnterHandler
	{
		public HexGrid grid;
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (grid != null)
			{
				grid.CursorInside = true;
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (grid != null)
			{
				grid.CursorInside = false;
			}
		}
	}
}
