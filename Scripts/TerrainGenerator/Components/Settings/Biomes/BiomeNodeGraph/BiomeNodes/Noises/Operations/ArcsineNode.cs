using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Arcsine")]
	public class ArcsineNode : Node
	{
		[Input] public float input;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Arcsine();
			}
			else
			{
				return null;
			}
		}

		private float Arcsine()
		{
			input = GetInputValue("input", input);

			return math.asin(input);
		}
	}
}