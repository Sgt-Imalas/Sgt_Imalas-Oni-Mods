using Imalas_TwitchChaosEvents.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Attachments
{
	class Creature_Flipper : KMonoBehaviour, ISim200ms
	{
		[MyCmpReq]
		KBatchedAnimController animController;

		bool flippingActive = false;

		public void Sim200ms(float dt)
		{
			if (Grid.Element[Grid.PosToCell(this)].id == ModElements.InverseWater.SimHash)
			{
				if (!animController.TintColour.Equals(UIUtils.rgb(255, 173, 176)) || !flippingActive)
				{
					flippingActive = true;
					animController.flipY = true;
					animController.TintColour = UIUtils.rgb(255, 173, 176);
				}
			}
			else
			{
				if (!animController.TintColour.Equals(Color.white) || flippingActive)
				{
					flippingActive = false;
					animController.flipY = false;
					animController.TintColour = Color.white;
				}
			}
		}
	}
}

