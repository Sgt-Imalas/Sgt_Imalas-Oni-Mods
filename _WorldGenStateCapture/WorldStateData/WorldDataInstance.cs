using _WorldGenStateCapture.WorldStateData.Starmap.SpacemapItems;
using _WorldGenStateCapture.WorldStateData.WorldPOIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _WorldGenStateCapture.WorldStateData
{
    internal class WorldDataInstance
    {
        public int Seed;
        public string Coordinate, FullCoordinate;
        public List<string> StoryTraits = new List<string>();
        public List<AsteroidData> Asteroids = new List<AsteroidData>();
        public List<VanillaMap_Entry> StarmapEntries_Vanilla = null;
        public List<HexMap_Entry> StarmapEntries_SpacedOut = null;

    }
}
