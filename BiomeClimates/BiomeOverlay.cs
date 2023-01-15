using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ProcGen.SubWorld;
using static ResearchTypes;
using static STRINGS.SUBWORLDS;

namespace BiomeClimates
{
    internal class BiomeOverlay : OverlayModes.Mode
    {
        private static readonly Dictionary<string, MapOverlayEntry> ColorMap = new Dictionary<string, MapOverlayEntry>();
        public override string GetSoundName()
        {
            return "Harvest";
        }
        public override HashedString ViewMode()
        {
            return "Lights";
        }
        public static readonly HashedString ID = new HashedString("Imalas_BIOMETYPEOVERLAY");

        static void GetBiomeTypeFrom(int cell)
        {

            ZoneType biome = World.Instance.zoneRenderData.worldZoneTypes[cell];
            UpdateMapEntry(biome.ToString(), GetName(biome), biome);
        }
        private static void UpdateMapEntry(string key, string legend, ZoneType colorReference, GameObject go = null)
        {
            if (!ColorMap.TryGetValue(key, out MapOverlayEntry entry))
            {
                entry = new MapOverlayEntry() { Name = legend, Color = GetColor(colorReference) };
                ColorMap.Add(key, entry);
            }

            if (go != null && !entry.GameObjects.ContainsKey(go.GetInstanceID()))
            {
                entry.GameObjects.Add(go.GetInstanceID(), go);
            }
        }
        static Color GetColor(ZoneType zone)
        {
            Color color = Color.white;
            
            color = World.Instance.GetComponent<SubworldZoneRenderData>().zoneColours[(int)zone];
            color.a = 1f;
            return color;
        }
        static string BiomeName(ZoneType type)
        {
            switch (type)
            {
                case ZoneType.FrozenWastes:
                    return FROZEN.NAME;
                case ZoneType.CrystalCaverns:
                    return NIOBIUM.NAME; // TODO: Check. Per YAML files, niobium subworld should actually be ZoneType.OilField and CrystalCaverns would remain unused.
                case ZoneType.BoggyMarsh:
                    return MARSH.NAME;
                case ZoneType.Sandstone:
                    return SANDSTONE.NAME;
                case ZoneType.ToxicJungle:
                    return JUNGLE.NAME;
                case ZoneType.MagmaCore:
                    return MAGMA.NAME;
                case ZoneType.OilField:
                    return OIL.NAME;
                case ZoneType.Space:
                    return SPACE.NAME;
                case ZoneType.Ocean:
                    return OCEAN.NAME + " & "+ AQUATIC.NAME;
                case ZoneType.Rust:
                    return RUST.NAME;
                case ZoneType.Forest:
                    return FOREST.NAME;
                case ZoneType.Radioactive:
                    return RADIOACTIVE.NAME;
                case ZoneType.Swamp:
                    return SWAMP.NAME;
                case ZoneType.Wasteland:
                    return WASTELAND.NAME;
                case ZoneType.RocketInterior:
                    return "Rocket Interior";
                case ZoneType.Metallic:
                    return METALLIC.NAME;
                case ZoneType.Barren:
                    return BARREN.NAME;
                case ZoneType.Moo:
                    return MOO.NAME;
                default:
                    return "Unknown";


            }
        }
    }
}
