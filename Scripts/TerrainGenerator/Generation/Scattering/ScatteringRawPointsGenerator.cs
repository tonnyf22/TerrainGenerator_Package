using System;
using System.Collections.Generic;
using TerrainGenerator.Generation.Structure;
using TerrainGenerator.Components.Settings.Biomes.Scattering;
using UnityEngine;

namespace TerrainGenerator.Generation.Scattering
{
    public class ScatteringRawPointsGenerator
    {
        public static ScatteringRawPointsGenerator CreateScatteringPointsGenerator(Chunk chunk, string seed)
        {
            return new ScatteringRawPointsGenerator(
                chunk,
                seed);
        }

        public readonly Chunk chunk;
        public readonly string seed;
        private DeterministicRandom deterministicRandom;

        private int biomeIndex;
        private int scatteringIndex;

        public ScatteringRawPointsGenerator(Chunk chunk, string seed)
        {
            this.chunk = chunk;
            this.seed = seed;
            deterministicRandom = new DeterministicRandom(seed);
        }

        public List<Vector3> CreatePoints(int biomeIndex, int scatteringIndex, ObjectsScattering objectsScattering, BiomeScatteringSettings biomeScatteringSettings)
        {
            this.biomeIndex = biomeIndex;
            this.scatteringIndex = scatteringIndex;

            switch (biomeScatteringSettings.scatteringType)
            {
                case ScatteringType.Random:
                    return GeneratePointsRandom(
                        objectsScattering.isApplyScatteringSparseLevel,
                        objectsScattering.scatteringSparseLevel,
                        biomeScatteringSettings as BiomeRandomScatteringSettings);
                case ScatteringType.GridBased:
                    return GeneratePointsGridBased(
                        objectsScattering.isApplyScatteringSparseLevel,
                        objectsScattering.scatteringSparseLevel,
                        biomeScatteringSettings as BiomeGridBasedScatteringSettings);
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(biomeScatteringSettings.scatteringType),
                        biomeScatteringSettings.scatteringType,
                        "Unsupported ScatteringType.");
            }
        }

        private List<Vector3> GeneratePointsRandom(bool isApplyScatteringSparsing, int scatteringSparseLevel, BiomeRandomScatteringSettings biomeRandomScatteringSettings)
        {
            List<Vector3> points = new List<Vector3>();

            if (isApplyScatteringSparsing)
            {
                GeneratePointsRandomSparse(points, scatteringSparseLevel, biomeRandomScatteringSettings);
            }
            else
            {
                GeneratePointsRandomRegular(points, biomeRandomScatteringSettings);
            }

            return points;
        }

        private void GeneratePointsRandomSparse(List<Vector3> points, int scatteringSparseLevel, BiomeRandomScatteringSettings biomeRandomScatteringSettings)
        {
            int pointsNumber = biomeRandomScatteringSettings.targetResolution * biomeRandomScatteringSettings.targetResolution;
            int missedPoints = scatteringSparseLevel / 2;

            for (int index = 0; index < pointsNumber; index++)
            {
                if (missedPoints < scatteringSparseLevel)
                {
                    missedPoints++;
                }
                else
                {
                    missedPoints = 0;

                    CreateAndStorePointRandom(points, index);
                }
            }
        }

        private void GeneratePointsRandomRegular(List<Vector3> points, BiomeRandomScatteringSettings biomeRandomScatteringSettings)
        {
            int pointsNumber = biomeRandomScatteringSettings.targetResolution * biomeRandomScatteringSettings.targetResolution;

            for (int index = 0; index < pointsNumber; index++)
            {
                CreateAndStorePointRandom(points, index);
            }
        }

        private List<Vector3> GeneratePointsGridBased(bool isApplyScatteringSparsing, int scatteringSparseLevel, BiomeGridBasedScatteringSettings biomeGridBasedScatteringSettings)
        {
            List<Vector3> points = new List<Vector3>();

            if (isApplyScatteringSparsing)
            {
                GeneratePointsGridBasedSparse(points, scatteringSparseLevel, biomeGridBasedScatteringSettings);
            }
            else
            {
                GeneratePointsGridBasedRegular(points, biomeGridBasedScatteringSettings);
            }

            return points;
        }

