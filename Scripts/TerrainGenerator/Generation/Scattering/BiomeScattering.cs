namespace TerrainGenerator.Generation.Scattering
{
    public struct BiomeScattering
    {
        public readonly int biomeIndex;
        public readonly int scatteringIndex;

        public BiomeScattering(int biomeIndex, int scatteringIndex)
        {
            this.biomeIndex = biomeIndex;
            this.scatteringIndex = scatteringIndex;
        }
    }
}