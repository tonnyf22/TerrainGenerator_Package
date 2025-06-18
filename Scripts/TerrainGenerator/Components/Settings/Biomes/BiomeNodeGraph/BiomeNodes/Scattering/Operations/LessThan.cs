using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Scattering.Operations
{
	[CreateNodeMenu("Scattering/Operations/Less Than")]
    public class LessThanNode : Node
    {
        [Input] public float inputA;
        [Input] public float inputB;

        [Output] public bool value;

        public override object GetValue(NodePort port)
        {
			if (port.fieldName == "value")
			{
				return LessThan();
			}
			else
			{
				return null;
			}
        }

        public bool LessThan()
        {
			inputA = GetInputValue("inputA", inputA);
			inputB = GetInputValue("inputB", inputB);

            return inputA < inputB;
        }
    }
}
