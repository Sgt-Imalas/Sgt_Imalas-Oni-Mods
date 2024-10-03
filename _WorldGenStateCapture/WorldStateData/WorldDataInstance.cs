using _WorldGenStateCapture.WorldStateData.Starmap.SpacemapItems;
using System.Collections.Generic;

namespace _WorldGenStateCapture.WorldStateData
{
	internal class WorldDataInstance
	{
		public string coordinate;
		public string cluster;

		public List<string> dlcs;

		public List<AsteroidData> asteroids = new List<AsteroidData>();
		public List<VanillaMap_Entry> starMapEntriesVanilla = null;
		public List<HexMap_Entry> starMapEntriesSpacedOut = null;
	}
}
