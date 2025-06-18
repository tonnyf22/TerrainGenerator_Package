using UnityEngine;

namespace TerrainGenerator.Components.Settings.Biomes.Scattering
{
    public class BiomeScatteringSettings : ScriptableObject {
        [HideInInspector] public ScatteringType scatteringType;
        public int numberOfPreinstantiatedObjects = 30;
        public int targetResolution;
        public GameObject scatteringObject;
        [Space(20)]
        [Min(0.0f)]
        public float scatteringScale = 1.0f;
        public bool isApplyScaleRange;
        [Min(0.0f)]
        public float scatteringScaleMin;
        [Space(20)]
        [Min(0.0f)]
        public float scatteringRotation;
        public bool isApplyRotationRange;
        [Min(0.0f)]
        public float scatteringRotationMin;
        [Space(20)]
        public float scatteringMinHeight = 0.0f;
        public float scatteringMaxHeight = 1000.0f;
    }
}