using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	/// <summary>
	/// Tints buildings bright red to indicate deprecation
	/// </summary>
	class DeprecationTint : KMonoBehaviour
	{
		static Tag IgnoreMaterialColor = new Tag("NoPaint");

		[MyCmpReq] KBatchedAnimController kbac;
		[SerializeField] public Color Tint = Color.red;
		[MyCmpReq] KPrefabID kprefab;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			if (!kprefab.HasTag(IgnoreMaterialColor))
				kprefab.AddTag(IgnoreMaterialColor);
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (!kprefab.HasTag(IgnoreMaterialColor))
				kprefab.AddTag(IgnoreMaterialColor);
			kbac.TintColour = Tint;
		}
	}
}
