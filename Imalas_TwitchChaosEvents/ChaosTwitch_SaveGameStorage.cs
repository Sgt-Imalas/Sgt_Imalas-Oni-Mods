using Imalas_TwitchChaosEvents.Elements;
using KSerialization;

namespace Imalas_TwitchChaosEvents
{
	internal class ChaosTwitch_SaveGameStorage : KMonoBehaviour
	{
		public static ChaosTwitch_SaveGameStorage Instance;

		[Serialize] public bool hasUnlockedTacoRecipe;
		[Serialize] public float lastTacoRain = 0;
		[Serialize] public int MigrationState = 0;
		[Serialize] public bool InvertedWaterGotSpawned = false;
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Instance = this;

			Migrate();
		}
		void Migrate()
		{
			if(MigrationState < 1)
			{
				var reverseWater = ModElements.InverseWater.SimHash;
				for (int i = 0; i < (Grid.HeightInCells * Grid.WidthInCells); i++) 
				{
					if(Grid.Element[i].id == reverseWater)
					{
						InvertedWaterGotSpawned = true;
						break;
					}
				}
				MigrationState = 1;
			}
		}
	}
}
