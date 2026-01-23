using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSmallSculptures.Content.Scripts
{
	internal class MarbleSculptureAnimScaler : SculptureAnimScaler
	{
		public MarbleSculptureAnimScaler()
		{
			TargetAnimScaleMultiplier = 1.5f;
		}

		public override HashSet<string> GetScaleableAnims() => ModAssets.MarbleSculptureScaleableSkins;
	}
}
