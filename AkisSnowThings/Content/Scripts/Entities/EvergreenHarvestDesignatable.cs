using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using UtilLibs;

namespace AkisSnowThings.Content.Scripts.Entities
{
	internal class EvergreenHarvestDesignatable : HarvestDesignatable
	{
		[Serialize]
		bool freshlyPlanted = true;

		public override void OnSpawn()
		{
			base.OnSpawn();
			if(freshlyPlanted)
			{
				SetHarvestWhenReady(false);
				freshlyPlanted = false;
			}
		}
	}
}
