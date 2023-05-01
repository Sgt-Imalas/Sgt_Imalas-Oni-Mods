using HarmonyLib;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Database.MonumentPartResource;
using static DetailsScreen;

namespace UtilLibs
{
    public class UIUtils
    {

        public static void GiveAllChildObjects(GameObject start)
        {
            var SubObjects = start.GetComponentsInChildren<UnityEngine.Object>(); //finding the pesky tooltip; maybe usefull l8er
            foreach (var v in SubObjects)
            {
                Debug.Log(v);
            }
        }

        public static Color rgb(float r, float g, float b)
        {
            return new Color(r / 255, g / 255, b / 255);
        }
        public static Color rgba(float r, float g, float b,double a)
        {
            return new Color(r / 255, g / 255, b / 255,(float) a);
        }

        public static Color Lighten(Color original, float percentage)
        {
            return Color.Lerp(original, Color.white, percentage/100f);// To lighten by 50% 
        }
        public static Color Darken(Color original, float percentage)
        {
            return Color.Lerp(original, Color.black, percentage / 100f);// To lighten by 50% 
        }


        /// <summary>
        /// Adds an Action to a click on a button.
        /// </summary>
        /// <param name="parent">Transform parent where the button is located</param>
        /// <param name="subCompName">child component path</param>
        /// <param name="action">Action to add to the button click</param>
        /// <param name="clearOnclick">Should the previous OnClick Actions be discarded?</param>
        /// <returns></returns>
        public static bool AddActionToButton(Transform parent, string subCompName, System.Action action, bool clearOnclick = true)
        {
            var comp = parent.Find(subCompName);
            if (comp == null)
                return false;
            var button = TryFindComponent<KButton>(comp);
            if (button == null) return false;
            if(clearOnclick)
                button.ClearOnClick();
            button.onClick += action;
            return true;
        }
        public static T TryFindComponent<T> (Transform parent, string subCompName="")
        {
            Transform BtTransform;
            if (subCompName != "")
                BtTransform = parent.Find(subCompName);
            else
                BtTransform = parent;
            if (BtTransform == null)
                return default(T);
            BtTransform.TryGetComponent<T>(out var button);
            return button;
        }

        public static bool TryChangeText(Transform transform, string subCompName, string newText)
        {
            Transform textToChangeComp;
            if (subCompName != "")
                textToChangeComp = transform.Find(subCompName);
            else
                textToChangeComp = transform;

            if (textToChangeComp == null)
                return false;
            var textToChange = textToChangeComp.GetComponent<LocText>();
            if (textToChange == null)
                return false;
            textToChange.key = string.Empty;
            textToChange.text = newText;
           
            return true;
        }
        public static ToolTip AddSimpleTooltipToObject(Transform go, string tooltip, bool alignCenter = false, float wrapWidth = 0, bool onBottom = false)
        {
            if (go == null)
                return null;
            var tt = go.gameObject.AddOrGet<ToolTip>();
            tt.UseFixedStringKey = false;
            tt.enabled = true;
            tt.tooltipPivot = alignCenter ? new Vector2(0.5f, onBottom? 1f : 0f) : new Vector2(1f, onBottom ? 1f : 0f);
            tt.tooltipPositionOffset = onBottom ? new Vector2(0f, -20f) : new Vector2(0f, 20f);
            tt.parentPositionAnchor = new Vector2(0.5f, 0.5f);
            if (wrapWidth > 0)
            {
                tt.WrapWidth = wrapWidth;
                tt.SizingSetting = ToolTip.ToolTipSizeSetting.MaxWidthWrapContent;
            }
            ToolTipScreen.Instance.SetToolTip(tt);
            tt.SetSimpleTooltip(tooltip);
            return tt;
        }

        public static void RemoveSimpleTooltipOnObject(Transform go)
        {
            if (go == null)
                return;
            if(go.gameObject.TryGetComponent<ToolTip>(out var tt))
                GameObject.Destroy(tt);
        }
        public static Transform GetShellWithoutFunction(Transform origin, string subCompName = "", string copyName = "copy")
        {

            var toCopy = origin.Find(subCompName);
            if (toCopy == null)
                return null;
            var window = Util.KInstantiateUI(toCopy.gameObject);
            window.SetActive(false);
            var copy = window.transform;
            UnityEngine.Object.Destroy(window);
            var newScreen = Util.KInstantiateUI(copy.gameObject, origin.gameObject, true);
            newScreen.name = copyName;

            return newScreen.transform;
        }

