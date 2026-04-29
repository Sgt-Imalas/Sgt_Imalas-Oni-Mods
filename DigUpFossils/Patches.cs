using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DigUpFossils
{
	internal class Patches
	{

		[HarmonyPatch(typeof(DigTool), nameof(DigTool.OnDragTool))]
		public class DigTool_OnDragTool_Patch
		{
			public static void Postfix(DigTool __instance, int cell)
			{
				var potentialFossil = Grid.Objects[cell, (int)ObjectLayer.Building];
				if (potentialFossil != null && potentialFossil.TryGetComponent<FossilBits>(out var bits) && !bits.MarkedForDig)
					bits.OnSidescreenButtonPressed();

			}
		}
	}
}
