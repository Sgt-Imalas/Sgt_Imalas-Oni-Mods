using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static global::STRINGS.ROOMS;

namespace UtilLibs
{
    public static class LocalisationUtil
    {
        static Type stringType;

        /// <summary>
        /// Rebuild those strings bc they didn't translate from loading the class to early..
        /// </summary>
        public static void FixRoomConstrains()
        {
            SgtLogger.l("fixing room constraint strings");
            RoomConstraints.CEILING_HEIGHT_4.name = string.Format(CRITERIA.CEILING_HEIGHT.NAME, "4");
            RoomConstraints.CEILING_HEIGHT_4.description = string.Format(CRITERIA.CEILING_HEIGHT.DESCRIPTION, "4");
            RoomConstraints.CEILING_HEIGHT_6.name = string.Format(CRITERIA.CEILING_HEIGHT.NAME, "6");
            RoomConstraints.CEILING_HEIGHT_6.description = string.Format(CRITERIA.CEILING_HEIGHT.DESCRIPTION, "6");
            RoomConstraints.MINIMUM_SIZE_12.name = string.Format(CRITERIA.MINIMUM_SIZE.NAME, "12");
            RoomConstraints.MINIMUM_SIZE_12.description = string.Format(CRITERIA.MINIMUM_SIZE.DESCRIPTION, "12");
            RoomConstraints.MINIMUM_SIZE_24.name = string.Format(CRITERIA.MINIMUM_SIZE.NAME, "24");
            RoomConstraints.MINIMUM_SIZE_24.description = string.Format(CRITERIA.MINIMUM_SIZE.DESCRIPTION, "24");
            RoomConstraints.MINIMUM_SIZE_32.name = string.Format(CRITERIA.MINIMUM_SIZE.NAME, "32");
            RoomConstraints.MINIMUM_SIZE_32.description = string.Format(CRITERIA.MINIMUM_SIZE.DESCRIPTION, "32");
            RoomConstraints.MAXIMUM_SIZE_64.name = string.Format(CRITERIA.MAXIMUM_SIZE.NAME, "64");
            RoomConstraints.MAXIMUM_SIZE_64.description = string.Format(CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "64");
            RoomConstraints.MAXIMUM_SIZE_96.name = string.Format(CRITERIA.MAXIMUM_SIZE.NAME, "96");
            RoomConstraints.MAXIMUM_SIZE_96.description = string.Format(CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "96");
            RoomConstraints.MAXIMUM_SIZE_120.name = string.Format(CRITERIA.MAXIMUM_SIZE.NAME, "120");
            RoomConstraints.MAXIMUM_SIZE_120.description = string.Format(CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "120");
            RoomConstraints.NO_INDUSTRIAL_MACHINERY.name = CRITERIA.NO_INDUSTRIAL_MACHINERY.NAME;
            RoomConstraints.NO_INDUSTRIAL_MACHINERY.description = CRITERIA.NO_INDUSTRIAL_MACHINERY.DESCRIPTION;
            RoomConstraints.NO_COTS.name = CRITERIA.NO_COTS.NAME;
            RoomConstraints.NO_COTS.description = CRITERIA.NO_COTS.DESCRIPTION;
            RoomConstraints.NO_LUXURY_BEDS.name = CRITERIA.NO_COTS.NAME; RoomConstraints.NO_LUXURY_BEDS.description = CRITERIA.NO_COTS.DESCRIPTION;
            RoomConstraints.NO_OUTHOUSES.name = CRITERIA.NO_OUTHOUSES.NAME; RoomConstraints.NO_OUTHOUSES.description = CRITERIA.NO_OUTHOUSES.DESCRIPTION;
            RoomConstraints.NO_MESS_STATION.name = CRITERIA.NO_MESS_STATION.NAME; RoomConstraints.NO_MESS_STATION.description = CRITERIA.NO_MESS_STATION.DESCRIPTION;
            RoomConstraints.HAS_LUXURY_BED.name = CRITERIA.HAS_LUXURY_BED.NAME; RoomConstraints.HAS_LUXURY_BED.description = CRITERIA.HAS_LUXURY_BED.DESCRIPTION;
            RoomConstraints.HAS_BED.name = CRITERIA.HAS_BED.NAME; RoomConstraints.HAS_BED.description = CRITERIA.HAS_BED.DESCRIPTION;
            RoomConstraints.SCIENCE_BUILDINGS.name = CRITERIA.SCIENCE_BUILDINGS.NAME; RoomConstraints.SCIENCE_BUILDINGS.description = CRITERIA.SCIENCE_BUILDINGS.DESCRIPTION;
            RoomConstraints.BED_SINGLE.name = CRITERIA.BED_SINGLE.NAME; RoomConstraints.BED_SINGLE.description = CRITERIA.BED_SINGLE.DESCRIPTION;
            RoomConstraints.LUXURY_BED_SINGLE.name = CRITERIA.LUXURYBEDTYPE.NAME; RoomConstraints.LUXURY_BED_SINGLE.description = CRITERIA.LUXURYBEDTYPE.DESCRIPTION;
            RoomConstraints.BUILDING_DECOR_POSITIVE.name = CRITERIA.BUILDING_DECOR_POSITIVE.NAME; RoomConstraints.BUILDING_DECOR_POSITIVE.description = CRITERIA.BUILDING_DECOR_POSITIVE.DESCRIPTION;
            RoomConstraints.DECORATIVE_ITEM.name = string.Format(CRITERIA.DECORATIVE_ITEM.NAME, 1); RoomConstraints.DECORATIVE_ITEM.description = string.Format(CRITERIA.DECORATIVE_ITEM.DESCRIPTION, 1);
            RoomConstraints.DECORATIVE_ITEM_2.name = string.Format(CRITERIA.DECORATIVE_ITEM.NAME, 2); RoomConstraints.DECORATIVE_ITEM_2.description = string.Format(CRITERIA.DECORATIVE_ITEM.DESCRIPTION, 2);
            RoomConstraints.DECORATIVE_ITEM_SCORE_20.name = CRITERIA.DECOR20.NAME; RoomConstraints.DECORATIVE_ITEM_SCORE_20.description = CRITERIA.DECOR20.DESCRIPTION;
            RoomConstraints.POWER_STATION.name = CRITERIA.POWERSTATION.NAME; RoomConstraints.POWER_STATION.description = CRITERIA.POWERSTATION.DESCRIPTION;
            RoomConstraints.FARM_STATION.name = CRITERIA.FARMSTATIONTYPE.NAME; RoomConstraints.FARM_STATION.description = CRITERIA.FARMSTATIONTYPE.DESCRIPTION;
            RoomConstraints.RANCH_STATION.name = CRITERIA.RANCHSTATIONTYPE.NAME; RoomConstraints.RANCH_STATION.description = CRITERIA.RANCHSTATIONTYPE.DESCRIPTION;
            RoomConstraints.SPICE_STATION.name = CRITERIA.SPICESTATION.NAME; RoomConstraints.SPICE_STATION.description = CRITERIA.SPICESTATION.DESCRIPTION;
            RoomConstraints.COOK_TOP.name = CRITERIA.COOKTOP.NAME; RoomConstraints.COOK_TOP.description = CRITERIA.COOKTOP.DESCRIPTION;
            RoomConstraints.REFRIGERATOR.name = CRITERIA.REFRIGERATOR.NAME; RoomConstraints.REFRIGERATOR.description = CRITERIA.REFRIGERATOR.DESCRIPTION;
            RoomConstraints.REC_BUILDING.name = CRITERIA.RECBUILDING.NAME; RoomConstraints.REC_BUILDING.description = CRITERIA.RECBUILDING.DESCRIPTION;
            RoomConstraints.MACHINE_SHOP.name = CRITERIA.MACHINESHOPTYPE.NAME; RoomConstraints.MACHINE_SHOP.description = CRITERIA.MACHINESHOPTYPE.DESCRIPTION;
            RoomConstraints.LIGHT.name = CRITERIA.LIGHTSOURCE.NAME; RoomConstraints.LIGHT.description = CRITERIA.LIGHTSOURCE.DESCRIPTION;
            RoomConstraints.DESTRESSING_BUILDING.name = CRITERIA.DESTRESSINGBUILDING.NAME; RoomConstraints.DESTRESSING_BUILDING.description = CRITERIA.DESTRESSINGBUILDING.DESCRIPTION;
            RoomConstraints.MASSAGE_TABLE.name = CRITERIA.MASSAGE_TABLE.NAME; RoomConstraints.MASSAGE_TABLE.description = CRITERIA.MASSAGE_TABLE.DESCRIPTION;
            RoomConstraints.MESS_STATION_SINGLE.name = CRITERIA.MESSTABLE.NAME; RoomConstraints.MESS_STATION_SINGLE.description = CRITERIA.MESSTABLE.DESCRIPTION;
            RoomConstraints.TOILET.name = CRITERIA.TOILETTYPE.NAME; RoomConstraints.TOILET.description = CRITERIA.TOILETTYPE.DESCRIPTION;
            RoomConstraints.FLUSH_TOILET.name = CRITERIA.FLUSHTOILETTYPE.NAME; RoomConstraints.FLUSH_TOILET.description = CRITERIA.FLUSHTOILETTYPE.DESCRIPTION;
            RoomConstraints.WASH_STATION.name = CRITERIA.WASHSTATION.NAME; RoomConstraints.WASH_STATION.description = CRITERIA.WASHSTATION.DESCRIPTION;
            RoomConstraints.ADVANCEDWASHSTATION.name = CRITERIA.ADVANCEDWASHSTATION.NAME; RoomConstraints.ADVANCEDWASHSTATION.description = CRITERIA.ADVANCEDWASHSTATION.DESCRIPTION;
            RoomConstraints.CLINIC.name = CRITERIA.CLINIC.NAME; RoomConstraints.CLINIC.description = CRITERIA.CLINIC.DESCRIPTION;
            RoomConstraints.PARK_BUILDING.name = CRITERIA.PARK.NAME; RoomConstraints.PARK_BUILDING.description = CRITERIA.PARK.DESCRIPTION;
            RoomConstraints.IS_BACKWALLED.name = CRITERIA.IS_BACKWALLED.NAME; RoomConstraints.IS_BACKWALLED.description = CRITERIA.IS_BACKWALLED.DESCRIPTION;
            RoomConstraints.WILDANIMAL.name = CRITERIA.WILDANIMAL.NAME; RoomConstraints.WILDANIMAL.description = CRITERIA.WILDANIMAL.DESCRIPTION;
            RoomConstraints.WILDANIMALS.name = CRITERIA.WILDANIMALS.NAME; RoomConstraints.WILDANIMALS.description = CRITERIA.WILDANIMALS.DESCRIPTION;
            RoomConstraints.WILDPLANT.name = CRITERIA.WILDPLANT.NAME; RoomConstraints.WILDPLANT.description = CRITERIA.WILDPLANT.DESCRIPTION;
            RoomConstraints.WILDPLANTS.name = CRITERIA.WILDPLANTS.NAME; RoomConstraints.WILDPLANTS.description = CRITERIA.WILDPLANTS.DESCRIPTION;
        }

        public static void ManualTranslationPatch(Harmony harmony, Type type)
        {
            stringType = type;
            var m_TargetMethod = AccessTools.Method("Localization, Assembly-CSharp:Initialize");
            //var m_Transpiler = AccessTools.Method(typeof(CharacterSelectionController_Patch), "Transpiler");
            var m_Postfix = AccessTools.Method(typeof(LocalisationUtil), "Postfix");

            harmony.Patch(m_TargetMethod, postfix: new HarmonyMethod(m_Postfix));
        }
        public static void Postfix()
        {
            if (stringType != null)
                LocalisationUtil.Translate(stringType, true);
        }

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
