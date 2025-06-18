using System.Collections.Generic;
using UnityEngine;

namespace TerrainGenerator.Generation.Biome
{
    public class BiomesDistribution
    {
        public static BiomesDistribution CreateBiomesDistribution(int numberOfBiomes, string seed, float biomeCellSize, float biomeSubcellSize)
        {
            return new BiomesDistribution(
                numberOfBiomes,
                seed,
                biomeCellSize,
                biomeSubcellSize
            );
        }

        public readonly int numberOfBiomes;
        public readonly string seed;
        public readonly float biomeCellSize;
        public readonly float biomeSubcellSize;
        private DeterministicRandom deterministicRandom;

        private Dictionary<BiomeCellCoordinate, BiomeSourcePoint> biomeCellsToSources = new Dictionary<BiomeCellCoordinate, BiomeSourcePoint>();
        private Dictionary<BiomeSubcellCoordinate, BiomeSubsourcePoint> biomeSubcellsToSubsources = new Dictionary<BiomeSubcellCoordinate, BiomeSubsourcePoint>();
        public readonly Dictionary<int, List<BiomeSubsourcePoint>> biomeIndexToSubsources = new Dictionary<int, List<BiomeSubsourcePoint>>();
        
        public BiomesDistribution(int numberOfBiomes, string seed, float biomeCellSize, float biomeSubcellSize)
        {
            this.numberOfBiomes = numberOfBiomes;
            this.seed = seed;
            this.biomeCellSize = biomeCellSize;
            this.biomeSubcellSize = biomeSubcellSize;
            deterministicRandom = new DeterministicRandom(seed);
        }

        // point to biome subcell
        public BiomeSubcellCoordinate LocatePointInBiomeSubgridAsBiomeSubcellCoordinates(float x, float z)
        {
            return new BiomeSubcellCoordinate(
                Mathf.FloorToInt(x / biomeSubcellSize),
                Mathf.FloorToInt(z / biomeSubcellSize)
            );
        }

        // biome subcell to point
        private Vector3 LocateBiomeSubcellCoordinatesAsPoint(BiomeSubcellCoordinate biomeSubcellCoordinate)
        {
            return new Vector3(
                biomeSubcellCoordinate.x * biomeSubcellSize,
                0.0f,
                biomeSubcellCoordinate.z * biomeSubcellSize
            );
        }

        // point to biom cell
        private BiomeCellCoordinate LocatePointInBiomeGridAsBiomeCellCoordinate(float x, float z)
        {
            return new BiomeCellCoordinate(
                Mathf.FloorToInt(x / biomeCellSize),
                Mathf.FloorToInt(z / biomeCellSize)
            );
        }

        // biom cell to point
        private Vector3 LocateBiomeCellCoordinatesAsPoint(BiomeCellCoordinate biomeCellCoordinate)
        {
            return new Vector3(
                biomeCellCoordinate.x * biomeCellSize,
                0.0f,
                biomeCellCoordinate.z * biomeCellSize
            );
        }

        // biome subcell to biom cell
        private BiomeCellCoordinate LocateBiomeSubcellCoordinateAsBiomeCellCoordinate(BiomeSubcellCoordinate biomeSubcellCoordinate)
        {
            Vector3 point = LocateBiomeSubcellCoordinatesAsPoint(biomeSubcellCoordinate);
            BiomeCellCoordinate biomeCellCoordinate = LocatePointInBiomeGridAsBiomeCellCoordinate(point.x, point.z);

            return biomeCellCoordinate;
        }

        // biome cell to biome source point
        private BiomeSourcePoint LocateBiomeSourcePointByBiomeCellCoordinates(BiomeCellCoordinate biomeCellCoordinate)
        {
            if (biomeCellsToSources.ContainsKey(biomeCellCoordinate))
            {
                return biomeCellsToSources[biomeCellCoordinate];
            }
            else
            {
                (float sourceX, float sourceZ) = CalculateSourcePointCoordinates(biomeCellCoordinate);
                int biomeIndex = CalculateSourcePointBiomeIndex(sourceX, sourceZ);

                BiomeSourcePoint biomeSourcePoint = new BiomeSourcePoint(
                    biomeIndex,
                    sourceX,
                    sourceZ
                );

                biomeCellsToSources.Add(biomeCellCoordinate, biomeSourcePoint);

                return biomeSourcePoint;
            }
        }

