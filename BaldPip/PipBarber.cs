using System;
using System.Collections.Generic;
using System.Text;

namespace BaldPip
{
	internal class PipBarber : KMonoBehaviour
	{
		[MyCmpReq] KBatchedAnimController kbac;
		public override void OnSpawn()
		{
			base.OnSpawn();
			Shave();
		}
		void Shave()
		{
			kbac.SetSymbolVisiblity("sq_leaf", false);
		}
	}
}
