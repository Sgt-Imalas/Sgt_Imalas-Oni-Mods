using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SaveGameModLoader.ModFilter
{
    internal class OnHoverReveal : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject Target;
        public bool ShouldToggle = true;



        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (ShouldToggle && Target != null)
                Target.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(ShouldToggle && Target != null)
                Target.SetActive(false);
        }
    }
}
