using System.Collections.Generic;
using TerrainGenerator.Components.Settings.Biomes.Scattering;
using TerrainGenerator.Generation.Biome;
using TerrainGenerator.Generation.Structure;
using UnityEngine;

namespace TerrainGenerator.Generation.Scattering
{
    public class BiomePreviewerScatteringModifiedPointsGenerator : IScatteringModifiedPointsGenerator
    {
        public static BiomePreviewerScatteringModifiedPointsGenerator CreateScatteringObjectsGenerator(Chunk chunk, string seed, int biomeSystemSettingsIndexToPreview, Dictionary<int, BiomeScatteringSettings[]> biomesScatteringSettings, BiomeGraphInterpreter biomeGraphInterpreter)
        {
            return new BiomePreviewerScatteringModifiedPointsGenerator(
                chunk,
                seed,
                biomeSystemSettingsIndexToPreview,
                biomesScatteringSettings,
                biomeGraphInterpreter);
        }

        public readonly Chunk chunk;
        public readonly string seed;
        public readonly int biomeSystemSettingsIndexToPreview;
        public readonly Dictionary<int, BiomeScatteringSettings[]> biomesScatteringSettings;
        public readonly BiomeGraphInterpreter biomeGraphInterpreter;

        private readonly ScatteringRawPointsGenerator scatteringPointsGenerator;
        
        private DeterministicRandom deterministicRandom;

        private RaycastHit hitDown;
        private Dictionary<BiomeScattering, List<ScatteringObjectParameters>> biomeScatteringObjectsParameters;
        private DetalizationLevel detalizationLevel;
        private List<ScatteringObjectParameters> scatteringObjectsParameters;

        public BiomePreviewerScatteringModifiedPointsGenerator(Chunk chunk, string seed, int biomeSystemSettingsIndexToPreview, Dictionary<int, BiomeScatteringSettings[]> biomesScatteringSettings, BiomeGraphInterpreter biomeGraphInterpreter)
        {
            this.chunk = chunk;
            this.seed = seed;
            this.biomeSystemSettingsIndexToPreview = biomeSystemSettingsIndexToPreview;
            this.biomesScatteringSettings = biomesScatteringSettings;
            this.biomeGraphInterpreter = biomeGraphInterpreter;
            scatteringPointsGenerator = ScatteringRawPointsGenerator.CreateScatteringPointsGenerator(
                chunk,
                seed
            );
            deterministicRandom = new DeterministicRandom(seed);
        }

        public Dictionary<BiomeScattering, List<ScatteringObjectParameters>> CreateBiomeScatteringObjectsParameters(DetalizationLevel detalizationLevel)
        {
            biomeScatteringObjectsParameters = new Dictionary<BiomeScattering, List<ScatteringObjectParameters>>();
            this.detalizationLevel = detalizationLevel;

            LoopThroughEachBiomeScattering(biomeSystemSettingsIndexToPreview);

            return biomeScatteringObjectsParameters;
        }

        private void LoopThroughEachBiomeScattering(int biomeIndex)
        {
            for (int scatteringIndex = 0; scatteringIndex < biomesScatteringSettings[biomeIndex].Length; scatteringIndex++)
            {
                scatteringObjectsParameters = new List<ScatteringObjectParameters>();

                if (IsNoScatteringsForBiome(biomeIndex, scatteringIndex))
                {
                    continue;
                }

                BiomeScattering biomeScattering = new BiomeScattering(
                    biomeIndex,
                    scatteringIndex
                );

                List<Vector3> pointsRaw = CreateRawScatteringPoints(
                    detalizationLevel,
                    biomeIndex,
                    scatteringIndex);

                LoopThroughRawPointsOfBiomeScattering(biomeScattering, pointsRaw);

                biomeScatteringObjectsParameters.Add(biomeScattering, scatteringObjectsParameters);

                scatteringObjectsParameters = null;
            }
        }

        private bool IsNoScatteringsForBiome(int biomeIndex, int scatteringIndex)
        {
            return biomesScatteringSettings[biomeIndex].Length - 1 < scatteringIndex;
        }

        private List<Vector3> CreateRawScatteringPoints(DetalizationLevel detalizationLevel, int biomeIndex, int scatteringIndex)
        {
            return scatteringPointsGenerator.CreatePoints(
                biomeIndex,
                scatteringIndex,
                detalizationLevel.scattering,
                biomesScatteringSettings[biomeIndex][scatteringIndex]);
        }

