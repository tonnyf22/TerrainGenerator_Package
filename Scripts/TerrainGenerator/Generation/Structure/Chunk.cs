using System;
using System.Collections.Generic;
using TerrainGenerator.Generation.Appearance;
using TerrainGenerator.Generation.Scattering;
using TerrainGenerator.Generation.Surface;
using TerrainGenerator.Generation.Water;
using UnityEngine;

namespace TerrainGenerator.Generation.Structure
{
    public class Chunk
    {
        public static ChunkCoordinates LocatePointAsChunkCoordinates(float x, float z, float chunkSize)
        {
            return new ChunkCoordinates(
                Mathf.FloorToInt(x / chunkSize),
                Mathf.FloorToInt(z / chunkSize)
            );
        }

        public static Vector3 LocateChunkCoordinatesAsPoint(ChunkCoordinates chunkCoordinates, float chunkSize)
        {
            return new Vector3(
                chunkCoordinates.x * chunkSize,
                0.0f,
                chunkCoordinates.z * chunkSize
            );
        }

        public static Vector3 LocateCenterOfChunkAsPoint(ChunkCoordinates chunkCoordinates, float chunkSize)
        {
            return new Vector3(
                (chunkCoordinates.x + 0.5f) * chunkSize,
                0.0f,
                (chunkCoordinates.z + 0.5f) * chunkSize
            );
        }

        public static float LocateCenterOfChunkCoordinateAsPoint(int coordinate, float chunkSize)
        {
            return (coordinate + 0.5f) * chunkSize;
        }

        public static Vector3 LocatePointInChunkAsPoint(float x, float z, ChunkCoordinates chunkCoordinates, float chunkSize)
        {
            return new Vector3(
                chunkCoordinates.x * chunkSize + x,
                0.0f,
                chunkCoordinates.z * chunkSize + z
            );
        }

        public static Vector3 LocatePointInChunkAsPoint(float x, float y, float z, ChunkCoordinates chunkCoordinates, float chunkSize)
        {
            return new Vector3(
                chunkCoordinates.x * chunkSize + x,
                y,
                chunkCoordinates.z * chunkSize + z
            );
        }

        public static bool IsPointInChunk(float x, float z, ChunkCoordinates chunkCoordinates, float chunkSize)
        {
            float chunkBeginX = chunkSize * chunkCoordinates.x;
            float chunkEndX = chunkSize * (chunkCoordinates.x + 1);
            float chunkBeginZ = chunkSize * chunkCoordinates.z;
            float chunkEndZ = chunkSize * (chunkCoordinates.z + 1);

            if (x >= chunkBeginX && x <= chunkEndX && z >= chunkBeginZ && z <= chunkEndZ)
            {
                return true;
            }

            return false;
        }

        public static Chunk CreateChunk(float chunkSize, ChunkCoordinates chunkCoordinates, int numberOfDetalizationLevels, GameObject parentGameObject)
        {
            return new Chunk(
                chunkSize,
                chunkCoordinates,
                numberOfDetalizationLevels,
                parentGameObject);
        }

        public readonly float chunkSize;
        public readonly ChunkCoordinates chunkCoordinates;
        public readonly int numberOfDetalizationLevels;
        public readonly GameObject chunkGameObject;
        public readonly Dictionary<int, DetalizationLevel> detalizationLevels;
        public SurfaceMeshGenerator surfaceMeshGenerator { get; private set; }
        public ISurfaceDisplacementGenerator surfaceDisplacementGenerator { get; private set; }
        public ITextureBlendingGenerator textureBlendingGenerator { get; private set; }
        public WaterMeshGenerator waterMeshGenerator { get; private set; }
        public IScatteringModifiedPointsGenerator scatteringObjectsGenerator { get; private set; }

        public Chunk(float chunkSize, ChunkCoordinates chunkCoordinates, int numberOfDetalizationLevels, GameObject parentGameObject)
        {
            this.chunkSize = chunkSize;
            this.chunkCoordinates = chunkCoordinates;
            this.numberOfDetalizationLevels = numberOfDetalizationLevels;
            chunkGameObject = new GameObject("TerrainChunk");
            chunkGameObject.transform.parent = parentGameObject.transform;
            SetChunkGameObjectCoordinates();
            detalizationLevels = new Dictionary<int, DetalizationLevel>();
        }

        private void SetChunkGameObjectCoordinates()
        {
            chunkGameObject.transform.localPosition = new Vector3(
                chunkSize * chunkCoordinates.x,
                0.0f,
                chunkSize * chunkCoordinates.z
            );
        }

        public void AddSurfaceMeshGenerator(SurfaceMeshGenerator surfaceMeshGenerator)
        {
            if (this.surfaceMeshGenerator != null)
            {
                throw new ArgumentException($"Surface mesh generator already exists for this chunk.");
            }
            else
            {
                this.surfaceMeshGenerator = surfaceMeshGenerator;
            }
        }

        public void RemoveSurfaceMeshGenerator()
        {
            if (this.surfaceMeshGenerator == null)
            {
                throw new ArgumentException($"Surface mesh generator does not exist for this chunk.");
            }
            else
            {
                this.surfaceMeshGenerator = null;
            }
        }

