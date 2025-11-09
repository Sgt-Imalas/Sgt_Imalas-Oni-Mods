using LocalModLoader.DataClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LocalModLoader
{
	internal class HashDiffHelper
	{
		public static HashMap CollectAllRecursively(DirectoryInfo dir)
		{
			var obj = new HashMap();
			using (var sha = SHA256.Create())
			{
				CollectFolder(obj, sha, dir, dir);
			}
			return obj;
		}
		public static bool AllFilesUpToDate(DirectoryInfo target, HashMap remoteInfo, out HashSet<string> diffs)
		{
			var local = CollectAllRecursively(target);
			return remoteInfo.HasDifferences(local, out diffs);
		}

		public static void CollectFolder(HashMap obj, SHA256 sha, DirectoryInfo root, DirectoryInfo current)
		{
			foreach (var folder in current.GetDirectories())
			{
				CollectFolder(obj, sha, root, folder);
			}
			foreach (var file in current.GetFiles())
			{
				if (file.Name.StartsWith("._"))//mac
					continue;
				string relativePath = Path.Combine(file.FullName.Replace(root.FullName, string.Empty));
				var stream = File.OpenRead(file.FullName);
				var hashBytes = sha.ComputeHash(stream);

				obj.FileHashes[relativePath] = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
			}
		}
	}
}
