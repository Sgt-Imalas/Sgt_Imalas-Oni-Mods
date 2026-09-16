namespace _SgtsModUpdater.Model
{
	internal class KPlayerPrefs
	{
		public Dictionary<string, string> strings;
		public Dictionary<string, int> ints;
		public Dictionary<string, float> floats;

		public bool SpacedOutEnabled() => ints != null && ints.TryGetValue("EXPANSION1_ID.ENABLED", out var enabled) && enabled == 1;
	}
}
