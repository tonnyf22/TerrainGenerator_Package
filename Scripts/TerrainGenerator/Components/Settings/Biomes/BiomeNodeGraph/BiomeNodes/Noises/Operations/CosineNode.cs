using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Cosine")]
	public class CosineNode : Node
	{
		[Input] public float input;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Cosine();
			}
			else
			{
				return null;
			}
		}

		private float Cosine()
		{
			input = GetInputValue("input", input);

			return math.cos(input);
		}
	}
}