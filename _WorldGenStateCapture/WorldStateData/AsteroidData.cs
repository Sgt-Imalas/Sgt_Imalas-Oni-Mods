using _WorldGenStateCapture.WorldStateData.WorldPOIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _WorldGenStateCapture.WorldStateData
{
    internal class AsteroidData
    {
        public string Id;
        public int OffsetX, OffsetY; //bottom left corner of the asteroid 
        public int SizeX, SizeY;


        public List<string> WorldTraits;
        public List<string> StoryTraits;
        public List<MapPOI> POIs = new List<MapPOI>();
        public List<MapGeyser> Geysers = new List<MapGeyser>();
    }
}