        private (float, float) CalculateSourcePointCoordinates(BiomeCellCoordinate biomeCellCoordinate)
        {
            float rawOffsetX = deterministicRandom.Value01(biomeCellCoordinate.x, biomeCellCoordinate.z, 190337);
            float rawOffsetZ = deterministicRandom.Value01(133719, biomeCellCoordinate.z, biomeCellCoordinate.x);

            Vector3 biomeCellPoint = LocateBiomeCellCoordinatesAsPoint(biomeCellCoordinate);

            float sourceX = biomeCellPoint.x + rawOffsetX * biomeCellSize;
            float sourceZ = biomeCellPoint.z + rawOffsetZ * biomeCellSize;

            return (sourceX, sourceZ);
        }

        private int CalculateSourcePointBiomeIndex(float sourceX, float sourceZ)
        {
            float rawBiomeValue = deterministicRandom.Value01(sourceX, sourceZ);
            int biomeIndex = Mathf.FloorToInt(rawBiomeValue / (1.0f / numberOfBiomes));

            return biomeIndex;
        }

        // biome subcell to biome subsource point
        public BiomeSubsourcePoint LocateBiomeSubsourcePointByBiomeSubcellCoordinates(BiomeSubcellCoordinate biomeSubcellCoordinate)
        {
            if (biomeSubcellsToSubsources.ContainsKey(biomeSubcellCoordinate))
            {
                return biomeSubcellsToSubsources[biomeSubcellCoordinate];
            }
            else
            {
                BiomeCellCoordinate biomeCellCoordinate = LocateBiomeSubcellCoordinateAsBiomeCellCoordinate(biomeSubcellCoordinate);

                BiomeCellCoordinate[] surroundingBiomeCellCoordinates = CalculateSurroundingBiomeCellCoordinates(biomeCellCoordinate);
                BiomeSourcePoint[] surroundingBiomeSourcePoints = CalculateSurroundingBiomeSourcePoints(surroundingBiomeCellCoordinates);

                (float subsourceX, float subsourceZ) = CalculateSubsourcePointCoordinates(biomeSubcellCoordinate);
                int biomeIndex = CalculateSubsourcePointBiomeIndex(subsourceX, subsourceZ, surroundingBiomeSourcePoints);

                BiomeSubsourcePoint biomeSubsourcePoint = new BiomeSubsourcePoint(
                    biomeIndex,
                    subsourceX,
                    subsourceZ
                );

                biomeSubcellsToSubsources.Add(biomeSubcellCoordinate, biomeSubsourcePoint);
                if (!biomeIndexToSubsources.ContainsKey(biomeIndex))
                {
                    biomeIndexToSubsources.Add(biomeIndex, new List<BiomeSubsourcePoint>());
                }
                biomeIndexToSubsources[biomeIndex].Add(biomeSubsourcePoint);

                return biomeSubsourcePoint;
            }
        }

        private BiomeCellCoordinate[] CalculateSurroundingBiomeCellCoordinates(BiomeCellCoordinate biomeCellCoordinate)
        {
            List<BiomeCellCoordinate> biomeCellCoordinates = new List<BiomeCellCoordinate>();

            for (int shiftX = -1; shiftX < 2; shiftX++)
            {
                for (int shiftZ = -1; shiftZ < 2; shiftZ++)
                {
                    if (shiftX != 0 || shiftZ != 0)
                    {
                        biomeCellCoordinates.Add(
                            new BiomeCellCoordinate(
                                biomeCellCoordinate.x + shiftX,
                                biomeCellCoordinate.z + shiftZ));
                    }
                }
            }

            return biomeCellCoordinates.ToArray();
        }

