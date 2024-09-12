using TMPro;
using UnityEngine;

namespace UtilLibs
{
	internal class TMPImportFix : KMonoBehaviour
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
