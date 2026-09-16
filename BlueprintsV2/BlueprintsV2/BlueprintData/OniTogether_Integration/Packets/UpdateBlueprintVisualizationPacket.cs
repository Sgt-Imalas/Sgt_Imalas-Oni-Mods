using BlueprintsV2.BlueprintData;
using ONI_Together.Networking.Packets.Architecture;
using ONI_Together_API;
using System.IO;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration.Packets
{
	internal class UpdateBlueprintVisualizationPacket : IPacket
	{
		public UpdateBlueprintVisualizationPacket() { SenderId = SessionInfoAPI.LocalUserID; }
		public UpdateBlueprintVisualizationPacket(Vector2I pos)
		{
			SenderId = SessionInfoAPI.LocalUserID;
			x = pos.x;
			y = pos.y;
		}
		ulong SenderId;
		int x, y;

		public void Deserialize(BinaryReader reader)
		{
			SenderId = reader.ReadUInt64();
			x = reader.ReadInt32();
			y = reader.ReadInt32();
		}
		public void Serialize(BinaryWriter writer)
		{
			writer.Write(SenderId);
			writer.Write(x);
			writer.Write(y);
		}
		public void OnDispatched()
		{
			BlueprintState.MoveDisplayBlueprintForPlayer(SenderId, x, y);
		}
	}
}
