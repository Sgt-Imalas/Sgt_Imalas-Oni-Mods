using Cheese.CheeseGerms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.MISC.STATUSITEMS;

namespace Cheese.ModElements
{
    [SkipSaveFileSerialization]
    internal class RandomTickManager : KMonoBehaviour, ISim200ms
    {
        const int CHUNK_EDGE = 8;

        static int CHUNK_SIZE = CHUNK_EDGE * CHUNK_EDGE;
        private static int widthInChunks;
        private static int heightInChunks;
        private static int chunkCount;
        private static ushort milkIdx;
        private static Element CheeseElement;
        private static byte cheeseMakingGermsIdx;

        private static int[] RandomCellAccess;
        private static int ReshuffleIndex = 0;


        static float MilkToCheeseRate = 1f/4f;
        static int GermsPerKGCheese = 100;
        static float MaxMassCreatedKG = 20;
        static int MinimumGermThreshold = 2000;
        static float MaxPercentageBacteriaUsed = 0.5f;


        public override void OnSpawn()
        {
            base.OnSpawn();

            SetChunks();
        }
        public void Sim200ms(float dt)
        {
            ReshuffleIndex++;
            if (ReshuffleIndex == CHUNK_SIZE)
            {
                Shuffle();
                ReshuffleIndex = 0;
                //SgtLogger.l("resetting and shuffling index");
            }

            UpdateCells();
        }
        public void UpdateCells()
        {
           // SgtLogger.l(RandomCellAccess[ReshuffleIndex]+" <- cell, index -> "+ ReshuffleIndex);
            Parallel.For(0, chunkCount, (chunk) => MakeCheeseIn(GetRandomCellInChunk(chunk)));
        }
        void MakeCheeseIn(int cell)
        {

            if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
                return;

            var element = Grid.Element[cell];
            var elementIdx = element.idx;
            float diseaseCount = Grid.DiseaseCount[cell];
            var diseaseIdx = Grid.DiseaseIdx[cell];

            //SgtLogger.l($"{elementIdx} == {milkIdx} && {diseaseIdx} == {cheeseMakingGermsIdx}");

            if(elementIdx == milkIdx && diseaseIdx == cheeseMakingGermsIdx && diseaseCount >= MinimumGermThreshold)
            {
                var maxMassToConsume = Mathf.Min(Grid.Mass[cell], MaxMassCreatedKG / MilkToCheeseRate);
                var germKgAvailable = (diseaseCount * MaxPercentageBacteriaUsed) / GermsPerKGCheese;
                maxMassToConsume = Mathf.Min(maxMassToConsume, germKgAvailable);
                var massToProduce = maxMassToConsume * MilkToCheeseRate;

                HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add((mass_cb_info, data) =>
                {
                    if ((double)mass_cb_info.mass <= 0.0)
                        return;
                    SpawnCheeseChunk(cell, massToProduce, mass_cb_info);

                }, (object)null, "Cheese made in world");

                SimMessages.ConsumeMass(cell, SimHashes.Milk, maxMassToConsume, (byte)1, handle.index);
            }

        }

        private void SpawnCheeseChunk(int cell, float mass, Sim.MassConsumedCallback callback)
        {
            //SgtLogger.l($"{mass} , {callback.temperature} , {callback.diseaseIdx}, {callback.diseaseCount}");
            CheeseElement.substance.SpawnResource(Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore), mass, callback.temperature, callback.diseaseIdx, Mathf.RoundToInt( callback.diseaseCount*MilkToCheeseRate));
            
        }


        private int GetRandomCellInChunk(int chunk)
        {
            ChunkOffset(chunk, out var x, out var y);

            int currentRandomIndex = RandomCellAccess[ReshuffleIndex] + chunk % (CHUNK_SIZE - 1);

            x += currentRandomIndex % CHUNK_EDGE;
            y += currentRandomIndex / CHUNK_EDGE;
            return Grid.XYToCell(x, y);
        }
        public static int XYToChunk(int x, int y)
        {
            return x + y * widthInChunks;
        }

        public static void ChunkToXY(int chunk, out int x, out int y)
        {
            x = chunk % widthInChunks;
            y = chunk / widthInChunks;
        }

        public static void ChunkOffset(int chunk, out int x, out int y)
        {
            x = chunk % widthInChunks * CHUNK_EDGE;
            y = chunk / widthInChunks * CHUNK_EDGE;
        }



        private static void SetChunks()
        {
            widthInChunks = Grid.WidthInCells / CHUNK_EDGE;
            heightInChunks = Grid.HeightInCells / CHUNK_EDGE;
            chunkCount = widthInChunks * heightInChunks;

            milkIdx = ElementLoader.GetElementIndex(SimHashes.Milk);
            cheeseMakingGermsIdx = Db.Get().Diseases.GetIndex(CheeseMakingBacteria.ID);
            CheeseElement = ElementLoader.GetElement(ModElementRegistration.Cheese.id);

            RandomCellAccess = Enumerable.Range(0, CHUNK_SIZE).ToArray();
            Shuffle();
        }
        public static void Shuffle()
        {
            System.Random rng = new System.Random();   // i.e., java.util.Random.
            int n = RandomCellAccess.Length;        // The number of items left to shuffle (loop invariant).
            while (n > 1)
            {
                int k = rng.Next(n);  // 0 <= k < n.
                n--;                     // n is now the last pertinent index;
                int temp = RandomCellAccess[n];     // swap array[n] with array[k] (does nothing if k == n).
                RandomCellAccess[n] = RandomCellAccess[k];
                RandomCellAccess[k] = temp;
            }
        }
    }
}
