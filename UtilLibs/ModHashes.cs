namespace UtilLibs
{
	/// <summary>
	/// Courtesy of Aki
	/// </summary>

	public class ModHashes
	{
		private readonly int value;
		private readonly string name;
		private readonly GameHashes hash;

		public ModHashes(string name)
		{
			this.name = name;
			value = Hash.SDBMLower(name);
			hash = (GameHashes)value;
		}

		public static implicit operator GameHashes(ModHashes modHashes)
		{
			return modHashes.hash;
		}

		public static implicit operator int(ModHashes modHashes)
		{
			return modHashes.value;
		}

		public static implicit operator string(ModHashes modHashes)
		{
			return modHashes.name;
		}

		public override string ToString()
		{
			return name;
		}

		public static int SDBMLower(string s)
		{
			if (s == null)
			{
				return 0;
			}

			uint num = 0u;
			for (int i = 0; i < s.Length; i++)
			{
				char c = char.ToLowerInvariant(s[i]);
				num = c + (num << 6) + (num << 16) - num;
			}

			return (int)num;
		}
	}
}
