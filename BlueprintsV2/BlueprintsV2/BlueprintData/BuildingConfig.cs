
using BlueprintsV2.ModAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlueprintsV2.BlueprintData
{

	/// <summary>
	/// Describes an individual building inside of a blueprint.
	/// </summary>
	public sealed class BuildingConfig : IEquatable<BuildingConfig>
	{

		/// <summary>
		/// The offset from the bottom left of a blueprint.
		/// </summary>
		
		[JsonConverter(typeof(Vector2IConverter))]
		public Vector2I Offset { get; set; } = new Vector2I(0, 0);

		/// <summary>
		/// The <see cref="BuildingDef"/> of the blueprint this represents.
		/// </summary>
		public BuildingDef BuildingDef { get; set; }

		/// <summary>
		/// The elements the building is to be constructed from.
		/// </summary>
		public List<Tag> SelectedElements { get; } = new List<Tag>();

		/// <summary>
		/// The orientation of the building.
		/// </summary>
		public Orientation Orientation { get; set; } = 0;


		/// <summary>
		/// Replaces Flags in V2 BPss for better versatility,
		/// any custom data stored for that building, inluding conduit directions
		/// </summary>
		public Dictionary<string, JObject> AdditionalBuildingData = null;

		private int ConduitFlags = -1;


		public bool HasAnyBuildingData => AdditionalBuildingData != null && AdditionalBuildingData.Any();

		public bool TryGetDataValue(string id, out JObject data)
		{
			data = null;
			return AdditionalBuildingData != null && AdditionalBuildingData.TryGetValue(id, out data);
		}

		//old binary method, not compatible with new data format
		///// <summary>
		///// Appends the building config to the given binary writer.
		///// </summary>
		///// <param name="binaryWriter">The <see cref="BinaryWriter"/> encsapsulating the stream to write to</param>
		//[Obsolete]
		//public void WriteBinary(BinaryWriter binaryWriter)
		//{
		//    //To prevent crashes. Should never actually happen, though,
		//    if (BuildingDef == null)
		//    {
		//        Debug.Log("Error when writing building config: No building definition.");
		//        return;
		//    }

		//    binaryWriter.Write(Offset.X);
		//    binaryWriter.Write(Offset.y);
		//    binaryWriter.Write(BuildingDef.PrefabID);
		//    binaryWriter.Write(SelectedElements.Count);
		//    SelectedElements.ForEach(selectedElement => binaryWriter.Write(selectedElement.GetHash()));
		//    binaryWriter.Write((int)Orientation);
		//    binaryWriter.Write(-1);
		//    binaryWriter.Write(AdditionalBuildingData.Count);
		//    AdditionalBuildingData.ToList().ForEach(entry =>
		//    {
		//        binaryWriter.Write(entry.Key);
		//        binaryWriter.Write(entry.Value.ToString());
		//    });
		//}

		public void ApplyGlobalMaterialOverrides()
		{
			var ingredients = BuildingDef.CraftRecipe.Ingredients;
			var elements = new List<Tag>(SelectedElements.Count);
			for (int i = 0; i < ingredients.Count; ++i)
			{
				var ingredient = ingredients[i];
				Tag selectedElement;
				if (i < SelectedElements.Count)
				{
					selectedElement = SelectedElements[i];
				}
				else
				{
					//should never happen in vanilla, just in case to prevent crash. (can potentially happen if another mod adds a new ingredient to the recipe or alters it)
					selectedElement = ModAssets.GetFirstAvailableMaterial(ingredient.tag, ingredient.amount);
				}
				var key = BlueprintSelectedMaterial.GetBlueprintSelectedMaterial(selectedElement, ingredient.tag, BuildingDef.PrefabID);

				if (ModAssets.TryGetReplacementTag(key, out var replacement))
				{
					selectedElement = replacement;
				}
				elements.Add(selectedElement);
			}
			SelectedElements.Clear();
			SelectedElements.AddRange(elements);
		}

		public bool IsValid()
		{
			return BuildingDef != null;
		}

		/// <summary>
		/// Writes the building config to the given JSON writer.
		/// Doesn't write unnecessary information to save space.
		/// </summary>
		/// <param name="jsonWriter">The <see cref="JsonWriter"/> encsapsulating the stream to write to</param>
		public void WriteJson(JsonWriter jsonWriter)
		{
			if (BuildingDef == null)
			{
				Debug.Log("Error when writing building config: No building definition.");
				return;
			}

			jsonWriter.WriteStartObject();

			if (Offset.x != 0 || Offset.y != 0)
			{
				jsonWriter.WritePropertyName("offset");
				jsonWriter.WriteStartObject();

				if (Offset.x != 0)
				{
					jsonWriter.WritePropertyName("x");
					jsonWriter.WriteValue(Offset.x);
				}

				if (Offset.y != 0)
				{
					jsonWriter.WritePropertyName("y");
					jsonWriter.WriteValue(Offset.y);
				}

				jsonWriter.WriteEndObject();
			}

			jsonWriter.WritePropertyName("buildingdef");
			jsonWriter.WriteValue(BuildingDef.PrefabID);

			jsonWriter.WritePropertyName("selected_elements");
			jsonWriter.WriteStartArray();
			SelectedElements.ForEach(elementTag => jsonWriter.WriteValue(elementTag.GetHash()));
			jsonWriter.WriteEndArray();

			if (Orientation != 0)
			{
				jsonWriter.WritePropertyName("orientation");
				jsonWriter.WriteValue((int)Orientation);
			}
			//compatibility for old bp mod
			if (GetConduitFlags(out int flags))
			{
				jsonWriter.WritePropertyName("flags");
				jsonWriter.WriteValue(flags);
			}
			if (AdditionalBuildingData != null)
			{
				jsonWriter.WritePropertyName("buildingData");
				jsonWriter.WriteStartArray();
				AdditionalBuildingData.ToList().ForEach(dataEntry =>
				{
					if (dataEntry.Key == null || dataEntry.Value == null)
						return;
					JObject data = new JObject()
					{
					new JProperty("Key",dataEntry.Key),
					new JProperty ("Value",dataEntry.Value)
					};
					data.WriteTo(jsonWriter);
				}
				);
				jsonWriter.WriteEndArray();
			}

			jsonWriter.WriteEndObject();
		}

		/// <summary>
		/// Reads a portion of a binary stream to populate this building config.
		/// OBSOLETE!, kept in for compatibility with old binary blueprints
		/// </summary>
		/// <param name="binaryReader">The <see cref="BinaryReader"/> encapsulating the binary information to read</param>
		/// <returns>True if the read succeeded, false otherwise</returns>

		public bool ReadBinary(BinaryReader binaryReader)
		{
			try
			{
				Offset = new Vector2I(binaryReader.ReadInt32(), binaryReader.ReadInt32());
				BuildingDef = Assets.GetBuildingDef(binaryReader.ReadString());

				int selectedElementCount = binaryReader.ReadInt32();
				for (int i = 0; i < selectedElementCount; ++i)
				{
					Tag elementTag;

					//Only add the tag to the list if it describes a valid element in game.
					if (ElementLoader.GetElement(elementTag = new Tag(binaryReader.ReadInt32())) != null)
					{
						SelectedElements.Add(elementTag);
					}
				}

				Orientation = (Orientation)binaryReader.ReadInt32();
				int oldFlagSystemValue = binaryReader.ReadInt32();
				if (oldFlagSystemValue != -1)
				{
					SetConduitFlags(oldFlagSystemValue);
				}
				return true;
			}

			catch (System.Exception e)
			{
				//SgtLogger.error(e.Message);
				return false;
			}
		}

		/// <summary>
		/// Reads a JSON object to populate this building config.
		/// </summary>
		/// <param name="rootObject">The <see cref="JObject"/> to use to read from</param>
		public void ReadJson(JObject rootObject)
		{
			JToken offsetToken = rootObject.SelectToken("offset");
			JToken buildingDefToken = rootObject.SelectToken("buildingdef");
			JToken selectedElementsToken = rootObject.SelectToken("selected_elements");
			JToken orientationToken = rootObject.SelectToken("orientation");
			JToken flagsToken = rootObject.SelectToken("flags");
			JToken buildingDataToken = rootObject.SelectToken("buildingData");

			if (offsetToken != null && offsetToken.Type == JTokenType.Object)
			{
				JToken xToken = offsetToken.SelectToken("x");
				JToken yToken = offsetToken.SelectToken("y");

				if (xToken != null && xToken.Type == JTokenType.Integer || yToken != null && yToken.Type == JTokenType.Integer)
				{
					Offset = new Vector2I(xToken == null ? 0 : xToken.Value<int>(), yToken == null ? 0 : yToken.Value<int>());
				}
			}

			if (buildingDefToken != null && buildingDefToken.Type == JTokenType.String)
			{
				BuildingDef = Assets.GetBuildingDef(buildingDefToken.Value<string>());
			}

			if (selectedElementsToken != null && selectedElementsToken.Type == JTokenType.Array)
			{
				JArray selectedElementTokens = selectedElementsToken.Value<JArray>();

				if (selectedElementTokens != null)
				{
					foreach (JToken selectedElement in selectedElementTokens)
					{
						if (selectedElement.Type == JTokenType.Integer)
						{
							Tag selectedTag = new Tag(selectedElement.Value<int>());
							if(ElementLoader.GetElement(selectedTag) != null || Assets.GetPrefab(selectedTag) != null)
								SelectedElements.Add(selectedTag);
						}
					}
				}
			}

			if (orientationToken != null && orientationToken.Type == JTokenType.Integer)
			{
				Orientation = (Orientation)orientationToken.Value<int>();
			}

			if (flagsToken != null && flagsToken.Type == JTokenType.Integer)
			{
				var value = flagsToken.Value<int>();
				if (value != -1)
				{
					SetConduitFlags(value);
				}
			}
			if (buildingDataToken != null)
			{
				JArray additionalDataTokens = buildingDataToken.Value<JArray>();

				if (additionalDataTokens != null)
				{
					foreach (JObject dataToken in additionalDataTokens)
					{
						string key = dataToken.GetValue("Key").Value<string>();
						JObject value = dataToken.GetValue("Value").Value<JObject>();
						if (key == null || key.Length == 0 || value == null)
							continue;

						SetBuildingData(key, value);
					}
				}
			}
		}

		/// <summary>
		/// Tests two <see cref="BuildingConfig"/> for equality.
		/// </summary>
		/// <param name="otherBuildingConfig">The other <see cref="BuildingConfig"/> to test for equality</param>
		/// <returns>True if the two objects are equal, false otherwise</returns>
		public bool Equals(BuildingConfig otherBuildingConfig)
		{
			return otherBuildingConfig != null && Offset == otherBuildingConfig.Offset && BuildingDef == otherBuildingConfig.BuildingDef && Orientation == otherBuildingConfig.Orientation;
		}
		internal void SetBuildingData(string Id, JObject data)
		{
			if (Id.IsNullOrWhiteSpace() || data == null)
				return;

			if (AdditionalBuildingData == null)
				AdditionalBuildingData = new();
			AdditionalBuildingData[Id] = data;

		}
		internal void SetConduitFlags(int flag)
		{
			ConduitFlags = flag; 
			//SetBuildingData(API_Consts.ConduitFlagID,
			//	new JObject()
			//	{
			//		{ API_Consts.ConduitFlagID, flag }
			//	});
		}
		internal bool GetConduitFlags(out int flags)
		{
			flags = ConduitFlags;
			//if (AdditionalBuildingData != null && AdditionalBuildingData.TryGetValue(API_Consts.ConduitFlagID, out var value)
			//	&& value.SelectToken(API_Consts.ConduitFlagID) != null)
			//{
			//	JToken token = value.SelectToken(API_Consts.ConduitFlagID);

			//	if (token != null && token.Type == JTokenType.Integer)
			//	{
			//		flags = token.Value<int>();
			//		return true;
			//	}
			//}
			return flags != -1;
		}
	}
}
