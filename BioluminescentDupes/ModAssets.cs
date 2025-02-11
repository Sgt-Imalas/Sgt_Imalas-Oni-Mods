using Database;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
