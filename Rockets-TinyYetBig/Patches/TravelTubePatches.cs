using HarmonyLib;
using ONITwitchLib.Logger;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    internal class TravelTubePatches
    {

        //[HarmonyPatch]
        //public class NavGrid_TargetMethod_Patch
        //{
        //    public static MethodBase TargetMethod()
        //    {
        //        // NavGrid.Transition is not yet loaded when the patches run, 
        //        var type = typeof(NavGrid);
        //        return AccessTools.Constructor(type, new[]
        //        {
        //            typeof(string),
        //            typeof(NavGrid.Transition[]),
        //            typeof(NavGrid.NavTypeData[]),
        //            typeof(CellOffset[]),
        //            typeof(NavTableValidator[]),
        //            typeof(int),
        //            typeof(int),
        //            typeof(int)
        //        });
        //    }

        //    public static void Prefix(string id, ref NavGrid.Transition[] transitions, ref NavGrid.NavTypeData[] nav_type_data, ref NavTableValidator[] validators)
        //    {
        //        if (id != MinionConfig.MINION_NAV_GRID_NAME)
        //        {
        //            return;
        //        }
        //        SgtLogger.l("Patching navgrid");


        //        var setA = new NavGrid.Transition[]
        //        {
        //                new NavGrid.Transition(NavType.Tube, NavType.Tube, 0, 2, NavAxis.NA, false, false, false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[]
        //                {
        //                    new NavOffset(NavType.Tube, 0, 2),
        //                    new NavOffset(NavType.Tube, 0, -2)
        //                }, new NavOffset[0], animSpeed: 3.3f)
        //                ,
        //            new NavGrid.Transition(NavType.Tube, NavType.Tube, 0, -2, NavAxis.NA, false, false, false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[]
        //            {
        //                    new NavOffset(NavType.Tube, 0, 2),
        //                    new NavOffset(NavType.Tube, 0, -2)
        //            }, new NavOffset[0], animSpeed: 2.2f)
        //        };

        //        SgtLogger.l("transitioncount: " + setA.Length);
        //        setA = MirrorTransitions(setA);

        //        SgtLogger.l("transitioncount: " + setA.Length);
        //        //transitions = transitions.AddRangeToArray(setA);

        //        transitions = CombineTransitions(setA, transitions);


        //    }
        //    #region combining
        //    private static NavGrid.Transition[] CombineTransitions(NavGrid.Transition[] setA, NavGrid.Transition[] setB)
        //    {
        //        var array = new NavGrid.Transition[setA.Length + setB.Length];
        //        Array.Copy(setA, array, setA.Length);
        //        Array.Copy(setB, 0, array, setA.Length, setB.Length);
        //        Array.Sort(array, (x, y) => x.cost.CompareTo(y.cost));
        //        return array;
        //    }

        //    private static NavGrid.Transition[] MirrorTransitions(NavGrid.Transition[] transitions)
        //    {
        //        var list = new List<NavGrid.Transition>();
        //        foreach (var transition in transitions)
        //        {
        //            list.Add(transition);
        //            if (transition.x != 0 || transition.start == NavType.RightWall || transition.end == NavType.RightWall || transition.start == NavType.LeftWall || transition.end == NavType.LeftWall)
        //            {
        //                var transition2 = transition;
        //                transition2.x = -transition2.x;
        //                transition2.voidOffsets = MirrorOffsets(transition.voidOffsets);
        //                transition2.solidOffsets = MirrorOffsets(transition.solidOffsets);
        //                transition2.validNavOffsets = MirrorNavOffsets(transition.validNavOffsets);
        //                transition2.invalidNavOffsets = MirrorNavOffsets(transition.invalidNavOffsets);
        //                transition2.start = NavGrid.MirrorNavType(transition2.start);
        //                transition2.end = NavGrid.MirrorNavType(transition2.end);
        //                list.Add(transition2);
        //            }
        //        }
        //        list.Sort((NavGrid.Transition x, NavGrid.Transition y) => x.cost.CompareTo(y.cost));
        //        return list.ToArray();
        //    }

        //    private static CellOffset[] MirrorOffsets(CellOffset[] offsets)
        //    {
        //        var list = new List<CellOffset>();
        //        for (var i = 0; i < offsets.Length; i++)
        //        {
        //            var cellOffset = offsets[i];
        //            cellOffset.x = -cellOffset.x;
        //            list.Add(cellOffset);
        //        }
        //        return list.ToArray();
        //    }

        //    private static NavOffset[] MirrorNavOffsets(NavOffset[] offsets)
        //    {
        //        var list = new List<NavOffset>();

        //        for (var i = 0; i < offsets.Length; i++)
        //        {
        //            var navOffset = offsets[i];

        //            navOffset.navType = NavGrid.MirrorNavType(navOffset.navType);
        //            navOffset.offset.x = -navOffset.offset.x;

        //            list.Add(navOffset);
        //        }

        //        return list.ToArray();
        //    }
        //    #endregion
        //}

        //public static class InsertingAdditional
        //{
        //    /// <summary>
        //    /// Setting ClusterID to custom cluster if it should load
        //    /// 
        //    /// </summary>
           
            
            
            
            
        //    public static void Prefix(string id, ref NavGrid.Transition[] transitions) //MinionConfig.MINION_NAV_GRID_NAME
        //    {
        //        //CustomLayout
        //        if (id == MinionConfig.MINION_NAV_GRID_NAME)
        //        {
        //            SgtLogger.l("patching navgrid");
        //            var ToInsertList = transitions.ToList();
        //            var item1 = (
        //                new NavGrid.Transition(NavType.Tube, NavType.Tube, 0, 2, NavAxis.NA, false, false, false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[]
        //                {
        //                    new NavOffset(NavType.Tube, 0, 2),
        //                    new NavOffset(NavType.Tube, 0, -2)
        //                }, new NavOffset[0], animSpeed: 3.3f));
        //            var item2 = (
        //                new NavGrid.Transition(NavType.Tube, NavType.Tube, 0, -2, NavAxis.NA, false, false, false, 15, "", new CellOffset[0], new CellOffset[0], new NavOffset[]
        //                {
        //                    new NavOffset(NavType.Tube, 0, 2),
        //                    new NavOffset(NavType.Tube, 0, -2)
        //                }, new NavOffset[0], animSpeed: 2.2f));



        //            transitions = ToInsertList.ToArray();

        //            foreach (var transition in transitions)
        //            {
        //                SgtLogger.l(transition.ToString());
        //            }
        //        }
        //    }
        //}

        //[HarmonyPatch(typeof(GameNavGrids))]
        //[HarmonyPatch(nameof(GameNavGrids.CreateDuplicantNavigation))]
        //public static class Db_Init_Patch
        //{
        //    // using System; will allow using Type insted of System.Type
        //    // using System.Reflection; will allow using MethodInfo instead of System.Reflection.MethodInfo
        //    static System.Reflection.MethodBase GetMethodInfo(System.Type classType, string methodName, Type[] constructorAttributes = null)
        //    {
        //        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public
        //                                            | System.Reflection.BindingFlags.NonPublic
        //                                            | System.Reflection.BindingFlags.Static
        //                                            | System.Reflection.BindingFlags.Instance;
        //        System.Reflection.MethodBase method;
        //        if (classType.ToString() == methodName)
        //        {
        //            method = classType.GetConstructor(flags, null ,constructorAttributes, null);
        //        }
        //        else
        //            method = classType.GetMethod(methodName, flags);


        //        if (method == null)
        //            Debug.Log($"Error - {methodName} method is null...");

        //        return method;
        //    }

        //    public static void Prefix()
        //    {
        //        System.Reflection.MethodBase patched = GetMethodInfo(typeof(NavGrid), "NavGrid",new Type[] { typeof(string), typeof(NavGrid.Transition[]), typeof(NavGrid.NavTypeData[]), typeof(CellOffset[]), typeof(NavTableValidator[]), typeof(int), typeof(int), typeof(int) });
        //        System.Reflection.MethodInfo postfix = GetMethodInfo(typeof(InsertingAdditional), "Prefix") as MethodInfo;
        //        // TODO: Update line below
        //        Harmony harmony = new Harmony("Rocketry Expanded");
        //        harmony.Patch(patched, null, new HarmonyMethod(postfix));
        //    }
        //}

    }
}
