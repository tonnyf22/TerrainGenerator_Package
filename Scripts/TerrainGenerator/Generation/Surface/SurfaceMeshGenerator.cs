using System;
using System.Collections.Generic;
using TerrainGenerator.Components.Settings.Chunks;
using TerrainGenerator.Generation.Structure;
using UnityEngine;

namespace TerrainGenerator.Generation.Surface
{
    public class SurfaceMeshGenerator
    {
        public static SurfaceMeshGenerator CreateSurfaceMeshGenerator(Chunk chunk)
        {
            return new SurfaceMeshGenerator(chunk);
        }

        public readonly Chunk chunk;

        public SurfaceMeshGenerator(Chunk chunk)
        {
            this.chunk = chunk;
        }

        public Mesh CreateMesh(DetalizationLevel detalizationLevel)
        {
            Mesh mesh = new Mesh();

            GenerateVertices(mesh, detalizationLevel.meshResolution);
            GenerateUVs(mesh, detalizationLevel.meshResolution);
            GenerateTriangles(mesh, detalizationLevel.meshFillType, detalizationLevel.meshResolution);

            return mesh;
        }

        private void GenerateVertices(Mesh mesh, int meshResolution)
        {
            List<Vector3> vertices = new List<Vector3>();
            float verticesGapSize = chunk.chunkSize / (meshResolution - 1);

            for (int xIndex = 0; xIndex < meshResolution; xIndex++)
            {
                for (int zIndex = 0; zIndex < meshResolution; zIndex++)
                {
                    CreateAndStoreVertex(xIndex, zIndex, verticesGapSize, vertices);
                }
            }

            mesh.SetVertices(vertices);
        }

        private void GenerateUVs(Mesh mesh, int meshResolution)
        {
            Vector2[] uv = new Vector2[meshResolution * meshResolution];

            for (int xIndex = 0; xIndex < meshResolution; xIndex++)
            {
                for (int zIndex = 0; zIndex < meshResolution; zIndex++)
                {
                    int index = zIndex + xIndex * meshResolution;

                    uv[index] = new Vector2(
                        zIndex * 1.0f / (meshResolution - 1),
                        xIndex * 1.0f / (meshResolution - 1));
                }
            }

            mesh.uv = uv;
        }

        private void GenerateTriangles(Mesh mesh, MeshFillType meshFillType, int meshResolution)
        {
            switch (meshFillType)
            {
                case MeshFillType.QuadGrid:
                    GenerateTrianglesQuadGridMesh(mesh, meshResolution);
                    break;
                case MeshFillType.QuadGridStars:
                    GenerateTrianglesQuadGridStarsMesh(mesh, meshResolution);
                    break;
                case MeshFillType.QuadGridZigzag:
                    GenerateTrianglesQuadGridZigzagMesh(mesh, meshResolution);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(meshFillType),
                        meshFillType,
                        "Unsupported MeshFillType.");
            }
        }

        private void GenerateTrianglesQuadGridMesh(Mesh mesh, int meshResolution)
        {
            List<int> triangles = new List<int>();

            for (int xIndex = 0; xIndex < meshResolution - 1; xIndex++)
            {
                for (int zIndex = 0; zIndex < meshResolution - 1; zIndex++)
                {
                    CreateAndStoreTriangleQuadGrid(xIndex, zIndex, meshResolution, triangles);
                }
            }

            mesh.SetTriangles(triangles, 0, false);
        }

        private void GenerateTrianglesQuadGridStarsMesh(Mesh mesh, int meshResolution)
        {
            List<int> triangles = new List<int>();

            for (int xIndex = 0; xIndex < meshResolution - 1; xIndex++)
            {
                for (int zIndex = 0; zIndex < meshResolution - 1; zIndex++)
                {
                    CreateAndStoreTriangleQuadGridStars(xIndex, zIndex, meshResolution, triangles);
                }
            }

            mesh.SetTriangles(triangles, 0, false);
        }

        private void GenerateTrianglesQuadGridZigzagMesh(Mesh mesh, int meshResolution)
        {
            List<int> triangles = new List<int>();

            for (int xIndex = 0; xIndex < meshResolution - 1; xIndex++)
            {
                for (int zIndex = 0; zIndex < meshResolution - 1; zIndex++)
                {
                    CreateAndStoreTriangleQuadGridZigzag(xIndex, zIndex, meshResolution, triangles);
                }
            }

            mesh.SetTriangles(triangles, 0, false);
        }

        private void CreateAndStoreVertex(int xIndex, int zIndex, float verticesGapSize, List<Vector3> verticesBlock)
        {
            float xCoordinate = xIndex * verticesGapSize;
            float zCoordinate = zIndex * verticesGapSize;

            Vector3 vertex = new Vector3(xCoordinate, 0.0f, zCoordinate);

            verticesBlock.Add(vertex);
        }

        private void CreateAndStoreTriangleQuadGrid(int xIndex, int zIndex, int meshResolution, List<int> triangles)
        {
            int v1 = xIndex * meshResolution + zIndex;
            int v2 = xIndex * meshResolution + zIndex + 1;
            int v3 = (xIndex + 1) * meshResolution + zIndex;
            int v4 = (xIndex + 1) * meshResolution + zIndex + 1;

            triangles.AddRange(
                new int[]{
                    v1, v2, v4,
                    v4, v3, v1
                });
        }

        private void CreateAndStoreTriangleQuadGridStars(int xIndex, int zIndex, int meshResolution, List<int> triangles)
        {
            int v1 = xIndex * meshResolution + zIndex;
            int v2 = xIndex * meshResolution + zIndex + 1;
            int v3 = (xIndex + 1) * meshResolution + zIndex;
            int v4 = (xIndex + 1) * meshResolution + zIndex + 1;

            if (xIndex % 2 == zIndex % 2)
            {
                triangles.AddRange(
                    new int[]{
                        v1, v2, v4,
                        v4, v3, v1
                    });
            }
            else // if (xIndex % 2 != zIndex % 2)
            {
                triangles.AddRange(
                    new int[]{
                        v3, v1, v2,
                        v2, v4, v3
                    });
            }
        }

        private void CreateAndStoreTriangleQuadGridZigzag(int xIndex, int zIndex, int meshResolution, List<int> triangles)
        {
            int v1 = xIndex * meshResolution + zIndex;
            int v2 = xIndex * meshResolution + zIndex + 1;
            int v3 = (xIndex + 1) * meshResolution + zIndex;
            int v4 = (xIndex + 1) * meshResolution + zIndex + 1;

            if (zIndex % 2 == 0)
            {
                triangles.AddRange(
                    new int[]{
                        v1, v2, v4,
                        v4, v3, v1
                    });
            }
            else // if (zIndex % 2 == 1)
            {
                triangles.AddRange(
                    new int[]{
                        v3, v1, v2,
                        v2, v4, v3
                    });
            }
        }
    }
}