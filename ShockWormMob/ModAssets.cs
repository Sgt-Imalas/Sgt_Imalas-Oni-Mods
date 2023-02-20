using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShockWormMob
{
    internal class ModAssets
    {
       public  static Tag ShockwormConfigSpeciesID = TagManager.Create("ShockWormSpecies");
       public  static Tag GroneHogSpeciesID = TagManager.Create("GroneHogSpecies");

        public static List<GameplayEvent> events = new List<GameplayEvent>();
        public static void InitEventTest()
        {
            events.Clear();
            events.Add(Db.Get().GameplayEvents.HatchSpawnEvent);

            events.Add(Db.Get().GameplayEvents.PartyEvent);

            events.Add(Db.Get().GameplayEvents.EclipseEvent);

            events.Add(Db.Get().GameplayEvents.SatelliteCrashEvent);

            events.Add(Db.Get().GameplayEvents.FoodFightEvent);

            events.Add(Db.Get().GameplayEvents.PrickleFlowerBlightEvent);

            return;
            events.Add(Db.Get().GameplayEvents.BonusDream1);

            events.Add(Db.Get().GameplayEvents.BonusDream2);

            events.Add(Db.Get().GameplayEvents.BonusDream3);

            events.Add(Db.Get().GameplayEvents.BonusDream4);

            events.Add(Db.Get().GameplayEvents.BonusToilet1);

            events.Add(Db.Get().GameplayEvents.BonusToilet2);

            events.Add(Db.Get().GameplayEvents.BonusToilet3);

            events.Add(Db.Get().GameplayEvents.BonusToilet4);

            events.Add(Db.Get().GameplayEvents.BonusResearch);

            events.Add(Db.Get().GameplayEvents.BonusDigging1);

            events.Add(Db.Get().GameplayEvents.BonusStorage);

            events.Add(Db.Get().GameplayEvents.BonusBuilder);

            events.Add(Db.Get().GameplayEvents.BonusOxygen);

            events.Add(Db.Get().GameplayEvents.BonusAlgae);

            events.Add(Db.Get().GameplayEvents.BonusGenerator);

            events.Add(Db.Get().GameplayEvents.BonusDoor);

            events.Add(Db.Get().GameplayEvents.BonusHitTheBooks);

            events.Add(Db.Get().GameplayEvents.BonusLitWorkspace);

            events.Add(Db.Get().GameplayEvents.BonusTalker);
        }
    }
}
