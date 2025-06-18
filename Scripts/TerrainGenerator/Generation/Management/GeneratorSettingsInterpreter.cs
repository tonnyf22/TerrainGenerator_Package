using System.Collections.Generic;
using TerrainGenerator.Components.Settings.Chunks;
using TerrainGenerator.Components.Settings.Biomes;
using TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph;
using TerrainGenerator.Components.Settings.Biomes.Scattering;
using TerrainGenerator.Generation.Biome;

namespace TerrainGenerator.Generation.Management
{
    public class GeneratorSettingsInterpreter
    {
        public static GeneratorSettingsInterpreter CreateGeneratorSettingsInterpreter(string seed, ChunksSettings chunksSettings, BiomesSystemSettings biomesSystemSettings)
        {
            return new GeneratorSettingsInterpreter(
                seed,
                chunksSettings,
                biomesSystemSettings
            );
        }

        public readonly string seed;
        public readonly ChunksSettings chunksSettings;
        public readonly BiomesSystemSettings biomesSystemSettings;

        private Dictionary<int, BiomeScatteringSettings[]> biomesScatteringSettings;

        public GeneratorSettingsInterpreter(string seed, ChunksSettings chunksSettings, BiomesSystemSettings biomesSystemSettings)
        {
            this.seed = seed;
            this.chunksSettings = chunksSettings;
            this.biomesSystemSettings = biomesSystemSettings;
            biomesScatteringSettings = CreateBiomesScatteringSettings();
        }

        private Dictionary<int, BiomeScatteringSettings[]> CreateBiomesScatteringSettings()
        {
            biomesScatteringSettings = new Dictionary<int, BiomeScatteringSettings[]>();

            for (int indexBiome = 0; indexBiome < biomesSystemSettings.biomesSettings.Length; indexBiome++)
            {
                BiomeScatteringSettings[] scatteringSettings = new BiomeScatteringSettings[biomesSystemSettings.biomesSettings[indexBiome].biomeScatteringSettings.Length];
                for (int indexScattering = 0; indexScattering < scatteringSettings.Length; indexScattering++)
                {
                    scatteringSettings[indexScattering] = biomesSystemSettings.biomesSettings[indexBiome].biomeScatteringSettings[indexScattering];
                }
                biomesScatteringSettings.Add(indexBiome, scatteringSettings);
            }

            return biomesScatteringSettings;
        }

        public BiomeGraphInterpreter CreateBiomeGraphInterpreter()
        {
            BiomeGraph[] biomeGraphs = CreateBiomeGraphs();
            return BiomeGraphInterpreter.CreateBiomeGraphInterpreter(biomeGraphs);
        }

        private BiomeGraph[] CreateBiomeGraphs()
        {
            BiomeGraph[] biomeGraphs = new BiomeGraph[biomesSystemSettings.biomesSettings.Length];
            int index = 0;
            foreach (BiomeSettings biomeSettings in biomesSystemSettings.biomesSettings)
            {
                biomeGraphs[index] = biomeSettings.biomeNodeGraph.Copy() as BiomeGraph;
                index++;
            }
            return biomeGraphs;
        }

        public BiomesDistribution CreateBiomeDistribution()
        {
            return BiomesDistribution.CreateBiomesDistribution(
                biomesSystemSettings.biomesSettings.Length,
                seed,
                biomesSystemSettings.biomeGridCellSize,
                biomesSystemSettings.biomeSubgridCellSize);
        }

        public Dictionary<int, BiomeScatteringSettings[]> GetBiomesScatteringSettings()
        {
            return biomesScatteringSettings;
        }

        public ChunkLODSettings GetChunkLODSettings(int indexLOD)
        {
            return chunksSettings.chunkLODSettings[indexLOD];
        }

        public BiomeSettings GetBiomeSettings(int biomeIndex)
        {
            return biomesSystemSettings.biomesSettings[biomeIndex];
        }
    }
}