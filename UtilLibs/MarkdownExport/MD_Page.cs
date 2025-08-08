using Newtonsoft.Json;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.MarkdownExport
{
	public class MD_Page : IMD_File
	{
		public string Title;
		public List<IMD_Entry> Entries;

		public MD_Page(string fileName)
		{
			this.Title = fileName;
		}
		public MD_BuildingEntry AddBuilding(string buildingId)
		{
			var entry = new MD_BuildingEntry(buildingId);
			if(Entries == null)
				Entries = [entry];
			else
				Entries.Add(entry);
			return entry;
		}


		public void CreateMarkdownFiles(string inheritedPath)
		{
			var fileInfo = new FileInfo(System.IO.Path.Combine(inheritedPath, Title + ".md"));

			FileStream fcreate = fileInfo.Open(FileMode.Create);			
			using (var streamWriter = new StreamWriter(fcreate))
			{
				streamWriter.WriteLine("# " + Title);
				foreach (var entry in Entries)
				{
					streamWriter.WriteLine(entry.FormatAsMarkdown());
				}
			}
		}
	}
}
