using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;
using UnityEngine.UI;

namespace SkillsInfoScreen.UI.UIComponents
{
	internal class DuplicantEntry : KMonoBehaviour
	{
		IAssignableIdentity Minion;
		MinionResume Resume;
		StoredMinionIdentity Stored;
		MinionIdentity Identity;
		Image MinionImage;
		GameObject TraitPrefab, TraitContainer;
		LocText MinionName;
		Image XP_Progressbar;
		LocText XP_Progress, SkillpointsInfo;
		GameObject AttributePrefab, SpacerPrefab;
		Dictionary<string, AttributeMinionEntry> Attributes = [];
		Dictionary<string, GameObject> Traits = [];

		public void Init(IAssignableIdentity minion)
		{
			ModAssets.LoadColors();

			Minion = minion;
			if (minion is MinionIdentity minionIdentity)
			{
				Identity = minionIdentity;
				Resume = minionIdentity.GetComponent<MinionResume>();
			}
			else if (minion is StoredMinionIdentity stored)
			{
				Stored = stored;
			}

			XP_Progressbar = transform.Find("XP/XPBar/fill").gameObject.GetComponent<Image>();
			XP_Progress = transform.Find("XP/XPBar/amountText").gameObject.GetComponent<LocText>();
			SkillpointsInfo = transform.Find("XP/SkillPoints").gameObject.GetComponent<LocText>();


			AttributePrefab = transform.Find("AttributeInfo").gameObject;
			AttributePrefab.SetActive(false);
			SpacerPrefab = transform.Find("Spacer").gameObject;
			MinionName = transform.Find("NameTraitContainer/DupeName").gameObject.GetComponent<LocText>();
			MinionName.SetText(minion.GetProperName());
			MinionImage = transform.Find("IconContainer/Icon").gameObject.GetComponent<Image>();
			if(Identity != null)
			{
				MinionImage.gameObject.AddOrGet<MinionSelectButton>().Minion = Identity;
			}
				

			HashedString personalityResourceID = HashedString.Invalid;
			if (Identity != null)
				personalityResourceID = Identity.personalityResourceId;
			else if (Stored != null)
				personalityResourceID = Stored.personalityResourceId;

			MinionImage.sprite = Db.Get().Personalities.Get(personalityResourceID).GetMiniIcon();

			TraitContainer = transform.Find("NameTraitContainer").gameObject;
			TraitPrefab = transform.Find("NameTraitContainer/TraitPrefab").gameObject;
			TraitPrefab.SetActive(false);

			InitAttributes();
			InitTraits();
		}

		internal void Refresh()
		{
			foreach (AttributeMinionEntry att in Attributes.Values)
				att.Refresh();
			RefreshTraits();
			RefreshXP();
		}
		void RefreshXP()
		{
			int totalSkillPoints = 0;
			string availableSkillPoints = string.Empty;
			string skillPointsText = string.Empty;
			float totalExperience = 0;
			if(Resume != null)
			{
				totalSkillPoints = Resume.TotalSkillPointsGained;
				availableSkillPoints = Resume.AvailableSkillpoints.ToString();
				totalExperience = Resume.TotalExperienceGained;
			}
			else if(Stored != null)
			{
				totalExperience = Stored.TotalExperienceGained;
				totalSkillPoints = MinionResume.CalculateTotalSkillPointsGained(totalExperience);
				
			}
			skillPointsText = availableSkillPoints != string.Empty ? $"{availableSkillPoints}/{totalSkillPoints}" : totalSkillPoints.ToString();
			skillPointsText += " " + global::STRINGS.UI.SKILLS_SCREEN.SORT_BY_SKILL_AVAILABLE;
			SkillpointsInfo.SetText(skillPointsText);

			float previousExperienceBar = MinionResume.CalculatePreviousExperienceBar(totalSkillPoints);
			float nextExperienceBar = MinionResume.CalculateNextExperienceBar(totalSkillPoints);
			float currentXPPercentage = (totalExperience - previousExperienceBar) / (nextExperienceBar - previousExperienceBar);
			this.XP_Progress.SetText($"{Mathf.RoundToInt(totalExperience - previousExperienceBar).ToString()} / {Mathf.RoundToInt(nextExperienceBar - previousExperienceBar).ToString()}");
			this.XP_Progressbar.fillAmount = currentXPPercentage;
		}

		void RefreshTraits()
		{
			foreach (var traitGO in Traits.Values)
				traitGO.SetActive(false);
			//too cluttered ui
			return;

			//var minionTraits = Minion.GetComponent<Traits>().TraitList;
			//minionTraits.RemoveAll(t => t.Id.Contains("BaseTrait") || DUPLICANTSTATS.JOYTRAITS.Any(j => j.id == t.Id) || DUPLICANTSTATS.STRESSTRAITS.Any(s => s.id == t.Id) || t.Id == "StressShocker");
			//bool tooManyTraits = minionTraits.Count > 4;
			//if (tooManyTraits)
			//{
			//	AddOrGetTraitContainer(TooMany, minionTraits.Count + " " + global::STRINGS.UI.CHARACTERCONTAINER_TRAITS_TITLE, null);
			//}
			//else
			//{
			//	foreach (var trait in Minion.GetComponent<Traits>().TraitList)
			//	{
			//		AddOrGetTraitContainer(trait.Id, trait.GetName(), trait.PositiveTrait);
			//	}
			//}
		}
		const string TooMany = "TooManyTraitsEntry";

		void AddOrGetTraitContainer(string traitID, string traitName, bool? goodTrait)
		{
			if (!Traits.TryGetValue(traitID, out var TraitGO))
			{
				TraitGO = Util.KInstantiateUI(TraitPrefab, TraitContainer, true);
				Traits.Add(traitID, TraitGO);
			}
			TraitGO.GetComponentInChildren<LocText>().SetText(traitName);
			TraitGO.SetActive(true);
			if (goodTrait.HasValue)
				TraitGO.GetComponent<Image>().color = goodTrait.Value ? ModAssets.Good : ModAssets.Bad;
			else
				TraitGO.GetComponent<Image>().color = Color.gray;

		}

		void InitTraits()
		{

		}

		void InitAttributes()
		{
			var attributeDb = Db.Get().Attributes;
			var stats = DUPLICANTSTATS.ALL_ATTRIBUTES.OrderBy(id => global::STRINGS.UI.StripLinkFormatting(attributeDb.TryGet(id)?.Name ?? "unknown"));

			foreach (var attributeId in stats)
			{
				if (attributeId == "SpaceNavigation" && !DlcManager.IsExpansion1Active())
					continue;

				var attribute = attributeDb.TryGet(attributeId);

				var attributeEntryGO = Util.KInstantiateUI(AttributePrefab, gameObject);
				attributeEntryGO.SetActive(true);
				var entry = attributeEntryGO.AddOrGet<AttributeMinionEntry>();
				entry.Init(Minion, attribute);
				Attributes[attributeId] = entry;

				if (attributeId != stats.Last())
					Util.KInstantiateUI(SpacerPrefab, gameObject);
			}
		}
	}
}
