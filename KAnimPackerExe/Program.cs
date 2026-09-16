using _KAnimPackerExe;
using _KAnimPackerExe.ModConverters;
class Program
{
	static void Main(string[] args)
	{
		if (args.Length > 0)
		{
			foreach (string path in args)
			{
				Console.WriteLine("Converting: " + args[0]);
				if (path.EndsWith(".png") || Directory.Exists(path))
				{
					KanimPackHelper.ConvertSingleFile(path);
				}
			}
			Console.ReadLine();
		}
		else
		{
			///Ronivans Legacy:
			RonivanAIO.PackModDirectories();
		}
	}
}



