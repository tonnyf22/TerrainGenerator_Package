Shader "Custom/TerrainGeneratorShader"
{
    Properties
    {
        _BaseColor   ("Base Color", Color)        = (1,1,1,1)
        _Tile        ("UV Tiling", Vector)        = (10,10,0,0)

        _UnderTex    ("Below-Zero Texture", 2D)   = "white" {}

        _BiomeTex0   ("Biome 0 Texture", 2D)      = "white" {}
        _BiomeTex1   ("Biome 1 Texture", 2D)      = "white" {}
        _BiomeTex2   ("Biome 2 Texture", 2D)      = "white" {}
        _BiomeTex3   ("Biome 3 Texture", 2D)      = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        LOD 200

        // ——— Forward pass: textured + lit + receives shadows ———
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Uniforms
            float4 _BaseColor;
            float4 _Tile;

            TEXTURE2D(_UnderTex);    SAMPLER(sampler_UnderTex);
            TEXTURE2D(_BiomeTex0);   SAMPLER(sampler_BiomeTex0);
            TEXTURE2D(_BiomeTex1);   SAMPLER(sampler_BiomeTex1);
            TEXTURE2D(_BiomeTex2);   SAMPLER(sampler_BiomeTex2);
            TEXTURE2D(_BiomeTex3);   SAMPLER(sampler_BiomeTex3);

            // Vertex input
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                float2 uv2        : TEXCOORD1;
            };

            // Data passed to fragment
            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;     // unused for biome, but kept if needed
                float2 uv2          : TEXCOORD1;     // biome index
                float3 worldPos     : TEXCOORD2;
                float3 normalWS     : NORMAL;
                float4 shadowCoord  : TEXCOORD3;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // clip‐space pos
                OUT.positionCS  = TransformObjectToHClip(IN.positionOS);
                // pass-through UVs (optional)
                OUT.uv          = IN.uv;
                // biome index
                OUT.uv2         = IN.uv2;
                // world‐space data
                OUT.normalWS    = TransformObjectToWorldNormal(IN.normalOS);
                OUT.worldPos    = TransformObjectToWorld(IN.positionOS);

                // build perspective-correct shadow coord
                VertexPositionInputs vIn;
                vIn.positionCS = OUT.positionCS;
                vIn.positionWS = OUT.worldPos;
                OUT.shadowCoord = GetShadowCoord(vIn);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // world-space XY → tiled UV for all terrain texturing
                float2 uvW = IN.worldPos.xz * _Tile.xy;

                // 1) Underwater override?
                if (IN.worldPos.y < 0.0)
                {
                    // always use _UnderTex below zero
                    half4 underCol = SAMPLE_TEXTURE2D(_UnderTex, sampler_UnderTex, uvW);

                    // lighting
                    float3 N       = normalize(IN.normalWS);
                    Light   Ld     = GetMainLight();
                    float3 L       = normalize(Ld.direction);
                    float  NdotL   = saturate(dot(N, L));
                    float  shadow  = MainLightRealtimeShadow(IN.shadowCoord);
                    float3 ambient = underCol.rgb * 0.1 * _BaseColor.rgb;
                    float3 diffuse = underCol.rgb * _BaseColor.rgb * Ld.color.rgb * NdotL * shadow;

                    return half4(ambient + diffuse, underCol.a * _BaseColor.a);
                }

                // 2) Normal biome splat path
                int biomeIndex = (int)floor(IN.uv2.x * 10.0 + 0.5);

                half4 splat;
                switch (biomeIndex)
                {
                    case 0: splat = SAMPLE_TEXTURE2D(_BiomeTex0, sampler_BiomeTex0, uvW); break;
                    case 1: splat = SAMPLE_TEXTURE2D(_BiomeTex1, sampler_BiomeTex1, uvW); break;
                    case 2: splat = SAMPLE_TEXTURE2D(_BiomeTex2, sampler_BiomeTex2, uvW); break;
                    default: splat = SAMPLE_TEXTURE2D(_BiomeTex3, sampler_BiomeTex3, uvW); break;
                }

                // lighting
                float3 N       = normalize(IN.normalWS);
                Light   Ld     = GetMainLight();
                float3 L       = normalize(Ld.direction);
                float  NdotL   = saturate(dot(N, L));
                float  shadow  = MainLightRealtimeShadow(IN.shadowCoord);
                float3 ambient = splat.rgb * 0.1 * _BaseColor.rgb;
                float3 diffuse = splat.rgb * _BaseColor.rgb * Ld.color.rgb * NdotL * shadow;

                return half4(ambient + diffuse, splat.a * _BaseColor.a);
            }
            ENDHLSL
        }

        // ——— Shadow Caster pass: writes depth into URP’s shadow maps ———
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct AttributesShadow
            {
                float3 positionOS : POSITION;
            };

            struct VaryingsShadow
            {
                float4 positionCS : SV_POSITION;
            };

            VaryingsShadow vertShadow(AttributesShadow IN)
            {
                VaryingsShadow OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                return OUT;
            }

            half4 fragShadow(VaryingsShadow IN) : SV_Target
            {
                // depth only; color doesn’t matter
                return 0;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
