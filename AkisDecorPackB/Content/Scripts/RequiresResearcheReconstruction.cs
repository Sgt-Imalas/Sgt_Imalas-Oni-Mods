using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisDecorPackB.Content.Scripts
{
	internal class RequiresResearcheReconstruction : KMonoBehaviour, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			var desc = new Descriptor();
			desc.SetupDescriptor(STRINGS.UI.BUILDINGEFFECTS.DECORPACKB_REQUIRESSCIENCE, STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.DECORPACKB_REQUIRESSCIENCE, Descriptor.DescriptorType.Requirement);

			return [desc];

		}
	}
}
