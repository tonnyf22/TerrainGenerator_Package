using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Add")]
	public class AddNode : Node
	{
		[Input] public float inputA;
		[Input] public float inputB;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Add();
			}
			else
			{
				return null;
			}
		}

		private float Add()
		{
			inputA = GetInputValue("inputA", inputA);
			inputB = GetInputValue("inputB", inputB);

			return inputA + inputB;
		}
	}
}