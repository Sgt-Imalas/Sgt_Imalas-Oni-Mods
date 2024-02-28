using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressedCritters.Critters.Hatches
{
    // Decompiled with JetBrains decompiler
    // Type: CompressedHatchTuning
    // Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
    // MVID: 5FAA631D-B74E-46D2-A5CA-F769AB27C29E
    // Assembly location: X:\SteamLibrary\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll

    using System.Collections.Generic;
    using TUNING;
    using UnityEngine;
    using static EggProtectionMonitor.Instance;

    public static class CompressedHatchTuning
    {
        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = CompressedHatchConfig.ID+"Egg".ToTag(),
      weight = 0.98f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = CompressedHatchHardConfig.ID+"Egg".ToTag(),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = CompressedHatchVeggyConfig.ID+"Egg".ToTag(),
      weight = 0.02f
    }
  };
        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_HARD = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = CompressedHatchConfig.ID+"Egg".ToTag(),
      weight = 0.32f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = CompressedHatchHardConfig.ID+"Egg".ToTag(),
      weight = 0.65f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = CompressedHatchHardConfig.ID+"Egg".ToTag(),
      weight = 0.02f
    }
  };
        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_VEGGIE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = CompressedHatchConfig.ID+"Egg".ToTag(),
      weight = 0.33f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = CompressedHatchVeggyConfig.ID+"Egg".ToTag(),
      weight = 0.67f
    }
  };
        public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_METAL = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = CompressedHatchConfig.ID+"Egg".ToTag(),
      weight = 0.11f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = CompressedHatchHardConfig.ID+"Egg".ToTag(),
      weight = 0.22f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = CompressedHatchHardConfig.ID+"Egg".ToTag(),
      weight = 0.67f
    }
  };
        public static float SIZEMULTIPLIER = 8;
        public static float STANDARD_CALORIES_PER_CYCLE = 700000f * SIZEMULTIPLIER;
        public static float STANDARD_STARVE_CYCLES = 10f;
        public static float STANDARD_STOMACH_SIZE = STANDARD_CALORIES_PER_CYCLE * STANDARD_STARVE_CYCLES;
        public static int PEN_SIZE_PER_CREATURE = Mathf.RoundToInt(CREATURES.SPACE_REQUIREMENTS.TIER3 * SIZEMULTIPLIER);
        public static float EGG_MASS = 2f * SIZEMULTIPLIER;
    }

}
