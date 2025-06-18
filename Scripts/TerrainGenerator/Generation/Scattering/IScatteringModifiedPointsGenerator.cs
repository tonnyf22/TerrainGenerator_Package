using System.Collections.Generic;
using TerrainGenerator.Generation.Structure;

namespace TerrainGenerator.Generation.Scattering
{
    public interface IScatteringModifiedPointsGenerator
    {
        public Dictionary<BiomeScattering, List<ScatteringObjectParameters>> CreateBiomeScatteringObjectsParameters(DetalizationLevel detalizationLevel);
    }
}