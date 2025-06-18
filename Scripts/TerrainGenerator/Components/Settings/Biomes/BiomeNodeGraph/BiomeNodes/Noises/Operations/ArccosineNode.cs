using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Arccosine")]
	public class ArccosineNode : Node
	{
		[Input] public float input;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Arccosine();
			}
			else
			{
				return null;
			}
		}

		private float Arccosine()
		{
			input = GetInputValue("input", input);

			return math.acos(input);
		}
	}
}