using ONI_MP.Networking.Packets.Architecture;
using ONI_MP_API.Networking;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BathTub.MP
{
	internal class SoundPacket : IPacket
	{
		public SoundPacket() { }
		public SoundPacket(string file, CSteamID senderId, float volume)
		{
			SoundFile = file;
			SenderId = senderId;
			volumePercentage = volume;
		}

		string SoundFile = string.Empty;
		CSteamID SenderId;
		float volumePercentage = 1f;

		public void Deserialize(BinaryReader reader)
		{
			Console.WriteLine($"sound packet deserialized");
			SoundFile = reader.ReadString();
			SenderId = new CSteamID(reader.ReadUInt64());
			volumePercentage = reader.ReadSingle();
		}
		public void Serialize(BinaryWriter writer)
		{
			Console.WriteLine($"sound packet serialized");
			writer.Write(SoundFile);
			writer.Write(SenderId.m_SteamID);
			writer.Write(volumePercentage);
		}

		public void OnDispatched()
		{
			Console.WriteLine($"sound packet received with ID: {SoundFile}");
			ModAssets.PlaySoundFromMP(SoundFile, volumePercentage);

			if (ONI_MP_API.SessionInfoAPI.IsHost)
			{
				//re-broadcast to other clients
				HashSet<CSteamID> exclude = [SenderId, ONI_MP_API.SessionInfoAPI.LocalSteamID];
				PacketSenderAPI.SendToAllExcluding(this, exclude);
			}
		}
	}
}
