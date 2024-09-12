using KSerialization;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.Behaviors
{
	internal class EditableFrame : KMonoBehaviour
	{
		[MyCmpGet] Painting painting;
		[MyCmpGet] KBatchedAnimController kbac;
		[Serialize][SerializeField] public int FrameSymbolIndex = 0;

		public override void OnSpawn()
		{
			base.OnSpawn();
			//kbac.symbolOverrideController.AddSymbolOverride();
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
		}
		private static Dictionary<int, KAnim.Build.SymbolFrameInstance> _symbolOverrides = null;

		public static Dictionary<int, KAnim.Build.SymbolFrameInstance> SymbolOverrides => FrameSymbolOverrides();
		static Dictionary<int, KAnim.Build.SymbolFrameInstance> FrameSymbolOverrides()
		{
			if (_symbolOverrides != null)
				return _symbolOverrides;

			_symbolOverrides = new();

			var anim = Assets.GetAnim("painting_frames_kanim");
			if (anim == null)
			{
				SgtLogger.error("frame override kanim was null");
				return _symbolOverrides;
			}
			var kanimData = anim.GetData();
			var build = kanimData.build;
			var frameSymbol = build.GetSymbol("frame");

			if (frameSymbol == null)
			{
				SgtLogger.error("frame override symbol was null");
				return _symbolOverrides;
			}
			for (int i = 0; i < frameSymbol.numFrames; i++)
			{
				_symbolOverrides[i] = frameSymbol.GetFrame(i);
			}

			return _symbolOverrides;
		}

	}
}
