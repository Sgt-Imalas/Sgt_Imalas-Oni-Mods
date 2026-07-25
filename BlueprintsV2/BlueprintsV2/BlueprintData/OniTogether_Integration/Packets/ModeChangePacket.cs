using BlueprintsV2.BlueprintData;
using HarmonyLib;
using ONI_Together.Networking.Packets.Architecture;
using ONI_Together_API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UtilLibs;
using static BlueprintsV2.BlueprintData.BlueprintState;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration.Packets
{
	public class ModeChangePacket : IPacket
	{
		public ulong SenderId;

		public bool AdvancedMaterialReplacement = false;
		public bool ForceBuild = false;
		public bool MaterialReplacementInSnapshots = false;
		public bool IsPlacingSnapshot = false;
		public Orientation BlueprintOrientation;
		public bool FlippedX, FlippedY;
		public PermittedRotations Permitted;
		public int _state = 0;
		public float originShiftX = 0, originShiftY = 0;
		public bool applySettings = true;
		public bool UseToolPriority;
		public List<string> BlockedPlacementFilterLayers;
		public ModeChangePacket()
		{
			SenderId = SessionInfoAPI.LocalUserID;
		}
		public void Deserialize(BinaryReader reader)
		{
			SenderId = reader.ReadUInt64();
			AdvancedMaterialReplacement = reader.ReadBoolean();
			ForceBuild = reader.ReadBoolean();
			MaterialReplacementInSnapshots = reader.ReadBoolean();
			IsPlacingSnapshot = reader.ReadBoolean();
			BlueprintOrientation = (Orientation)reader.ReadInt32();
			FlippedX = reader.ReadBoolean();
			FlippedY = reader.ReadBoolean();
			Permitted = (PermittedRotations)reader.ReadInt32();
			_state = reader.ReadInt32();
			originShiftX = reader.ReadSingle();
			originShiftY = reader.ReadSingle();
			applySettings = reader.ReadBoolean();
			UseToolPriority = reader.ReadBoolean();
			int count = reader.ReadInt32();
			BlockedPlacementFilterLayers = [];
			for (int i = 0; i < count; i++)
				BlockedPlacementFilterLayers[i] = reader.ReadString();

		}
		public void Serialize(BinaryWriter writer)
		{
			writer.Write(SenderId);
			writer.Write(AdvancedMaterialReplacement);
			writer.Write(ForceBuild);
			writer.Write(MaterialReplacementInSnapshots);
			writer.Write(IsPlacingSnapshot);
			writer.Write((int)BlueprintOrientation);
			writer.Write(FlippedX);
			writer.Write(FlippedY);
			writer.Write((int)Permitted);
			writer.Write(_state);
			writer.Write(originShiftX);
			writer.Write(originShiftY);
			writer.Write(applySettings);
			writer.Write(UseToolPriority);
			writer.Write(BlockedPlacementFilterLayers.Count);
			for (int i = 0; i < BlockedPlacementFilterLayers.Count; i++)
				writer.Write(BlockedPlacementFilterLayers[i]);
		}
		public void OnDispatched()
		{
			if (SenderId == SessionInfoAPI.LocalUserID)
				return;
			BlueprintState.UpdateRemoteState(SenderId, this);
		}
	}
}
