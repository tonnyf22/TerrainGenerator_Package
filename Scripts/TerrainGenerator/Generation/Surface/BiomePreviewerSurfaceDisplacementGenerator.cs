using TerrainGenerator.Generation.Biome;
using TerrainGenerator.Generation.Structure;
using UnityEngine;

namespace TerrainGenerator.Generation.Surface
{
    public class BiomePreviewerSurfaceDisplacementGenerator : ISurfaceDisplacementGenerator
    {
        public static BiomePreviewerSurfaceDisplacementGenerator CreateDisplacementGenerator(Chunk chunk, int biomeSystemSettingsIndexToPreview, BiomeGraphInterpreter biomeGraphInterpreter)
        {
            return new BiomePreviewerSurfaceDisplacementGenerator(
                chunk,
                biomeSystemSettingsIndexToPreview,
                biomeGraphInterpreter
            );
        }

        public readonly Chunk chunk;
        public readonly int biomeSystemSettingsIndexToPreview;
        public readonly BiomeGraphInterpreter biomeGraphInterpreter;

        public BiomePreviewerSurfaceDisplacementGenerator(Chunk chunk, int biomeSystemSettingsIndexToPreview, BiomeGraphInterpreter biomeGraphInterpreter)
        {
            this.chunk = chunk;
            this.biomeSystemSettingsIndexToPreview = biomeSystemSettingsIndexToPreview;
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

                float height = GetHeightFromBiomeInVertexGlobal(vertexGlobal);

                displacedVertices[vertexIndex] = new Vector3(
                    vertexLocal.x,
                    height,
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

        private float GetHeightFromBiomeInVertexGlobal(Vector3 vertexGlobal)
        {
            return biomeGraphInterpreter.GetBiomeHeight(
                biomeSystemSettingsIndexToPreview,
                vertexGlobal.x,
                vertexGlobal.z);
        }
    }
}
