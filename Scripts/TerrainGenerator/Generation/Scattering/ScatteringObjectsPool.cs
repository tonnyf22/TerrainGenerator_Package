using System.Collections.Generic;
using TerrainGenerator.Components.Settings.Biomes;
using UnityEngine;

namespace TerrainGenerator.Generation.Scattering
{
    public class ScatteringObjectsPool
    {
        public static ScatteringObjectsPool CreateScatteringObjectsPool(BiomeSettings[] biomeSettings, GameObject parentGameObject)
        {
            return new ScatteringObjectsPool(
                biomeSettings,
                parentGameObject);
        }

        public BiomeSettings[] biomeSettings;
        public readonly GameObject scatteringObjectsPoolsGameObject;
        public readonly Dictionary<BiomeScattering, Queue<GameObject>> pools;

        public ScatteringObjectsPool(BiomeSettings[] biomeSettings, GameObject parentGameObject)
        {
            this.biomeSettings = biomeSettings;
            scatteringObjectsPoolsGameObject = new GameObject("ScatteringObjectsPool");
            scatteringObjectsPoolsGameObject.transform.parent = parentGameObject.transform;
            pools = new Dictionary<BiomeScattering, Queue<GameObject>>();
            CreatePools();
            PreInstantiateGameObjects();
        }

        private void CreatePools()
        {
            for (int biomeIndex = 0; biomeIndex < biomeSettings.Length; biomeIndex++)
            {
                for (int scatteringIndex = 0; scatteringIndex < biomeSettings[biomeIndex].biomeScatteringSettings.Length; scatteringIndex++)
                {
                    BiomeScattering biomeScattering = new BiomeScattering(
                        biomeIndex,
                        scatteringIndex);
                    Queue<GameObject> queue = new Queue<GameObject>(
                        biomeSettings[biomeIndex]
                            .biomeScatteringSettings[scatteringIndex]
                            .numberOfPreinstantiatedObjects
                    );

                    pools.Add(
                        biomeScattering,
                        queue);
                }
            }
        }

        private void PreInstantiateGameObjects()
        {
            int indexPool = 0;
            foreach (var pool in pools)
            {
                int times = biomeSettings[pool.Key.biomeIndex]
                    .biomeScatteringSettings[pool.Key.scatteringIndex]
                    .numberOfPreinstantiatedObjects;
                for (int time = 0; time < times; time++)
                {
                    GameObject instance = InstantiateBiomeScatteringObject(
                        pool.Key.biomeIndex,
                        pool.Key.scatteringIndex);
                    instance.transform.parent = scatteringObjectsPoolsGameObject.transform;
                    instance.SetActive(false);

                    pool.Value.Enqueue(instance);
                }

                indexPool++;
            }
        }

        private GameObject InstantiateBiomeScatteringObject(int biomeIndex, int scatteringIndex)
        {
            GameObject prefab = 
                biomeSettings[biomeIndex]
                    .biomeScatteringSettings[scatteringIndex]
                    .scatteringObject;

            GameObject instance = GameObject.Instantiate(prefab);

            return instance;
        }

        public GameObject GetScatteringObject(BiomeScattering biomeScattering)
        {
            if (pools[biomeScattering].TryDequeue(out GameObject gameObject))
            {
                gameObject.transform.parent = null;
                gameObject.SetActive(true);
            }
            else
            {
                gameObject = InstantiateBiomeScatteringObject(
                    biomeScattering.biomeIndex,
                    biomeScattering.scatteringIndex);
            }

            return gameObject;
        }

        public void PutScatteringObject(BiomeScattering biomeScattering, GameObject gameObject)
        {
            gameObject.SetActive(false);
            gameObject.transform.parent = scatteringObjectsPoolsGameObject.transform;

            pools[biomeScattering].Enqueue(gameObject);
        }
    }
}