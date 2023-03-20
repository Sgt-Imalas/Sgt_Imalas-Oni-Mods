using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UtilLibs.UIcmp //Source: Aki
{
    public class FToggle : KMonoBehaviour, IEventSystemHandler, IPointerDownHandler, IPointerEnterHandler
    {
        [MyCmpReq]
        public Toggle toggle;
        public event System.Action OnClick;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                PlaySound(UISoundHelper.Click);
                OnClick?.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                PlaySound(UISoundHelper.MouseOver);
            }
        }
    }
}
