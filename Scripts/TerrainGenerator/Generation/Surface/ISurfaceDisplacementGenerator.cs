using UnityEngine;

namespace TerrainGenerator.Generation.Surface
{
    public interface ISurfaceDisplacementGenerator
    {
        public void ApplyDisplacementToMesh(Mesh mesh);
    }
}
