using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static MotdBox_ImageButtonLayoutElement;

namespace DarkTheme
{
	internal static class ModAssets
	{
		static readonly Color darkBg = UIUtils.rgb(49, 51, 56);
		//static readonly Color darkBg = Color.darkRed;
		static readonly HashSet<string> _bgNames = ["BG", "BACKGROUND"];
		static readonly HashSet<string> _skipNames = ["CHECKBOX", "COLORCHIP", "PIN"];
		static readonly Dictionary<TextStyleSetting, TextStyleSetting> _invertedStyles = [];
		static readonly Dictionary<ColorStyleSetting, ColorStyleSetting> _invertedColors = [];
		static readonly HashSet<TextStyleSetting> originallyInvertedTexts = [];
		static readonly HashSet<ColorStyleSetting> originallyInvertedColors = [];
		static readonly HashSet<MultiToggle> flippedToggles = [];
		static readonly HashSet<Image> flippedImages = [];

		public static bool IsWhite(this ColorStyleSetting colorStyle)
		{
			if (colorStyle == null)
				return false;

			return colorStyle.activeColor.IsWhite()
				|| colorStyle.inactiveColor.IsWhite()
				|| colorStyle.disabledColor.IsWhite()
				|| colorStyle.disabledActiveColor.IsWhite()
				|| colorStyle.hoverColor.IsWhite()
				|| colorStyle.disabledhoverColor.IsWhite();
		}
		public static bool IsWhite(this Color color)
		{
			if (color == Color.white) return true;
			return (color.r > 0.8f && color.g > 0.8f && color.b > 0.8f);
		}
		public static Color Invert(this Color color)
		{
			var newCol = new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);
			if (newCol.IsWhite())
				newCol = darkBg;
			return newCol;
		}
		public static Color InvertIfGreyscale(this Color color)
		{
			if (color.g != color.b || color.b != color.r || color.r != color.g)
				return color;
			return color.Invert();
		}

		public static bool DarkenBackgrounds(this Transform target, bool whiteBackgroundFound, int level)
		{
			SgtLogger.l("DarkeningProcess: " + new string('-', level) + target.name);
			int childrenCount = target.childCount;
			if (childrenCount <= 0)
				return whiteBackgroundFound;

			for (int i = 0; i < childrenCount; i++)
			{
				Transform child = target.GetChild(i);
				string childName = child.name.ToUpperInvariant();

				if (_skipNames.Contains(childName))
					continue;

				if (child.TryGetComponent<Image>(out var image) && !flippedImages.Contains(image))
				{
					if (_bgNames.Contains(childName))
					{
						if (image.color.IsWhite())
						{
							whiteBackgroundFound = true;
							image.color = darkBg;
						}
						if (image is KImage kImage && kImage.colorStyleSetting != null && kImage.colorStyleSetting.IsWhite())
						{
							kImage.colorStyleSetting = GetInvertedColorStyle(kImage.colorStyleSetting);
						}
					}
					else if (image.color == Color.black)
					{
						image.color = Color.white;
					}
					flippedImages.Add(image);
				}
				if (whiteBackgroundFound && child.TryGetComponent<AlternateSiblingColor>(out var siblingColor) && (siblingColor.evenColor.IsWhite() || siblingColor.oddColor.IsWhite()))
				{
					siblingColor.evenColor = siblingColor.evenColor.Invert();
					siblingColor.oddColor = siblingColor.oddColor.Invert();
					whiteBackgroundFound = true;
				}
				if (whiteBackgroundFound && child.TryGetComponent<MultiToggle>(out var toggle) && toggle.states.Any())
				{
					if (!flippedToggles.Contains(toggle))
					{
						for (int t = 0; t < toggle.states.Length; ++t)
						{
							toggle.states[t].color = toggle.states[t].color.InvertIfGreyscale();
							toggle.states[t].color_on_hover = toggle.states[t].color_on_hover.InvertIfGreyscale();
						}
						toggle.stateDirty = true;
						flippedToggles.Add(toggle);
					}
				}
				if (whiteBackgroundFound && child.TryGetComponent<SetTextStyleSetting>(out var setter) && setter.style != null && setter.style.textColor == Color.black)
				{
					setter.SetStyle(GetInvertedTextStyle(setter.style));
				}
				if (whiteBackgroundFound && child.TryGetComponent<LocText>(out var locText))
				{
					if (locText.textStyleSetting != null && locText.textStyleSetting.textColor == Color.black)
						locText.textStyleSetting = GetInvertedTextStyle(locText.textStyleSetting);
					if (locText.color == Color.black)
						locText.color = Color.white;
				}
				if (DarkenBackgrounds(child, whiteBackgroundFound, level + 1))
					whiteBackgroundFound = true;
			}
			return whiteBackgroundFound;
		}

		private static TextStyleSetting GetInvertedTextStyle(TextStyleSetting textStyleSetting)
		{
			if (originallyInvertedTexts.Contains(textStyleSetting))
				return textStyleSetting;

			if (!_invertedStyles.TryGetValue(textStyleSetting, out var inverted))
			{
				inverted = ScriptableObject.CreateInstance<TextStyleSetting>();
				inverted.sdfFont = textStyleSetting.sdfFont;
				inverted.fontSize = textStyleSetting.fontSize;
				inverted.textColor = Color.white;
				inverted.style = textStyleSetting.style;
				inverted.enableWordWrapping = textStyleSetting.enableWordWrapping;

				_invertedStyles[textStyleSetting] = inverted;
			}
			return inverted;
		}
		private static ColorStyleSetting GetInvertedColorStyle(ColorStyleSetting colorStyleSetting)
		{
			if (originallyInvertedColors.Contains(colorStyleSetting))
				return colorStyleSetting;

			if (!_invertedColors.TryGetValue(colorStyleSetting, out var inverted))
			{

				inverted = ScriptableObject.CreateInstance<ColorStyleSetting>();
				inverted.activeColor = colorStyleSetting.activeColor.Invert();
				inverted.inactiveColor = colorStyleSetting.inactiveColor.Invert();
				inverted.disabledColor = colorStyleSetting.disabledColor.Invert();
				inverted.disabledActiveColor = colorStyleSetting.disabledActiveColor.Invert();
				inverted.hoverColor = colorStyleSetting.hoverColor.Invert();
				inverted.disabledhoverColor = colorStyleSetting.disabledhoverColor.Invert();

				_invertedColors[colorStyleSetting] = inverted;
			}
			return inverted;
		}


		public static bool DarkenBackgrounds(this GameObject gameObject) => DarkenBackgrounds(gameObject.transform, false, 0);
		public static bool DarkenBackgrounds(this MonoBehaviour monoBehaviour) => DarkenBackgrounds(monoBehaviour.transform, false, 0);

		internal static void DarkenScreenPrefabs()
		{
			ScreenPrefabs.Instance.TagFilterScreen.DarkenBackgrounds();
			ScreenPrefabs.Instance.ColonyDiagnosticScreen.DarkenBackgrounds();
		}
	}
}
