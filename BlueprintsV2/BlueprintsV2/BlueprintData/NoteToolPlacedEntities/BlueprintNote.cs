using BlueprintsV2.BlueprintData;
using KSerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static BlueprintsV2.STRINGS.BLUEPRINTS_BLUEPRINTNOTE;
using static STRINGS.MISC.STATUSITEMS;
using static STRINGS.UI.CLUSTERMAP.ASTEROIDS;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities
{
	public class BlueprintNote : KMonoBehaviour
	{
		public static string FILTERLAYER = ("BLUEPRINTV2_FILTER_NOTES");
		[Serialize]
		public bool SeatIndicator = false;
		[MyCmpReq] protected InfoDescription description;
		[MyCmpReq] protected KSelectable selectable;
		[MyCmpReq] protected Filterable filterable;
		protected MeshRenderer renderer;
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			renderer = GetComponentInChildren<MeshRenderer>();
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (SeatIndicator)
				Seat();

			Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);
			Subscribe((int)GameHashes.Cancel, Cancel);
		}
		private void OnRefreshUserMenu(object data)
		{
			Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_cancel", DELETE_NOTE.NAME, new System.Action(this.OnCancel), tooltipText: DELETE_NOTE.TOOLTIP));
		}
		protected void Cancel(object _ = null) => OnCancel();
		protected void OnCancel()
		{
			DetailsScreen.Instance.Show(false);
			this.DeleteObject();
		}
		void Seat()
		{
			SetDescription();
			Grid.Objects[Grid.PosToCell(this), (int)ModAssets.BlueprintNotesLayer] = this.gameObject;
			//gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		}
		public virtual void SetDescription()
		{

		}
		protected void RefreshSelection()
		{
			if (selectable.IsSelected)
			{
				DetailsScreen.Instance.target = null;
				DetailsScreen.Instance.Refresh(gameObject);///should refresh screen, make sure to not have infinite loop by having selection changed event trigger this again with no changes
			}
		}
		public virtual BlueprintNoteData GetNoteData()
		{
			return new BlueprintNoteData();
		}
	}
}
