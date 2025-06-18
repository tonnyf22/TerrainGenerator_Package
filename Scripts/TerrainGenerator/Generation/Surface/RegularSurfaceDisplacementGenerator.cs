using System.Collections.Generic;
using TerrainGenerator.Generation.Biome;
using TerrainGenerator.Generation.Structure;
using UnityEngine;

namespace TerrainGenerator.Generation.Surface
{
    public class RegularSurfaceDisplacementGenerator : ISurfaceDisplacementGenerator
    {
        public static RegularSurfaceDisplacementGenerator CreateDisplacementGenerator(Chunk chunk, int displacementInterblendLevel, float displacementInfluenceLevel, BiomesDistribution biomesDistribution, BiomeGraphInterpreter biomeGraphInterpreter)
        {
            return new RegularSurfaceDisplacementGenerator(
                chunk,
                displacementInterblendLevel,
                displacementInfluenceLevel,
                biomesDistribution,
                biomeGraphInterpreter
            );
        }

        public readonly Chunk chunk;
        public readonly int displacementInterblendLevel;
        public readonly float displacementInfluenceLevel;
        public readonly BiomesDistribution biomesDistribution;
        public readonly BiomeGraphInterpreter biomeGraphInterpreter;

        public RegularSurfaceDisplacementGenerator(Chunk chunk, int displacementInterblendLevel, float displacementInfluenceLevel, BiomesDistribution biomesDistribution, BiomeGraphInterpreter biomeGraphInterpreter)
        {
            this.chunk = chunk;
            this.displacementInterblendLevel = displacementInterblendLevel;
            this.displacementInfluenceLevel = displacementInfluenceLevel;
            this.biomesDistribution = biomesDistribution;
            this.biomeGraphInterpreter = biomeGraphInterpreter;
        }

        public void ApplyDisplacementToMesh(Mesh mesh)
        {
            Vector3[] meshVertices = mesh.vertices;
            Vector3[] displacedVertices = new Vector3[meshVertices.Length];

            for (int vertexIndex = 0; vertexIndex < meshVertices.Length; vertexIndex++)
            {
                Vector3 vertexLocal = meshVertices[vertexIndex];
                Vector3 vertexGlobal = CalculateGlobalCoordinatesOfVertexLocal(vertexLocal);

                float weightedHeight = CalculateWeightedHeightInPoint(vertexGlobal);

                displacedVertices[vertexIndex] = new Vector3(
                    vertexLocal.x,
                    weightedHeight,
                    vertexLocal.z
                );
            }

            mesh.SetVertices(displacedVertices);
        }

        private Vector3 CalculateGlobalCoordinatesOfVertexLocal(Vector3 vertexLocal)
        {
            return Chunk.LocatePointInChunkAsPoint(
                vertexLocal.x,
                vertexLocal.z,
                chunk.chunkCoordinates,
                chunk.chunkSize);
        }

        private float CalculateWeightedHeightInPoint(Vector3 vertexGlobal)
        {
            BiomeSubcellCoordinate biomeSubcellCoordinate = biomesDistribution.LocatePointInBiomeSubgridAsBiomeSubcellCoordinates(vertexGlobal.x, vertexGlobal.z);
            BiomeSubsourcePoint biomeSubsourcePoint = biomesDistribution.LocateBiomeSubsourcePointByBiomeSubcellCoordinates(biomeSubcellCoordinate);
            BiomeSubsourcePoint[] biomeSubsourcePoints = biomesDistribution.CalculateSurroundingBiomeSubsourcePoints(biomeSubcellCoordinate);

            bool areAllSurroundingBiomesSame = AreAllSurroundingBiomesSame(biomeSubsourcePoint, biomeSubsourcePoints);

            if (areAllSurroundingBiomesSame)
            {
                float weightedHeight = GetHeightFromBiomeInVertexGlobal(vertexGlobal, biomeSubsourcePoint);

                return weightedHeight;
            }
            else
            {
                float[] distances = CalculateDistancesToBiomeSubsourcePointsFromVertexGlobal(vertexGlobal, biomeSubsourcePoint, biomeSubsourcePoints);
                bool[] pointsFilter = CalculateSubsourcePointsFilter(distances);
                float[] weights = CalculateWeightsOfDistances(distances, pointsFilter);

                float[] heights = GetHeightsFromBiomesInVertexGlobal(vertexGlobal, biomeSubsourcePoint, biomeSubsourcePoints, pointsFilter);

                float weightedHeight = CalculateWeightedHeight(weights, heights);

                return weightedHeight;
            }
        }

