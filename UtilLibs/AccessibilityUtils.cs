using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs
{
	public static class AccessibilityUtils
	{
		private static Color _logicBad, _logicWarning, _logicGood;

		public static Color LogicBad
		{
			get
			{
				if (_logicBad == default)
				{
					InitializeColors();
				}
				return _logicBad;
			}
		}
		public static Color LogicWarning
		{
			get
			{
				if (_logicWarning == default)
				{
					InitializeColors();
				}
				return _logicWarning;
			}
		}
		public static Color LogicGood
		{
			get
			{
				if (_logicGood == default)
				{
					InitializeColors();
				}
				return _logicGood;
			}
		}
		static void InitializeColors()
		{

			//colorblindness color support
			var colourGood = (Color)GlobalAssets.Instance.colorSet.logicOn;
			colourGood.a = 1; //a is 0 for these by default, but that doesnt allow tinting the symbols here
			_logicGood = colourGood;

			var colourBad = (Color)GlobalAssets.Instance.colorSet.logicOff;
			colourBad.a = 1;
			_logicBad = colourBad;

			//no direct yellow in logic, using crop color yellow
			var colourWarning = (Color)GlobalAssets.Instance.colorSet.cropGrowing;
			colourWarning.a = 1;
			_logicWarning = colourWarning;
		}
	}
}
