using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util_TwitchIntegrationLib;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// all dupes lose their hair (set symbol visibility)
	/// </summary>
	internal class HairLossEvent : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_HairLossEvent";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.HAIRLOSS.NAME;

		public EventWeight EventWeight => EventWeight.WEIGHT_COMMON;

		public Action<object> EventAction => (object data) =>
		{
			ToastManager.InstantiateToast(
				STRINGS.CHAOSEVENTS.HAIRLOSS.TOAST,
				 STRINGS.CHAOSEVENTS.HAIRLOSS.TOASTTEXT
				 );

			ToggleMinionHair(true);


			GameScheduler.Instance.Schedule("undo hairloss", 600
				, _ =>
			{
				ToggleMinionHair(false);
			});
		};
		public void ToggleMinionHair(bool hide)
		{
			var slots = Db.Get().AccessorySlots;

			var hairSymbol = slots.Hair.targetSymbolId;
			var hatSymbol = slots.Hat.targetSymbolId;
			var hatHairSymbol = slots.HatHair.targetSymbolId;

			foreach (MinionIdentity minion in Components.LiveMinionIdentities)
			{
				if(minion!=null && minion.TryGetComponent<KBatchedAnimController>(out var kbac) && minion.TryGetComponent<Accessorizer>(out var accessorizer))
				{

					if (hide)
					{
						kbac.SetSymbolVisiblity(hairSymbol, false);
						kbac.SetSymbolVisiblity(hatHairSymbol, false);
						kbac.SetSymbolVisiblity(hatSymbol, false);
					}
					else
						accessorizer.UpdateHairBasedOnHat();

				}
			}
		}

		public Func<object, bool> Condition => (data) => true;

		public Danger EventDanger => Danger.None;

	}
}
