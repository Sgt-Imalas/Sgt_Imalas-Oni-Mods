using HarmonyLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    class GroupRocketsAndCollapsePlanetsPatchescs
    {
        [HarmonyPatch(typeof(WorldSelector))]
        [HarmonyPatch("SortRows")]
        public static class WorldSelectorReplacement__WIP
        {
            enum WorldType
            {
                planet =0,
                spaceStation = 1,
                rocket = 2,
            }
            static List<KeyValuePair<int, MultiToggle>> Expander = new List<KeyValuePair<int, MultiToggle>>();
            public static bool Prefix(WorldSelector __instance)
            {
                if (Config.Instance.EnableAdvWorldSelector)
                {
                    var asteroids = ListPool<KeyValuePair<int, MultiToggle>, WorldSelector>.Allocate();
                    var spaceStations = ListPool<KeyValuePair<int, MultiToggle>, WorldSelector>.Allocate();
                    var rockets = ListPool<KeyValuePair<int, MultiToggle>, WorldSelector>.Allocate();
                    
                    var OutputList = ListPool<KeyValuePair<int, MultiToggle>, WorldSelector>.Allocate();

                    foreach(var worldKV in __instance.worldRows)
                    {
                        if(SpaceStationManager.WorldIsRocketInterior(worldKV.Key))
                            rockets.Add(worldKV);
                        else if(SpaceStationManager.WorldIsSpaceStationInterior(worldKV.Key))
                            spaceStations.Add(worldKV);
                        else
                            asteroids.Add(worldKV);
                    }

                    OutputList.AddRange(asteroids);
                    ///sth. sth. StationHeader thing
                    OutputList.AddRange(spaceStations);

                    foreach (KeyValuePair<int, MultiToggle> keyValuePair1 in OutputList)
                    {
                        SetAnchors(keyValuePair1.Value, false);
                    }

                    foreach (var rocket in rockets)
                    {
                        WorldContainer rocketWorld = ClusterManager.Instance.GetWorld(rocket.Key);
                        if(rocketWorld.ParentWorldId != rocketWorld.id && rocketWorld.ParentWorldId != (int)ClusterManager.INVALID_WORLD_IDX && !SpaceStationManager.WorldIsRocketInterior(rocketWorld.ParentWorldId))
                        {
                           int insertionIndex = OutputList.FindIndex(kvp => kvp.Key == rocketWorld.ParentWorldId);
                            OutputList.Insert(insertionIndex + 1, rocket);
                            SetAnchors(rocket.Value, true);
                        }
                        else
                        {
                            OutputList.Add(rocket);
                            SetAnchors(rocket.Value, false);
                        }
                    }

                    for (int index = 0; index < OutputList.Count; ++index)
                        OutputList[index].Value.transform.SetSiblingIndex(index);
                    
                    rockets.Recycle();
                    asteroids.Recycle();
                    spaceStations.Recycle();
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
        [HarmonyPatch("OnSpawn")]
        public static class GibStructure
        { 
            public static void Postfix(WorldSelector __instance)
            {
                //Debug.Log("WorldSelector Start");
                //UIUtils.ListAllChildren(__instance.transform);
                //Debug.Log("WorldSelector End");
            }
        }
    }
}
