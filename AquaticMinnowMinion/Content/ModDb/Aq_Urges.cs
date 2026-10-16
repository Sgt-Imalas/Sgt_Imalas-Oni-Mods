using Database;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_Urges
	{
		public static Urge MoisturizeMe;
		public static void Register(Urges __instance)
		{
			MoisturizeMe = __instance.Add(new Urge(nameof(MoisturizeMe)));
		}
	}
}
