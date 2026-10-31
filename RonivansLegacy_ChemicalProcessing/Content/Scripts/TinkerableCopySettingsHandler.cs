using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class TinkerableCopySettingsHandler : KMonoBehaviour
	{
		/// <summary>
		/// The SpaceTreeBranch / Bonbon tree branch crashes when any myCmpadd/get call is added to any component on it...
		/// </summary>
		//[MyCmpAdd] 
		CopyBuildingSettings copyBuildingSettings;
		//[MyCmpReq] 
		Tinkerable TinkerTarget;

		public override void OnSpawn()
		{
			copyBuildingSettings = gameObject.AddOrGet<CopyBuildingSettings>();
			TryGetComponent(out TinkerTarget);
			this.Sub((int)GameHashes.CopySettings, OnCopySettings);
			base.OnSpawn();
		}
		public override void OnCleanUp()
		{
			this.UnsubAll();
			base.OnCleanUp();
		}
		private void OnCopySettings(object data)
		{
			if (data is not GameObject other || !other.TryGetComponent<TinkerableCopySettingsHandler>(out var source))
				return;
			//SgtLogger.l("Tinkerable Copy Settings");
			bool tinkerable = source.GetTinkerableAllowed();
			SetTinkerableAllowed(tinkerable);
		}
		public void SetTinkerableAllowed(bool tinkerable)
		{
			if (TinkerTarget.userMenuAllowed == tinkerable)
				return;
			TinkerTarget.userMenuAllowed = tinkerable;
			TinkerTarget.UpdateChore();
		}
		public bool GetTinkerableAllowed()
		{
			return TinkerTarget.userMenuAllowed;
		}
	}
}
