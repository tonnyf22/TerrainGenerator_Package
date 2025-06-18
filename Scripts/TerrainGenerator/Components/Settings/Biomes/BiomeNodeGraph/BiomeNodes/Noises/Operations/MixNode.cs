using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Mix")]
	public class MixNode : Node
	{
		[Input, Range(0, 1)] public float factor;
		[Input] public float inputA;
		[Input] public float inputB;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Mix();
			}
			else
			{
				return null;
			}
		}

		private float Mix()
		{
			factor = GetInputValue("factor", factor);
			factor = math.clamp(factor, 0.0f, 1.0f);  // precaution

			inputA = GetInputValue("inputA", inputA);
			inputB = GetInputValue("inputB", inputB);

			return factor * inputA + (1 - factor) * inputB;
		}
	}
}