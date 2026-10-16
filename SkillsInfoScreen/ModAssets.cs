using Klei.AI;
using Newtonsoft.Json;
using SkillsInfoScreen.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtilLibs;

namespace SkillsInfoScreen
{
	internal class ModAssets
	{
		public static GameObject ScreenGO;

		public static Settings Config = new();
		static string ConfigPath;
		internal static void LoadConfig()
		{
			ConfigPath = Path.Combine(IO_Utils.ConfigsFolder, "AttributeInfoScreen", "Config.json");

			Directory.CreateDirectory(Directory.GetParent(ConfigPath).FullName);

			if (File.Exists(ConfigPath) && IO_Utils.ReadFromFile<Settings>(ConfigPath, out var setting))
				Config = setting;
			else
				Config = new Settings();
		}

		public class Settings
		{
			public bool IncludeTemporaryEffects = true;
			public bool SortPerWorld = true;
			public bool TintValue = true;
			public float PosX = 0, PosY = -45f, Width = 1400, Height = 720;

			[JsonIgnore]
			public Vector2 SizeDelta => new(Width, Height);
			[JsonIgnore]
			public Vector3 LocalPosition => new(PosX, PosY);

			internal void OnResized(RectTransform rectTransform)
			{
				Width = rectTransform.sizeDelta.x;
				Height = rectTransform.sizeDelta.y;
				Write();
			}
			internal void OnMoved(Transform transform)
			{
				PosX = transform.localPosition.x;
				PosY = transform.localPosition.y;
				Write();
			}

			internal void Write()
			{
				IO_Utils.WriteToFile(this, ConfigPath);
			}
		}
		public static string GetAttributeTooltip(IAssignableIdentity identity, Klei.AI.Attribute attribute)
		{
			string tooltip = string.Empty;
			if (identity is MinionIdentity minion)
			{
				if (minion.TryGetComponent<Modifiers>(out var modifiers))
				{
					var instance = modifiers.attributes.Get(attribute.Id);

					tooltip = instance.GetAttributeValueTooltip();
				}
			}
			else if (identity is StoredMinionIdentity storedMinion)
			{
				tooltip = storedMinion.minionModifiers.attributes.Get(attribute.Id).GetAttributeValueTooltip();
			}
			return tooltip;
		}

		public static Dictionary<string, float> MaxSkillLevels = [];
		public static int GetAttributeLevel(IAssignableIdentity identity, Klei.AI.Attribute attribute)
		{
			if(identity.IsNullOrDestroyed()) 
				return 0;
			int level = -1;
			if (identity is MinionIdentity minion && !minion.IsNullOrDestroyed() && !minion.gameObject.IsNullOrDestroyed() && minion.TryGetComponent<Modifiers>(out var modifiers))
			{
				AttributeInstance instance = modifiers.attributes.Get(attribute.Id);
				level = (int)GetTotalDisplayValue(instance);

			}
			else if (identity is StoredMinionIdentity storedMinion && !storedMinion.IsNullOrDestroyed() && !storedMinion.gameObject.IsNullOrDestroyed())
			{
				AttributeInstance instance = storedMinion.minionModifiers.attributes.Get(attribute.Id);
				level = (int)GetTotalDisplayValue(instance);

			}

			if (level > MaxSkillLevels[attribute.Id])
				MaxSkillLevels[attribute.Id] = level;
			return level;
		}
		public static float GetTotalDisplayValue(AttributeInstance instance)
		{
			bool includeNonTraitEffects = Config.IncludeTemporaryEffects;
			float value = instance.GetBaseValue();
			float multiplier = 0f;
			for (int i = 0; i != instance.Modifiers.Count; i++)
			{
				AttributeModifier attributeModifier = instance.Modifiers[i];
				//SgtLogger.l(attributeModifier.Description + " " + attributeModifier.Value);
				if (!includeNonTraitEffects && !IsTraitModifier(attributeModifier))
				{
					//SgtLogger.l(attributeModifier.GetDescription() + " is not a trait modifier?");
					continue;
				}

				if (!attributeModifier.IsMultiplier)
				{
					value += attributeModifier.Value;
				}
				else
				{
					multiplier += attributeModifier.Value;
				}
			}

			if (multiplier != 0f)
			{
				value += Mathf.Abs(value) * multiplier;
			}

			return value;
		}

