using System.Collections.Generic;

namespace _WorldGenStateCapture.WorldStateData.WorldPOIs
{
	internal class POITracker : KMonoBehaviour
	{
		public string targetId;
		public override void OnSpawn()
		{
			base.OnSpawn();
			var position = this.transform.GetPosition();

			var myWorld = this.GetMyWorld();
			if (!ModAssets.currentPOIs.ContainsKey(myWorld))
				ModAssets.currentPOIs[myWorld] = new List<MapPOI>();

			ModAssets.currentPOIs[myWorld].Add(new MapPOI()
			{
				id = targetId,
				x = (int)position.x,
				y = (int)position.y,
			});
		}
	}
}
