using System.Collections.Generic;
using TerrainGenerator.Components.Settings.Biomes.Scattering;
using TerrainGenerator.Generation.Biome;
using TerrainGenerator.Generation.Structure;
using UnityEngine;

namespace TerrainGenerator.Generation.Scattering
{
    public class RegularScatteringModifiedPointsGenerator : IScatteringModifiedPointsGenerator
    {
        public static RegularScatteringModifiedPointsGenerator CreateScatteringObjectsGenerator(Chunk chunk, string seed, float scatteringInfluenceLevel, Dictionary<int, BiomeScatteringSettings[]> biomesScatteringSettings, BiomesDistribution biomesDistribution, BiomeGraphInterpreter biomeGraphInterpreter)
        {
            return new RegularScatteringModifiedPointsGenerator(
                chunk,
                seed,
                scatteringInfluenceLevel,
                biomesScatteringSettings,
                biomesDistribution,
                biomeGraphInterpreter);
        }

        public readonly Chunk chunk;
        public readonly string seed;
        public readonly float scatteringInfluenceLevel;
        public readonly Dictionary<int, BiomeScatteringSettings[]> biomesScatteringSettings;
        public readonly BiomesDistribution biomesDistribution;
        public readonly BiomeGraphInterpreter biomeGraphInterpreter;

        private readonly ScatteringRawPointsGenerator scatteringPointsGenerator;
        
        private DeterministicRandom deterministicRandom;

        private float distanceLimit;

        private RaycastHit hitDown;
        private Dictionary<BiomeScattering, List<ScatteringObjectParameters>> biomeScatteringObjectsParameters;
        private DetalizationLevel detalizationLevel;
        private List<ScatteringObjectParameters> scatteringObjectsParameters;

        public RegularScatteringModifiedPointsGenerator(Chunk chunk, string seed, float scatteringInfluenceLevel, Dictionary<int, BiomeScatteringSettings[]> biomesScatteringSettings, BiomesDistribution biomesDistribution, BiomeGraphInterpreter biomeGraphInterpreter)
        {
            this.chunk = chunk;
            this.seed = seed;
            this.scatteringInfluenceLevel = scatteringInfluenceLevel;
            this.biomesScatteringSettings = biomesScatteringSettings;
            this.biomesDistribution = biomesDistribution;
            this.biomeGraphInterpreter = biomeGraphInterpreter;
            scatteringPointsGenerator = ScatteringRawPointsGenerator.CreateScatteringPointsGenerator(
                chunk,
                seed
            );
            deterministicRandom = new DeterministicRandom(seed);
            distanceLimit = CalculateDistanceLimit();
        }

        private float CalculateDistanceLimit()
        {
            return biomesDistribution.biomeSubcellSize * 2.83f * scatteringInfluenceLevel;
        }

        public Dictionary<BiomeScattering, List<ScatteringObjectParameters>> CreateBiomeScatteringObjectsParameters(DetalizationLevel detalizationLevel)
        {
            biomeScatteringObjectsParameters = new Dictionary<BiomeScattering, List<ScatteringObjectParameters>>();
            this.detalizationLevel = detalizationLevel;

            LoopThroughEachBiomeSubsources();

            return biomeScatteringObjectsParameters;
        }

        private void LoopThroughEachBiomeSubsources()
        {
            foreach (int biomeIndex in biomesDistribution.biomeIndexToSubsources.Keys)
            {
                LoopThroughEachBiomeScattering(biomeIndex);
            }
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

                // filter by distance to subsource points
                if(IsNotClosestSubsourcePointWithinDistanceLimit(biomeScattering.biomeIndex, point))
                {
                    continue;
                }

                // filter by biome graph
                if (IsNotKeepPointByBiomeGraph(biomeScattering.biomeIndex, biomeScattering.scatteringIndex,  point))
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

        private bool IsNotClosestSubsourcePointWithinDistanceLimit(int biomeIndex, Vector3 point)
        {
            float[] distances = CalcuateDistancesToBiomeSubsourcePointsFromPoint(
                point,
                biomesDistribution.biomeIndexToSubsources[biomeIndex]);
            float distanceMin = CalculateDistanceMin(distances);

            return distanceMin > distanceLimit;
        }

        private float[] CalcuateDistancesToBiomeSubsourcePointsFromPoint(Vector3 point, List<BiomeSubsourcePoint> biomeSubsourcePoints)
        {
            float[] distances = new float[biomeSubsourcePoints.Count];

            for (int index = 0; index < distances.Length; index++)
            {
                distances[index] = CalculateDistanceBetweenPoints(
                    point.x,
                    point.z,
                    biomeSubsourcePoints[index].x,
                    biomeSubsourcePoints[index].z);
            }

            return distances;
        }

        private float CalculateDistanceBetweenPoints(float fromX, float fromZ, float toX, float toZ)
        {
            return Mathf.Sqrt(Mathf.Pow(fromX - toX, 2) + Mathf.Pow(fromZ - toZ, 2));
        }

        private float CalculateDistanceMin(float[] distances)
        {
            float distanceMin = float.MaxValue;

            foreach (var distance in distances)
            {
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                }
            }

            return distanceMin;    
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
            float scaleCalculated = 1.0f;

            bool isApplyScaleRange = biomesScatteringSettings[biomeIndex][scatteringIndex].isApplyScaleRange;
            float scaleMinSettings = biomesScatteringSettings[biomeIndex][scatteringIndex].scatteringScaleMin;
            float scaleSettings = biomesScatteringSettings[biomeIndex][scatteringIndex].scatteringScale;

            if (isApplyScaleRange)
            {
                float rawOffsetScale = deterministicRandom.Value01(
                    biomeIndex * scatteringIndex + placePoint.x + placePoint.z,
                    placePoint.z * biomeIndex + placePoint.z,
                    placePoint.x * scatteringIndex + placePoint.x);
                scaleCalculated *= scaleMinSettings + rawOffsetScale * (scaleSettings - scaleMinSettings);
            }
            else
            {
                scaleCalculated *= scaleSettings;
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
                    placePoint.x * biomeIndex - placePoint.x,
                    biomeIndex * scatteringIndex - placePoint.x - placePoint.z,
                    placePoint.z * scatteringIndex - placePoint.z);
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