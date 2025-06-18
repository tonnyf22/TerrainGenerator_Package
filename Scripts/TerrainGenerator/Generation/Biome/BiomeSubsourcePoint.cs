namespace TerrainGenerator.Generation.Biome
{
    public struct BiomeSubsourcePoint
    {
        public readonly int biomeIndex;
        public readonly float x;
        public readonly float z;

        public BiomeSubsourcePoint(int biomeIndex, float x, float z)
        {
            this.biomeIndex = biomeIndex;
            this.x = x;
            this.z = z;
        }
    }
}