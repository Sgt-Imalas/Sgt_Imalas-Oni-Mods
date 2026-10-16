using KSerialization;
using UnityEngine;
using static BathTub.STRINGS.ITEMS.INDUSTRIAL_PRODUCTS;

namespace BathTub.Duck
{
	internal class DuckNoises : KMonoBehaviour, ISidescreenButtonControl
	{
		[Serialize]
		[SerializeField]
		public bool STFU = false;

		private System.Action<object> m_onSelectObjectDelegate;
		private SchedulerHandle NextQuackHandle;

		public string SidescreenButtonText => STFU ? BT_RUBBERDUCKIE.BUTTON_OFF : BT_RUBBERDUCKIE.BUTTON_ON;

		public string SidescreenButtonTooltip => BT_RUBBERDUCKIE.TOOLTIP;

		public override void OnSpawn()
		{
			this.m_onSelectObjectDelegate = new System.Action<object>(this.OnSelect);
			base.OnSpawn();
			this.Subscribe((int)GameHashes.SelectObject, m_onSelectObjectDelegate);
			ScheduleNextQuack();
		}
		public override void OnCleanUp()
		{
			this.Unsubscribe((int)GameHashes.SelectObject, m_onSelectObjectDelegate);
			if (NextQuackHandle.IsValid)
				NextQuackHandle.ClearScheduler();

			base.OnCleanUp();
		}
		public void ScheduleNextQuack()
		{
			var time = new KRandom().Next(30, 200);
			NextQuackHandle = GameScheduler.Instance.Schedule("Quack", time, Quack);
		}
		public void Quack(object o)
		{
			if (!STFU && CameraController.Instance.IsVisiblePos(this.transform.GetPosition())) //no offscreen quacks
				ModAssets.PlayRandomQuack(this);
			ScheduleNextQuack();
		}

		void OnSelect(object data)
		{
			bool selected = ((Boxed<bool>)data).value;
			if (!selected) //on deselect
				return;
			ModAssets.PlayRandomSqueak(this);
		}

		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
		}

		public bool SidescreenEnabled()
		{
			return true;
		}

		public bool SidescreenButtonInteractable()
		{
			return true;
		}

		public void OnSidescreenButtonPressed()
		{
			ToggleSTFU();
		}

		private void ToggleSTFU()
		{
			STFU = !STFU;
		}

		public int HorizontalGroupID()
		{
			return -1;
		}

		public int ButtonSideScreenSortOrder()
		{
			return 20;
		}
	}
}
