Shader "Custom/LiquidSurface"
{
    Properties
    {
        _HeightMap("Ripple HeightMap", 2D) = "black" {}
        _MainColor("Water Color", Color) = (0.2, 0.4, 0.8, 1)
        _Displace("Displacement Strength", Range(0, 0.5)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _HeightMap;
            float4 _HeightMap_ST;
            float4 _MainColor;
            float _Displace;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float2 uv = TRANSFORM_TEX(IN.uv, _HeightMap);

                // Sample ripple height
                float h = tex2Dlod(_HeightMap, float4(uv, 0, 0)).r;

                // Apply displacement to vertex height
                float3 displaced = IN.positionOS.xyz;
                displaced.y += h * _Displace;

                OUT.positionHCS = TransformObjectToHClip(displaced);
                OUT.uv = uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Simple flat water color, can extend with lighting/reflections later
                return _MainColor;
            }
            ENDHLSL
        }
    }
}