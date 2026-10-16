using Karambolo.PO;
using Microsoft.Build.Framework;


namespace _POVerify
{
	public class ValidatePoFiles : Microsoft.Build.Utilities.Task
	{
		[Required]
		public ITaskItem[] Files { get; set; } = Array.Empty<ITaskItem>();
		public override bool Execute()
		{
			return ValidateFiles();
		}

		bool ValidateFiles()
		{
			if (!Files.Any())
			{
				Log.LogMessage("No PO files found in mod, skipping validation");
			}
			Log.LogMessage(MessageImportance.High, $"Validating {Files.Count()} PO files");

			bool success = true;

			foreach (var file in Files)
			{
				var path = file.ItemSpec;

				if (!File.Exists(path))
					continue;

				if (!ValidatePOFile(path))
					success = false;
			}
			if(success)
				Log.LogMessage(MessageImportance.High, "All PO files validated successfully.");
			return success;
		}

		bool ValidateXML(POParseResult result)
		{
			foreach (var entry in result.Catalog.Keys)
			{
				if (entry.Id == string.Empty || entry.Id == null)
					continue;
				string translated = result.Catalog.GetTranslation(entry);
				if (translated == string.Empty)
					continue;

				if (!ValidTag(entry, ref translated, "style")
				 || !ValidTag(entry, ref translated, "link")
					)
					return false;

			}

			return true;
		}

		bool ValidTag(POKey key, ref string check, string tag)
		{
			string tagStart = $"<{tag}=\"";
			int openTagStart = check.IndexOf(tagStart);
			if (openTagStart < 0)
				return true;

			int openTagEnd = check.IndexOf("\">", openTagStart + tagStart.Length);
			if (openTagStart != -1 && openTagEnd < openTagStart) //include the backspaces
			{
				Log.LogError($"invalid opening tag for {openTagEnd} != {openTagStart + tagStart.Length + tag.Length} {tag}: {check}");
				return false;
			}
			int closingTag = check.IndexOf($"</{tag}>", openTagEnd);
			if (openTagStart != -1 && closingTag <= openTagStart)
			{
				Log.LogError($" no closing tag for {tag}: {check}");
				return false;
			}
			return openTagStart != -1 && openTagEnd + 6 > openTagStart;
		}


		bool ValidatePOFile(string filePath)
		{
			try
			{
				using var reader = new StreamReader(filePath);
				var parser = new POParser();
				var result = parser.Parse(reader);
				if (result.Success && ValidateXML(result))
				{
					Log.LogMessage(MessageImportance.High, $"{filePath} validated successfully.");
					return true;
				}
				Log.LogError($"{filePath} is an invalid PO file!");
				foreach (var diagnostic in result.Diagnostics)
				{
					Log.LogError($"{filePath}: {diagnostic}");
				}
				return false;
			}
			catch (Exception ex)
			{
				Log.LogError($"{filePath}: exception while parsing PO file: {ex.Message}");

				return false;
			}
		}
	}
}
