// See https://aka.ms/new-console-template for more information
using _KAnimPackerExe;
using System.Reflection;


var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
//Console.WriteLine($"{exe}: {path}");

var converter = new KanimPackHelper("texconv.exe", @"E:\ONIModding\ModsSource\ModsSolution\RonivansLegacy_ChemicalProcessing\ModAssets\anim\");
converter.Execute();

