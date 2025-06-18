using UnityEngine;

namespace TerrainGenerator.Components.Settings.Biomes.Scattering
{
    [CreateAssetMenu(fileName = "BiomeGridBasedScatteringSettings", menuName = "Terrain Generator/Biomes/Scattering/Biome Grid Based Scattering Settings")]
    public class BiomeGridBasedScatteringSettings : BiomeScatteringSettings
    {
        [Space(20)]
        [Min(0.0f)]
        public float randomStep;
        public bool isApplyStepRange;
        [Min(0.0f)]
        public float randomStepMin;

        public BiomeGridBasedScatteringSettings()
        {
            scatteringType = ScatteringType.GridBased;
        }
    }
}