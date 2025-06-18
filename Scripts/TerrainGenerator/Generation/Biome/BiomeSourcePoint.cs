namespace TerrainGenerator.Generation.Biome
{
    public struct BiomeSourcePoint
    {
        public readonly int biomeIndex;
        public readonly float x;
        public readonly float z;

        public BiomeSourcePoint(int biomeIndex, float x, float z)
        {
            this.biomeIndex = biomeIndex;
            this.x = x;
            this.z = z;
        }
    }
}