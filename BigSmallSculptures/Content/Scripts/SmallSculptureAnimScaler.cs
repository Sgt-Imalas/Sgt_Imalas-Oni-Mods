using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSmallSculptures.Content.Scripts
{
	internal class SmallSculptureAnimScaler : SculptureAnimScaler
	{
		public SmallSculptureAnimScaler()
		{
			TargetAnimScaleMultiplier = 0.5f;
		}

		public override HashSet<string> GetScaleableAnims() => ModAssets.SmallSculptureScaledSkins;
	}
}
