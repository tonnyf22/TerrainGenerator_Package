using System;
using TerrainGenerator.Components.Settings.Chunks;
using UnityEngine;

namespace TerrainGenerator.Generation.Structure
{
    public class DetalizationLevel
    {
        public static DetalizationLevel CreateDetalizationLevel(int levelIndex, MeshFillType meshFillType, int meshResolution, GameObject parentGameObject)
        {
            return new DetalizationLevel(
                levelIndex,
                meshFillType,
                meshResolution,
                parentGameObject
            );
        }

        public readonly int levelIndex;
        public readonly MeshFillType meshFillType;
        public readonly int meshResolution;
        public readonly GameObject detalizationLevelGameObject;
        public WaterCovering waterCovering { get; private set; }
        public ObjectsScattering scattering { get; private set; }

        public DetalizationLevel(int levelIndex, MeshFillType meshFillType, int meshResolution, GameObject parentGameObject)
        {
            this.levelIndex = levelIndex;
            this.meshFillType = meshFillType;
            this.meshResolution = meshResolution;
            detalizationLevelGameObject = new GameObject("ChunkDetalizationLevel");
            detalizationLevelGameObject.transform.parent = parentGameObject.transform;
            SetDetalizationLevelGameObjectCoordinates();
        }

        private void SetDetalizationLevelGameObjectCoordinates()
        {
            detalizationLevelGameObject.transform.localPosition = Vector3.zero;
        }

        public void Show()
        {
            if (!detalizationLevelGameObject.activeSelf)
            {
                scattering.SetupScattering();

                detalizationLevelGameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (detalizationLevelGameObject.activeSelf)
            {
                detalizationLevelGameObject.SetActive(false);

                scattering.ReleaseScattering();
            }
        }

        public void AddWaterCovering(WaterCovering waterCovering)
        {
            if (this.waterCovering != null)
            {
                throw new ArgumentException($"Water covering already exists for this detalization level.");
            }
            else
            {
                this.waterCovering = waterCovering;
            }
        }

        public void RemoveWaterCovering()
        {
            if (this.waterCovering == null)
            {
                throw new ArgumentException($"Water covering does not exist for this detalization level.");
            }
            else
            {
                this.waterCovering = null;
            }
        }

        public void AddScattering(ObjectsScattering scattering)
        {
            if (this.scattering != null)
            {
                throw new ArgumentException($"Scattering already exists for this detalization level.");
            }
            else
            {
                this.scattering = scattering;
            }
        }

        public void RemoveScattering(int biomeIndex)
        {
            if (this.scattering == null)
            {
                throw new ArgumentException($"Scattering does not exist for this detalization level.");
            }
            else
            {
                this.scattering = null;
            }
        }

        public void ApplySurfaceMesh(Mesh mesh, Material material)
        {
            if (!detalizationLevelGameObject.TryGetComponent(out MeshFilter meshFilter))
            {
                meshFilter = detalizationLevelGameObject.AddComponent<MeshFilter>();
            }
            if (!detalizationLevelGameObject.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer = detalizationLevelGameObject.AddComponent<MeshRenderer>();
            }

            meshFilter.mesh = mesh;
            meshRenderer.material = material;
        }

        public Mesh GetSurfaceMesh()
        {
            if (!detalizationLevelGameObject.TryGetComponent<MeshFilter>(out var meshFilter))
            {
                throw new ArgumentException($"Mesh filter does not exist on this detalization level game object.");
            }
            else
            {
                return detalizationLevelGameObject.GetComponent<MeshFilter>().mesh;
            }
        }

        public void ApplySurfaceCollision(Mesh mesh)
        {
            if (!detalizationLevelGameObject.TryGetComponent(out MeshCollider meshCollider))
            {
                meshCollider = detalizationLevelGameObject.AddComponent<MeshCollider>();
            }

            meshCollider.sharedMesh = mesh;
        }
    }
}