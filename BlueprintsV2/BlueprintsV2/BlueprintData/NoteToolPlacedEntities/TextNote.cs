using BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
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
		public static Dictionary<string, Sprite> SymbolMap = [];

		[Serialize] Color SymbolTint = UIUtils.rgb(23, 120, 189);
		[Serialize] string Title = "Note Title";
		[Serialize] string Text = "";
		[Serialize] string Symbol = string.Empty;

		public override BlueprintNoteData GetNoteData(Vector2I? newPosition = null)
		{
			return new BlueprintNoteData()
			{
				Type = BlueprintNoteData.NoteType.Text,
				Title = this.Title,
				Text = this.Text,
				Symbol = this.Symbol,
				SymbolTint = this.SymbolTint,
				Location = newPosition ?? default
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
			var name = Title;
			selectable?.SetName(name);
			this.gameObject.name = name;
			description?.description = Text;
			if (!Symbol.IsNullOrWhiteSpace() && SymbolMap.TryGetValue(Symbol, out var sprite))
			{
				renderer?.sprite = sprite;
			}
			this.Tint = SymbolTint;
			RefreshTint();

			base.SetDescription();
		}

		internal void SetInfo(string title, string text, string symbol, Color tint, bool shouldSeat = false)
		{
			Title = title;
			Text = text;
			Symbol = symbol;
			SymbolTint = tint;
			SeatIndicator = shouldSeat;
			SetDescription();
		}
		internal void UpdateInfo(string title = null, string text = null, string symbol = null, Color? tint = null)
		{
			if (title != null) Title = title;
			if (text != null) Text = text;
			if (tint != null && tint.HasValue) SymbolTint = tint.Value;
			if(symbol != null)Symbol = symbol;
			SetDescription();
		}

		public static BlueprintNote Create(int cell, string title, string text, string symbol, Color color, bool seat = false)
		{
			var infoIndicator = Util.KInstantiate(Assets.GetPrefab(TextNoteConfig.ID));
			Grid.Objects[cell, (int)ModAssets.BlueprintNotesLayer] = infoIndicator;
			Vector3 posCbc = Grid.CellToPosCBC(cell, MopTool.Instance.visualizerLayer);
			posCbc.z -= 0.15f;
			infoIndicator.transform.SetPosition(posCbc);
			if (infoIndicator.TryGetComponent<TextNote>(out var info))
			{
				info.SetInfo(title, text, symbol, color, seat);
			}
			infoIndicator.SetActive(true);
			return info;
		}
	}
}
