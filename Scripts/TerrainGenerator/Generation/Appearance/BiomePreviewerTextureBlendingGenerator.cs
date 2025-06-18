using TerrainGenerator.Generation.Structure;
using UnityEngine;

namespace TerrainGenerator.Generation.Appearance
{
    public class BiomePreviewerTextureBlendingGenerator : ITextureBlendingGenerator
    {
        public static BiomePreviewerTextureBlendingGenerator CreateTextureBlendingGenerator(Chunk chunk, int biomeSystemSettingsIndexToPreview)
        {
            return new BiomePreviewerTextureBlendingGenerator(
                chunk,
                biomeSystemSettingsIndexToPreview);
        }

        public readonly Chunk chunk;
        public readonly int biomeSystemSettingsIndexToPreview;

        public BiomePreviewerTextureBlendingGenerator(Chunk chunk, int biomeSystemSettingsIndexToPreview)
        {
            this.chunk = chunk;
            this.biomeSystemSettingsIndexToPreview = biomeSystemSettingsIndexToPreview;
        }

        public void ApplyTextureBlendingMetadataToMesh(Mesh mesh)
        {
            Vector3[] meshVertices = mesh.vertices;
            Vector2[] meshUV2Meta = new Vector2[meshVertices.Length];

            for (int vertexIndex = 0; vertexIndex < meshVertices.Length; vertexIndex++)
            {
                meshUV2Meta[vertexIndex] = new Vector2(
                    biomeSystemSettingsIndexToPreview * 0.1f,
                    0.0f
                );
            }

            mesh.SetUVs(1, meshUV2Meta);
        }
    }
}
