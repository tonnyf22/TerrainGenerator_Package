using System;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises
{
	[CreateNodeMenu("Noises/Cellular Noise")]
	public class CellularNoiseNode : Node
	{
		public enum Dimension { _2D, _3D };
		[NodeEnum] public Dimension dimension;

		[Input] public float x;
		[Input] public float z;
		[Input] public float y;

		[Input] public float offsetX;
		[Input] public float offsetZ;
		[Input] public float offsetY;

		[Input] public float scale = 1.0f;

		[Output] public float valueF1;
		[Output] public float valueF2;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "valueF1" || port.fieldName == "valueF2")
			{
				float2 value;

				switch (dimension)
				{
					case Dimension._2D:
						value = CellularNoise2D();
						break;
					case Dimension._3D:
						value = CellularNoise3D();
						break;
					default:
						throw new ArgumentOutOfRangeException(
							nameof(dimension),
							dimension,
							"Unsupported Dimension.");
				}

				switch (port.fieldName)
				{
					case "valueF1":
						return value.x;
					case "valueF2":
						return value.y;
					default:
						throw new ArgumentOutOfRangeException(
							nameof(port.fieldName),
							port.fieldName,
							"Unsupported PortFieldName.");
				}
			}
			else
			{
				return null;
			}
		}

		private float2 CellularNoise2D()
		{
			x = GetInputValue("x", x);
			z = GetInputValue("z", z);
			offsetX = GetInputValue("offsetX", offsetX);
			offsetZ = GetInputValue("offsetZ", offsetZ);
			scale = GetInputValue("scale", scale);

			float2 xz = new float2(
				(x + offsetX) * scale,
				(z + offsetZ) * scale);

			return noise.cellular(xz);
		}

		private float2 CellularNoise3D()
		{
			x = GetInputValue("x", x);
			z = GetInputValue("z", z);
			y = GetInputValue("y", y);
			offsetX = GetInputValue("offsetX", offsetX);
			offsetZ = GetInputValue("offsetZ", offsetZ);
			offsetY = GetInputValue("offsetY", offsetY);
			scale = GetInputValue("scale", scale);

			float3 xzy = new float3(
				(x + offsetX) * scale,
				(z + offsetZ) * scale,
				(y + offsetY) * scale);

			return noise.cellular(xzy);
		}
	}
}