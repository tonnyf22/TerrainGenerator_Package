using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Inverse")]
	public class InverseNode : Node
	{
		[Input] public float input;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Inverse();
			}
			else
			{
				return null;
			}
		}

		private float Inverse()
		{
			input = GetInputValue("input", input);

			return input * (-1);
		}
	}
}