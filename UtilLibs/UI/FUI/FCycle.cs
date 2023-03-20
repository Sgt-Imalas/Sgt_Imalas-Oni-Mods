using System;
using System.Collections.Generic;
using UnityEngine;

namespace UtilLibs.UIcmp //Source: Aki
{
    public class FCycle : KMonoBehaviour
    {
        public event System.Action OnChange;

        [SerializeField]
        public FButton leftArrow;

        [SerializeField]
        public FButton rightArrow;

        [SerializeField]
        public LocText label;

        [SerializeField]
        public LocText description;

        private int currentIndex = 0;

        [SerializeField]
        public List<Option> Options;

        [Serializable]
        public class Option
        {
            public string id;
            public string title;
            public string description;

            public Option(string id, string title, string description = null)
            {
                this.id = id;
                this.title = title;
                this.description = description;
            }
        }

        public void Initialize(FButton leftButton, FButton rightButton, LocText label, LocText description = null)
        {
            leftArrow = leftButton;
            rightArrow = rightButton;

            this.label = label;
            this.description = description;

            leftArrow.OnClick += CycleLeft;
            rightArrow.OnClick += CycleRight;
        }


        public override void OnSpawn()
        {
            base.OnSpawn();
            UpdateLabel();
        }

        private bool HasOptions => Options.Count > 0;

        public string Value
        {
            get => Options.Count >= currentIndex ? Options[currentIndex].id : default;

            set
            {
                var index = Options.FindIndex(x => x.id == value);

                if (currentIndex == index)
                {
                    return;
                }

                if (index != -1)
                {
                    currentIndex = index;
                }
                else
                {
                 SgtLogger.warning($"Invalid option ID given \"{value}\"");
                    currentIndex = 0;
                }

                UpdateLabel();
            }
        }

        public void CycleLeft()
        {
            if (HasOptions)
            {
                currentIndex = (currentIndex + Options.Count - 1) % Options.Count;
                UpdateLabel();
                OnChange?.Invoke();
            }
        }

        public void CycleRight()
        {
            if (HasOptions)
            {
                currentIndex = (currentIndex + 1) % Options.Count;
                UpdateLabel();
                OnChange?.Invoke();
            }
        }

        public void UpdateLabel()
        {
            if (Options.Count >= currentIndex)
            {
                Value = Options[currentIndex].id;

                label.SetText(Options[currentIndex].title);

                if (description != null)
                {
                    description.SetText(Options[currentIndex].description);
                }
            }
        }
    }
}