        public static Transform TryInsertNamedCopy(Transform parent, string subCompName = "", string copyName = "copy")
        {
            var toCopy = parent.Find(subCompName);
            if (toCopy == null)
                return null;
            var copy = Util.KInstantiateUI(toCopy.transform.gameObject, parent.gameObject,true);
            copy.name = copyName;
            return copy.transform;
        }


        public static bool FindAndRemove<T>(Transform parent, string subCompName="")
        {

            var toRemove = TryFindComponent<T>(parent, subCompName) as UnityEngine.Object;
            if(toRemove != null)
            {
#if DEBUG
                Debug.Log("Removing " + subCompName);
#endif
                UnityEngine.Object.Destroy(toRemove);
                return true;
            }
            return false;
        }
        public static bool FindAndDisable(Transform parent, string name)
        {
#if DEBUG
            //Debug.Log("Disabling " + name);
#endif
            var toDisable = parent.Find(name);
            if (toDisable == null)
                return false;
            toDisable.gameObject.SetActive(false);
            return true;
        }
        public static bool FindAndDestroy(Transform parent, string name)
        {
#if DEBUG
            //Debug.Log("Disabling " + name);
#endif
            var toDisable = parent.Find(name);
            if (toDisable == null)
                return false;
            UnityEngine.Object.Destroy(toDisable.gameObject);
            return true;
        }

        public static void ListComponents(GameObject instance)
        {
            int count = instance.GetComponents(typeof(Component)).Count();
            Console.WriteLine(count + " objects found");
            foreach (var comp in instance.GetComponents(typeof(UnityEngine.Object)))
            {
                if (comp != null)
                {
                    Console.WriteLine("Type: " +comp.GetType().ToString() + ", Name ->" + comp.name);
                }
            }
        }

        public static void ListAllChildren(Transform parent, int level = 0, int maxDepth = 10)
        {
            if (level >= maxDepth) return;

            foreach (Transform child in parent)
            {
                Console.WriteLine(string.Concat(Enumerable.Repeat('-', level)) + child.name);
                ListAllChildren(child, level + 1);
            }
        }
        public static void ListAllChildrenPath(Transform parent,string path = "/" ,int level = 0, int maxDepth = 10)
        {
            if (level >= maxDepth) return;

            foreach (Transform child in parent)
            {
                var newpath = string.Concat(path + child.name + "/");
                Console.WriteLine(newpath);
                ListAllChildrenPath(child, newpath, level + 1);
            }
        }




        public static void ListAllChildrenWithComponents(Transform parent, int level = 0, int maxDepth = 10)
        {
            if (level >= maxDepth) return;

            ListComponents(parent.gameObject);
            foreach (Transform child in parent)
            {
                Console.WriteLine(string.Concat(Enumerable.Repeat('-', level)) + child.name);
                ListAllChildrenWithComponents(child, level + 1);
            }
        }

        public static void ListChildrenHR(HierarchyReferences parent, int level = 0, int maxDepth = 10)
        {
            if (level >= maxDepth) return;

            foreach (var child in parent.references)
            {
                Console.WriteLine(string.Concat(Enumerable.Repeat('-', level)) + child.Name +", " + child.behaviour.ToString());
                ListAllChildren(child.behaviour.transform,level+1);
            }
        }

        /// <summary>
        /// Create sidescreen by cloning an already existing one.
        /// </summary>
        public static GameObject AddClonedSideScreen<T>(string name, string originalName, Type originalType)
        {
            bool elementsReady = GetElements(out List<SideScreenRef> screens, out GameObject contentBody);
            if (elementsReady)
            {
                var oldPrefab = FindOriginal(originalName, screens);
                var newPrefab = Copy<T>(oldPrefab, contentBody, name, originalType);

                screens.Add(NewSideScreen(name, newPrefab));
                return contentBody;
            }
            return null;
        }

