namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	/// <summary>
	/// mirrored from (old) NoManualDelivery behavior, Automatable that starts off with the "Automation only" disabled
	/// fallback class if no manual delivery is not found
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
