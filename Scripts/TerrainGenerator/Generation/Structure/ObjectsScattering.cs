using System;
using System.Collections.Generic;
using TerrainGenerator.Generation.Scattering;
using UnityEngine;

namespace TerrainGenerator.Generation.Structure
{
    public class ObjectsScattering
    {
        public static ObjectsScattering CreateScattering(bool isApplyScatteringSparseLevel, int scatteringSparceLevel, GameObject parentGameObject)
        {
            return new ObjectsScattering(
                isApplyScatteringSparseLevel,
                scatteringSparceLevel,
                parentGameObject);
        }

        public readonly bool isApplyScatteringSparseLevel;
        public readonly int scatteringSparseLevel;
        public readonly GameObject objectsScatteringGameObject;
        public ScatteringObjectsPool scatteringObjectsPool { get; private set; }

        private Dictionary<BiomeScattering, List<ScatteringObjectParameters>> biomeScatteringObjectsParameters;
        private Dictionary<BiomeScattering, List<GameObject>> biomeScatteringObjects;

        public ObjectsScattering(bool isApplyScatteringSparseLevel, int scatteringSparseLevel, GameObject parentGameObject)
        {
            this.isApplyScatteringSparseLevel = isApplyScatteringSparseLevel;
            this.scatteringSparseLevel = scatteringSparseLevel;
            objectsScatteringGameObject = new GameObject("Scattering");
            objectsScatteringGameObject.transform.parent = parentGameObject.transform;
            SetObjectsScatteringGameObjectCoordinates();
        }

        private void SetObjectsScatteringGameObjectCoordinates()
        {
            objectsScatteringGameObject.transform.localPosition = Vector3.zero;
        }

        public void SaveBiomeScatteringObjectsParameters(Dictionary<BiomeScattering, List<ScatteringObjectParameters>> biomeScatteringObjectsParameters)
        {
            if (this.biomeScatteringObjectsParameters != null)
            {
                throw new ArgumentException($"Biome scattering objects parameters already exists for this object scattering.");
            }
            else
            {
                this.biomeScatteringObjectsParameters = biomeScatteringObjectsParameters;

                biomeScatteringObjects = new Dictionary<BiomeScattering, List<GameObject>>(biomeScatteringObjectsParameters.Count);
            }
        }

        public void ApplyScatteringObjectsPool(ScatteringObjectsPool scatteringObjectsPool)
        {
            if (this.scatteringObjectsPool != null)
            {
                throw new ArgumentException($"Scattering objects pool already exists for this objects scattering.");
            }
            else
            {
                this.scatteringObjectsPool = scatteringObjectsPool;
            }
        }

        public void RemoveScatteringObjectsPool()
        {
            if (this.scatteringObjectsPool == null)
            {
                throw new ArgumentException($"Scattering objects pool does not exist for this objects scattering.");
            }
            else
            {
                this.scatteringObjectsPool = null;
            }
        }

        public void SetupScattering()
        {
            foreach (var pair in biomeScatteringObjectsParameters)
            {
                List<GameObject> listGameObjects = new List<GameObject>();

                for (int indexObject = 0; indexObject < pair.Value.Count; indexObject++)
                {
                    // get object
                    // setup objects' transform, parent
                    // add object to list

                    GameObject gameObject = scatteringObjectsPool.GetScatteringObject(pair.Key);

                    gameObject.transform.parent = objectsScatteringGameObject.transform;

                    gameObject.transform.position = pair.Value[indexObject].position;
                    gameObject.transform.localScale = pair.Value[indexObject].scale;
                    gameObject.transform.rotation = pair.Value[indexObject].rotation;

                    listGameObjects.Add(gameObject);
                }
                
                biomeScatteringObjects.Add(pair.Key, listGameObjects);
            }
        }

        public void ReleaseScattering()
        {
            // go through biomeScatteringObjects
            // unparent each object
            // put object back to pool

            foreach (var pair in biomeScatteringObjects)
            {
                for (int indexObject = 0; indexObject < pair.Value.Count; indexObject++)
                {
                    GameObject gameObject = pair.Value[indexObject];

                    gameObject.transform.parent = null;

                    scatteringObjectsPool.PutScatteringObject(pair.Key, gameObject);
                }
            }

            biomeScatteringObjects.Clear();
        }
    }
}