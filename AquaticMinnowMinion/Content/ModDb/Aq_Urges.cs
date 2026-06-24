using Database;
using System;
using System.Collections.Generic;
using System.Text;

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
