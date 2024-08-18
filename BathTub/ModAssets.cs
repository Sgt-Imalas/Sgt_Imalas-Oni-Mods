using BathTub.Duck;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BathTub
{
    internal class ModAssets
    {
        public static bool RoomsExpandedActive = false;

        public static GameObject FoamFX;

        internal static void InitSounds()
        {
            //SgtLogger.l("initiating foam particle system");
            //var movingGo = FoamFX.transform.Find("SparkleStreakMovement")?.gameObject;
            //var stationaryGo = FoamFX.transform.Find("SparkleStreakIdle")?.gameObject;

            //if (movingGo != null 
            //    && movingGo.TryGetComponent<ParticleSystem>(out var ps_moving) 
            //    && movingGo.TryGetComponent<ParticleSystemRenderer>(out var psr_moving))
            //{
            //    UtilMethods.ListAllPropertyValues(psr_moving);
            //    UtilMethods.ListAllFieldValues(psr_moving);
            //}
            //if (stationaryGo != null 
            //    && stationaryGo.TryGetComponent<ParticleSystem>(out var ps_stationary)
            //    && stationaryGo.TryGetComponent<ParticleSystemRenderer>(out var psr_stationary))
            //{

            //}
            InitQuacks();
            InitSqueaks();
        }
        internal static void PlayRandomQuack(DuckNoises duckNoises)
        {
            Debug.Log("Quack");
            string sound = Sounds.Quacks.GetRandom();
            SoundUtils.PlaySound(sound, SoundUtils.GetSFXVolume(),false,duckNoises.gameObject);
        }
        internal static void PlayRandomSqueak(DuckNoises duckNoises)
        {
            Debug.Log("Squeak");
            string sound = Sounds.Squeaks.GetRandom();
            SoundUtils.PlaySound(sound, SoundUtils.GetSFXVolume()*0.65f, true);
        }
        public static class Sounds
        {
            public static List<string> Quacks = new List<string>()
            {
                "Quack_1",
                "Quack_2",
                "Quack_3",
                "Quack_4",
            }; 
            public static List<string> Squeaks = new List<string>()
            {
                "Squeak_1",
                "Squeak_2",
                "Squeak_3",
                "Squeak_4",
                "Squeak_5",
                "Squeak_6",
                "Squeak_7",
                "Squeak_8",
                "Squeak_9",
                "Squeak_10",
            };
        }

        static void InitSqueaks()
        {
            foreach(string sound in Sounds.Squeaks)
            {
                SoundUtils.LoadSound(sound, sound+".wav");
            }

        }
        static void InitQuacks()
        {
            foreach (string sound in Sounds.Quacks)
            {
                SoundUtils.LoadSound(sound, sound + ".wav");
            }
        }
    }
}
