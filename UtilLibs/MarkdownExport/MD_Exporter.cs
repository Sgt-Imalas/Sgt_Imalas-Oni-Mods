using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.MarkdownExport
{
	public class MD_Exporter
	{
		string TargetDirectory;

		public MD_Directory root;

		public static MD_Exporter Instance;

		public Dictionary<string, Dictionary<Tag, List<Tag>>> RandomRecipeResults = [];
		public Dictionary<string, Dictionary<Tag, List<Tag>>> RandomRecipeOccurences = [];



		public static MD_Exporter Create(string directory)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(directory));
			MD_Exporter exporter = new MD_Exporter();
			exporter.TargetDirectory = directory;
			exporter.root = new MD_Directory(exporter.TargetDirectory);
			Instance = exporter;
			return exporter;
		}
		public void Export()
		{
			if (root == null)
			{
				throw new InvalidOperationException("Root directory is not set.");
			}
			root.CreateMarkdownFiles("");
		}
	}
}
