using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalModLoader.DataClasses
{
	internal class HashMap
	{
		public Dictionary<string, string> FileHashes = [];
		public bool TryGet(string file, out string hash) => FileHashes.TryGetValue(file, out hash);
		public bool HasDifferences(HashMap other, out HashSet<string> diffs)
		{
			diffs = new HashSet<string>();
			foreach (var kvp in this.FileHashes)
			{
				if (other.TryGet(kvp.Key, out string otherHash) && otherHash == kvp.Value)
					continue;
				diffs.Add(kvp.Key);
			}
			return diffs.Any();
		}
	}
}
