using HarmonyLib;
using PeterHan.PLib.Core;
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

        /// <summary>
        /// Copies the sounds from one animation to another animation.
        /// </summary>
        /// <param name="dstAnim">The destination anim file name.</param>
        /// <param name="srcAnim">The source anim file name.</param>
        public static void CopySoundsToAnim(string dstAnim, string srcAnim)
        {

            Debug.Log("Trying to add audio from {0} to {1}".F(srcAnim, dstAnim));
            if (string.IsNullOrEmpty(dstAnim))
                throw new ArgumentNullException(nameof(dstAnim));
            if (string.IsNullOrEmpty(srcAnim))
                throw new ArgumentNullException(nameof(srcAnim));
            var anim = Assets.GetAnim(dstAnim);
            if (anim != null)
            {
                var audioSheet = GameAudioSheets.Get();
                var animData = anim.GetData();
                // For each anim in the kanim, look for existing sound events under the old
                // anim's file name
                for (int i = 0; i < animData.animCount; i++)
                {
                    string animName = animData.GetAnim(i)?.name ?? "";
                    var events = audioSheet.GetEvents(srcAnim + "." + animName);
                    if (events != null)
                    {
#if DEBUG
                        Debug.Log("Adding {0:D} audio event(s) to anim {1}.{2}".F(events.
                            Count, dstAnim, animName));
#endif
                        audioSheet.events[dstAnim + "." + animName] = events;
                    }
                }
            }
            else
                Debug.LogWarning("Destination animation \"{0}\" not found!".F(dstAnim));
        }
        public static void CopyRocketSoundsToAnim(string dstAnim, string srcAnim)
        {

            Debug.Log("Trying to add audio from {0} to {1}".F(srcAnim, dstAnim));
            if (string.IsNullOrEmpty(dstAnim))
                throw new ArgumentNullException(nameof(dstAnim));
            if (string.IsNullOrEmpty(srcAnim))
                throw new ArgumentNullException(nameof(srcAnim));
            var anim = Assets.GetAnim(dstAnim);
            if (anim != null)
            {
                var audioSheet = GameAudioSheets.Get();
                var animData = anim.GetData();
                // For each anim in the kanim, look for existing sound events under the old
                // anim's file name
                for (int i = 0; i < animData.animCount; i++)
                {
                    string animName = animData.GetAnim(i)?.name ?? "";
                    var events = audioSheet.GetEvents(srcAnim + "." + animName);
                    if (events != null)
                    {
#if DEBUG
                        foreach(var e in events)
                        {

                        }

                        Debug.Log("Adding {0:D} audio event(s) to anim {1}.{2}".F(events.
                            Count, dstAnim, animName));
#endif
                        audioSheet.events[dstAnim + "." + animName] = events;
                    }
                }
            }
            else
                Debug.LogWarning("Destination animation \"{0}\" not found!".F(dstAnim));
        }
    }
}
