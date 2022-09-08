using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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

        /// <summary>
        /// Adds an Action to a click on a button.
        /// </summary>
        /// <param name="parent">Transform parent where the button is located</param>
        /// <param name="subCompName">child component path</param>
        /// <param name="action">Action to add to the button click</param>
        /// <param name="clearOnclick">Should the previous OnClick Actions be discarded?</param>
        /// <returns></returns>
        public static bool AddActionToButton(Transform parent, string subCompName, System.Action action, bool clearOnclick = false)
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
            var button = BtTransform.GetComponent<T>();
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
            textToChange.text = newText;
           
            return true;
        }
        public static Transform TryInsertNamedCopy(Transform parent, string subCompName = "", string copyName = "copy")
        {
            var toCopy = parent.Find(subCompName);
            if (toCopy == null)
                return null;
            var copy = Util.KInstantiateUI(toCopy.gameObject, parent.gameObject,true);
            copy.name = copyName;
            return copy.transform;
        }

        public static bool FindAndDisable(Transform parent, string name)
        {
#if DEBUG
            Debug.Log("Disabling " + name);
#endif
            var toDisable = parent.Find(name);
            if (toDisable == null)
                return false;
            toDisable.gameObject.SetActive(false);
            return true;
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
            foreach (var v in screens)
            {
               // Debug.Log(v.name);
            }
            contentBody = detailsScreen.Field("sideScreenContentBody").GetValue<GameObject>();
            return screens != null && contentBody != null;
        }

        private static SideScreenContent FindOriginal(string name, List<SideScreenRef> screens)
        {
            var result = screens.Find(s => s.name == name).screenPrefab;

            if (result == null)
                Debug.LogWarning("Could not find a sidescreen with the name " + name);

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
    }
}
