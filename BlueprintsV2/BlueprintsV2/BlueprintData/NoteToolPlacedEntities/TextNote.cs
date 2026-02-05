using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities
{
	internal class TextNote : BlueprintNote
	{

		[Serialize] Color SymbolTint = UIUtils.rgb(23, 120, 189);
		[Serialize] string Title = "Note Title";
		[Serialize] string Text = "";

		public override BlueprintNoteData GetNoteData()
		{
			return new BlueprintNoteData()
			{
				Type = BlueprintNoteData.NoteType.Text,
				Title = this.Title,
				Text = this.Text,
				SymbolTint = this.SymbolTint,
			};
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			SetDescription();
			RefreshSelection();
		}

		public override void SetDescription()
		{
			base.SetDescription();
			var name = Title;
			selectable?.SetName(name);
			this.gameObject.name = name;
			description?.description = Text;
			renderer?.material?.color = SymbolTint;
		}

		internal void SetInfo(string title, string text, Color tint, bool shouldSeat = false)
		{
			Title = title;
			Text = text;
			SymbolTint = tint;
			SeatIndicator = shouldSeat;
			SetDescription();
		}
		internal void UpdateInfo(string title = null, string text = null, Color? tint = null)
		{
			if (title != null) Title = title;
			if(text != null) Text = text;
			if (tint != null && tint.HasValue) SymbolTint = tint.Value;
			SetDescription();
		}
	}
}
