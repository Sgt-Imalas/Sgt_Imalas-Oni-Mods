using ImageMagick;
using Microsoft.Build.Framework;
using System.Diagnostics;
using System.IO.Compression;

namespace _KAnimPacker
{
	public class PackKanims : Microsoft.Build.Utilities.Task
	{
		[Required] public string Executable { get; set; }
		[Required] public string InputDirectory { get; set; }
		public override bool Execute()
		{
			if (!File.Exists(Executable))
			{
				Log.LogError($"texconv executable not found: {Executable}"); return false;
			}
			if (!Directory.Exists(InputDirectory))
			{
				Log.LogError($"Input directory not found: {InputDirectory}");
				return false;
			}
			string outputDirectory = InputDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + "_packed";
			try
			{
				if (Directory.Exists(outputDirectory))
					Directory.Delete(outputDirectory, true);

				Directory.CreateDirectory(outputDirectory);
				ProcessDirectory(InputDirectory, outputDirectory);
				return !Log.HasLoggedErrors;
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex, true);
				return false;
			}
		}
		private void ProcessDirectory(string inputDirectory, string outputDirectory)
		{
			Directory.CreateDirectory(outputDirectory);
			string[] pngFiles = Directory.GetFiles(inputDirectory, "*.png", SearchOption.TopDirectoryOnly);

			if (pngFiles.Any())
			{
				ProcessPackedDirectory(inputDirectory, outputDirectory);
				return;
			}

			foreach (string file in Directory.GetFiles(inputDirectory, "*", SearchOption.TopDirectoryOnly))
			{
				string destination = Path.Combine(outputDirectory, Path.GetFileName(file));
				File.Copy(file, destination, true);
			}

			foreach (string directory in Directory.GetDirectories(inputDirectory))
			{
				string name = Path.GetFileName(directory);
				string destination = Path.Combine(outputDirectory, name);
				ProcessDirectory(directory, destination);
			}
		}
		private void ProcessPackedDirectory(string inputDirectory, string outputDirectory)
		{
			Log.LogMessage(MessageImportance.Normal, $"Packing: {inputDirectory}");
			string tempDirectory = Path.Combine(Path.GetTempPath(), "TexconvTask", Guid.NewGuid().ToString("N"));

			try
			{
				Directory.CreateDirectory(tempDirectory);
				foreach (string file in Directory.GetFiles(inputDirectory, "*", SearchOption.TopDirectoryOnly))
				{
					if (Path.GetExtension(file).ToLowerInvariant() == ".png")
						continue;

					string destination = Path.Combine(tempDirectory, Path.GetFileName(file));
					File.Copy(file, destination, true);
				}
				foreach (string png in Directory.GetFiles(inputDirectory, "*.png", SearchOption.TopDirectoryOnly))
				{
					ConvertPngToDds(png, tempDirectory);
				}
				foreach (string directory in Directory.GetDirectories(inputDirectory))
				{
					string name = Path.GetFileName(directory);
					ProcessDirectory(directory, Path.Combine(tempDirectory, name));
				}
				string zipPath = outputDirectory + ".zip";
				if (File.Exists(zipPath))
					File.Delete(zipPath);

				ZipFile.CreateFromDirectory(tempDirectory, zipPath, CompressionLevel.Optimal, false);
				Directory.Delete(outputDirectory, true);
			}
			finally { if (Directory.Exists(tempDirectory)) Directory.Delete(tempDirectory, true); }
		}
		private void ConvertPngToDds(string png, string outputDirectory)
		{
			string inputPath = Path.GetFullPath(png);
			string normalized = string.Empty;
			try
			{
				normalized = CreateNormalizedTempPng(inputPath);

				var psi = new ProcessStartInfo
				{
					FileName = Executable,
					//< Exec Command = 'texconv -f BC7_UNORM -vflip -m 1 -y "%(Texture.FullPath)" -o "$(TextureOutputDir)\%(Texture.RecursiveDir)\."' />
					Arguments = $"-f BC7_UNORM -vflip -m 1 -y \"{normalized}\" -o \"{outputDirectory}\"",
					WorkingDirectory = outputDirectory,
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};
				using var process = new Process();
				process.StartInfo = psi; process.OutputDataReceived += (sender, e) =>
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						Log.LogMessage(MessageImportance.Normal, e.Data);
					}
				};
				process.ErrorDataReceived += (sender, e) =>
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						Log.LogError(e.Data);
					}
				}; Log.LogMessage(MessageImportance.Normal, $"Converting: {png}");
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
				if (process.ExitCode != 0)
				{
					Log.LogError($"texconv failed for '{png}' " + $"with exit code {process.ExitCode}.");
				}
			}
			catch (Exception ex)
			{
				Log.LogError(ex.ToString());
				throw;
			}
			finally
			{
				if(File.Exists(normalized))
					File.Delete(normalized);
			}
		}
		private static string CreateNormalizedTempPng(string source)
		{
			string temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");

			using var image = new MagickImage(source);

			image.ColorSpace = ColorSpace.sRGB;
			image.Strip();
			image.Write(temp, MagickFormat.Png);

			return temp;
		}
	}
}
