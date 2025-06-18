using TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph;
using TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.KeyNodes;

namespace TerrainGenerator.Generation.Biome
{
    public class BiomeGraphInterpreter
    {
        public static BiomeGraphInterpreter CreateBiomeGraphInterpreter(BiomeGraph[] biomeGraphs)
        {
            return new BiomeGraphInterpreter(
                biomeGraphs
            );
        }

        public readonly BiomeGraph[] biomeGraphs;

        public BiomeGraphInterpreter(BiomeGraph[] biomeGraphs)
        {
            this.biomeGraphs = biomeGraphs;
        }

        public float GetBiomeHeight(int biomeIndex, float inputPointX, float inputPointZ)
        {
            InputNode inputNode = biomeGraphs[biomeIndex].nodes.Find(node => node is InputNode) as InputNode;
            inputNode.inputPointXInput = inputPointX;
            inputNode.inputPointZInput = inputPointZ;

            OutputNode outputNode = biomeGraphs[biomeIndex].nodes.Find(node => node is OutputNode) as OutputNode;
            return outputNode.GetHeightOutput();
        }

        public bool GetBiomeScatteringIsKeepPoint(int biomeIndex, int scatteringIndex, float inputPointX, float inputPointZ)
        {
            InputNode inputNode = biomeGraphs[biomeIndex].nodes.Find(node => node is InputNode) as InputNode;
            inputNode.inputPointXInput = inputPointX;
            inputNode.inputPointZInput = inputPointZ;

            OutputNode outputNode = biomeGraphs[biomeIndex].nodes.Find(node => node is OutputNode) as OutputNode;
            return outputNode.GetIsKeepPointOutput(scatteringIndex);
        }
    }
}