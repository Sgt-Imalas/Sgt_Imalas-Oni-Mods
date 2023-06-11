using HarmonyLib;
using KSerialization;
using ProcGen;
using Rockets_TinyYetBig.SpaceStations;
using Rockets_TinyYetBig.TwitchEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using YamlDotNet.Core.Tokens;
using static STRINGS.UI.CLUSTERMAP;

namespace Rockets_TinyYetBig.Patches
{
    class WorldSelector_Patches
    {
        static List<GameObject> worldEntries = new List<GameObject>();
        public static Dictionary<int, GameObject> collapseButtons = new Dictionary<int, GameObject>();
        public static Dictionary<int, bool> ShouldCollapseDic = new Dictionary<int, bool>();
        //public const int SpaceStationHeaderId = -101;
        //public static MultiToggle SpaceStationHeader = null;
        public const int RocketHeaderId = -102;
        public static MultiToggle RocketHeader = null;

        [HarmonyPatch(typeof(WorldSelector))]
        [HarmonyPatch(nameof(WorldSelector.SortRows))]
        public static class WorldSelectorReplacement
        {
            enum WorldType
            {
                planet = 0,
                spaceStation = 1,
                rocket = 2,
            }

            static void InitHeader(int HeaderId, MultiToggle Header)
            {
                //UIUtils.ListAllChildrenWithComponents(Header.transform);
                //UIUtils.ListAllChildren(Header.transform);


                var headerImg = UIUtils.TryFindComponent<Image>(Header.transform, "Content/ManualLayout/Icon/Image");
                var HeaderText = UIUtils.TryFindComponent<LocText>(Header.transform, "Content/ManualLayout/Label");
                //SgtLogger.debuglog(headerImg + ", " + HeaderText);
                switch (HeaderId)
                {
                    //case SpaceStationHeaderId:
                    //    HeaderText.key = "STRINGS.UI_MOD.COLLAPSIBLEWORLDSELECTOR.SPACESTATIONS";
                    //    headerImg.sprite = Assets.GetSprite("icon_category_ventilation");
                    //    break;

                    case RocketHeaderId:
                        HeaderText.key = "STRINGS.UI_MOD.COLLAPSIBLEWORLDSELECTOR.ROCKETS";
                        headerImg.sprite = Assets.GetSprite("icon_category_rocketry");
                        break;
                }



                var icon = AddCollapsible(HeaderId, Header.gameObject);
                ShouldCollapseDic[HeaderId] = false;

                Header.gameObject.SetActive(true);
                Header.onClick = () =>
                {
                    bool newBool = !ShouldCollapseDic[HeaderId];
                    ShouldCollapseDic[HeaderId] = newBool;
                    icon.sprite = newBool ? Assets.GetSprite("dash_arrow") : Assets.GetSprite("dash_arrow_down");
                    WorldSelector.Instance.SortRows();
                };
                //Header.gameObject.GetComponentInChildren<AlertVignette>().image.color = Color.clear;
                Header.toggle_image.color = Color.clear;
                UIUtils.FindAndDisable(Header.transform, "Content/ManualLayout/Status");
            }

