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

		[Required] public string Executable { get; set; }
		[Required] public string InputDirectory { get; set; }
		private int _converted = 0;
		private DateTime _start;
		public bool Execute()
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
				ProcessDirectory(InputDirectory, outputDirectory);
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
					ProcessDirectory(directory, Path.Combine(tempDirectory, name));
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
		private static string CreateNormalizedTempPng(string source)
		{
			string fileNameInput = Path.GetFileName(source);
			var targetDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
			Directory.CreateDirectory(targetDir);
			string temp = Path.Combine(targetDir, fileNameInput);
			Log.LogMessage("Temp file: " + temp);

			using var image = new MagickImage(source);

			image.ColorSpace = ColorSpace.sRGB;
			image.Strip();
			image.Write(temp, MagickFormat.Png);

			return temp;
		}
		public void Report()
		{
			Log.LogMessage($"Conversion finished for {InputDirectory}");
			Log.LogMessage($"Number of zipped folders created: {_converted}");
			Log.LogMessage($"Time taken: {(DateTime.Now - _start).TotalSeconds.ToString("0.00")} s");

		}
	}
}