        private void LoopThroughRawPointsOfBiomeScattering(BiomeScattering biomeScattering, List<Vector3> pointsRaw)
        {
            for (int index = 0; index < pointsRaw.Count; index++)
            {
                Vector3 point = pointsRaw[index];

                // filter by biome graph
                if (IsNotKeepPointByBiomeGraph(biomeScattering.biomeIndex, biomeScattering.scatteringIndex, point))
                {
                    continue;
                }

                // filter by raycast "down"
                if(IsNotRaycastDownHitSurface(point, out Vector3 pointHit))
                {
                    continue;
                }

                // filter by height range
                if (IsNotPointHeightWithinScatteringHeightRange(biomeScattering.biomeIndex, biomeScattering.scatteringIndex, pointHit))
                {
                    continue;
                }

                // store scattering object
                ScatteringObjectParameters scatteringObjectParameters = CalculateScatteringObjectParametersOnPointHit(
                    biomeScattering.biomeIndex,
                    biomeScattering.scatteringIndex,
                    pointHit
                );
                scatteringObjectsParameters.Add(scatteringObjectParameters);
            }
        }

        private bool IsNotKeepPointByBiomeGraph(int biomeIndex, int scatteringIndex, Vector3 point)
        {
            bool isKeepPoint = biomeGraphInterpreter.GetBiomeScatteringIsKeepPoint(
                biomeIndex,
                scatteringIndex,
                point.x,
                point.z);

            return !isKeepPoint;
        }

        private bool IsNotRaycastDownHitSurface(Vector3 point, out Vector3 pointHit)
        {
            Vector3 rayOrigin = new Vector3(
                point.x,
                Mathf.Pow(10, 5),
                point.z
            );
            bool isHitDown = Physics.Raycast(rayOrigin, Vector3.down, out hitDown);
            pointHit = hitDown.point;

            return !isHitDown;
        }

        private bool IsNotPointHeightWithinScatteringHeightRange(int biomeIndex, int scatteringIndex, Vector3 pointHit)
        {
            bool result = 
                pointHit.y >= biomesScatteringSettings[biomeIndex][scatteringIndex].scatteringMinHeight &&
                pointHit.y <= biomesScatteringSettings[biomeIndex][scatteringIndex].scatteringMaxHeight;
            result =! result;

            return result;
        }

        private ScatteringObjectParameters CalculateScatteringObjectParametersOnPointHit(int biomeIndex, int scatteringIndex, Vector3 pointHit)
        {
            Vector3 scatteringObjectScale = biomesScatteringSettings[biomeIndex][scatteringIndex].scatteringObject.transform.localScale;

            Vector3 position = pointHit;
            Vector3 scale = CalculateScatteringObjectScale(biomeIndex, scatteringIndex, pointHit, scatteringObjectScale);
            Quaternion rotation = CalculateScatteringObjectRotation(biomeIndex, scatteringIndex, pointHit);

            ScatteringObjectParameters scatteringObjectParameters = new ScatteringObjectParameters(
                position,
                scale,
                rotation
            );

            return scatteringObjectParameters;
        }

        private Vector3 CalculateScatteringObjectScale(int biomeIndex, int scatteringIndex, Vector3 placePoint, Vector3 scatteringObjectScale)
        {
            Vector3 scale;
            float scaleCalculated;

            bool isApplyScaleRange = biomesScatteringSettings[biomeIndex][scatteringIndex].isApplyScaleRange;
            float scaleMinSettings = biomesScatteringSettings[biomeIndex][scatteringIndex].scatteringScaleMin;
            float scaleSettings = biomesScatteringSettings[biomeIndex][scatteringIndex].scatteringScale;

            if (isApplyScaleRange)
            {
                float rawOffsetScale = deterministicRandom.Value01(
                    biomeIndex * scatteringIndex,
                    placePoint.z * 943,
                    placePoint.x * 143);
                scaleCalculated = scaleMinSettings + rawOffsetScale * (scaleSettings - scaleMinSettings);
            }
            else
            {
                scaleCalculated = scaleSettings;
            }

            scale = scatteringObjectScale * scaleCalculated;

            return scale;
        }

        private Quaternion CalculateScatteringObjectRotation(int biomeIndex, int scatteringIndex, Vector3 placePoint)
        {
            float rotationValue;

            bool isApplyRotationRange = biomesScatteringSettings[biomeIndex][scatteringIndex].isApplyRotationRange;
            float rotationMinSettings = biomesScatteringSettings[biomeIndex][scatteringIndex].scatteringRotationMin;
            float rotationSettings = biomesScatteringSettings[biomeIndex][scatteringIndex].scatteringRotation;

            if (isApplyRotationRange)
            {
                float rawOffsetRotation = deterministicRandom.Value01(
                    placePoint.x * 123,
                    biomeIndex * scatteringIndex,
                    placePoint.z * 456);
                rotationValue = rotationMinSettings + rawOffsetRotation * (rotationSettings - rotationMinSettings);
            }
            else
            {
                rotationValue = rotationSettings;
            }

            Quaternion rotationQuaternion = Quaternion.Euler(
                new Vector3(
                    0.0f,
                    rotationValue,
                    0.0f
                ));

            return rotationQuaternion;
        }
    }
}