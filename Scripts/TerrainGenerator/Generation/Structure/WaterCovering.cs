using TerrainGenerator.Components.Settings.Chunks;
using UnityEngine;

namespace TerrainGenerator.Generation.Structure
{
    public class WaterCovering
    {
        public static WaterCovering CreateWaterCovering(MeshFillType waterCoveringMeshFillType, int waterCoveringMeshResolution, GameObject parentGameObject)
        {
            return new WaterCovering(
                waterCoveringMeshFillType,
                waterCoveringMeshResolution,
                parentGameObject
            );
        }

        public readonly MeshFillType waterCoveringMeshFillType;
        public readonly int waterCoveringMeshResolution;
        public readonly GameObject waterCoveringGameObject;

        public WaterCovering(MeshFillType waterCoveringMeshFillType, int waterCoveringMeshResolution, GameObject parentGameObject)
        {
            this.waterCoveringMeshFillType = waterCoveringMeshFillType;
            this.waterCoveringMeshResolution = waterCoveringMeshResolution;
            waterCoveringGameObject = new GameObject("WaterCovering");
            waterCoveringGameObject.transform.parent = parentGameObject.transform;
            SetWaterCoveringGameObjectCoordinates();
        }

        private void SetWaterCoveringGameObjectCoordinates()
        {
            waterCoveringGameObject.transform.localPosition = Vector3.zero;
        }

        public void ApplyWaterCoveringMesh(Mesh mesh, Material material)
        {
            if (mesh.triangles.Length == 0)
            {
                return;
            }

            if (!waterCoveringGameObject.TryGetComponent(out MeshFilter meshFilter))
            {
                meshFilter = waterCoveringGameObject.AddComponent<MeshFilter>();
            }
            if (!waterCoveringGameObject.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer = waterCoveringGameObject.AddComponent<MeshRenderer>();
            }

            meshFilter.mesh = mesh;
            meshRenderer.material = material;
        }

        public void ApplyWaterCoveringCollision(Mesh mesh)
        {
            if (mesh.triangles.Length == 0)
            {
                return;
            }

            if (!waterCoveringGameObject.TryGetComponent(out MeshCollider meshCollider))
            {
                meshCollider = waterCoveringGameObject.AddComponent<MeshCollider>();
            }

            meshCollider.sharedMesh = mesh;
        }
    }
}