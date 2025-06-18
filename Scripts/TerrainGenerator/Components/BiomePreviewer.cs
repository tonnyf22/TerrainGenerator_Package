using System;
using NaughtyAttributes;
using TerrainGenerator.Components.Settings.Biomes;
using TerrainGenerator.Components.Settings.Chunks;
using TerrainGenerator.Generation.Management;
using UnityEngine;

namespace TerrainGenerator.Components
{
    public class BiomePreviewer : MonoBehaviour
    {
        public string seed;
        [Min(1)]
        public int chunksPerQuadrantDimension = 2;

        [Range(0,10)]
        public int chunkLODIndexToPreview = 0;
        [Expandable]
        public ChunksSettings chunksSettings;
        [Range(0,10)]
        public int biomeSystemSettingsIndexToPreview = 0;
        [Expandable]
        public BiomesSystemSettings biomesSystemSettings;

        public Material materialSurface;
        public Material materialWater;

        private BiomePreviewerChunksGenerator biomePreviewerChunksGenerator;

        void Awake()
        {
            if (chunkLODIndexToPreview > chunksSettings.chunkLODSettings.Length - 1)
            {
                throw new ArgumentException("Chunk LOD index to preview is out of range.");
            }

            if (biomeSystemSettingsIndexToPreview > biomesSystemSettings.biomesSettings.Length - 1)
            {
                throw new ArgumentException("Biome system settings index to preview is out of range.");
            }

            biomePreviewerChunksGenerator = BiomePreviewerChunksGenerator.CreateBiomePreviewerChunksGenerator(
                seed,
                chunkLODIndexToPreview,
                chunksSettings,
                biomeSystemSettingsIndexToPreview,
                biomesSystemSettings,
                gameObject);
            
            // debug
            biomePreviewerChunksGenerator.materialSurface = materialSurface;
            biomePreviewerChunksGenerator.materialWater = materialWater;
        }

        void Start()
        {
            biomePreviewerChunksGenerator.GenerateChunks(chunksPerQuadrantDimension);
        }
    }
}