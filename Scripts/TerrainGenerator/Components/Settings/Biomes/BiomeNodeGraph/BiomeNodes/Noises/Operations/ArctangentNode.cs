using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Arctangent")]
	public class ArctangentNode : Node
	{
		[Input] public float input;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Arctangent();
			}
			else
			{
				return null;
			}
		}

		private float Arctangent()
		{
			input = GetInputValue("input", input);

			return math.atan(input);
		}
	}
}