using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.Research
{
	class DeepSpaceAnalyzerWorkable : ComplexFabricatorWorkable
	{
		[MyCmpReq] DeepSpaceAnalyzer deepSpaceAnalyzer;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			SetOffsets([new CellOffset(0, 0)]);
			attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
			attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
			skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
			skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
			requiredSkillPerk = Db.Get().SkillPerks.CanUseClusterTelescope.Id;
		}
		public override float GetEfficiencyMultiplier(WorkerBase worker)
		{
			float baseMult = base.GetEfficiencyMultiplier(worker);
			return baseMult * deepSpaceAnalyzer.GetWorkMultiplier();
		}
	}
}
