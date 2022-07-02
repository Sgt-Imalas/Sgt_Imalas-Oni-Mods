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
        public static void ListChildren(Transform parent, int level = 0, int maxDepth = 10)
        {
            if (level >= maxDepth) return;

            foreach (Transform child in parent)
            {
                Console.WriteLine(string.Concat(Enumerable.Repeat('-', level)) + child.name);
                ListChildren(child, level + 1);
            }
        }

        public static void ListChildrenHR(HierarchyReferences parent, int level = 0, int maxDepth = 10)
        {
            if (level >= maxDepth) return;

            foreach (var child in parent.references)
            {
                Console.WriteLine(string.Concat(Enumerable.Repeat('-', level)) + child.Name +", " + child.behaviour.ToString());
                ListChildren(child.behaviour.transform,level+1);
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
