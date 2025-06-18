using UnityEngine;

namespace TerrainGenerator.Generation.Appearance
{
    public interface ITextureBlendingGenerator
    {
        public void ApplyTextureBlendingMetadataToMesh(Mesh mesh);
    }
}