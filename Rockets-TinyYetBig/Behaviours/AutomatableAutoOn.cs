namespace Rockets_TinyYetBig.Behaviours
{
	/// <summary>
	/// mirrored from NoManualDelivery, Automatable that starts off with the "Automation only" disabled
	/// </summary>
	internal class AutomatableAutoOn : Automatable
	{
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			SetAutomationOnly(false);
		}
	}
}
