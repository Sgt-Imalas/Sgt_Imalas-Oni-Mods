using ItemDropPrevention.Content.Scripts;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ItemDropPrevention
{
	internal class ModAssets
	{
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
