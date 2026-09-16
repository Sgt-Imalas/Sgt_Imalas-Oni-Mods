using HarmonyLib;
using KMod;

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
