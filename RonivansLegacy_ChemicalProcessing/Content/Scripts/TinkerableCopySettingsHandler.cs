using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Components;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class TinkerableCopySettingsHandler : KMonoBehaviour
	{
		[MyCmpAdd] CopyBuildingSettings copyBuildingSettings;
		[MyCmpReq] Tinkerable TinkerTarget;

		private static readonly EventSystem.IntraObjectHandler<TinkerableCopySettingsHandler> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<TinkerableCopySettingsHandler>((component, data) => component.OnCopySettings(data));
		public override void OnSpawn()
		{
			this.Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
			base.OnSpawn();
		}
		public override void OnCleanUp()
		{
			this.Unsubscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
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
