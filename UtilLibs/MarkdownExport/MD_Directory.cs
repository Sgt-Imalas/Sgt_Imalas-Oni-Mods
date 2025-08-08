using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.MarkdownExport
{
	public class MD_Directory : IMD_File
	{
		public string Name;
		public List<MD_Directory> SubDirectories = [];
		public List<MD_Page> Files = [];

		public MD_Directory(string targetDirectory)
		{
			this.Name = targetDirectory;
		}
		public MD_Directory SubDir(string subDirName)
		{
			if (SubDirectories == null)
				SubDirectories = [];
			var subDir = new MD_Directory(subDirName);
			SubDirectories.Add(subDir);
			return subDir;
		}
		public MD_Page File(string fileName)
		{
			if (Files == null)
				Files = [];
			var file = new MD_Page(fileName);
			Files.Add(file);
			return file;
		}

		public void CreateMarkdownFiles(string inheritedPath)
		{
			var nest = Path.Combine(inheritedPath, this.Name);
			Directory.CreateDirectory(nest);

			foreach (var subDir in SubDirectories)
			{
				subDir.CreateMarkdownFiles(nest);
			}
			foreach(var file in Files)
			{
				file.CreateMarkdownFiles(nest);
			}
		}
	}
}
