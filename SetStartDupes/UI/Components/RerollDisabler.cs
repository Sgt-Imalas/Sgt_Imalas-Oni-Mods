using System.Collections.Generic;

namespace SetStartDupes.UI.Components
{
	internal class RerollDisabler : KMonoBehaviour
	{
		[MyCmpGet] KButton button;
		static int RemainingRolls = -1;

		LocText Text;
		string originalText;
		CharacterContainer Container;

		public static HashSet<RerollDisabler> ActiveElements = [];
		bool destroyed = false;

		public override void OnSpawn()
		{
			if (destroyed) return;
			ActiveElements.Add(this);
			base.OnSpawn();


			button.onClick += OnRerolled;
			Text = GetComponentInChildren<LocText>();
			originalText = Text.text;

			Container = transform.parent.GetComponent<CharacterContainer>();
			if (Container != null)
			{
				Container.archetypeDropDown.onEntrySelectedAction += OnDropDownSelected;
				Container.modelDropDown.onEntrySelectedAction += OnDropDownSelected;
			}
			RefreshState();
		}
		public override void OnCleanUp()
		{
			if (destroyed) return;
			ActiveElements.Remove(this);
			base.OnCleanUp();
		}
		public static void OnRerolled()
		{
			if (!HasLimitConfigured()) return;

			RemainingRolls--;
			RefreshAllActiveItems();
		}

		void OnDropDownSelected(IListableOption option, object o) => OnRerolled();

		void DisableRolling()
		{
			if (!HasLimitConfigured()) return;

			if (button != null)
				button.isInteractable = false;
			if (Container != null)
			{
				ModAssets.ToggleVisibilityTraitLockButton(Container, false);
				Container.archetypeDropDown.openButton.isInteractable = false;
				Container.modelDropDown.openButton.isInteractable = false;
			}
		}

		void RefreshState()
		{
			if (destroyed)
				return;
			if (RemainingRolls <= 0)
				DisableRolling();
			RefreshText();
		}

		void RefreshText()
		{
			if (!HasLimitConfigured()) return;
			Text?.SetText(originalText + $" ({RemainingRolls})");
		}

		internal void SelfDestruct()
		{
			if(Text != null)
				Text.SetText(originalText);
			if(button != null)
				button.onClick -= OnRerolled;
			if (Container != null)
			{
				Container.archetypeDropDown.onEntrySelectedAction -= OnDropDownSelected;
				Container.modelDropDown.onEntrySelectedAction -= OnDropDownSelected;
			}
			destroyed = true;
			Destroy(this);
		}
		static bool HasLimitConfigured() => Config.Instance.RerollDuringGame_Limiter > 0;
		internal static void RefreshCounter()
		{
			if (!HasLimitConfigured())
				return;

			var limiter = Config.Instance.RerollDuringGame_Limiter;
			RemainingRolls = limiter;
			RefreshAllActiveItems();
		}
		public static void RefreshAllActiveItems()
		{
			foreach (var existing in ActiveElements)
			{
				existing.RefreshState();
			}
		}
	}
}
