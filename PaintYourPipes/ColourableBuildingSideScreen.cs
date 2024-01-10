using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using YamlDotNet.Core.Tokens;

namespace PaintYourPipes
{
    internal class ColourableBuildingSideScreen
    {
        public static GameObject colorPickerContainerPrefab, colorPickerSwatchEntryPrefab;
        public static List<Color> SwatchColors;
        public static Dictionary<Tuple<float,float,float>, GameObject> SwatchColorsHighlightsDictionary;

        public static ColourableBuilding Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }
        private static ColourableBuilding _target;

        public static bool ColorInOverlay = false;

        private static GameObject screenInstance = null;
        private static void InitUI()
        {
            if (screenInstance != null)
                return;

            screenInstance = Util.KInstantiateUI(colorPickerContainerPrefab);
            screenInstance.SetActive(true);
            screenInstance.transform.SetPosition(screenInstance.transform.GetPosition() + new Vector3(0.0f, 75f, 0.0f));
            SwatchColorsHighlightsDictionary = new Dictionary<Tuple<float, float, float>, GameObject>();

            for (int index = 0; index < SwatchColors.Count; ++index)
            {
                GameObject SwatchGO = Util.KInstantiateUI(colorPickerSwatchEntryPrefab, screenInstance, true);
                SwatchGO.TryGetComponent<Image>(out var image);
                var color = SwatchColors[index];
                image.color = color;
                SwatchGO.TryGetComponent<KButton>(out var button);
                if (SwatchGO.TryGetComponent<HierarchyReferences>(out var references))
                {

                    SwatchColorsHighlightsDictionary[new(color.r,color.g,color.b)] = references.GetReference("selected").gameObject;
                }

                button.onClick += (() =>
                {
                    if (Target != null)
                        Target.SetColor(color);
                    RefreshActiveIndicators();
                });
            }
        }

        static void RefreshActiveIndicators()
        {
            if (SwatchColorsHighlightsDictionary == null)
                return;

            foreach(var key in SwatchColorsHighlightsDictionary.Keys)
            {
                SwatchColorsHighlightsDictionary[key].SetActive(false);
            }
            if (Target == null)
                return;

            var colorTuple = new Tuple<float, float, float>(Target.TintColor.r, Target.TintColor.g, Target.TintColor.b);
            if (SwatchColorsHighlightsDictionary.ContainsKey(colorTuple))
            {
                SwatchColorsHighlightsDictionary[colorTuple].SetActive(true);
            }
        }



        public static void RefreshUIState(Transform parentTarget)
        {
            InitUI();
            screenInstance.transform.SetParent(parentTarget, false);

            if (Target != null)
            {
                screenInstance.SetActive(true); 
                RefreshActiveIndicators();
            }
            else
            {

                screenInstance.SetActive(false);
            }
        }
        public static void Destroy()
        {
            UnityEngine.Object.Destroy(screenInstance);
            screenInstance = null;
        }
    }
}
