using ImageMagick.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _KAnimPackerExe.ModConverters
{
	internal class RonivanAIO : IModConverter
	{
		public static void PackModDirectories()
		{
			//KAnims:
			new KanimPackHelper(@"E:\ONIModding\ModsSource\ModsSolution\RonivansLegacy_ChemicalProcessing\ModAssets\anim\").ConvertToKanims();
			//Textures:
			new KanimPackHelper(@"E:\ONIModding\ModsSource\ModsSolution\RonivansLegacy_ChemicalProcessing\ModAssets\assets\textures").ConvertSingularTextures();
		}
	}
}
