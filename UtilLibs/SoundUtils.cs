using FMOD;
using FMODUnity;
using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UtilLibs
{
	public static class SoundUtils
	{
		#region copySoundsFromExistingAnim
		public static void DumpAllGetSounds()
		{
			var trav = Traverse.Create(GlobalAssets.Instance);
			var dic = trav.Field("SoundTable").GetValue<Dictionary<string, string>>();
			Debug.Log("Dic found? - " + (dic.Count > 0));
			foreach (var sound in dic)
			{
				Debug.Log(sound);
			}
		}

		private delegate void CreateSoundDelegate(AudioSheets instance, string file_name,
			string anim_name, string type, float min_interval, string sound_name, int frame,
			string dlcId);

		private static readonly DetouredMethod<CreateSoundDelegate> CREATE_SOUND =
			typeof(AudioSheets).DetourLazy<CreateSoundDelegate>("CreateSound");

		/// <summary>
		/// Copies the sounds from one animation to another animation. Since Hot Shots this
		/// method only copies sounds present in the base game audio sheets, not any sounds
		/// that may have been added by other mods.
		/// </summary>
		/// <param name="dstAnim">The destination anim file name.</param>
		/// <param name="srcAnim">The source anim file name.</param>
		public static void CopySoundsToAnim(string dstAnim, string srcAnim)
		{
			if (string.IsNullOrEmpty(dstAnim))
				throw new ArgumentNullException(nameof(dstAnim));
			if (string.IsNullOrEmpty(srcAnim))
				throw new ArgumentNullException(nameof(srcAnim));
			if (Assets.GetAnim(dstAnim) != null)
			{
				var audioSheet = GameAudioSheets.Get();
				// Go through sound infos on the old audio sheets, much easier than deep
				// copying all the sound infos with a giant switch case
				try
				{
					foreach (var sheet in audioSheet.sheets)
					{
						var infos = sheet.soundInfos;
						int n = infos.Length;
						for (int i = 0; i < n; i++)
						{
							var soundInfo = infos[i];
							if (DlcManager.IsContentSubscribed(soundInfo.RequiredDlcId) &&
									soundInfo.File == srcAnim)
								CreateAllSounds(audioSheet, dstAnim, soundInfo, sheet.
									defaultType);
						}
					}
				}
				catch (Exception e)
				{
					PUtil.LogWarning("Unable to copy sound files from {0} to {1}:".F(srcAnim,
						dstAnim));
					PUtil.LogExcWarn(e);
				}
			}
			else
				PUtil.LogWarning("Destination animation \"{0}\" not found!".F(dstAnim));
		}

		/// <summary>
		/// Calls out to the base game CreateSound delegate in AudioSheets.
		/// </summary>
		/// <param name="sheet">The location where the sound event will be stored.</param>
		/// <param name="file">The animation file name.</param>
		/// <param name="type">The event type to create.</param>
		/// <param name="info">Used for the minimum interval, DLC ID, and anim name.</param>
		/// <param name="sound">The sound name to play.</param>
		/// <param name="frame">The frame index to start the sound.</param>
		/// <returns>1 if the sound was created, or 0 if the sound was not created.</returns>
		private static int CreateSound(AudioSheets sheet, string file, string type,
				AudioSheet.SoundInfo info, string sound, int frame)
		{
			int n = 0;
			if (!string.IsNullOrEmpty(sound) && CREATE_SOUND != null)
			{
				CREATE_SOUND.Invoke(sheet, file, info.Anim, type, info.MinInterval, sound,
					frame, info.RequiredDlcId);
				n = 1;
			}
			return n;
		}

		/// <summary>
		/// Creates all of the sounds in the prefab (master) audio sheets, but with a different
		/// animation name substituted.
		/// </summary>
		/// <param name="sheet">The location where the sound event will be stored.</param>
		/// <param name="animFile">The substitute anim file name to use instead.</param>
		/// <param name="info">The sounds to be created.</param>
		/// <param name="defaultType">The sound type to use if the type is blank.</param>
		private static void CreateAllSounds(AudioSheets sheet, string animFile,
				AudioSheet.SoundInfo info, string defaultType)
		{
			string type = info.Type;
			int n;
			if (string.IsNullOrEmpty(type))
				type = defaultType;
			// Believe it or not this is better than trying to re-create a variant of
			// CreateSound again
			n = CreateSound(sheet, animFile, type, info, info.Name0, info.Frame0);
			n += CreateSound(sheet, animFile, type, info, info.Name1, info.Frame1);
			n += CreateSound(sheet, animFile, type, info, info.Name2, info.Frame2);
			n += CreateSound(sheet, animFile, type, info, info.Name3, info.Frame3);
			n += CreateSound(sheet, animFile, type, info, info.Name4, info.Frame4);
			n += CreateSound(sheet, animFile, type, info, info.Name5, info.Frame5);
			n += CreateSound(sheet, animFile, type, info, info.Name6, info.Frame6);
			n += CreateSound(sheet, animFile, type, info, info.Name7, info.Frame7);
			n += CreateSound(sheet, animFile, type, info, info.Name8, info.Frame8);
			n += CreateSound(sheet, animFile, type, info, info.Name9, info.Frame9);
			n += CreateSound(sheet, animFile, type, info, info.Name10, info.Frame10);
			n += CreateSound(sheet, animFile, type, info, info.Name11, info.Frame11);
		}
		#endregion

		/// <summary>
		/// Curtesy of Aki
		/// </summary>
		#region playCustomSounds
		public static float soundMultiplier = -10;
		public static float GetSFXVolume() => KPlayerPrefs.GetFloat("Volume_SFX") * KPlayerPrefs.GetFloat("Volume_Master");

		private static readonly Dictionary<string, FMOD.Sound> sounds = new Dictionary<string, FMOD.Sound>();

		public static bool LoadSound(string key, string fileName, bool looping = false, bool oneAtATime = false)
		{

			var path = Path.Combine(UtilMethods.ModPath, "assets");
			var soundFile = Path.Combine(path, fileName);
			if (!File.Exists(soundFile))
			{
				SgtLogger.error("Sound file does not exist: " + soundFile);
			}

			var mode = FMOD.MODE._2D | FMOD.MODE._3D | FMOD.MODE._3D_WORLDRELATIVE | FMOD.MODE.CREATESAMPLE;

			if (looping)
				mode |= FMOD.MODE.LOOP_NORMAL;

			if (oneAtATime)
				mode |= FMOD.MODE.UNIQUE;

			var r1 = RuntimeManager.CoreSystem.createSound(soundFile, mode, out var sound);
			if (r1 == FMOD.RESULT.OK)
			{
				sounds.Add(key, sound);
				return true;
			}
			else
			{
				SgtLogger.error($"AutoUtil: Failed to create sound. (key={key}, error={r1})");
				return false;
			}
		}
		public static int PlaySound(string key, float volume = 1f, bool global = false, GameObject attached = null, Vector3 position = default)
		{
			if (sounds.TryGetValue(key, out var sound))
			{
				var r1 = RuntimeManager.CoreSystem.getMasterChannelGroup(out var cg);
				if (r1 == FMOD.RESULT.OK)
				{
					var r2 = RuntimeManager.CoreSystem.playSound(sound, cg, true, out var channel);
					if (r2 == FMOD.RESULT.OK)
					{

						if (position == default)
							position = attached == null ? SoundListenerController.Instance.transform.position : attached.transform.position;

						var pos = position.ToFMODVector();
						var vel = new FMOD.VECTOR();

						channel.set3DAttributes(ref pos, ref vel);
						channel.setVolume(volume);
						channel.setMode(global ? MODE._2D : MODE._3D_WORLDRELATIVE);

						channel.setPaused(false);

						channel.getIndex(out var index);

						//if (global|| attached !=null)
						//{
						//    FollowCamera(channel, attached);
						//}

						return index;
					}
					else
					{
						SgtLogger.error($"AutoUtil: Failed to create sound instance. (key={key}, error={r2})");
					}
				}
				else
				{
					SgtLogger.error($"AutoUtil: Failed to get master channel group. (key={key}, error={r1})");
				}
			}
			else
			{
				Debug.LogWarning($"AudioUtil: Tried to play sound that does not exist. (key={key})");
			}

			return -1;
		}

		public static int PlaySound(string key, Vector3 position, float volume = 1f)
		{
			if (sounds.TryGetValue(key, out var sound))
			{
				var r1 = RuntimeManager.CoreSystem.getMasterChannelGroup(out var cg);
				if (r1 == FMOD.RESULT.OK)
				{
					var r2 = RuntimeManager.CoreSystem.playSound(sound, cg, true, out var channel);
					if (r2 == FMOD.RESULT.OK)
					{
						var pos = CameraController.Instance.GetVerticallyScaledPosition(position, false).ToFMODVector();
						var vel = new FMOD.VECTOR();
						channel.set3DAttributes(ref pos, ref vel);
						channel.setVolume(volume);

						channel.setPaused(false);

						channel.getIndex(out var index);
						return index;
					}
					else
					{
						SgtLogger.error($"AutoUtil: Failed to create sound instance. (key={key}, error={r2})");
					}
				}
				else
				{
					SgtLogger.error($"AutoUtil: Failed to get master channel group. (key={key}, error={r1})");
				}
			}
			else
			{
				Debug.LogWarning($"AudioUtil: Tried to play sound that does not exist. (key={key})");
			}

			return -1;
		}

		public static FMOD.Channel CreateSound(string key)
		{
			if (sounds.TryGetValue(key, out var sound))
			{
				var r1 = RuntimeManager.CoreSystem.getMasterChannelGroup(out var cg);
				if (r1 == FMOD.RESULT.OK)
				{
					var r2 = RuntimeManager.CoreSystem.playSound(sound, cg, true, out var channel);
					if (r2 == FMOD.RESULT.OK)
					{
						return channel;
					}
					else
					{
						SgtLogger.error($"AutoUtil: Failed to create sound instance. (key={key}, error={r2})");
					}
				}
				else
				{
					SgtLogger.error($"AutoUtil: Failed to get master channel group. (key={key}, error={r1})");
				}
			}
			else
			{
				Debug.LogWarning($"AudioUtil: Tried to play sound that does not exist. (key={key})");
			}

			return new FMOD.Channel();
		}

		public static void StopSound(int channelID)
		{
			if (channelID < 0)
				return;

			var r1 = RuntimeManager.CoreSystem.getChannel(channelID, out var channel);
			if (r1 == FMOD.RESULT.OK)
			{
				channel.stop();
				channel.clearHandle();
			}
		}
	}
	#endregion
}

