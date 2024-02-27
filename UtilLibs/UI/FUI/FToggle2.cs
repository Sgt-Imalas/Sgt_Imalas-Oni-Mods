using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UtilLibs.UIcmp //Source: Aki
{
    public class FToggle2 : KMonoBehaviour, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler
    {
        [SerializeField]
        public Image mark;

        public event System.Action OnClick;
        public event System.Action OnChange;

        private bool _interactable;

        public bool Interactable => _interactable;

        public void SetInteractable(bool interactable)
        {
            _interactable = interactable;
            if(mark!= null)
            {
                mark.color = _interactable ? Color.white : new Color(1,1,1,0.5f);
            }

        }

        private bool on;

        public bool On
        {
            get => on;
            set
            {
                on = value;
                if (mark != null)
                {
                    mark.enabled = value;
                }

                OnChange?.Invoke();
            }
        }

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();

            if(mark==null)
                mark = gameObject.GetComponentInChildren<Image>();
        }

        public void SetCheckmark(string path)
        {
            mark = transform.Find(path).GetComponent<Image>();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
        }

        public void Toggle() => On = !On;
        public void SetOn(bool toggleOn) => On = toggleOn;

        public void OnPointerUp(PointerEventData eventData)
        {
            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                PlaySound(UISoundHelper.Click);
                Toggle();
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
