using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

		public Exporter EntityIconPath(string path)
		{
			entityExportPath=path;
			return this;
		}

		public static void WriteUISprite(string path, string fileName, KAnimFile kanimFile)
		{
			if (kanimFile == null) return;

			var UISprite = Def.GetUISpriteFromMultiObjectAnim(kanimFile);

			if (UISprite != null && UISprite != Assets.GetSprite("unknown"))
			{
				MarkdownUtil.WriteUISpriteToFile(UISprite, path, fileName);
			}
		}

		static string entityExportPath = null;
		static HashSet<Tag> ToExportEntities;

		public void ExportEntityIcons()
		{
			if (entityExportPath == null)
				return;
			foreach (var tag in ToExportEntities)
			{
				var entiy = Assets.GetPrefab(tag);
				if (entiy == null)
					continue;
				var UISprite = Def.GetUISprite(entiy).first;
				if (UISprite != null && UISprite != Assets.GetSprite("unknown"))
				{
					MarkdownUtil.WriteUISpriteToFile(UISprite, entityExportPath, tag.ToString());
				}

			}
		}

		internal static void AddEntity(Tag tag)
		{
			if(ToExportEntities == null)
				ToExportEntities = new HashSet<Tag>();
			ToExportEntities.Add(tag);
		}
	}
}
