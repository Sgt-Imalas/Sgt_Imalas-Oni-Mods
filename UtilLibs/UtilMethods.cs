using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
    public static class UtilMethods
    {
        public static float GetKelvinFromC(float degreeC)
        {
            return degreeC + 273.15f;
        }
        public static string ModPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static bool IsCellInSpaceAndVacuum(int _cell)
        {
            if (Grid.IsValidCell(_cell))
                return true;
            return (Grid.IsCellOpenToSpace(_cell) || IsCellInRocket(_cell)) && (Grid.Mass[_cell] == 0 || Grid.Element[_cell].id == SimHashes.Unobtanium);
        }
        private static bool IsCellInRocket(int _cell)
        {
            WorldContainer w = Grid.IsValidCell(_cell) && (int)Grid.WorldIdx[_cell] != (int)ClusterManager.INVALID_WORLD_IDX ? ClusterManager.Instance.GetWorld((int)Grid.WorldIdx[_cell]) : (WorldContainer)null;
            return w.IsModuleInterior;
        }
        public static void GetSounds()
        {
            var trav = Traverse.Create(GlobalAssets.Instance);
            var dic = trav.Field("SoundTable").GetValue<Dictionary<string, string>>();
            Debug.Log("Dic found? - " + (dic.Count > 0));
            foreach (var sound in dic)
            {
                Debug.Log(sound);
            }
        }
    }
}
