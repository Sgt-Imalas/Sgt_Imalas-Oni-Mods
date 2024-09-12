using TMPro;
using UnityEngine;

namespace UtilLibs.UIcmp //Source: Aki
{
	// There is an issue with how TMP imports itself and alighnment has to be reapplied
	class TMPFixer : KMonoBehaviour
	{
		[SerializeField]
		public TextAlignmentOptions alignment;

		[MyCmpReq]
		private LocText text;

		public override void OnSpawn()
		{
			base.OnSpawn();
			text.alignment = alignment;
			Destroy(this);
		}
	}
}
