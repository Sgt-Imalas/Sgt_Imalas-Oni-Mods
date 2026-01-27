using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BigSmallSculptures.Content.Scripts
{
	internal class SculptureAnimScaler : KMonoBehaviour
	{
		[MyCmpReq] internal Sculpture sculpture;
		[MyCmpReq] internal KBatchedAnimController kbac;

		float defaultAnimScale = 0.005f;
		int handle = -1;

		static bool dumpd = false;
		public override void OnSpawn()
		{
			if(!dumpd)
			{
				ModAssets.BogdanovIt();
				dumpd = true;
			}


			base.OnSpawn();
			ScaleKbac();
			handle = Subscribe((int)GameHashes.ArtableStateChanged, OnArtableChanged);
		}
		public override void OnCleanUp()
		{
			Unsubscribe(handle);
			base.OnCleanUp();
		}
		void OnArtableChanged(object data)
		{
			ScaleKbac();
		}

		void ScaleKbac()
		{
			var stage = sculpture.CurrentStage;
			if(ModAssets.TryGetCachedSkinScaleModifier(stage, out float TargetAnimScaleMultiplier))
			{
				SgtLogger.l($"Scaling sculpture anims for {gameObject.name} stage {stage} by {TargetAnimScaleMultiplier}x");
				kbac.animScale = defaultAnimScale * TargetAnimScaleMultiplier;
				kbac.SetDirty();
			}
			else
			{
				kbac.animScale = defaultAnimScale;
				kbac.SetDirty();
			}
		}

	}
}