        public void AddSurfaceDisplacementGenerator(ISurfaceDisplacementGenerator surfaceDisplacementGenerator)
        {
            if (this.surfaceDisplacementGenerator != null)
            {
                throw new ArgumentException($"Surface displacement generator already exists for this chunk.");
            }
            else
            {
                this.surfaceDisplacementGenerator = surfaceDisplacementGenerator;
            }
        }

        public void RemoveSurfaceDisplacementGenerator()
        {
            if (this.surfaceDisplacementGenerator == null)
            {
                throw new ArgumentException($"Surface displacement generator does not exist for this chunk.");
            }
            else
            {
                this.surfaceDisplacementGenerator = null;
            }
        }

        public void AddTextureBlendingGenerator(ITextureBlendingGenerator textureBlendingGenerator)
        {
            if (this.textureBlendingGenerator != null)
            {
                throw new ArgumentException($"Texture blending generator already exists for this chunk.");
            }
            else
            {
                this.textureBlendingGenerator = textureBlendingGenerator;
            }
        }

        public void RemoveTextureBlendingGenerator()
        {
            if (this.textureBlendingGenerator == null)
            {
                throw new ArgumentException($"Texture blending generator does not exist for this chunk.");
            }
            else
            {
                this.textureBlendingGenerator = null;
            }
        }

        public void AddWaterMeshGenerator(WaterMeshGenerator waterMeshGenerator)
        {
            if (this.waterMeshGenerator != null)
            {
                throw new ArgumentException($"Water mesh generator already exists for this chunk.");
            }
            else
            {
                this.waterMeshGenerator = waterMeshGenerator;
            }
        }

        public void RemoveWaterMeshGenerator()
        {
            if (this.waterMeshGenerator == null)
            {
                throw new ArgumentException($"Water mesh generator does not exist for this chunk.");
            }
            else
            {
                this.waterMeshGenerator = null;
            }
        }

        public void AddScatteringObjectsGenerator(IScatteringModifiedPointsGenerator scatteringObjectsGenerator)
        {
            if (this.scatteringObjectsGenerator != null)
            {
                throw new ArgumentException($"Scattering objects generator already exists for this chunk.");
            }
            else
            {
                this.scatteringObjectsGenerator = scatteringObjectsGenerator;
            }
        }

        public void RemoveScatteringObjectsGenerator()
        {
            if (this.scatteringObjectsGenerator == null)
            {
                throw new ArgumentException($"Scattering objects generator does not exist for this chunk.");
            }
            else
            {
                this.scatteringObjectsGenerator = null;
            }
        }

        public void Show()
        {
            if (!chunkGameObject.activeSelf)
            {
                chunkGameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (chunkGameObject.activeSelf)
            {
                chunkGameObject.SetActive(false);

                foreach (var pair in detalizationLevels)
                {
                    pair.Value.Hide();
                }
            }
        }

        public void AddDetalizationLevel(int levelIndex, DetalizationLevel detalizationLevel)
        {
            if (detalizationLevels.ContainsKey(levelIndex))
            {
                throw new ArgumentException($"Detalization level index {levelIndex} already exists for this chunk.");
            }
            else
            {
                detalizationLevels.Add(levelIndex, detalizationLevel);
            }
        }

        public DetalizationLevel GetDetalizationLevel(int levelIndex)
        {
            if (!detalizationLevels.ContainsKey(levelIndex))
            {
                throw new ArgumentException($"Detalization level index {levelIndex} does not exist for this chunk.");
            }
            else
            {
                return detalizationLevels[levelIndex];
            }
        }

        public bool IsDetalizationLevelExists(int levelIndex)
        {
            return detalizationLevels.ContainsKey(levelIndex);
        }

        public void RemoveDetalizationLevel(int levelIndex)
        {
            if (!detalizationLevels.ContainsKey(levelIndex))
            {
                throw new ArgumentException($"Detalization level index {levelIndex} does not exist for this chunk.");
            }
            else
            {
                detalizationLevels.Remove(levelIndex);
            }
        }

        public void ShowDetalizationLevel(int levelIndex)
        {
            if (!detalizationLevels.ContainsKey(levelIndex))
            {
                throw new ArgumentException($"Detalization level index {levelIndex} does not exist for this chunk.");
            }
            else
            {
                detalizationLevels[levelIndex].Show();
            }
        }

        public void HideDetalizationLevelsExcept(int levelIndex)
        {
            if (!detalizationLevels.ContainsKey(levelIndex))
            {
                throw new ArgumentException($"Detalization level index {levelIndex} does not exist for this chunk.");
            }
            else
            {
                for (int index = 0; index < numberOfDetalizationLevels; index++)
                {
                    if (!detalizationLevels.ContainsKey(index))
                    {
                        continue;
                    }
                    else if (index == levelIndex)
                    {
                        continue;
                    }
                    else
                    {
                        detalizationLevels[index].Hide();
                    }
                }
            }
        }

        public void HideDetalizationLevel(int levelIndex)
        {
            if (!detalizationLevels.ContainsKey(levelIndex))
            {
                throw new ArgumentException($"Detalization level index {levelIndex} does not exist for this chunk.");
            }
            else
            {
                detalizationLevels[levelIndex].Hide();
            }
        }
    }
}
