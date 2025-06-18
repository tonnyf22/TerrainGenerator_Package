using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Tangent")]
	public class TangentNode : Node
	{
		[Input] public float input;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Tangent();
			}
			else
			{
				return null;
			}
		}

		private float Tangent()
		{
			input = GetInputValue("input", input);

			return math.tan(input);
		}
	}
}