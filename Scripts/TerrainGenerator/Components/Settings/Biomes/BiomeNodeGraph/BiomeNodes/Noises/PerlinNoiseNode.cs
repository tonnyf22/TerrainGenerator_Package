using System;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace TerrainGenerator.Components.Settings.Biomes.BiomeNodeGraph.BiomeNodes.Noises
{
    [CreateNodeMenu("Noises/Perlin Noise")]
    public class PerlinNoiseNode : Node
    {
        public enum Dimension { _2D, _3D, _4D };
        [NodeEnum] public Dimension dimension;

        [Input] public float x;
        [Input] public float z;
        [Input] public float y;
        [Input] public float w;

        [Input] public float offsetX;
        [Input] public float offsetZ;
        [Input] public float offsetY;
        [Input] public float offsetW;

        [Input] public float scale = 1.0f;

        [Output] public float value;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "value")
            {
                switch (dimension)
                {
                    case Dimension._2D:
                        return PerlinNoise2D();
                    case Dimension._3D:
                        return PerlinNoise3D();
                    case Dimension._4D:
                        return PerlinNoise4D();
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

        private float PerlinNoise2D()
        {
            x = GetInputValue("x", x);
            z = GetInputValue("z", z);
            offsetX = GetInputValue("offsetX", offsetX);
            offsetZ = GetInputValue("offsetZ", offsetZ);
            scale = GetInputValue("scale", scale);

            float2 xz = new float2(
                (x + offsetX) * scale,
                (z + offsetZ) * scale);

            return noise.cnoise(xz);
        }

        private float PerlinNoise3D()
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

            return noise.cnoise(xzy);
        }

        private float PerlinNoise4D()
        {
            x = GetInputValue("x", x);
            z = GetInputValue("z", z);
            y = GetInputValue("y", y);
            w = GetInputValue("w", w);
            offsetX = GetInputValue("offsetX", offsetX);
            offsetZ = GetInputValue("offsetZ", offsetZ);
            offsetY = GetInputValue("offsetY", offsetY);
            offsetW = GetInputValue("offsetW", offsetW);
            scale = GetInputValue("scale", scale);

            float4 xzyw = new float4(
                (x + offsetX) * scale,
                (z + offsetZ) * scale,
                (y + offsetY) * scale,
                (w + offsetW) * scale);

            return noise.cnoise(xzyw);
        }
    }
}