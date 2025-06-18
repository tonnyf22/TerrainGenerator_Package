using TerrainGenerator.Generation.Biome;
using TerrainGenerator.Generation.Structure;
using UnityEngine;

namespace TerrainGenerator.Generation.Appearance
{
    public class RegularTextureBlendingGenerator : ITextureBlendingGenerator
    {
        public static RegularTextureBlendingGenerator CreateTextureBlendingGenerator(Chunk chunk, BiomesDistribution biomesDistribution)
        {
            return new RegularTextureBlendingGenerator(
                chunk,
                biomesDistribution
            );
        }

        public readonly Chunk chunk;
        public readonly BiomesDistribution biomesDistribution;

        public RegularTextureBlendingGenerator(Chunk chunk, BiomesDistribution biomesDistribution)
        {
            this.chunk = chunk;
            this.biomesDistribution = biomesDistribution;
        }

        public void ApplyTextureBlendingMetadataToMesh(Mesh mesh)
        {
            Vector3[] meshVertices = mesh.vertices;
            Vector2[] meshUV2Meta = new Vector2[meshVertices.Length];

            for (int vertexIndex = 0; vertexIndex < meshVertices.Length; vertexIndex++)
            {
                Vector3 vertexLocal = meshVertices[vertexIndex];
                Vector3 vertexGlobal = CalculateGlobalCoordinatesOfVertexLocal(vertexLocal);

                int biomeIndex = GetBiomeIndexOfClosestSubsourcePoint(vertexGlobal);

                meshUV2Meta[vertexIndex] = new Vector2(
                    biomeIndex * 0.1f,
                    0.0f
                );
            }

            mesh.SetUVs(1, meshUV2Meta);
        }

        private Vector3 CalculateGlobalCoordinatesOfVertexLocal(Vector3 vertexLocal)
        {
            return Chunk.LocatePointInChunkAsPoint(
                vertexLocal.x,
                vertexLocal.z,
                chunk.chunkCoordinates,
                chunk.chunkSize);
        }

        private int GetBiomeIndexOfClosestSubsourcePoint(Vector3 vertexGlobal)
        {
            BiomeSubcellCoordinate biomeSubcellCoordinate = biomesDistribution.LocatePointInBiomeSubgridAsBiomeSubcellCoordinates(vertexGlobal.x, vertexGlobal.z);
            BiomeSubsourcePoint biomeSubsourcePoint = biomesDistribution.LocateBiomeSubsourcePointByBiomeSubcellCoordinates(biomeSubcellCoordinate);
            BiomeSubsourcePoint[] biomeSubsourcePoints = biomesDistribution.CalculateSurroundingBiomeSubsourcePoints(biomeSubcellCoordinate);

            bool areAllSurroundingBiomesSame = AreAllSurroundingBiomesSame(biomeSubsourcePoint, biomeSubsourcePoints);

            if (areAllSurroundingBiomesSame)
            {
                return biomeSubsourcePoint.biomeIndex;
            }
            else
            {
                float[] distances = CalculateDistancesToBiomeSubsourcePointsFromVertexGlobal(vertexGlobal, biomeSubsourcePoint, biomeSubsourcePoints);
                int minDistanceIndex = GetMinDistanceIndex(distances);
                int biomeIndex = GetBiomeIndexByMinDistanceIndex(minDistanceIndex, biomeSubsourcePoint, biomeSubsourcePoints);

                return biomeIndex;
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

            return distance;
        }

        private int GetMinDistanceIndex(float[] distances)
        {
            float minDistance = distances[0];
            int minDistanceIndex = 0;
            for (int index = 0; index < distances.Length; index++)
            {
                if (distances[index] < minDistance)
                {
                    minDistance = distances[index];
                    minDistanceIndex = index;
                }
            }
            return minDistanceIndex;
        }

        private int GetBiomeIndexByMinDistanceIndex(int minDistanceIndex, BiomeSubsourcePoint biomeSubsourcePoint, BiomeSubsourcePoint[] biomeSubsourcePoints)
        {
            if (minDistanceIndex == 0)
            {
                return biomeSubsourcePoint.biomeIndex;
            }
            else
            {
                return biomeSubsourcePoints[minDistanceIndex - 1].biomeIndex;
            }
        }
    }
}