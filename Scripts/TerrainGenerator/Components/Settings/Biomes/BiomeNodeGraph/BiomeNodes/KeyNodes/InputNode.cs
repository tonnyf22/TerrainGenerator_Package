using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.KeyNodes
{
	[CreateNodeMenu("Key Nodes/Input")]
	public class InputNode : Node
	{
		[HideInInspector] public float inputPointXInput;
		[HideInInspector] public float inputPointZInput;

		[Output] public float inputPointX;
		[Output] public float inputPointZ;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "inputPointX")
			{
				return inputPointXInput;
			}
			else if (port.fieldName == "inputPointZ")
			{
				return inputPointZInput;
			}
			else
			{
				return null;
			}
		}
	}
}