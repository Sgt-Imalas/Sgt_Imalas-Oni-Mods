namespace Rockets_TinyYetBig.Patches
{
	/// <summary>
	/// Expermiments with power consuming rocket modules
	/// </summary>
	internal class PowerPatches
	{
		//[HarmonyPatch(typeof(CircuitManager), "PowerFromBatteries")]
		//public static class PowerFromBatteries
		//{
		//    public static void Postfix(float joules_needed, List<Battery> batteries, IEnergyConsumer c)
		//    {
		//        SgtLogger.l(joules_needed + "", "JoulesNeeded");
		//        foreach(var battery in batteries)
		//        {
		//            SgtLogger.l(battery.Name + "", "Battery");
		//        }
		//        SgtLogger.l(c.Name + ", "+c.WattsUsed+", "+c.WattsNeededWhenActive, "watts");

		//    }
		//}
	}
}