		static Dictionary<AttributeModifier, bool> IsTraitModifierCache = [];
		static bool IsTraitModifier(AttributeModifier modifier)
		{
			if (IsTraitModifierCache.TryGetValue(modifier, out var result))
				return result;


			string description = modifier.GetDescription();
			if (description == Strings.Get("STRINGS.DUPLICANTS.MODIFIERS.SKILLLEVEL.NAME"))
				return true;

			foreach (var trait in Db.Get().traits.resources)
			{
				if (trait.GetName().Contains(description))
				{
					IsTraitModifierCache[modifier] = true;
					return true;
				}
			}
			IsTraitModifierCache[modifier] = false;
			return false;
		}

		public static void LoadAssets()
		{
			var bundle = AssetUtils.LoadAssetBundle("attributescreen_assets", platformSpecific: true);
			ScreenGO = bundle.LoadAsset<GameObject>("Assets/UIs/AttributeInfoScreen.prefab");
			//UIUtils.ListAllChildren(Assets.transform);
			ScreenGO.AddOrGet<UnityAttributeInfoScreen>();

			var TMPConverter = new TMPConverter();
			TMPConverter.ReplaceAllText(ScreenGO);
		}
		public static Color Bad, Medium, Good;
		public static bool RainbowGradient = false;
		static bool colorsInitialized = false;
		public static void LoadColors()
		{
			if (colorsInitialized)
				return;
			colorsInitialized = true;

			int colorBlindnessMode = KPlayerPrefs.GetInt(GraphicsOptionsScreen.ColorModeKey);
			RainbowGradient = colorBlindnessMode == 0;

			Good = (Color)GlobalAssets.Instance.colorSet.cropGrown;
			Good.a = 1; //a is 0 for these by default, but that doesnt allow tinting the symbols here
			Good = UIUtils.Darken(Good, 10);

			Medium = (Color)GlobalAssets.Instance.colorSet.cropGrowing;
			Medium.a = 1;
			Medium = UIUtils.Darken(Medium, 10);

			Bad = (Color)GlobalAssets.Instance.colorSet.cropHalted;
			Bad.a = 1;
			Bad = UIUtils.Darken(Bad, 10);
		}
		public static string ColorAttributeText(int value, string attributeId) => UIUtils.EmboldenText(UIUtils.ColorText(value.ToString(), GetIntensityColor(value, attributeId)));
		public static Color GetIntensityColor(float level, string attributeId)
		{
			LoadColors();

			if (RainbowGradient)
			{
				level = Mathf.Clamp(level, 0, 45);

				//return ColorGradient.Evaluate(level / 45f);

				float hsvShift = (level * 6f) / 360f;
				return UIUtils.Darken(UIUtils.HSVShift(Color.red, hsvShift * 100), 25);
			}
			float maxLevel = MaxSkillLevels[attributeId];
			float midPoint = maxLevel / 2;

			///This is a way less resource intensive lerp than color.lerp
			float levelMinusMidPoint = level - midPoint;

			float r_bad = Bad.r * Bad.r;
			float g_bad = Bad.g * Bad.g;
			float b_bad = Bad.b * Bad.b;

			float r_mid = Medium.r * Medium.r;
			float g_mid = Medium.g * Medium.g;
			float b_mid = Medium.b * Medium.b;

			float r_gud = Good.r * Good.r;
			float g_gud = Good.g * Good.g;
			float b_gud = Good.b * Good.b;

			float r_final, g_final, b_final;

			if (level >= midPoint)
			{
				float lerp = (levelMinusMidPoint / midPoint);
				float lerpInv = 1 - lerp;

				r_final = Mathf.Sqrt(r_mid * lerpInv + r_gud * lerp);
				g_final = Mathf.Sqrt(g_mid * lerpInv + g_gud * lerp);
				b_final = Mathf.Sqrt(b_mid * lerpInv + b_gud * lerp);
			}
			else
			{
				float lerp = (level / midPoint);
				float lerpInv = 1 - lerp;

				r_final = Mathf.Sqrt(r_bad * lerpInv + r_mid * lerp);
				g_final = Mathf.Sqrt(g_bad * lerpInv + g_mid * lerp);
				b_final = Mathf.Sqrt(b_bad * lerpInv + b_mid * lerp);
			}
			return new Color(r_final, g_final, b_final);
		}
	}
}
