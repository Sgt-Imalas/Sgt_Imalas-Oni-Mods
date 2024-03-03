using Dupery;
using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UtilLibs;
using static STRINGS.UI.FRONTEND;

namespace DuperyFixed
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            DuperyPatches.DirectoryName = Path.GetDirectoryName(assemblyLocation);
        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            DuperyPatches.Mods = mods;
            DuperyPatches.ModStaticID = this.mod.staticID;
            base.OnAllModsLoaded(harmony, mods);
        }
    }
}
