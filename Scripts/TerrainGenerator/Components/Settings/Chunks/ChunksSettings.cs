using NaughtyAttributes;
using UnityEngine;

namespace TerrainGenerator.Components.Settings.Chunks
{
    [CreateAssetMenu(fileName = "ChunksSettings", menuName = "Terrain Generator/Chunks/Chunks Settings")]
    public class ChunksSettings : ScriptableObject
    {
        public float chunkSize;
        [Space(20)]
        [Expandable]
        public ChunkLODSettings[] chunkLODSettings;
        [Space(20)]
        public float chunkPreloadMaxDistance;
    }
}
