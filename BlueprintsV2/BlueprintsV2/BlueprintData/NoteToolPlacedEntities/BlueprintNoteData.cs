using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities
{
	public class BlueprintNoteData
	{
		public enum NoteType
		{
			Invalid = -1,
			Text,
			Element,
		}
		[JsonIgnore]
		//ignore in all data 
		public Vector2I Location;

		public NoteType Type = NoteType.Invalid;
		///Text Note Data
		public string Text, Title, Symbol;
		public Color SymbolTint;
		///Element Note Data
		public SimHashes ElementId;
		public float ElementMass;
		public float ElementTemperature;

		public Sprite GetNoteSprite()
		{
			if (!Symbol.IsNullOrWhiteSpace() && TextNote.SymbolMap.TryGetValue(Symbol, out var sprite))
				return sprite;
			return ModAssets.Note_Placer_Sprite;
		}

		public bool IsValid() => Type != NoteType.Invalid;

		public void WriteDataJson(JsonTextWriter jsonWriter)
		{
			jsonWriter.WriteStartObject();
			jsonWriter.WritePropertyName("x"); jsonWriter.WriteValue(Location.x);
			jsonWriter.WritePropertyName("y"); jsonWriter.WriteValue(Location.y);
			jsonWriter.WritePropertyName("type"); jsonWriter.WriteValue((int)Type);
			if (Type == NoteType.Text)
			{
				jsonWriter.WritePropertyName("title"); jsonWriter.WriteValue(Title);
				jsonWriter.WritePropertyName("text"); jsonWriter.WriteValue(Text);
				jsonWriter.WritePropertyName("tinthex"); jsonWriter.WriteValue(SymbolTint.ToHexString());
				jsonWriter.WritePropertyName("symbol"); jsonWriter.WriteValue(Symbol);
			}
			else if (Type == NoteType.Element)
			{
				jsonWriter.WritePropertyName("id"); jsonWriter.WriteValue((int)ElementId);
				jsonWriter.WritePropertyName("mass"); jsonWriter.WriteValue(ElementMass);
				jsonWriter.WritePropertyName("temp"); jsonWriter.WriteValue(ElementTemperature);
			}
			jsonWriter.WriteEndObject();
		}
		public bool ReadDataJson(JToken jsonObject)
		{
			JToken xToken = jsonObject.SelectToken("x");
			JToken yToken = jsonObject.SelectToken("y");
			Location = Vector2I.minusone;

			if (xToken != null && xToken.Type == JTokenType.Integer || yToken != null && yToken.Type == JTokenType.Integer)
			{
				Location = new(xToken == null ? 0 : xToken.Value<int>(), yToken == null ? 0 : yToken.Value<int>());
			}
			if (Location == Vector2I.minusone)
				return false;

			JToken typeToken = jsonObject.SelectToken("type");
			if (typeToken == null || typeToken.Type != JTokenType.Integer)
				return false;
			Type = (NoteType)typeToken.Value<int>();
			if (Type == NoteType.Text)
			{
				JToken titleToken = jsonObject.SelectToken("title");
				JToken textToken = jsonObject.SelectToken("text");
				JToken tinthexToken = jsonObject.SelectToken("tinthex");
				if (titleToken == null || titleToken.Type != JTokenType.String)
					return false;
				if (textToken == null || textToken.Type != JTokenType.String)
					return false;
				if (tinthexToken == null || tinthexToken.Type != JTokenType.String)
					return false;
				Title = titleToken.Value<string>();
				Text = textToken.Value<string>();
				SymbolTint = Util.ColorFromHex(tinthexToken.Value<string>());

				JToken symbolToken = jsonObject.SelectToken("symbol");
				if (symbolToken != null && symbolToken.Type == JTokenType.String)
					Symbol = symbolToken.Value<string>();

				return true;
			}
			else if (Type == NoteType.Element)
			{
				JToken idToken = jsonObject.SelectToken("id");
				JToken massToken = jsonObject.SelectToken("mass");
				JToken tempToken = jsonObject.SelectToken("temp");
				if (idToken == null || idToken.Type != JTokenType.Integer)
					return false;
				if (massToken == null || massToken.Type != JTokenType.Float && massToken.Type != JTokenType.Integer)
					return false;
				if (tempToken == null || tempToken.Type != JTokenType.Float && tempToken.Type != JTokenType.Integer)
					return false;
				ElementId = (SimHashes)idToken.Value<int>();
				ElementMass = massToken.Value<float>();
				ElementTemperature = tempToken.Value<float>();
				return true;
			}
			return false;
		}

		internal BlueprintNoteData GetClone()
		{
			return new BlueprintNoteData()
			{
				Location = this.Location,
				Type = this.Type,
				Text = this.Text,
				Title = this.Title,
				Symbol = this.Symbol,
				SymbolTint = this.SymbolTint,
				ElementId = this.ElementId,
				ElementMass = this.ElementMass,
				ElementTemperature = this.ElementTemperature,
			};
		}
		public static BlueprintNoteData CreateTextNote(Vector2I location, string title, string text, string icon, Color tint)
		{
			return new BlueprintNoteData()
			{
				Location = location,
				Type = NoteType.Text,
				Title = title,
				Text = text,
				Symbol = icon,
				SymbolTint = tint,
			};
		}
		public static BlueprintNoteData CreateElementNote(Vector2I location, SimHashes elementId, float mass, float temperature)
		{
			return new BlueprintNoteData()
			{
				Location = location,
				Type = NoteType.Element,
				ElementId = elementId,
				ElementMass = mass,
				ElementTemperature = temperature,
			};
		}
	}
}
