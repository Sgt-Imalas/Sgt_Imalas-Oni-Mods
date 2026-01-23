using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BigSmallSculptures.Content.Scripts
{
	internal abstract class SculptureAnimScaler : KMonoBehaviour
	{
		[MyCmpReq] internal Sculpture sculpture;
		[MyCmpReq] internal KBatchedAnimController kbac;
		[SerializeField] public float TargetAnimScaleMultiplier = 1f;

		float defaultAnimScale = 0.005f;
		int handle = -1;
		public override void OnSpawn()
		{
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
			if(GetScaleableAnims().Contains(stage))
			{
				SgtLogger.l($"Scaling sculpture anims for {gameObject.name} stage {stage} by {TargetAnimScaleMultiplier}x");
				kbac.animScale *= TargetAnimScaleMultiplier;
				kbac.SetDirty();
			}
			else
			{
				SgtLogger.l($"Resetting sculpture anim scale for {gameObject.name} stage {stage} to default");
				kbac.animScale = defaultAnimScale;
				kbac.SetDirty();
			}
		}

		public abstract HashSet<string> GetScaleableAnims();
	}
}
