using BlueprintsV2.BlueprintData;
using ONI_Together.Networking.Packets.Architecture;
using ONI_Together_API;
using System.IO;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration.Packets
{
	internal class StopBlueprintVisualizationPacket : IPacket
	{
		public StopBlueprintVisualizationPacket()
		{
			SenderId = SessionInfoAPI.LocalUserID;
		}
		ulong SenderId;


		public void Deserialize(BinaryReader reader)
		{
			SenderId = reader.ReadUInt64();
		}
		public void Serialize(BinaryWriter writer)
		{
			writer.Write(SenderId);
		}
		public void OnDispatched()
		{
			BlueprintState.ClearDisplayBlueprintForPlayer(SenderId);
		}
	}
}
