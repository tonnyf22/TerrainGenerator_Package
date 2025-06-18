using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Power")]
	public class PowerNode : Node
	{
		[Input] public float input;
		[Input, Min(1.0f)] public float power = 1.0f;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Power();
			}
			else
			{
				return null;
			}
		}

		private float Power()
		{
			input = GetInputValue("input", input);
			power = GetInputValue("power", power);

			return math.pow(input, power);
		}
	}
}