using BlueprintsV2.BlueprintData;
using ONI_Together.Networking.Packets.Architecture;
using ONI_Together_API;
using System.IO;
using System.Text;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration.Packets
{
	internal class StartBlueprintVisualizationPacket : IPacket
	{
		public StartBlueprintVisualizationPacket() { }
		public StartBlueprintVisualizationPacket(Blueprint bp, Vector2I pos)
		{
			SenderId = SessionInfoAPI.LocalUserID;
			blueprint = bp;
			x = pos.x; 
			y = pos.y;
		}
		ulong SenderId;
		int x, y;
		Blueprint blueprint;

		string GetCompressed()
		{
			var sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			blueprint.WriteJsonString(sw);
			return (sb.ToString().CompressString());
		}

		public void Deserialize(BinaryReader reader)
		{
			SenderId = reader.ReadUInt64();
			x = reader.ReadInt32();
			y = reader.ReadInt32();
			string uncompressedBp = reader.ReadString().DecompressString();
			if (!ModAssets.TryImportBlueprintFromString(uncompressedBp, out blueprint, false))
			{
				SgtLogger.warning("[MP] could not visualize blueprint received from other player");
				blueprint = null;
				return;
			}
		}
		public void Serialize(BinaryWriter writer)
		{
			writer.Write(SenderId);
			writer.Write(x);
			writer.Write(y);
			writer.Write(GetCompressed());
		}
		public void OnDispatched()
		{
			if (blueprint == null)
				return;
			BlueprintState.SetDisplayBlueprintForPlayer(blueprint, SenderId);
		}
	}
}
