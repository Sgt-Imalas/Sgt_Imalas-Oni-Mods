using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
	class VentTintable : KMonoBehaviour
	{
		[MyCmpReq] Storage ContentStorage;
		[MyCmpGet] KBatchedAnimController kbac;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Subscribe((int)GameHashes.VentAnimatingChanged, UpdateTint);
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe((int)GameHashes.VentAnimatingChanged, UpdateTint);
		}

		void UpdateTint(object data)
		{
			if (data == null || data is not Boxed<Color32> boxedTintColor)
				return;

			

			kbac.SetSymbolTint("tint", boxedTintColor.value);
		}
	}
}
