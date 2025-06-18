using UnityEngine;

namespace TerrainGenerator.Generation.Scattering
{
    public struct ScatteringObjectParameters
    {
        public readonly Vector3 position;
        public readonly Vector3 scale;
        public readonly Quaternion rotation;

        public ScatteringObjectParameters(Vector3 position, Vector3 scale, Quaternion rotation)
        {
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
        }
    }
}
