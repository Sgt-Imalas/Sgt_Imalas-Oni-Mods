using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BlueprintsV2.BlueprintData
{
	/// <summary>
	/// Custom JSON converter for the Vector2I class.
	/// unused for now, but may be useful in the future.
	/// </summary>
	public class Vector2IConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Vector2I);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;

			JObject obj = JObject.Load(reader);
			int x = obj["x"]?.Value<int>() ?? 0;
			int y = obj["y"]?.Value<int>() ?? 0;

			return new Vector2I(x, y);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is Vector2I vector)
			{
				writer.WriteStartObject();
				writer.WritePropertyName("x");
				writer.WriteValue(vector.x);
				writer.WritePropertyName("y");
				writer.WriteValue(vector.y);
				writer.WriteEndObject();
			}
			else
			{
				writer.WriteNull();
			}
		}
	}
}
