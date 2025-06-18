using System;

namespace TerrainGenerator.Generation
{
    public class DeterministicRandom
    {
        public readonly string seed;
        public readonly int seedInt;
        private readonly int[] multipliers = { 138, 217, 931 };

        public DeterministicRandom(string seed)
        {
            this.seed = seed;
            seedInt = TransformSeedStringToInt(seed);
        }

        private int TransformSeedStringToInt(string seed)
        {
            return seed.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

        // 0-1
        public float Value01(float x1)
        {
            float[] data = new float[] { x1 };
            float value = Value01(data);

            return value;
        }

        // 0-1
        public float Value01(float x1, float x2)
        {
            float[] data = new float[] { x1, x2 };
            float value = Value01(data);

            return value;
        }

        // 0-1
        public float Value01(float x1, float x2, float x3)
        {
            float[] data = new float[] { x1, x2, x3 };
            float value = Value01(data);

            return value;
        }

        private float Value01(float[] inputData)
        {
            int[] inputDataRawInts = FloatsToRawInts(inputData);
            int seedLocal = seedInt;
            for (int index = 0; index < inputDataRawInts.Length; index++)
            {
                seedLocal += inputDataRawInts[index] * multipliers[index];
            }
            Random random = new Random(seedLocal);

            float value = (float)random.NextDouble();

            return value;
        }

        private int[] FloatsToRawInts(float[] inputData)
        {
            int[] inputDataRawInts = new int[inputData.Length];

            for (int index = 0; index < inputDataRawInts.Length; index++)
            {
                inputDataRawInts[index] = BitConverter.SingleToInt32Bits(inputData[index]);
            }

            return inputDataRawInts;
        }
    }
}
