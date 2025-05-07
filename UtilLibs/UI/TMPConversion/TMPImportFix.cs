using TMPro;
using UnityEngine;

namespace UtilLibs
{
	internal class TMPImportFix : KMonoBehaviour
	{
		[SerializeField]
		public TextOverflowModes textOverflow;
		[SerializeField]
		public TextAlignmentOptions alignment;

		[SerializeField]
		public float fontSizeMin;
		[SerializeField]
		public float fontSizeMax;

		[SerializeField]
		public bool autoResize;

		[MyCmpReq]
		private LocText text;

		public override void OnSpawn()
		{
			base.OnSpawn();
			text.alignment = alignment;
			text.overflowMode = textOverflow;

			text.fontSizeMax = fontSizeMax;
			text.fontSizeMin = fontSizeMin;
			text.enableAutoSizing = autoResize;
			Destroy(this);
		}
	}
}
