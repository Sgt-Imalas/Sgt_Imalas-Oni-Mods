using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Localization;

namespace Dupery
{
    [HarmonyPatch(typeof(Localization), "Initialize")]
    public class Localization_Initialize
    {
        public static void Postfix() => Translate(typeof(STRINGS));

        public static void Translate(Type root)
        {
            RegisterForTranslation(root);
            LocString.CreateLocStringKeys(root, null);
            GenerateStringsTemplate(root, Path.Combine(DuperyPatches.DirectoryName, "strings"));
        }
    }
}
