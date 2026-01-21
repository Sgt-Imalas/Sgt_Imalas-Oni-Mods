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

		internal void SetInfo(string title, string text, Color tint, bool shouldSeat = false)
		{
			Title = title;
			Text = text;
			SymbolTint = tint;

			SeatIndicator = shouldSeat;

			RefreshSelection();
		}
	}
}
