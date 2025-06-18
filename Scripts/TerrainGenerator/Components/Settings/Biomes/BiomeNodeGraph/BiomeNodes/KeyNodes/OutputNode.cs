using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.KeyNodes
{
	[CreateNodeMenu("Key Nodes/Output"), NodeWidth(250)]
	public class OutputNode : Node
	{
		[Input] public float height;
		[Input(dynamicPortList = true)] public bool[] isKeepPoint;

		public float GetHeightOutput()
		{
			height = GetInputValue("height", height);
			return height;
		}

		public bool GetIsKeepPointOutput(int scatteringIndex)
		{
			isKeepPoint[scatteringIndex] = GetInputValue($"isKeepPoint {scatteringIndex}", isKeepPoint[scatteringIndex]);
			return isKeepPoint[scatteringIndex];
		}
	}
}