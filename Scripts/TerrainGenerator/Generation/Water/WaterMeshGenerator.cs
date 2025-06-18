using System;
using System.Collections.Generic;
using TerrainGenerator.Components.Settings.Chunks;
using TerrainGenerator.Generation.Structure;
using UnityEngine;

namespace TerrainGenerator.Generation.Water
{
    public class WaterMeshGenerator
    {
        public static WaterMeshGenerator CreateWaterMeshGenerator(Chunk chunk)
        {
            return new WaterMeshGenerator(chunk);
        }

        public readonly Chunk chunk;

        public WaterMeshGenerator(Chunk chunk)
        {
            this.chunk = chunk;
        }

        public Mesh CreateMesh(DetalizationLevel detalizationLevel, WaterCovering waterCovering)
        {
            Mesh mesh = new Mesh();

            GenerateVertices(mesh, waterCovering.waterCoveringMeshResolution);
            GenerateTriangles(mesh, waterCovering.waterCoveringMeshFillType, waterCovering.waterCoveringMeshResolution, detalizationLevel.detalizationLevelGameObject.layer);

            return mesh;
        }

        private void GenerateVertices(Mesh mesh, int waterCoveringMeshResolution)
        {
            List<Vector3> vertices = new List<Vector3>();
            float verticesGapSize = chunk.chunkSize / (waterCoveringMeshResolution - 1);

            for (int xIndex = 0; xIndex < waterCoveringMeshResolution; xIndex++)
            {
                for (int zIndex = 0; zIndex < waterCoveringMeshResolution; zIndex++)
                {
                    float xCoordinate = xIndex * verticesGapSize;
                    float zCoordinate = zIndex * verticesGapSize;

                    Vector3 vertex = new Vector3(xCoordinate, 0.0f, zCoordinate);

                    vertices.Add(vertex);
                }
            }

            mesh.SetVertices(vertices);
        }

        private void GenerateTriangles(Mesh mesh, MeshFillType waterCoveringMeshFillType, int waterCoveringMeshResolution, int detalizationLevelGameObjectLayerIndex)
        {
            switch (waterCoveringMeshFillType)
            {
                case MeshFillType.QuadGrid:
                    GenerateTrianglesQuadGridMesh(mesh, waterCoveringMeshResolution, detalizationLevelGameObjectLayerIndex);
                    break;
                case MeshFillType.QuadGridStars:
                    GenerateTrianglesQuadGridStarsMesh(mesh, waterCoveringMeshResolution, detalizationLevelGameObjectLayerIndex);
                    break;
                case MeshFillType.QuadGridZigzag:
                    GenerateTrianglesQuadGridZigzagMesh(mesh, waterCoveringMeshResolution, detalizationLevelGameObjectLayerIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(waterCoveringMeshFillType),
                        waterCoveringMeshFillType,
                        "Unsupported MeshFillType.");
            }
        }

        private void GenerateTrianglesQuadGridMesh(Mesh mesh, int waterCoveringMeshResolution, int detalizationLevelGameObjectLayerIndex)
        {
            List<int> triangles = new List<int>();

            for (int xIndex = 0; xIndex < waterCoveringMeshResolution - 1; xIndex++)
            {
                for (int zIndex = 0; zIndex < waterCoveringMeshResolution - 1; zIndex++)
                {
                    int v1 = xIndex * waterCoveringMeshResolution + zIndex;
                    int v2 = xIndex * waterCoveringMeshResolution + zIndex + 1;
                    int v3 = (xIndex + 1) * waterCoveringMeshResolution + zIndex;
                    int v4 = (xIndex + 1) * waterCoveringMeshResolution + zIndex + 1;

                    if (IsMeshCellCoverUnderwaterSurface(v1, v2, v3, v4, mesh, detalizationLevelGameObjectLayerIndex))
                    {
                        triangles.AddRange(
                            new int[]{
                                v1, v2, v4,
                                v4, v3, v1
                            });
                    }
                }
            }

            mesh.SetTriangles(triangles, 0, false);
        }

