namespace BionicBoostersPlus.Content.Scripts
{
	internal class BionicUpgrade_SkilledWorkerOverclocked : BionicUpgrade_SkilledWorker
	{
		public class OC_Instance : Instance
		{
			public OC_Instance(IStateMachineTarget master, Def def)
			: base(master, def)
			{
			}

			public override float GetCurrentWattageCost()
			{
				return base.Data.WattageCost;
			}
			
			public override string GetCurrentWattageCostName()
			{
				float currentWattageCost = GetCurrentWattageCost();
				string text = "<b>" + ((currentWattageCost >= 0f) ? "+" : "-") + "</b>";
				return string.Format(global::STRINGS.DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.STANDARD_ACTIVE_TEMPLATE, upgradeComponent.GetProperName(), text + GameUtil.GetFormattedWattage(currentWattageCost));
			}
		}
		public override void InitializeStates(out BaseState default_state)
		{
			base.InitializeStates(out default_state);
			root.TriggerOnExit(GameHashes.BionicUpgradeWattageChanged);
		}
	}
}
