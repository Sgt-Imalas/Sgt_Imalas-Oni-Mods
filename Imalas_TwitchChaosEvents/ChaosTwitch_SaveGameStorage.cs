using KSerialization;

namespace Imalas_TwitchChaosEvents
{
	internal class ChaosTwitch_SaveGameStorage : KMonoBehaviour
	{
		public static ChaosTwitch_SaveGameStorage Instance;

		[Serialize] public bool hasUnlockedTacoRecipe;
		[Serialize] public float lastTacoRain = 0;
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Instance = this;
		}
	}
}
