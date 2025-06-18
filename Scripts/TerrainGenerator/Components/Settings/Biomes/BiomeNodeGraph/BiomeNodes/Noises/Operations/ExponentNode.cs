using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Exponent")]
	public class ExponentNode : Node
	{
		[Input] public float input;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Exponent();
			}
			else
			{
				return null;
			}
		}

		private float Exponent()
		{
			input = GetInputValue("input", input);

			return math.exp(input);
		}
	}
}