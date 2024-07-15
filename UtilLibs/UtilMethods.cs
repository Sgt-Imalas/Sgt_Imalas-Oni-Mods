using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs
{
    public static class UtilMethods
    {
        public static void ListAllTypesWithAssemblies()
        {
            ///Gets all types + namespace 
            var q = AppDomain.CurrentDomain.GetAssemblies()
                   .SelectMany(t => t.GetTypes());
            q.ToList().ForEach(t => SgtLogger.l(t.Name+", AQN: "+t.AssemblyQualifiedName, t.Namespace));
        }
        public static void ListAllPropertyValues(object s)
        {
            SgtLogger.l("Listing all properties of: " + s.ToString());

            foreach (var p in s.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
            {
                Console.WriteLine(p + ": " + p.GetValue(s, null));
            }
        }
        public static void ListAllFieldValues(object s)
        {
            SgtLogger.l("Listing all fields of: " + s.ToString());

            foreach (var p in s.GetType().GetFields())
            {
                Console.WriteLine(p + ": " + p.GetValue(s));
            }
        }
        public static void ListAllComponents(GameObject s)
        {
            SgtLogger.l("Listing all Components of: " + s.ToString());

            foreach (var comp in s.GetComponents(typeof(UnityEngine.Object)))
            {
                if (comp != null)
                {
                    Console.WriteLine("Type: " + comp.GetType().ToString() + ", Name ->" + comp.name);
                }
            }
        }
        public static float GetCFromKelvin(float degreeK)
        {
            return degreeK - 273.15f;
        }
        public static float GetKelvinFromC(float degreeC)
        {
            return degreeC + 273.15f;
        }
        public static string ModPath => IO_Utils.ModPath;
        public static bool IsCellInSpaceAndVacuum(int _cell, int root)
        {
            if (!Grid.IsValidCell(_cell) || !Grid.AreCellsInSameWorld(_cell,root))
                return true;

            return (Grid.IsCellOpenToSpace(_cell) || IsCellInRocket(_cell)) && (Grid.Element[_cell].id == SimHashes.Vacuum || Grid.Element[_cell].id == SimHashes.Unobtanium);
        }
        private static bool IsCellInRocket(int _cell)
        {
            WorldContainer w = Grid.IsValidCell(_cell) && (int)Grid.WorldIdx[_cell] != (int)ClusterManager.INVALID_WORLD_IDX ? ClusterManager.Instance.GetWorld((int)Grid.WorldIdx[_cell]) : (WorldContainer)null;
            return w.IsModuleInterior;
        }
    }
}
