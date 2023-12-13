using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs;
using static AnimEventHandler;
using static ClusterTraitGenerationManager.SO_StarmapEditor.HexGrid;
using static Database.MonumentPartResource;

namespace ClusterTraitGenerationManager.SO_StarmapEditor
{
    internal class DeleteDragHandler : KMonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public HexGrid hexGrid;
        public Image highlight, secondaryHighlight = null; 
        public Color normal = UIUtils.Darken(Color.red,25f), highlighted = Color.red;
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponent(out HexDrag hexDragger) && hexDragger.IsPOI)
            {
                hexGrid.RemovePOI(hexDragger);
                Destroy(hexDragger.gameObject);
                highlight.color = normal;
            }
        }
        public override void OnSpawn()
        {
            base.OnSpawn();
            if (highlight == null)
                TryGetComponent<Image>(out highlight);

            highlight.color = normal;
            if(secondaryHighlight != null) secondaryHighlight.color = normal;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(eventData.dragging && eventData.pointerDrag.TryGetComponent(out HexDrag hexDragger) && hexDragger.IsPOI)
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
    }
}
