using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises.Operations
{
	[CreateNodeMenu("Noises/Operations/Logarithm")]
	public class LogarithmNode : Node
	{
		[Input] public float input;
		[Input] public float _base = math.E;

		[Output] public float value;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				return Logarithm();
			}
			else
			{
				return null;
			}
		}

		private float Logarithm()
		{
			input = GetInputValue("input", input);
			_base = GetInputValue("base", _base);

			return math.log(input) / math.log(_base);
		}
	}
}