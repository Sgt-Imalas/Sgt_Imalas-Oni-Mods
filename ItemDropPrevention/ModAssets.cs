using ItemDropPrevention.Content.Scripts;
using UnityEngine;

namespace ItemDropPrevention
{
	internal class ModAssets
	{
		public static Tag BlockedFromDoingStuff = TagManager.Create("MassMoveTo_BlockedFromMerging");
		internal static bool MarkForLaterDroppage(GameObject gameObject)
		{
			if (!gameObject.TryGetComponent<DroppablesHolder>(out var droppablesHolder))
			{
				return true;
			}
			droppablesHolder.MarkAllItemsForDrop();
			return false;
		}
	}
}
