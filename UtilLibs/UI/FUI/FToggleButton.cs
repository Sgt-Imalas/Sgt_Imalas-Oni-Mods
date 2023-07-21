using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UtilLibs.UIcmp;
using UnityEngine.UI;
using static STRINGS.BUILDING.STATUSITEMS.ACCESS_CONTROL;
using static Operational;

namespace UtilLibs.UI.FUI
{
    public class FToggleButton : KMonoBehaviour, IEventSystemHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event System.Action OnClick;
        public event System.Action OnDoubleClick;

        public event System.Action OnPointerEnterAction;
        public event System.Action OnPointerExitAction;

        private bool interactable;
        private Material material;
        
        [MyCmpGet]
        private Image image;

        [MyCmpReq]
        private Button button;

        [SerializeField]
        bool IsHighlighted = false;


        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            if (image == null && button != null)
            {
                image = button.image;
            }


            material = image.material;
            interactable = true;
        }

        public void SetInteractable(bool interactable)
        {
            if (interactable == this.interactable)
            {
                return;
            }

            this.interactable = interactable;
            button.interactable = interactable;
        }

        

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                //PlaySound(UISoundHelper.ClickOpen);
                if (!eventData.IsPointerMoving())
                {
                    ToggleSelection();
                    if (OnClick != null)
                    {
                        OnClick?.Invoke();
                    }
                }
            }
        }


        public void ToggleSelection()
        {
            IsHighlighted = !IsHighlighted;
            ChangeSelection(IsHighlighted);
        }
        public void ChangeSelection(bool _isHighlighted = false)
        {

            IsHighlighted = _isHighlighted;
            SetSelectedState();
        }
        void SetSelectedState()
        {
            if (IsHighlighted)
                button.OnSelect(null);
            else
                button.OnDeselect(null);
        }



        public void OnPointerEnter(PointerEventData eventData)
        {
            if (OnPointerEnterAction != null)
                OnPointerEnterAction.Invoke();

            if (!interactable)
            {
                return;
            }

            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                PlaySound(UISoundHelper.MouseOver);
            }
            SetSelectedState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (OnPointerExitAction != null)
                OnPointerExitAction.Invoke();
            SetSelectedState();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }

            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                PlaySound(UISoundHelper.ClickOpen);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnDoubleClick != null && eventData.clickCount == 2)
            {
                OnDoubleClick.Invoke();
            }
        }
    }
}
