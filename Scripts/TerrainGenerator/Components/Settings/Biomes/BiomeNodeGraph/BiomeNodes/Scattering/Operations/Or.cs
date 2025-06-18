using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Scattering.Operations
{
	[CreateNodeMenu("Scattering/Operations/Or")]
    public class OrNode : Node
    {
        [Input] public bool inputA;
        [Input] public bool inputB;

        [Output] public bool value;

        public override object GetValue(NodePort port)
        {
			if (port.fieldName == "value")
			{
				return Or();
			}
			else
			{
				return null;
			}
        }

        public bool Or()
        {
			inputA = GetInputValue("inputA", inputA);
			inputB = GetInputValue("inputB", inputB);

            return inputA | inputB;
        }
    }
}
