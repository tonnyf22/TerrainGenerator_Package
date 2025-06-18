using System.Collections.Generic;
using TerrainGenerator.Components.Settings.Biomes;
using TerrainGenerator.Components.Settings.Chunks;
using TerrainGenerator.Generation.Appearance;
using TerrainGenerator.Generation.Scattering;
using TerrainGenerator.Generation.Structure;
using TerrainGenerator.Generation.Surface;
using TerrainGenerator.Generation.Water;
using UnityEngine;

namespace TerrainGenerator.Generation.Management
{
    public class BiomePreviewerChunksGenerator
    {
        public static BiomePreviewerChunksGenerator CreateBiomePreviewerChunksGenerator(string seed, int chunkLODIndexToPreview, ChunksSettings chunksSettings, int biomeSystemSettingsIndexToPreview, BiomesSystemSettings biomesSystemSettings, GameObject generationParentGameObject)
        {
            return new BiomePreviewerChunksGenerator(
                seed,
                chunkLODIndexToPreview,
                chunksSettings,
                biomeSystemSettingsIndexToPreview,
                biomesSystemSettings,
                generationParentGameObject
            );
        }

        public readonly string seed;
        public readonly int chunkLODIndexToPreview;
        public readonly ChunksSettings chunksSettings;
        public readonly int biomeSystemSettingsIndexToPreview;
        public readonly BiomesSystemSettings biomesSystemSettings;
        public readonly GameObject generationParentGameObject;

        public readonly GeneratorSettingsInterpreter generatorSettingsInterpreter;
        public readonly ScatteringObjectsPool scatteringObjectsPool;

        // debug
        public Material materialSurface;
        public Material materialWater;

        private Dictionary<ChunkCoordinates, Chunk> chunks = new Dictionary<ChunkCoordinates, Chunk>();

        public BiomePreviewerChunksGenerator(string seed, int chunkLODIndexToPreview, ChunksSettings chunksSettings, int biomeSystemSettingsIndexToPreview, BiomesSystemSettings biomesSystemSettings, GameObject generationParentGameObject)
        {
            this.seed = seed;
            this.chunkLODIndexToPreview = chunkLODIndexToPreview;
            this.chunksSettings = chunksSettings;
            this.biomeSystemSettingsIndexToPreview = biomeSystemSettingsIndexToPreview;
            this.biomesSystemSettings = biomesSystemSettings;
            this.generationParentGameObject = generationParentGameObject;
            generatorSettingsInterpreter = GeneratorSettingsInterpreter.CreateGeneratorSettingsInterpreter(
                seed,
                chunksSettings,
                biomesSystemSettings
            );
            scatteringObjectsPool = ScatteringObjectsPool.CreateScatteringObjectsPool(
                biomesSystemSettings.biomesSettings,
                generationParentGameObject
            );
        }

