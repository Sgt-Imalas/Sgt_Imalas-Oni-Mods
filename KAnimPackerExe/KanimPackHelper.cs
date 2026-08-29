using ImageMagick;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO.Compression;

namespace _KAnimPackerExe
{
	public class KanimPackHelper
	{
		public KanimPackHelper(string exePath, string inputDir)
		{
			Executable = exePath;
			InputDirectory = inputDir;
		}
		public KanimPackHelper(string inputDir)
		{
			Executable = "texconv.exe";
			InputDirectory = inputDir;
		}

		[Required] public string Executable { get; set; }
		[Required] public string InputDirectory { get; set; }
		private int _converted = 0;
		private DateTime _start;
		public bool ConvertToKanims()
		{
			_start = DateTime.Now;
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
				ProcessDirectoryKANIM(InputDirectory, outputDirectory);
				return !Log.HasLoggedErrors;
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex, true);
				return false;
			}
			finally
			{
				Report();
			}
		}
		public bool ConvertSingularTextures()
		{
			_start = DateTime.Now;

			if (!File.Exists(Executable))
			{
				Log.LogError($"texconv executable not found: {Executable}");
				return false;
			}

			if (!Directory.Exists(InputDirectory))
			{
				Log.LogError($"Input directory not found: {InputDirectory}");
				return false;
			}

			string outputDirectory =
				InputDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + "_packed";

			try
			{
				if (Directory.Exists(outputDirectory))
					Directory.Delete(outputDirectory, true);

				Directory.CreateDirectory(outputDirectory);
				ProcessDirectoryTEX(InputDirectory, outputDirectory);

				return !Log.HasLoggedErrors;
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex, true);
				return false;
			}
			finally
			{
				Report();
			}
		}

		private void ProcessDirectoryTEX(string inputDirectory, string outputDirectory)
		{
			Directory.CreateDirectory(outputDirectory);

			// Only PNG files are processed.
			foreach (string png in Directory.GetFiles(inputDirectory, "*.png", SearchOption.TopDirectoryOnly))
			{
				ProcessPngFileTEX(png, outputDirectory);
			}

			// Continue processing subdirectories.
			foreach (string directory in Directory.GetDirectories(inputDirectory))
			{
				string name = Path.GetFileName(directory);
				string destination = Path.Combine(outputDirectory, name);

				ProcessDirectoryTEX(directory, destination);
			}
		}
		private void ProcessPngFileTEX(string png, string outputDirectory)
		{
			string fileName = Path.GetFileNameWithoutExtension(png);
			string tempDirectory = Path.Combine(
				Path.GetTempPath(),
				"TexconvTask",
				Guid.NewGuid().ToString("N"));

			try
			{
				Directory.CreateDirectory(tempDirectory);

				// Do not modify ConvertPngToDds.
				ConvertPngToDds(png, tempDirectory);

				string ddsFile = Path.Combine(tempDirectory, fileName + ".dds");

				if (!File.Exists(ddsFile))
				{
					Log.LogError($"DDS file was not created: {ddsFile}");
					return;
				}

				string zipPath = Path.Combine(outputDirectory, fileName + ".zip");

				if (File.Exists(zipPath))
					File.Delete(zipPath);

				// Create a ZIP containing exactly one DDS.
				ZipFile.CreateFromDirectory(
					tempDirectory,
					zipPath,
					CompressionLevel.Optimal,
					false);

				_converted++;
				Log.LogMessage($"Packing: {png} -> {zipPath}");
			}
			finally
			{
				if (Directory.Exists(tempDirectory))
					Directory.Delete(tempDirectory, true);
			}
		}

		private void ProcessDirectoryKANIM(string inputDirectory, string outputDirectory)
		{
			Directory.CreateDirectory(outputDirectory);
			string[] pngFiles = Directory.GetFiles(inputDirectory, "*.png", SearchOption.TopDirectoryOnly);

			if (pngFiles.Any())
			{
				ProcessPackedDirectoryKANIM(inputDirectory, outputDirectory);
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
				ProcessDirectoryKANIM(directory, destination);
			}
		}
		private void ProcessPackedDirectoryKANIM(string inputDirectory, string outputDirectory)
		{
			Log.LogMessage($"Packing: {inputDirectory}");
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
					ProcessDirectoryKANIM(directory, Path.Combine(tempDirectory, name));
				}
				string zipPath = outputDirectory + ".zip";
				if (File.Exists(zipPath))
					File.Delete(zipPath);

				ZipFile.CreateFromDirectory(tempDirectory, zipPath, CompressionLevel.Optimal, false);
				_converted++;
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
				string fileName = Path.GetFileNameWithoutExtension(png);
				normalized =  PNGConverter.CreateNormalizedTempPng(inputPath);

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
						Log.LogMessage(e.Data);
					}
				};
				process.ErrorDataReceived += (sender, e) =>
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						Log.LogError(e.Data);
					}
				}; Log.LogMessage($"Converting: {png}");
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
				if (File.Exists(normalized))
					File.Delete(normalized);
			}
		}
		public void Report()
		{
			Log.LogMessage($"Conversion finished for {InputDirectory}");
			Log.LogMessage($"Number of zipped folders created: {_converted}");
			Log.LogMessage($"Time taken: {(DateTime.Now - _start).TotalSeconds.ToString("0.00")} s");

		}
	}
}
