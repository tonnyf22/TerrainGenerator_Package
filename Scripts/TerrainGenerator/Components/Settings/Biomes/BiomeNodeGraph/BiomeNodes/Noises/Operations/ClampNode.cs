using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Clamp")]
	public class ClampNode : Node
	{
		[Input] public float input;

		[Input] public float min;
		[Input] public float max;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Clamp();
			}
			else
			{
				return null;
			}
		}

		private float Clamp()
		{
			input = GetInputValue("input", input);
			min = GetInputValue("min", min);
			max = GetInputValue("max", max);
			if (min > max)
			{
				(max, min) = (min, max);
			}

			if (input < min)
			{
				return min;
			}
			else if (input > max)
			{
				return max;
			}
			else
			{
				return input;
			}
		}
	}
}