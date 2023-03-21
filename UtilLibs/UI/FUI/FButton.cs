using EventSystem2Syntax;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UtilLibs.UIcmp //Source: Aki
{
    public class FButton : KMonoBehaviour, IEventSystemHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event System.Action OnClick;

        private bool interactable;
        private Material material;

        [MyCmpReq]
        private Image image;

        [MyCmpGet]
        private Button button;

        [SerializeField]
        public Color disabledColor = new Color(0.78f, 0.78f, 0.78f);

        [SerializeField]
        public Color normalColor = new Color(0.243f, 0.263f, 0.341f);

        [SerializeField]
        public Color hoverColor = new Color(0.345f, 0.373f, 0.702f);

        private bool allowRightClick = false;
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            material = image.material;
            interactable = true;
        }

        public void SetInteractable(bool interactable)
        {
            if(interactable == this.interactable)
            {
                return;
            }

            this.interactable = interactable;
            //image.material = interactable ? material : global::Assets.instance.UIPrefabAssets.TableScreenWidgets.DesaturatedUIMaterial;
            if(button == null)
            {
                image.color = interactable ? normalColor : disabledColor;
            }
            else
            {
                button.interactable = interactable;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(!interactable)
            {
                return;
            }
            SgtLogger.l(eventData.button.ToString(), "BUTTTTTTTTT");
            if (!allowRightClick && eventData.button != PointerEventData.InputButton.Left)
                return;

            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                PlaySound(UISoundHelper.ClickOpen);
                OnClick?.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }

            if (KInputManager.isFocused)
            {
                if(button == null)
                {
                    image.color = hoverColor;
                }

                KInputManager.SetUserActive();
                PlaySound(UISoundHelper.MouseOver);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(button == null)
            {
                image.color = normalColor;
            }
        }
    }
}
