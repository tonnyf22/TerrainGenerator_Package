using System;
using System.Collections.Generic;
using TerrainGenerator.Generation.Structure;
using UnityEngine;

namespace TerrainGenerator.Generation.Surface
{
    public static class MeshNormalsGenerator
    {
        public static void RecalculateMeshNormals(Mesh mesh)
        {
            mesh.RecalculateNormals();
        }

        public static void RecalculateMeshEdgeNormals(int detalizationLevelIndex, Chunk chunk, Chunk chunkNext)
        {
            int xDiff = chunkNext.chunkCoordinates.x - chunk.chunkCoordinates.x;
            int zDiff = chunkNext.chunkCoordinates.z - chunk.chunkCoordinates.z;

            Mesh mesh = chunk.GetDetalizationLevel(detalizationLevelIndex).GetSurfaceMesh();
            Mesh meshNext = chunkNext.GetDetalizationLevel(detalizationLevelIndex).GetSurfaceMesh();
            int meshResolution = chunk.GetDetalizationLevel(detalizationLevelIndex).meshResolution;

            List<Vector3> normals1 = new List<Vector3>();
            mesh.GetNormals(normals1);
            List<Vector3> normals2 = new List<Vector3>();
            meshNext.GetNormals(normals2);

            if (xDiff == -1 && zDiff == -1)
            {
                RecalculateNormalsSW(normals1, normals2, meshResolution);
            }
            else if (xDiff == -1 && zDiff == 0)
            {
                RecalculateNormalsW(normals1, normals2, meshResolution);
            }
            else if (xDiff == -1 && zDiff == 1)
            {
                RecalculateNormalsNW(normals1, normals2, meshResolution);
            }
            else if (xDiff == 0 && zDiff == -1)
            {
                RecalculateNormalsS(normals1, normals2, meshResolution);
            }
            else if (xDiff == 0 && zDiff == 1)
            {
                RecalculateNormalsN(normals1, normals2, meshResolution);
            }
            else if (xDiff == 1 && zDiff == -1)
            {
                RecalculateNormalsSE(normals1, normals2, meshResolution);
            }
            else if (xDiff == 1 && zDiff == 0)
            {
                RecalculateNormalsE(normals1, normals2, meshResolution);
            }
            else if (xDiff == 1 && zDiff == 1)
            {
                RecalculateNormalsNE(normals1, normals2, meshResolution);
            }
            else
            {
                throw new ArgumentException("Can't recalculate edge mesh normals for the same two chunks");
            }

            mesh.SetNormals(normals1);
            meshNext.SetNormals(normals2);
        }

        private static void RecalculateNormalsNE(List<Vector3> normals1, List<Vector3> normals2, int meshResolution)
        {
            int vertexIndex1 = meshResolution * meshResolution - 1;
            int vertexIndex2 = 0;

            CalculateNormalsPair(
                normals1,
                vertexIndex1,
                normals2,
                vertexIndex2);
        }

        private static void RecalculateNormalsSW(List<Vector3> normals1, List<Vector3> normals2, int meshResolution)
        {
            int vertexIndex1 = 0;
            int vertexIndex2 = meshResolution * meshResolution - 1;

            CalculateNormalsPair(
                normals1,
                vertexIndex1,
                normals2,
                vertexIndex2);
        }

        private static void RecalculateNormalsNW(List<Vector3> normals1, List<Vector3> normals2, int meshResolution)
        {
            int vertexIndex1 = meshResolution - 1;
            int vertexIndex2 = meshResolution * meshResolution - meshResolution;

            CalculateNormalsPair(
                normals1,
                vertexIndex1,
                normals2,
                vertexIndex2);
        }

        private static void RecalculateNormalsSE(List<Vector3> normals1, List<Vector3> normals2, int meshResolution)
        {
            int vertexIndex1 = meshResolution * meshResolution - meshResolution;
            int vertexIndex2 = meshResolution - 1;

            CalculateNormalsPair(
                normals1,
                vertexIndex1,
                normals2,
                vertexIndex2);
        }

        private static void RecalculateNormalsN(List<Vector3> normals1, List<Vector3> normals2, int meshResolution)
        {
            for (int index = 0; index < meshResolution; index++)
            {
                int vertexIndex1 = meshResolution * (index + 1) - 1;
                int vertexIndex2 = meshResolution * index;

                CalculateNormalsPair(
                    normals1,
                    vertexIndex1,
                    normals2,
                    vertexIndex2);
            }
        }

        private static void RecalculateNormalsS(List<Vector3> normals1, List<Vector3> normals2, int meshResolution)
        {
            for (int index = 0; index < meshResolution; index++)
            {
                int vertexIndex1 = meshResolution * index;
                int vertexIndex2 = meshResolution * (index + 1) - 1;

                CalculateNormalsPair(
                    normals1,
                    vertexIndex1,
                    normals2,
                    vertexIndex2);
            }
        }

        private static void RecalculateNormalsW(List<Vector3> normals1, List<Vector3> normals2, int meshResolution)
        {
            for (int index = 0; index < meshResolution; index++)
            {
                int vertexIndex1 = index;
                int vertexIndex2 = meshResolution * (meshResolution - 1) + index;

                CalculateNormalsPair(
                    normals1,
                    vertexIndex1,
                    normals2,
                    vertexIndex2);
            }
        }

        private static void RecalculateNormalsE(List<Vector3> normals1, List<Vector3> normals2, int meshResolution)
        {
            for (int index = 0; index < meshResolution; index++)
            {
                int vertexIndex1 = meshResolution * (meshResolution - 1) + index;
                int vertexIndex2 = index;

                CalculateNormalsPair(
                    normals1,
                    vertexIndex1,
                    normals2,
                    vertexIndex2);
            }
        }

        private static void CalculateNormalsPair(List<Vector3> normals1, int vertexIndex1, List<Vector3> normals2, int vertexIndex2)
        {
            Vector3 normal1 = normals1[vertexIndex1];
            Vector3 normal2 = normals2[vertexIndex2];

            Vector3 normal = (normal1 + normal2).normalized;

            normals1[vertexIndex1] = normal;
            normals2[vertexIndex2] = normal;
        }
    }
}