        private BiomeSourcePoint[] CalculateSurroundingBiomeSourcePoints(BiomeCellCoordinate[] surroundingBiomeCellCoordinates)
        {
            List<BiomeSourcePoint> biomeSourcePoints = new List<BiomeSourcePoint>();

            foreach (var item in surroundingBiomeCellCoordinates)
            {
                BiomeSourcePoint biomeSourcePoint = LocateBiomeSourcePointByBiomeCellCoordinates(item);
                biomeSourcePoints.Add(biomeSourcePoint);
            }

            return biomeSourcePoints.ToArray();
        }

        private (float, float) CalculateSubsourcePointCoordinates(BiomeSubcellCoordinate biomeSubcellCoordinate)
        {
            float rawOffsetX = deterministicRandom.Value01(biomeSubcellCoordinate.x, biomeSubcellCoordinate.z, 131488);
            float rawOffsetZ = deterministicRandom.Value01(biomeSubcellCoordinate.z, 234889, biomeSubcellCoordinate.x);

            Vector3 biomeSubcellPoint = LocateBiomeSubcellCoordinatesAsPoint(biomeSubcellCoordinate);

            float subsourceX = biomeSubcellPoint.x + rawOffsetX * biomeSubcellSize;
            float subsourceZ = biomeSubcellPoint.z + rawOffsetZ * biomeSubcellSize;

            return (subsourceX, subsourceZ);
        }

        private int CalculateSubsourcePointBiomeIndex(float subsourceX, float subsourceZ, BiomeSourcePoint[] surroundingBiomeSourcePoints)
        {
            float shortestSquaredDistance = float.MaxValue;
            int biomeIndexOfClosestSourcePoint = 0;

            foreach (var item in surroundingBiomeSourcePoints)
            {
                float squaredDistance = Mathf.Pow(item.x - subsourceX, 2) + Mathf.Pow(item.z - subsourceZ, 2);

                if (squaredDistance < shortestSquaredDistance)
                {
                    shortestSquaredDistance = squaredDistance;
                    biomeIndexOfClosestSourcePoint = item.biomeIndex;
                }
            }

            return biomeIndexOfClosestSourcePoint;
        }

        public BiomeSubsourcePoint[] CalculateSurroundingBiomeSubsourcePoints(BiomeSubcellCoordinate biomeSubcellCoordinate)
        {
            BiomeSubcellCoordinate[] biomeSubcellCoordinates = CalculateSurroundingBiomeSubcellCoordinates(biomeSubcellCoordinate);
            return CalculateSurroundingBiomeSubsourcePoints(biomeSubcellCoordinates);
        }

        private BiomeSubcellCoordinate[] CalculateSurroundingBiomeSubcellCoordinates(BiomeSubcellCoordinate biomeSubcellCoordinate)
        {
            List<BiomeSubcellCoordinate> biomeSubcellCoordinates = new List<BiomeSubcellCoordinate>();

            for (int shiftX = -1; shiftX < 2; shiftX++)
            {
                for (int shiftZ = -1; shiftZ < 2; shiftZ++)
                {
                    if (shiftX != 0 || shiftZ != 0)
                    {
                        biomeSubcellCoordinates.Add(
                            new BiomeSubcellCoordinate(
                                biomeSubcellCoordinate.x + shiftX,
                                biomeSubcellCoordinate.z + shiftZ));
                    }
                }
            }

            return biomeSubcellCoordinates.ToArray();
        }

        private BiomeSubsourcePoint[] CalculateSurroundingBiomeSubsourcePoints(BiomeSubcellCoordinate[] surroundingBiomeSubcellCoordinates)
        {
            List<BiomeSubsourcePoint> biomeSubsourcePoints = new List<BiomeSubsourcePoint>();

            foreach (var item in surroundingBiomeSubcellCoordinates)
            {
                BiomeSubsourcePoint biomeSubsourcePoint = LocateBiomeSubsourcePointByBiomeSubcellCoordinates(item);
                biomeSubsourcePoints.Add(biomeSubsourcePoint);
            }

            return biomeSubsourcePoints.ToArray();
        }
    }
}
