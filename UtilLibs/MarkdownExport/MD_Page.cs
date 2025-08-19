using Newtonsoft.Json;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UtilLibs.MarkdownExport.MD_Localization;
using static UtilLibs.MarkdownExport.MarkdownUtil;

namespace UtilLibs.MarkdownExport
{
	public class MD_Page : IMD_File
	{
		public string FileName;
		public string TitleKey;
		public List<IMD_Entry> Entries = [];
		public bool WritePage = true;

		public MD_Page(string fileName, string titleKey = null)
		{
			this.FileName = fileName;
			this.TitleKey = titleKey == null ? fileName : titleKey;
		}
		public MD_BuildingEntry AddBuilding(string buildingId)
		{
			var entry = new MD_BuildingEntry(buildingId);
			if (Entries == null)
				Entries = [entry];
			else
				Entries.Add(entry);
			return entry;
		}

		public MD_Page Add(IMD_Entry child)
		{
			Entries.Add(child);
			return this;
		}

		public void CreateMarkdownFiles(string inheritedPath)
		{
			if(!WritePage)
				return;

			var fileInfo = new FileInfo(System.IO.Path.Combine(inheritedPath, FileName + MD_Localization.GetSuffix() + ".md"));

			FileStream fcreate = fileInfo.Open(FileMode.Create);
			using (var streamWriter = new StreamWriter(fcreate))
			{
				streamWriter.WriteLine("# " + L(TitleKey.ToUpperInvariant()));
				foreach (var entry in Entries)
				{
					streamWriter.WriteLine(entry.FormatAsMarkdown());
				}
			}
		}
	}
}