        private bool AreAllSurroundingBiomesSame(BiomeSubsourcePoint biomeSubsourcePoint, BiomeSubsourcePoint[] biomeSubsourcePoints)
        {
            foreach (var biomeSubsourcePointItem in biomeSubsourcePoints)
            {
                if (biomeSubsourcePointItem.biomeIndex != biomeSubsourcePoint.biomeIndex)
                {
                    return false;
                }
            }

            return true;
        }

        private float GetHeightFromBiomeInVertexGlobal(Vector3 vertexGlobal, BiomeSubsourcePoint biomeSubsourcePoint)
        {
            return biomeGraphInterpreter.GetBiomeHeight(
                biomeSubsourcePoint.biomeIndex,
                vertexGlobal.x,
                vertexGlobal.z);
        }

        private float[] CalculateDistancesToBiomeSubsourcePointsFromVertexGlobal(Vector3 vertexGlobal, BiomeSubsourcePoint biomeSubsourcePoint, BiomeSubsourcePoint[] biomeSubsourcePoints)
        {
            float[] distances = new float[9];

            distances[0] = CalculateDistanceBetweenPoints(
                vertexGlobal.x,
                vertexGlobal.z,
                biomeSubsourcePoint.x,
                biomeSubsourcePoint.z);

            for (int index = 0; index < biomeSubsourcePoints.Length; index++)
            {
                distances[index + 1] = CalculateDistanceBetweenPoints(
                    vertexGlobal.x,
                    vertexGlobal.z,
                    biomeSubsourcePoints[index].x,
                    biomeSubsourcePoints[index].z);
            }

            return distances;
        }

        private float CalculateDistanceBetweenPoints(float fromX, float fromZ, float toX, float toZ)
        {
            float distance = Mathf.Sqrt(Mathf.Pow(fromX - toX, 2) + Mathf.Pow(fromZ - toZ, 2));

            if (distance < 1.0f)
            {
                return 1.0f;
            }

            return distance;
        }

        private bool[] CalculateSubsourcePointsFilter(float[] distances)
        {
            bool[] pointsFilter = new bool[9];
            pointsFilter[0] = true;

            float distanceLimit = biomesDistribution.biomeSubcellSize * 2.83f * displacementInfluenceLevel;
            for (int index = 1; index < pointsFilter.Length; index++)
            {
                if (distances[index] > distanceLimit)
                {
                    pointsFilter[index] = false;
                }
                else
                {
                    pointsFilter[index] = true;
                }
            }

            return pointsFilter;
        }

        private float[] CalculateWeightsOfDistances(float[] distances, bool[] pointsFilter)
        {
            List<float> weights = new List<float>();

            for (int index = 0; index < distances.Length; index++)
            {
                if (!pointsFilter[index])
                {
                    continue;
                }

                weights.Add(Weight(distances[index]));
            }

            return weights.ToArray();
        }

        private float Weight(float distance)
        {
            return 1.0f / Mathf.Pow(distance, displacementInterblendLevel);
        }

        private float[] GetHeightsFromBiomesInVertexGlobal(Vector3 vertexGlobal, BiomeSubsourcePoint biomeSubsourcePoint, BiomeSubsourcePoint[] biomeSubsourcePoints, bool[] pointsFilter)
        {
            List<float> heights = new List<float>();

            heights.Add(
                biomeGraphInterpreter.GetBiomeHeight(
                    biomeSubsourcePoint.biomeIndex,
                    vertexGlobal.x,
                    vertexGlobal.z));

            for (int index = 0; index < biomeSubsourcePoints.Length; index++)
            {
                if (!pointsFilter[index + 1])
                {
                    continue;
                }

                heights.Add(
                    biomeGraphInterpreter.GetBiomeHeight(
                        biomeSubsourcePoints[index].biomeIndex,
                        vertexGlobal.x,
                        vertexGlobal.z));
            }

            return heights.ToArray();
        }

        private float CalculateWeightedHeight(float[] weights, float[] heights)
        {
            float weightedHeight = 0.0f;

            float weightsSum = CalculateWeightsSum(weights);

            // E (weight / E weights_all * height_biome)

            for (int index = 0; index < weights.Length; index++)
            {
                weightedHeight += weights[index] / weightsSum * heights[index];
            }

            return weightedHeight;
        }

        private float CalculateWeightsSum(float[] weights)
        {
            float sum = 0.0f;

            foreach (var weight in weights)
            {
                sum += weight;
            }

            return sum;
        }
    }
}