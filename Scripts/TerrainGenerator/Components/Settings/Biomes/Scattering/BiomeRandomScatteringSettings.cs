using UnityEngine;

namespace TerrainGenerator.Components.Settings.Biomes.Scattering
{
    [CreateAssetMenu(fileName = "BiomeRandomScatteringSettings", menuName = "Terrain Generator/Biomes/Scattering/Biome Random Scattering Settings")]
    public class BiomeRandomScatteringSettings : BiomeScatteringSettings {
        public BiomeRandomScatteringSettings()
        {
            scatteringType = ScatteringType.Random;
        }
    }
}