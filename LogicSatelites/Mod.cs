using HarmonyLib;
using KMod;
using LogicSatellites.Buildings;
using PeterHan.PLib.Core;
using PeterHan.PLib.Lighting;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace LogicSatellites
{
    public class Mod : UserMod2
    {
        public static ILightShape LaserBeam;
        public override void OnLoad(Harmony harmony)
        {
            PUtil.InitLibrary(false);
            new POptions().RegisterOptions(this, typeof(Config));
            base.OnLoad(harmony);
            var lightManager = new PLightManager();
            LaserBeam = lightManager.Register("LS_SolarSatelliteBeam", LaserBeamHandler);
            SgtLogger.LogVersion(this);
        }
        public static void LaserBeamHandler(LightingArgs arg)
        {
            _LinearLightHelper(arg.Source, arg.SourceCell, arg.Range, arg.Brightness, 0f);
        }

        public static void _LinearLightHelper(GameObject source, int sourceCell, int range,
            IDictionary<int, float> brightness_map, float falloff)
        {

            var parallel_dir = new Vector2I(0, 0);
            var perpendicular_dir = new Vector2I(0, 0);
            parallel_dir.Y = -1;
            perpendicular_dir.X = 1;

            var start_offset = -1 / 2;
            var end_offset = 1 / 2;
            // Shoot parallel rays, terminating the loop whenever light is completely occluded.
            for (var j = start_offset; j <= end_offset; j++)
            {
                var brightness = 1f;
                for (var i = 0; i < range; i++)
                {
                    var total_offset = parallel_dir * i + perpendicular_dir * j;
                    var cell = Grid.OffsetCell(sourceCell, new CellOffset(total_offset.X, total_offset.Y));
                    if (Grid.IsValidCell(cell) && brightness != 0f)
                    {
                        brightness_map[cell] = brightness;
                        //brightness = brightness * (1f - falloff);
                        brightness = AdjustLightByTile(cell, brightness) * (1f - falloff);
                    }
                    else
                    {
                        break; // go to the next ray
                    }
                }
            }
        }

        public static float AdjustLightByTile(int cell, float brightness)
        {
            // If it's a Pneumatic Door, do not dim
            var obj = Grid.Objects[cell, (int)ObjectLayer.Building];
            if (obj != null)
            {
                var building = obj.GetComponent<Building>();
                if (building != null && building.Def.PrefabID == "Door") return brightness * .7f;
            }

            // Occlude slightly if mesh or airflow tile
            obj = Grid.Objects[cell, (int)ObjectLayer.FoundationTile];
            if (obj != null)
            {
                var name = obj.GetComponent<Building>().Def.PrefabID;
                if (name == "MeshTile")
                    return brightness * .9f;
                if (name == "GasPermeableMembrane") return brightness * .5f;
                if (obj.TryGetComponent<SolarReciever>(out var sat))
                    return 0f;
            }

            // Totally occlude if tile is solid.
            if (Grid.IsValidCell(cell) && !Grid.Transparent[cell] && Grid.Solid[cell]) return 0f;

            return brightness;
        }

        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);
            CompatibilityNotifications.FlagLoggingPrevention(mods);

        }
    }
}
