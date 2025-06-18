using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Root")]
	public class RootNode : Node
	{
		[Input] public float input;
		[Input, Min(1.0f)] public float index = 1.0f;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Root();
			}
			else
			{
				return null;
			}
		}

		private float Root()
		{
			input = GetInputValue("input", input);
			index = GetInputValue("index", index);

			return math.pow(input, 1.0f / index);
		}
	}
}