        /// <summary>
        /// Create sidescreen from a custom prefab.
        /// </summary>
        public static void AddCustomSideScreen<T>(string name, GameObject prefab)
        {
            bool elementsReady = GetElements(out List<SideScreenRef> screens, out GameObject contentBody);
            if (elementsReady)
            {
                var newScreen = prefab.AddComponent(typeof(T)) as SideScreenContent;
                screens.Add(NewSideScreen(name, newScreen));
            }
        }

        private static bool GetElements(out List<SideScreenRef> screens, out GameObject contentBody)
        {
            var detailsScreen = Traverse.Create(DetailsScreen.Instance);
            screens = detailsScreen.Field("sideScreens").GetValue<List<SideScreenRef>>();
#if DEBUG
            foreach (var v in screens)
            {
                //Debug.Log(v.name);
            }
#endif
            contentBody = detailsScreen.Field("sideScreenContentBody").GetValue<GameObject>();
            return screens != null && contentBody != null;
        }

        private static SideScreenContent FindOriginal(string name, List<SideScreenRef> screens)
        {
            var result = screens.Find(s => s.name == name).screenPrefab;

            if (result == null)
                SgtLogger.logwarning("Could not find a sidescreen with the name " + name, "SgtImalasUtils");

            return result;
        }

        private static SideScreenContent Copy<T>(SideScreenContent original, GameObject contentBody, string name, Type originalType)
        {
            var screen = Util.KInstantiateUI<SideScreenContent>(original.gameObject, contentBody).gameObject;
            UnityEngine.Object.Destroy(screen.GetComponent(originalType));

            var prefab = screen.AddComponent(typeof(T)) as SideScreenContent;
            prefab.name = name.Trim();

            screen.SetActive(false);
            return prefab;
        }

        private static SideScreenRef NewSideScreen(string name, SideScreenContent prefab)
        {
            return new SideScreenRef
            {
                name = name,
                offset = Vector2.zero,
                screenPrefab = prefab
            };
        }

        public static readonly List<Color> RainbowColors = new List<Color> {
                Color.HSVToRGB(0,1,1 ),
                Color.HSVToRGB(45f/360f,1,1 ),
                Color.HSVToRGB(90f/360f,1,1 ),
                Color.HSVToRGB(135f/360f,1,1 ),
                Color.HSVToRGB(180f/360f,1,1 ),
                Color.HSVToRGB(225f/360f,1,1 ),
                Color.HSVToRGB(270f/360f,1,1 ),
                Color.HSVToRGB(315f/360f,1,1 )
            };
        public static readonly List<Color> RainbowColorsDesaturated = new List<Color> {
                Color.HSVToRGB(0,0.65f,1 ),
                Color.HSVToRGB(45f/360f,0.65f,1 ),
                Color.HSVToRGB(90f/360f,0.65f,1 ),
                Color.HSVToRGB(135f/360f,0.65f,1 ),
                Color.HSVToRGB(180f/360f,0.65f,1 ),
                Color.HSVToRGB(225f/360f,0.65f,1 ),
                Color.HSVToRGB(270f/360f,0.65f,1 ),
                Color.HSVToRGB(315f/360f,0.65f,1 )
            };


        public static Color number_green = Util.ColorFromHex("367d48");
        public static Color number_red = Util.ColorFromHex("802024");


        public static string ColorNumber(float number, bool invert = false)
        {
            if (number > 0)
                return ColorText(number.ToString(), invert ? number_red : number_green);
            else if (number < 0)
                return ColorText(number.ToString(), !invert ? number_red : number_green);

            return number.ToString();
        }

        public static string ColorText(string text, string hex)
        {
            hex = hex.Replace("#", string.Empty);
            return "<color=#" + hex + ">" + text + "</color>";
        }
        public static string ColorText(string text, Color color)
        {
            return ColorText(text, Util.ToHexString(color));
        }
        public static string RainbowColorText(string text, bool useTrueColors = false)
        {
            var CharArray = text.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < CharArray.Length; i++)
            {
                sb.Append(ColorText(CharArray[i].ToString(), useTrueColors ? RainbowColors[i % RainbowColors.Count]: RainbowColorsDesaturated[i % RainbowColorsDesaturated.Count]));
            }
            return sb.ToString();
            
        }
    }
}
