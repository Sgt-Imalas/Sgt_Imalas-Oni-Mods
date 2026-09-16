using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration.Packets;
using ONI_Together_API;
using ONI_Together_API.Networking;
using System;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration
{
	internal class MP_Helpers
	{
		public static bool MPInstalledAndActive()
		{
			return MP_Mod_Info.MultiplayerModPresent && SessionInfoAPI.InSession;
		}

		internal static void HandleNoteUpdate(BlueprintNote blueprintNote) => HandleNoteCreation(blueprintNote);
		internal static void HandleNoteCreation(BlueprintNote blueprintNote)
		{
			if (!MPInstalledAndActive() || blueprintNote == null)
				return;

			PacketSenderAPI.SendToAllOtherPeers(new NoteCreateOrUpdatePacket(blueprintNote));
		}

		internal static void HandleNoteDeletion(BlueprintNote blueprintNote)
		{
			if(!MPInstalledAndActive() || blueprintNote == null)
				return;

			PacketSenderAPI.SendToAllOtherPeers(new NoteCancelPacket(blueprintNote.NaturalBuildingCell()));
		}

		internal static void HandleTextNoteCreation(int cell, string title, string text, Color color)
		{
			throw new NotImplementedException();
		}
	}
}
