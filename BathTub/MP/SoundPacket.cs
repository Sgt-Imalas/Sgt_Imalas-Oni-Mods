using ONI_Together.Networking.Packets.Architecture;
using ONI_Together_API.Networking;
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
		public SoundPacket(string file, ulong senderId, float volume)
		{
			SoundFile = file;
			SenderId = senderId;
			volumePercentage = volume;
		}

		string SoundFile = string.Empty;
		ulong SenderId;
		float volumePercentage = 1f;

		public void Deserialize(BinaryReader reader)
		{
			Console.WriteLine($"sound packet deserialized");
			SoundFile = reader.ReadString();
			SenderId = reader.ReadUInt64();
			volumePercentage = reader.ReadSingle();
		}
		public void Serialize(BinaryWriter writer)
		{
			Console.WriteLine($"sound packet serialized");
			writer.Write(SoundFile);
			writer.Write(SenderId);
			writer.Write(volumePercentage);
		}

		public void OnDispatched()
		{
			Console.WriteLine($"sound packet received with ID: {SoundFile}");
			ModAssets.PlaySoundFromMP(SoundFile, volumePercentage);

			if (ONI_Together_API.SessionInfoAPI.IsHost)
			{
				//re-broadcast to other clients
				HashSet<ulong> exclude = [SenderId, ONI_Together_API.SessionInfoAPI.LocalUserID];
				PacketSenderAPI.SendToAllExcluding(this, exclude);
			}
		}
	}
}
