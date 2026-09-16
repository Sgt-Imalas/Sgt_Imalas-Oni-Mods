using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONI_Together.Networking.Packets.Architecture;
using System.IO;
using System.Text;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration.Packets
{
	internal class NoteCreateOrUpdatePacket : IPacket
	{
		int cell;
		string serializedNoteData;

		public NoteCreateOrUpdatePacket() { }

		static StringBuilder sb = new();
		static StringWriter sw = new(sb);
		static JsonTextWriter jtw = new JsonTextWriter(sw);

		public NoteCreateOrUpdatePacket(BlueprintNote dataHandler)
		{
			if (dataHandler == null)
				return;
			cell = dataHandler.NaturalBuildingCell();

			sb.Clear();
			dataHandler.GetNoteData().WriteDataJson(jtw);
			string data = sb.ToString();
			//SgtLogger.l("Serializing Note Data:\n" + data);
			serializedNoteData = data.CompressString();
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(cell);
			writer.Write(serializedNoteData);
		}
		public void Deserialize(BinaryReader reader)
		{
			cell = reader.ReadInt32();
			serializedNoteData = reader.ReadString().DecompressString();
			//SgtLogger.l("DeserializedNoteData:\n" + serializedNoteData);
		}

		public void OnDispatched()
		{
			var data = new BlueprintNoteData();
			var token = JObject.Parse(serializedNoteData);

			if (data.ReadDataJson(token))
			{
				BlueprintNote.ClearExistingNote(cell);
				switch (data.Type)
				{
					case BlueprintNoteData.NoteType.Text:
						TextNote.Create(cell, data.Title, data.Text,data.Symbol, data.SymbolTint,true); 
						break;
					case BlueprintNoteData.NoteType.Element:
						ElementNote.Create(cell, data.ElementId,data.ElementMass, data.ElementTemperature, true);
						break;
				}
			}
			else
				SgtLogger.warning("Could not deserialize NoteData:\n"+serializedNoteData);
		}
	}
}
