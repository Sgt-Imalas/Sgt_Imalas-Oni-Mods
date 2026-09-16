using Database;

namespace BioluminescentDupes
{
	internal class ModAssets
	{
		public class _AssignableSlots
		{
			public static AssignableSlot TraitAddingItem;
			public const string TraitAddingSlotID = "BioluminescentDupes_TraitAddingItem";

			public static void Register(AssignableSlots parent)
			{
				TraitAddingItem = parent.Add(new AssignableSlot(TraitAddingSlotID, "Trait Upgrade", true));
			}
		}
	}
}