        private void GenerateTrianglesQuadGridStarsMesh(Mesh mesh, int waterCoveringMeshResolution, int detalizationLevelGameObjectLayerIndex)
        {
            List<int> triangles = new List<int>();

            for (int xIndex = 0; xIndex < waterCoveringMeshResolution - 1; xIndex++)
            {
                for (int zIndex = 0; zIndex < waterCoveringMeshResolution - 1; zIndex++)
                {
                    int v1 = xIndex * waterCoveringMeshResolution + zIndex;
                    int v2 = xIndex * waterCoveringMeshResolution + zIndex + 1;
                    int v3 = (xIndex + 1) * waterCoveringMeshResolution + zIndex;
                    int v4 = (xIndex + 1) * waterCoveringMeshResolution + zIndex + 1;

                    if (IsMeshCellCoverUnderwaterSurface(v1, v2, v3, v4, mesh, detalizationLevelGameObjectLayerIndex))
                    {
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
                }
            }

            mesh.SetTriangles(triangles, 0, false);
        }

        private void GenerateTrianglesQuadGridZigzagMesh(Mesh mesh, int waterCoveringMeshResolution, int detalizationLevelGameObjectLayerIndex)
        {
            List<int> triangles = new List<int>();

            for (int xIndex = 0; xIndex < waterCoveringMeshResolution - 1; xIndex++)
            {
                for (int zIndex = 0; zIndex < waterCoveringMeshResolution - 1; zIndex++)
                {
                    int v1 = xIndex * waterCoveringMeshResolution + zIndex;
                    int v2 = xIndex * waterCoveringMeshResolution + zIndex + 1;
                    int v3 = (xIndex + 1) * waterCoveringMeshResolution + zIndex;
                    int v4 = (xIndex + 1) * waterCoveringMeshResolution + zIndex + 1;

                    if (IsMeshCellCoverUnderwaterSurface(v1, v2, v3, v4, mesh, detalizationLevelGameObjectLayerIndex))
                    {
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

            mesh.SetTriangles(triangles, 0, false);
        }

        private bool IsMeshCellCoverUnderwaterSurface(int v1, int v2, int v3, int v4, Mesh mesh, int detalizationLevelGameObjectLayerIndex)
        {
            Vector3 v1GlobalPoint = Chunk.LocatePointInChunkAsPoint(mesh.vertices[v1].x, mesh.vertices[v1].z, chunk.chunkCoordinates, chunk.chunkSize);
            Vector3 v2GlobalPoint = Chunk.LocatePointInChunkAsPoint(mesh.vertices[v2].x, mesh.vertices[v2].z, chunk.chunkCoordinates, chunk.chunkSize);
            Vector3 v3GlobalPoint = Chunk.LocatePointInChunkAsPoint(mesh.vertices[v3].x, mesh.vertices[v3].z, chunk.chunkCoordinates, chunk.chunkSize);
            Vector3 v4GlobalPoint = Chunk.LocatePointInChunkAsPoint(mesh.vertices[v4].x, mesh.vertices[v4].z, chunk.chunkCoordinates, chunk.chunkSize);

            bool v1IsHit = Physics.Raycast(v1GlobalPoint, Vector3.down/* , float.MaxValue, detalizationLevelGameObjectLayerIndex*/);
            bool v2IsHit = Physics.Raycast(v2GlobalPoint, Vector3.down/* , float.MaxValue, detalizationLevelGameObjectLayerIndex*/);
            bool v3IsHit = Physics.Raycast(v3GlobalPoint, Vector3.down/* , float.MaxValue, detalizationLevelGameObjectLayerIndex*/);
            bool v4IsHit = Physics.Raycast(v4GlobalPoint, Vector3.down/* , float.MaxValue, detalizationLevelGameObjectLayerIndex*/);

            if (v1IsHit || v2IsHit || v3IsHit || v4IsHit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}