        private void GeneratePointsGridBasedSparse(List<Vector3> points, int scatteringSparseLevel, BiomeGridBasedScatteringSettings biomeGridBasedScatteringSettings)
        {
            int missedPoints = scatteringSparseLevel / 2;

            for (int xIndex = 0; xIndex < biomeGridBasedScatteringSettings.targetResolution; xIndex++)
            {
                for (int zIndex = 0; zIndex < biomeGridBasedScatteringSettings.targetResolution; zIndex++)
                {
                    if (missedPoints < scatteringSparseLevel)
                    {
                        missedPoints++;
                    }
                    else
                    {
                        missedPoints = 0;

                        CreateAndStorePointGridBased(
                            points,
                            xIndex,
                            zIndex,
                            biomeGridBasedScatteringSettings.targetResolution,
                            biomeGridBasedScatteringSettings.randomStep,
                            biomeGridBasedScatteringSettings.isApplyStepRange,
                            biomeGridBasedScatteringSettings.randomStepMin);
                    }
                }
            }
        }

        private void GeneratePointsGridBasedRegular(List<Vector3> points, BiomeGridBasedScatteringSettings biomeGridBasedScatteringSettings)
        {
            for (int xIndex = 0; xIndex < biomeGridBasedScatteringSettings.targetResolution; xIndex++)
            {
                for (int zIndex = 0; zIndex < biomeGridBasedScatteringSettings.targetResolution; zIndex++)
                {
                    CreateAndStorePointGridBased(
                        points,
                        xIndex,
                        zIndex,
                        biomeGridBasedScatteringSettings.targetResolution,
                        biomeGridBasedScatteringSettings.randomStep,
                        biomeGridBasedScatteringSettings.isApplyStepRange,
                        biomeGridBasedScatteringSettings.randomStepMin);
                }
            }
        }

        private void CreateAndStorePointRandom(List<Vector3> points, int index)
        {
            float rawOffsetX = deterministicRandom.Value01(index + biomeIndex + scatteringIndex, index, chunk.chunkCoordinates.x * chunk.chunkCoordinates.z);
            float rawOffsetZ = deterministicRandom.Value01(chunk.chunkCoordinates.z * chunk.chunkCoordinates.x, index + biomeIndex + scatteringIndex, index);

            float x = chunk.chunkSize * rawOffsetX;
            float z = chunk.chunkSize * rawOffsetZ;

            Vector3 point = Chunk.LocatePointInChunkAsPoint(x, z, chunk.chunkCoordinates, chunk.chunkSize);

            if (Chunk.IsPointInChunk(point.x, point.z, chunk.chunkCoordinates, chunk.chunkSize))
            {
                points.Add(point);
            }
        }

        private void CreateAndStorePointGridBased(List<Vector3> points, int xIndex, int zIndex, int targetResolution, float randomStep, bool isApplyStepRange, float randomStepMin)
        {
            float verticesGapSize = chunk.chunkSize / (targetResolution - 1);

            float rawRotationStep = deterministicRandom.Value01(
                chunk.chunkCoordinates.x * xIndex,
                chunk.chunkCoordinates.z * zIndex,
                xIndex * zIndex);

            float step;
            if (isApplyStepRange)
            {
                float rawOffsetStep = deterministicRandom.Value01(
                    xIndex * zIndex,
                    chunk.chunkCoordinates.z * zIndex,
                    chunk.chunkCoordinates.x * xIndex);
                step =
                    randomStepMin +
                    rawOffsetStep *
                        (randomStep -
                        randomStepMin);
            }
            else
            {
                step = randomStep;
            }

            float offsetStepX = Mathf.Cos(2 * Mathf.PI * rawRotationStep) * step;
            float offsetStepZ = Mathf.Sin(2 * Mathf.PI * rawRotationStep) * step;

            float x = xIndex * verticesGapSize + offsetStepX;
            float z = zIndex * verticesGapSize + offsetStepZ;

            Vector3 point = Chunk.LocatePointInChunkAsPoint(x, z, chunk.chunkCoordinates, chunk.chunkSize);

            if (Chunk.IsPointInChunk(point.x, point.z, chunk.chunkCoordinates, chunk.chunkSize))
            {
                points.Add(point);
            }
        }
    }
}