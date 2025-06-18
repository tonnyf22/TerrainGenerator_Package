using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Scattering.Operations
{
	[CreateNodeMenu("Scattering/Operations/Greater Than")]
    public class GreaterThanNode : Node
    {
        [Input] public float inputA;
        [Input] public float inputB;

        [Output] public bool value;

        public override object GetValue(NodePort port)
        {
			if (port.fieldName == "value")
			{
				return GreaterThan();
			}
			else
			{
				return null;
			}
        }

        public bool GreaterThan()
        {
			inputA = GetInputValue("inputA", inputA);
			inputB = GetInputValue("inputB", inputB);

            return inputA > inputB;
        }
    }
}
