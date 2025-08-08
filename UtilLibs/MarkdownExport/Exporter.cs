using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.MarkdownExport
{
	public class Exporter
	{
		string TargetDirectory;

		public MD_Directory root;

		public static Exporter Instance;

		public Dictionary<string, Dictionary<Tag, List<Tag>>> RandomRecipeResults = [];
		public Dictionary<string, Dictionary<Tag, List<Tag>>> RandomRecipeOccurences = [];

		public static Exporter Create(string directory)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(directory));
			Exporter exporter = new Exporter();
			exporter.TargetDirectory = directory;
			exporter.root = new MD_Directory(exporter.TargetDirectory);
			Instance = exporter;
			return exporter;
		}
		public void Export(string localizeKey = null)
		{
			if (root == null)
			{
				throw new InvalidOperationException("Root directory is not set.");
			}
			if(localizeKey != null)
				MD_Localization.SetLocalization(localizeKey);

			root.CreateMarkdownFiles("");
		}
	}
}
