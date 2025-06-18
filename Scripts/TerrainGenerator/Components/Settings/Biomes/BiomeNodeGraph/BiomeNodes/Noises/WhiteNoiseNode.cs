using System;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises
{
	[CreateNodeMenu("Noises/White Noise")]
	public class WhiteNoiseNode : Node
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

		[Output] public float value;

		private int seed = 1337;

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == "value")
			{
				switch (dimension)
				{
					case Dimension._2D:
						return WhiteNoise2D();
					case Dimension._3D:
						return WhiteNoise3D();
					default:
						throw new ArgumentOutOfRangeException(
							nameof(dimension),
							dimension,
							"Unsupported Dimension.");
				}
			}
			else
			{
				return null;
			}
		}

		private float WhiteNoise2D()
		{
			x = GetInputValue("x", x);
			z = GetInputValue("z", z);
			offsetX = GetInputValue("offsetX", offsetX);
			offsetZ = GetInputValue("offsetZ", offsetZ);
			scale = GetInputValue("scale", scale);

			float2 xz = new float2(
				(x + offsetX) * scale,
				(z + offsetZ) * scale);

			return WhiteNoise(xz);
		}

		private float WhiteNoise3D()
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

			return WhiteNoise(xzy);
		}

		private float WhiteNoise(float2 xz)
		{
            float[] data = new float[] { xz.x, xz.y };
            float value = Value(data);

            return value;
		}

		private float WhiteNoise(float3 xzy)
		{
            float[] data = new float[] { xzy.x, xzy.z, xzy.y };
            float value = Value(data);

            return value;
		}

		private float Value(float[] inputData)
		{
            return NormalizedHashFromData(inputData);
		}

        private float NormalizedHashFromData(float[] inputData)
        {
            int[] xRawInts = FloatsToRawInts(inputData);

            int hash;
            hash = MakeInitialCombinedHash(xRawInts);
            hash = XORShift(hash);

            float normalizedHash = NormalizeHash(hash);
            return normalizedHash;
        }

        private int[] FloatsToRawInts(float[] inputData)
        {
            int[] xRawInts = new int[inputData.Length];

            for (int index = 0; index < xRawInts.Length; index++)
            {
                xRawInts[index] = BitConverter.SingleToInt32Bits(inputData[index]);
            }

            return xRawInts;
        }

        private int MakeInitialCombinedHash(int[] xRawInts)
        {
            int hash = 0;
            for (int index = 0; index < xRawInts.Length; index++)
            {
                hash ^= 
                    xRawInts[index] *
					seed;
            }
            return hash;
        }

        private int XORShift(int hash)
        {
            hash ^= hash >> 13;
            hash ^= hash << 13;
            hash ^= hash >> 17;
            hash ^= hash << 5;
            return hash;
        }

        private float NormalizeHash(int hash)
        {
            return hash / (float)int.MaxValue;
        }
	}
}