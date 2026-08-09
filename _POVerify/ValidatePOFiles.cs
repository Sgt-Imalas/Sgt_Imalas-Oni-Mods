using Karambolo.PO;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Text;


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

		bool ValidatePOFile(string filePath)
		{
			try
			{
				using var reader = new StreamReader(filePath);
				var parser = new POParser();
				var result = parser.Parse(reader);
				if (result.Success)
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
