using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;

namespace NotUpdateDate
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			base.OnLoad(harmony);
			Debug.Log(this.mod.title+" - "+mod.label.version);
		}

	}
}