        public void GenerateChunks(int chunksPerQuadrantDimension)
        {
            for (int indexX = 0; indexX < chunksPerQuadrantDimension; indexX++)
            {
                for (int indexZ = 0; indexZ < chunksPerQuadrantDimension; indexZ++)
                {
                    for (int quadrantIndex = 1; quadrantIndex <= 4; quadrantIndex++)
                    {
                        ChunkCoordinates chunkCoordinates = CalculateChunkCoordinatesForQuadrant(
                            quadrantIndex,
                            indexX,
                            indexZ);

                        if (!chunks.ContainsKey(chunkCoordinates))
                        {
                            Chunk chunk = CreateChunkInChunkCoordinates(chunkCoordinates);
                            
                            CreateChunkDetalizationLevel(chunk, chunkLODIndexToPreview);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        private Chunk CreateChunkInChunkCoordinates(ChunkCoordinates chunkCoordinates)
        {
            // create
            Chunk chunk = Chunk.CreateChunk(
                chunksSettings.chunkSize,
                chunkCoordinates,
                chunksSettings.chunkLODSettings.Length,
                generationParentGameObject
            );

            // setup
            SurfaceMeshGenerator surfaceMeshGenerator = SurfaceMeshGenerator.CreateSurfaceMeshGenerator(
                chunk
            );
            chunk.AddSurfaceMeshGenerator(surfaceMeshGenerator);

            BiomePreviewerSurfaceDisplacementGenerator surfaceDisplacementGenerator = BiomePreviewerSurfaceDisplacementGenerator.CreateDisplacementGenerator(
                chunk,
                biomeSystemSettingsIndexToPreview,
                generatorSettingsInterpreter.CreateBiomeGraphInterpreter()  // TODO: make an object pool for BiomeGraphInterpreter objects
            );
            chunk.AddSurfaceDisplacementGenerator(surfaceDisplacementGenerator);

            BiomePreviewerTextureBlendingGenerator biomePreviewerTextureBlendingGenerator = BiomePreviewerTextureBlendingGenerator.CreateTextureBlendingGenerator(
                chunk,
                biomeSystemSettingsIndexToPreview
            );
            chunk.AddTextureBlendingGenerator(biomePreviewerTextureBlendingGenerator);

            WaterMeshGenerator waterMeshGenerator = WaterMeshGenerator.CreateWaterMeshGenerator(
                chunk
            );
            chunk.AddWaterMeshGenerator(waterMeshGenerator);

            BiomePreviewerScatteringModifiedPointsGenerator scatteringObjectsGenerator = BiomePreviewerScatteringModifiedPointsGenerator.CreateScatteringObjectsGenerator(
                chunk,
                seed,
                biomeSystemSettingsIndexToPreview,
                generatorSettingsInterpreter.GetBiomesScatteringSettings(),
                (chunk.surfaceDisplacementGenerator as BiomePreviewerSurfaceDisplacementGenerator).biomeGraphInterpreter
            );
            chunk.AddScatteringObjectsGenerator(scatteringObjectsGenerator);

            // store
            chunks.Add(chunkCoordinates, chunk);

            return chunk;
        }

        private ChunkCoordinates CalculateChunkCoordinatesForQuadrant(int quadrantIndex, int indexX, int indexZ)
        {
            int coeffX = 1;
            int coeffZ = 1;

            switch (quadrantIndex)
            {
                case 2:
                    coeffX *= -1;
                    break;
                case 3:
                    coeffX *= -1;
                    coeffZ *= -1;
                    break;
                case 4:
                    coeffZ *= -1;
                    break;
            }

            return new ChunkCoordinates(
                indexX * coeffX,
                indexZ * coeffZ
            );
        }

        private void CreateChunkDetalizationLevel(Chunk chunk, int indexLOD)
        {
            // setup
            DetalizationLevel detalizationLevel = DetalizationLevel.CreateDetalizationLevel(
                indexLOD,
                generatorSettingsInterpreter.GetChunkLODSettings(indexLOD).meshFillType,
                generatorSettingsInterpreter.GetChunkLODSettings(indexLOD).meshResolution,
                chunk.chunkGameObject
            );
            chunk.AddDetalizationLevel(indexLOD, detalizationLevel);

            ObjectsScattering objectsScattering = ObjectsScattering.CreateScattering(
                generatorSettingsInterpreter.GetChunkLODSettings(indexLOD).isApplyScatteringSparsing,
                generatorSettingsInterpreter.GetChunkLODSettings(indexLOD).scatteringSparseLevel,
                detalizationLevel.detalizationLevelGameObject
            );
            objectsScattering.ApplyScatteringObjectsPool(scatteringObjectsPool);
            detalizationLevel.AddScattering(objectsScattering);

            WaterCovering waterCovering = WaterCovering.CreateWaterCovering(
                generatorSettingsInterpreter.GetChunkLODSettings(indexLOD).waterCoveringMeshFillType,
                generatorSettingsInterpreter.GetChunkLODSettings(indexLOD).waterCoveringMeshResolution,
                detalizationLevel.detalizationLevelGameObject
            );
            detalizationLevel.AddWaterCovering(waterCovering);

            // generation
            Mesh surfaceMesh = chunk.surfaceMeshGenerator.CreateMesh(detalizationLevel);
            chunk.surfaceDisplacementGenerator.ApplyDisplacementToMesh(surfaceMesh);
            detalizationLevel.ApplySurfaceMesh(surfaceMesh, materialSurface);
            if (generatorSettingsInterpreter.GetChunkLODSettings(indexLOD).isApplyCollision)
            {
                detalizationLevel.ApplySurfaceCollision(surfaceMesh);
            }
            MeshNormalsGenerator.RecalculateMeshNormals(surfaceMesh);

            chunk.textureBlendingGenerator.ApplyTextureBlendingMetadataToMesh(surfaceMesh);

            if (generatorSettingsInterpreter.GetChunkLODSettings(indexLOD).isApplyScattering)
            {
                Dictionary<BiomeScattering, List<ScatteringObjectParameters>> biomeScatteringObjectsParameters = 
                    chunk.scatteringObjectsGenerator.CreateBiomeScatteringObjectsParameters(detalizationLevel);
                objectsScattering.SaveBiomeScatteringObjectsParameters(biomeScatteringObjectsParameters);
                objectsScattering.SetupScattering();
            }

            if (generatorSettingsInterpreter.GetChunkLODSettings(indexLOD).isApplyWaterCovering)
            {
                Mesh waterMesh = chunk.waterMeshGenerator.CreateMesh(
                    detalizationLevel,
                    waterCovering
                );
                waterCovering.ApplyWaterCoveringMesh(waterMesh, materialWater);
            }

            RecalculateChunkDetalizationLevelNormals(chunk, indexLOD);

            // hide extra
            chunk.HideDetalizationLevelsExcept(indexLOD);
        }

        private void RecalculateChunkDetalizationLevelNormals(Chunk chunk, int indexLOD)
        {
            for (int xOffset = -1; xOffset < 2; xOffset++)
            {
                for (int zOffset = -1; zOffset < 2; zOffset++)
                {
                    if (xOffset != 0 || zOffset != 0)
                    {
                        ChunkCoordinates chunkCoordinatesNext = new ChunkCoordinates(
                            chunk.chunkCoordinates.x + xOffset,
                            chunk.chunkCoordinates.z + zOffset);
                        if (chunks.ContainsKey(chunkCoordinatesNext))
                        {
                            if (chunks[chunkCoordinatesNext].IsDetalizationLevelExists(indexLOD))
                            {
                                MeshNormalsGenerator.RecalculateMeshEdgeNormals(
                                    indexLOD,
                                    chunk,
                                    chunks[chunkCoordinatesNext]
                                );
                            }
                        }
                    }
                }
            }
        }
    }
}