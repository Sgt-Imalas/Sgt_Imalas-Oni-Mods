using Database;
using HarmonyLib;
using Klei.AI;
using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NotUpdateDate
{
	internal class Patches
	{

        [HarmonyPatch(typeof(Manager), nameof(Manager.Subscribe))]
        public class KMod_Manager_Subscribe_Patch
        {
            public static bool Prefix() => false;
		}

		[HarmonyPatch(typeof(Manager), nameof(Manager.Unsubscribe))]
		public class KMod_Manager_Unsubscribe_Patch
		{
			public static bool Prefix() => false;
		}
		[HarmonyPatch(typeof(Manager), nameof(Manager.Update), [typeof(KMod.Mod),typeof(object)])]
		public class KMod_Manager_Update_Patch
		{
			public static bool Prefix() => false;
		}
	}
}
