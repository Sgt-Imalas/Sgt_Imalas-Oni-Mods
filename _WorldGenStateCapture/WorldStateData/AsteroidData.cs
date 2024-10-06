using _WorldGenStateCapture.WorldStateData.WorldPOIs;
using System.Collections.Generic;

namespace _WorldGenStateCapture.WorldStateData
{
	internal class AsteroidData
	{
		public string id;
		public int offsetX, offsetY; //bottom left corner of the asteroid 
		public int sizeX, sizeY;


		public List<string> worldTraits;
		public List<MapPOI> pointsOfInterest = new List<MapPOI>();
		public List<MapGeyser> geysers = new List<MapGeyser>();

		public string BiomesSVG;
	}
}
