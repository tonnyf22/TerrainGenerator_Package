using NaughtyAttributes;
using UnityEngine;

namespace TerrainGenerator.Components.Settings.Biomes
{
    [CreateAssetMenu(fileName = "BiomesSystemSettings", menuName = "Terrain Generator/Biomes/Biomes System Settings")]
    public class BiomesSystemSettings : ScriptableObject
    {
        public float biomeGridCellSize;
        public float biomeSubgridCellSize;
        [Space(20)]
        [Range(0.05f, 1.0f)]
        public float displacementInfluenceLevel = 1.0f;
        [Min(1)]
        public int displacementInterblendLevel = 1;
        [Space(20)]
        [Range(0.05f, 1.0f)]
        public float scatteringInfluenceLevel = 1.0f;
        [Space(20)]
        [Expandable]
        public BiomeSettings[] biomesSettings;
    }
}