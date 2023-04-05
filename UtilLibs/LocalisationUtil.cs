using KMod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
    public static class LocalisationUtil
    {
        public static void Translate(Type root, bool generateTemplate = false)
        {
            Localization.RegisterForTranslation(root);
            OverLoadStrings();
            LocString.CreateLocStringKeys(root, null);

            if (generateTemplate)
            {
                Localization.GenerateStringsTemplate(root, Path.Combine(Manager.GetDirectory(), "strings_templates"));
            }
        }

        // Loads user created translations
        private static void OverLoadStrings()
        {
            string code = Localization.GetLocale()?.Code;

            if (code.IsNullOrWhiteSpace()) return;

            string path = Path.Combine(UtilMethods.ModPath, "translations", Localization.GetLocale().Code + ".po");

            if (File.Exists(path))
            {
                Localization.OverloadStrings(Localization.LoadStringsFile(path, false));
                Debug.Log($"Found translation file for {code}.");
            }
        }
    }
}
