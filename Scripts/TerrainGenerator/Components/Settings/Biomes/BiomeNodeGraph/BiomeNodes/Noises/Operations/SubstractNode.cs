using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Substract")]
	public class SubstractNode : Node
	{
		[Input] public float inputA;
		[Input] public float inputB;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Substract();
			}
			else
			{
				return null;
			}
		}

		private float Substract()
		{
			inputA = GetInputValue("inputA", inputA);
			inputB = GetInputValue("inputB", inputB);

			return inputA - inputB;
		}
	}
}