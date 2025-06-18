namespace TerrainGenerator.Generation.Biome
{
    public struct BiomeSubcellCoordinate
    {
        public readonly int x;
        public readonly int z;

        public BiomeSubcellCoordinate(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
    }
}