            public static bool Prefix(WorldSelector __instance)
            {
                if (Config.Instance.EnableAdvWorldSelector)
                {
                    var asteroids = ListPool<KeyValuePair<int, MultiToggle>, WorldSelector>.Allocate();
                   // var spaceStations = ListPool<KeyValuePair<int, MultiToggle>, WorldSelector>.Allocate();
                    var rockets = ListPool<KeyValuePair<int, MultiToggle>, WorldSelector>.Allocate();

                    var OutputList = ListPool<KeyValuePair<int, MultiToggle>, WorldSelector>.Allocate();

                    foreach (var worldKV in __instance.worldRows)
                    {
                        //SgtLogger.l(worldKV.Key + ", " + ClusterManager.Instance.GetWorld(worldKV.Key));
                        if (SpaceStationManager.WorldIsRocketInterior(worldKV.Key) || edgeCases.Contains(worldKV.Key))
                        {
                            rockets.Add(worldKV);
                        }
                        //else if (SpaceStationManager.WorldIsSpaceStationInterior(worldKV.Key))
                        //{
                        //    spaceStations.Add(worldKV);
                        //}
                        else
                        {
                            if (collapseButtons.ContainsKey(worldKV.Key))
                                collapseButtons[worldKV.Key].SetActive(false);
                            asteroids.Add(worldKV);
                        }
                    }

                    asteroids.OrderBy(a => ClusterManager.Instance.GetWorld(a.Key).DiscoveryTimestamp);

                    OutputList.AddRange(asteroids);

                    //if (spaceStations.Count > 0)
                    //{
                    //    spaceStations.OrderBy(a => ClusterManager.Instance.GetWorld(a.Key).DiscoveryTimestamp);
                    //    if (SpaceStationHeader == null)
                    //    {
                    //        SpaceStationHeader = Util.KInstantiateUI(__instance.worldRowPrefab, __instance.worldRowContainer).GetComponent<MultiToggle>();
                    //        InitHeader(SpaceStationHeaderId, SpaceStationHeader);
                    //    }
                    //    OutputList.Add(new KeyValuePair<int, MultiToggle>(SpaceStationHeaderId, SpaceStationHeader));
                    //    SpaceStationHeader.gameObject.SetActive(true);

                    //    OutputList.AddRange(spaceStations);

                    //}
                    //else
                    //{
                    //    if (SpaceStationHeader != null)
                    //    {
                    //        SpaceStationHeader.gameObject.SetActive(false);
                    //    }
                    //}

                    if (rockets.Count > 0)
                    {

                        if (RocketHeader == null)
                        {
                            RocketHeader = Util.KInstantiateUI(__instance.worldRowPrefab, __instance.worldRowContainer).GetComponent<MultiToggle>();
                            InitHeader(RocketHeaderId, RocketHeader);
                        }
                        OutputList.Add(new KeyValuePair<int, MultiToggle>(RocketHeaderId, RocketHeader));
                    }
                    if (RocketHeader != null)
                    {
                        RocketHeader.gameObject.SetActive(false);
                    }

                    //foreach (var keyValuePair1 in spaceStations)
                    //{
                    //    if(keyValuePair1.Value!=null&& ShouldCollapseDic.ContainsKey(SpaceStationHeaderId))
                    //        keyValuePair1.Value.gameObject.SetActive(value: !ShouldCollapseDic[SpaceStationHeaderId]);
                    //}

                    foreach (var keyValuePair1 in OutputList)
                    {
                        SetAnchors(keyValuePair1.Value, false);
                    }

                    foreach (var rocket in rockets)
                    {
                        WorldContainer rocketWorld = ClusterManager.Instance.GetWorld(rocket.Key);
                        bool Collapse = false;

                        if(rocketWorld==null)
                        {
                            SgtLogger.warning("The world with the id: "+rocket.Key + " was null!");
                            continue;

                        }
                        if (rocketWorld.ParentWorldId != rocketWorld.id && rocketWorld.ParentWorldId != (int)ClusterManager.INVALID_WORLD_IDX)
                        {
                            if (collapseButtons.ContainsKey(rocketWorld.ParentWorldId))
                                collapseButtons[rocketWorld.ParentWorldId].SetActive(true);

                            int insertionIndex = OutputList.FindIndex(kvp => kvp.Key == rocketWorld.ParentWorldId);
                            if (insertionIndex >= 0)
                            {
                                OutputList.Insert(insertionIndex + 1, rocket);
                            }
                            else
                            {
                                OutputList.Add(rocket);
                            }
                            SetAnchors(rocket.Value, !SpaceStationManager.WorldIsRocketInterior(rocketWorld.ParentWorldId));

                            if (collapseButtons.ContainsKey(rocketWorld.ParentWorldId))
                                Collapse = ShouldCollapseDic[rocketWorld.ParentWorldId];                            
                        }
                        else
                        {
                            OutputList.Add(rocket);
                            Collapse = ShouldCollapseDic[RocketHeaderId];
                            SetAnchors(rocket.Value, false);
                            RocketHeader.gameObject.SetActive(true);
                        }
                        if(rocket.Value!= null)
                        {
                            rocket.Value.gameObject.SetActive(value: !Collapse);
                        }
                    }

                    for (int index22 = 0; index22 < OutputList.Count; ++index22)
                    {
                        if (OutputList[index22].Value != null)
                            OutputList[index22].Value.gameObject.transform.SetSiblingIndex(index22);
                    }

                    rockets.Recycle();
                    asteroids.Recycle();
                    //spaceStations.Recycle();
                    OutputList.Recycle();
                    return false;
                }
                return true;
            }
            static void SetAnchors(MultiToggle item, bool intented)
            {
                if (item.TryGetComponent(out HierarchyReferences refs))
                {
                    refs.GetReference<RectTransform>("Indent").anchoredPosition = intented ? Vector2.right * 32f : Vector2.zero;
                    refs.GetReference<RectTransform>("Status").anchoredPosition = intented ? Vector2.right * -8f : Vector2.right * 24f;
                }
            }
        }

