using FMOD;
using Klei.AI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace SkillsInfoScreen.UI.UIComponents
{
	internal class AttributeMinionEntry : KMonoBehaviour
	{
		Attribute Attribute;
		Image XP_Progressbar;
		Image XP_ProgressBarBG;
		LocText XP_Progress, XPLevelInfo, TotalLevelInfo;
		AttributeLevels Levels;
		IAssignableIdentity Identity;
		ToolTip TT;
		StoredMinionIdentity Stored;


		public void Init(IAssignableIdentity assignableIdentity, Attribute attribute)
		{
			Identity = assignableIdentity;
			Attribute = attribute;
			if (assignableIdentity is MinionIdentity mi)
			{
				Levels = mi.GetComponent<AttributeLevels>();
			}
			else if (assignableIdentity is StoredMinionIdentity stored)
				Stored = stored;

			XP_Progressbar = transform.Find("XPBar/fill").gameObject.GetComponent<Image>();
			XP_ProgressBarBG = XP_Progressbar.transform.parent.GetComponent<Image>();
			XP_Progress = transform.Find("XPBar/amountText").gameObject.GetComponent<LocText>();
			XPLevelInfo = transform.Find("XPBar/levelText").gameObject.GetComponent<LocText>();
			TotalLevelInfo = transform.Find("SkillPoints").gameObject.GetComponent<LocText>();
			TT = UIUtils.AddSimpleTooltipToObject(TotalLevelInfo.gameObject, string.Empty);
			Refresh();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			Refresh();
		}
		public void Refresh()
		{
			string levelVal = "0", currentLvlXp = "0", maxLvlXp = string.Empty;
			float levelPercentage = 0;
			bool maxLevelReached = false;

			if (Levels != null)
			{
				levelVal = Levels.GetLevel(Attribute).ToString();
				//int maxLevelVal = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL;
				var level = Levels.GetAttributeLevel(Attribute.Id);
				if (level.maxGainedLevel <= 0)
				{
					currentLvlXp = " - ";
					maxLvlXp += string.Empty;
					levelPercentage = 0;
				}
				else
				{
					currentLvlXp = Mathf.RoundToInt(level.experience).ToString();
					maxLvlXp = Mathf.RoundToInt(level.GetExperienceForNextLevel()).ToString();
					levelPercentage = level.GetPercentComplete();
					if(level.level >= level.maxGainedLevel)
						maxLevelReached = true;
				}
			}
			else if (Stored != null)
			{
				var storedLevel = Stored.attributeLevels.FirstOrDefault(lvl => lvl.attributeId == Attribute.Id);

				levelVal = storedLevel.level.ToString();
				currentLvlXp = Mathf.RoundToInt(storedLevel.experience).ToString();
				maxLvlXp = string.Empty;
				levelPercentage = 0;
			}
			XP_Progressbar.fillAmount = levelPercentage;
			XP_Progress.SetText(STRINGS.XP_VERY_SHORT + (maxLvlXp != string.Empty ? $"{currentLvlXp}/{maxLvlXp}" : currentLvlXp));
			XPLevelInfo.SetText(STRINGS.LEVEL_VERY_SHORT + levelVal);

			if (maxLevelReached)
				XP_ProgressBarBG.color = UIUtils.Lighten(Color.darkGreen,50);

			TT.SetSimpleTooltip(ModAssets.GetAttributeTooltip(Identity, Attribute));
			int totalLevel = (int)ModAssets.GetAttributeLevel(Identity, Attribute);
			TotalLevelInfo.SetText(ModAssets.Config.TintValue ? ModAssets.ColorAttributeText(totalLevel, Attribute.Id) : UIUtils.EmboldenText(totalLevel.ToString()));
		}
	}
}
