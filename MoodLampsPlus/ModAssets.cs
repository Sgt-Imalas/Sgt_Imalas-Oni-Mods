using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.ModAPIClasses;

namespace MoodLampsPlus
{
	internal class ModAssets
	{
		public static bool AddAmogusLamps = true;
		public static bool AddBathtubLamp = true;

		internal static void RegisterMoodLamps()
		{
			if (!DecorPackA_ModAPI.TryInitialize(true))
			{
				SgtLogger.warning("DecorpackA not found, cannot register custom moodlamps. Please install and enable DecorpackA to use this mod.");
				return;
			}
			RegisterMoodLamps_Mogus();
			RegisterMoodLamps_Bathtub();
			RegisterMoodLamps_Own();
		}

		public static string GetSourceInfo(string lampName) => lampName+"\n\n<size=80%>" + STRINGS.MOODLAMPSKINS.MLP_SOURCE + "</size>";

		/// <summary>
		/// Lamps that are also in amorbus mod
		/// </summary>
		static void RegisterMoodLamps_Mogus()
		{
			if (AddAmogusLamps == false)
			{
				SgtLogger.l("Amorbus already active, skipping duplicate registrations");
			}
			DecorPackA_ModAPI.AddMoodLamp(
					"AM_SittingMogus",
					STRINGS.MOODLAMPSKINS.SITTINGMOGUS,
					"moodlamp_amogus_sitting_kanim",
					Color.white,
					DecorPackA_ModAPI.LAMPCATEGORIES.CUSTOMIZABLE,
					GetSourceInfo(STRINGS.MOODLAMPSKINS.SITTINGMOGUS))
				.AddMoodLampTags(DecorPackA_ModAPI.TAGS.TINTABLE);


			DecorPackA_ModAPI.AddMoodLamp(
					"AM_StickbuggedLoL",
					STRINGS.MOODLAMPSKINS.STICKBUG,
					"moodlamp_stickbug_kanim",
					UIUtils.rgb(243, 241, 196),
					DecorPackA_ModAPI.LAMPCATEGORIES.MEDIA,
					GetSourceInfo(STRINGS.MOODLAMPSKINS.STICKBUG),
					playModeWhenOn: KAnim.PlayMode.Loop);
		}

		/// <summary>
		/// Lamp thats also in Bathtub mod
		/// </summary>
		static void RegisterMoodLamps_Bathtub()
		{
			if (AddBathtubLamp == false)
			{
				SgtLogger.l("BathTub already active, skipping duplicate registrations");
			}
			DecorPackA_ModAPI.AddMoodLamp(
					"BT_RubberDuckie",
					STRINGS.MOODLAMPSKINS.RUBBERDUCKIE,
					"moodlamp_duck_kanim",
					Color.white,
					DecorPackA_ModAPI.LAMPCATEGORIES.CUSTOMIZABLE, 
					GetSourceInfo(STRINGS.MOODLAMPSKINS.RUBBERDUCKIE))
				.AddMoodLampTags(DecorPackA_ModAPI.TAGS.TINTABLE);
		}

		/// <summary>
		/// Lamps added this mod
		/// </summary>
		static void RegisterMoodLamps_Own()
		{
			DecorPackA_ModAPI.AddMoodLamp(
					"MLP_NuclearBomblet",
					STRINGS.MOODLAMPSKINS.NUCLEAR_BOMBLET,
					"moodlamp_bomblet_kanim",
					0f,1.5f,0f,
					DecorPackA_ModAPI.LAMPCATEGORIES.MODS,
					GetSourceInfo(STRINGS.MOODLAMPSKINS.NUCLEAR_BOMBLET))
				.MakeMoodLampShifty(0.5f,1.75f,0.5f, 4f);

			DecorPackA_ModAPI.AddMoodLamp(
					"MLP_TheBodyIsRound",
					STRINGS.MOODLAMPSKINS.ROUND_PIP,
					"moodlamp_round_pip_kanim",
					1.1f, 1.26f, 1.48f,
					DecorPackA_ModAPI.LAMPCATEGORIES.CRITTERS,
					GetSourceInfo(STRINGS.MOODLAMPSKINS.ROUND_PIP));
		}

	}
}