        [HarmonyPatch(typeof(WorldSelector))]
        [HarmonyPatch(nameof(WorldSelector.OnSpawn))]
        public static class ResetHeaders
        {
            public static void Prefix(WorldSelector __instance)
            {
                RocketHeader = null;
                //SpaceStationHeader = null;
                edgeCases.Clear();
            }
        }
        static List<int> edgeCases = new List<int>();
        [HarmonyPatch(typeof(WorldSelector))]
        [HarmonyPatch(nameof(WorldSelector.AddWorld))]
        public static class AddButtonToWorld
        {
            public static void Postfix(object data)
            {
                if (Config.Instance.EnableAdvWorldSelector)
                {
                    int num = (int)data;
                    //SgtLogger.l("Adding world with id: " + num);
                    AttachCollapseButton(new KeyValuePair<int, MultiToggle>(num, WorldSelector.Instance.worldRows[num]));
                }
            }
        }

        [HarmonyPatch(typeof(WorldSelector))]
        [HarmonyPatch(nameof(WorldSelector.RemoveWorld))]
        public static class RemoveWorldFromRegisters
        {
            public static void Postfix(object data)
            {
                if (Config.Instance.EnableAdvWorldSelector)
                {
                    int num = (int)data;
                    if (edgeCases.Contains(num))
                    {
                        edgeCases.Remove(num);
                    }
                    if(ShouldCollapseDic.ContainsKey(num))
                    {
                        ShouldCollapseDic.Remove(num);
                    }
                    if (collapseButtons.ContainsKey(num))
                    {
                        collapseButtons.Remove(num);
                    }
                }
            }
        }


        public static void AttachCollapseButtons()
        {
            //foreach (var bt in collapseButtons)
            //{
            //    if (!bt.Value.IsNullOrDestroyed())
            //        Util.KDestroyGameObject(bt.Value);
            //}
            //collapseButtons.Clear();

            foreach (var worldKV in WorldSelector.Instance.worldRows)
            {
                AttachCollapseButton(worldKV);
            }
        }
        public static void AttachCollapseButton(KeyValuePair<int, MultiToggle> worldKV)
        {
            //SgtLogger.l("Adding world with id: " + worldKV.Key);
            var worldContainer = ClusterManager.Instance.GetWorld(worldKV.Key);
            if (!SpaceStationManager.WorldIsRocketInterior(worldKV.Key))
            {
                if (worldContainer.gameObject.TryGetComponent<AsteroidGridEntity>(out _) || SpaceStationManager.WorldIsSpaceStationInterior(worldKV.Key))
                {
                    ShouldCollapseDic[worldKV.Key] = false;
                    AddCollapsible(worldKV.Key, worldKV.Value.gameObject);
                    //SgtLogger.l("planet: " + worldKV.Key);
                }
                else
                {
                    edgeCases.Add(worldKV.Key);
                    //SgtLogger.l("abnormal: " + worldKV.Key);
                }
            }
        }


        [HarmonyPatch(typeof(WorldSelector))]
        [HarmonyPatch(nameof(WorldSelector.SpawnToggles))]
        public static class ChangeToCollapsibles
        {
            public static void Postfix()
            {
                if (Config.Instance.EnableAdvWorldSelector)
                {
                    AttachCollapseButtons();
                }
            }

        }
        public static Image AddCollapsible(int Key, GameObject Value)
        {
            var insertLocation = Value.transform.Find("Content/ManualLayout");
            var img = Value.transform.Find("Content/ManualLayout/Icon");

            var newIcon = Util.KInstantiateUI(img.gameObject, insertLocation.gameObject);
            newIcon.gameObject.SetActive(true);

            collapseButtons[Key] = newIcon;

            var icon = UIUtils.TryFindComponent<Image>(newIcon.transform, "Image");
            icon.transform.SetAsLastSibling();
            icon.sprite = Assets.GetSprite("dash_arrow_down");
            icon.transform.localScale = new Vector3(1.0f, 1.0f);
            icon.transform.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -35f, 35f);
            var btn = icon.FindOrAddUnityComponent<KButton>();
            btn.soundPlayer = new ButtonSoundPlayer();
            btn.onClick += () =>
            {
                bool newBool = !ShouldCollapseDic[Key];
                ShouldCollapseDic[Key] = newBool;
                icon.sprite = newBool ? Assets.GetSprite("dash_arrow") : Assets.GetSprite("dash_arrow_down");
                WorldSelector.Instance.SortRows();
            };
            btn.InitializeComponent();
            return icon;

        }
    }
}
