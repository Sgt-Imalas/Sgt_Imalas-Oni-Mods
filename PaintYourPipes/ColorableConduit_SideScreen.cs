using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using YamlDotNet.Core.Tokens;

namespace PaintYourPipes
{
    internal class ColorableConduit_SideScreen
    {
        public static GameObject colorPickerContainerPrefab, colorPickerSwatchEntryPrefab;
        public static List<Color> SwatchColors;
        public static Dictionary<Tuple<float,float,float>, GameObject> SwatchColorsHighlightsDictionary;

        public static ColorableConduit Target
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
        private static ColorableConduit _target;

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

                    SgtLogger.l("onclick");
                    if (Target != null)
                        Target.SetColor(color);
                    RefreshActiveIndicators();
                }); 
                button.onDoubleClick += (() =>
                {
                    SgtLogger.l("ondouble");
                    if (Target != null)
                        Target.SetSecondaryColor(color);
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

            Tuple<float, float, float> colorTupleSecondary=null;
            var sec = Target.GetSecondaryColor();
            if (sec.HasValue)
                colorTupleSecondary = new Tuple<float, float, float>((sec).Value.r, (sec).Value.g, (sec).Value.b);
            if (SwatchColorsHighlightsDictionary.ContainsKey(colorTuple))
            {
                var entry = SwatchColorsHighlightsDictionary[colorTuple];
                if(entry.TryGetComponent<Image>(out var img))
                {
                    img.color = Color.white;
                }
                
                SwatchColorsHighlightsDictionary[colorTuple].SetActive(true);
            }
            if (
                Target.TintColor.Equals(Target.SecondaryTintColor) ||
                !Target.SecondaryTintColor.HasValue)
                return;

            if (colorTupleSecondary != null && SwatchColorsHighlightsDictionary.ContainsKey(colorTupleSecondary))
            {
                var entry = SwatchColorsHighlightsDictionary[colorTupleSecondary];
                if (entry.TryGetComponent<Image>(out var img))
                {
                    img.color = Color.gray;
                }
                SwatchColorsHighlightsDictionary[colorTupleSecondary].SetActive(true);
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
