using AkisDecorPackB.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisDecorPackB.Content.Scripts
{
	internal class FloorLampUnderConstruction : KMonoBehaviour
	{
		[MyCmpReq] Constructable constructable;
		[MyCmpReq] KBatchedAnimController kbac;
		string paneId;
		public override void OnSpawn()
		{
			base.OnSpawn();
			UpdatePane();
		}

		private void UpdatePane()
		{
			if (constructable.SelectedElementsTags == null || constructable.selectedElementsTags.Count() < 2)
				return;
			UpdateAnim();
		}
		private void UpdateAnim()
		{
			kbac.enabled = true;

			var currentAnim = "place";
			if (paneId.IsNullOrWhiteSpace())
			{
				string element = constructable.SelectedElementsTags.Last().ToString();
				paneId = FloorLampPane.GetIdFromElement(element);
			}

			var pane = Mod_Db.FloorLampPanes.TryGet(paneId);
			if (pane != null)
			{
				kbac.SwapAnims([Assets.GetAnim(pane.animFile)]);
				kbac.Offset = new Vector2(0f, 0.5f);
				kbac.Play(currentAnim);
			}
		}
	}
}
