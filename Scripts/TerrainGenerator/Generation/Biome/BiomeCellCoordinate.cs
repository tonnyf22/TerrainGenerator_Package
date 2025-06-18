namespace TerrainGenerator.Generation.Biome
{
    public struct BiomeCellCoordinate
    {
        public readonly int x;
        public readonly int z;

        public BiomeCellCoordinate(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
    }
}