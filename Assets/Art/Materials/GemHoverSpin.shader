Shader "Custom/GemHoverSpin"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _SpinSpeed ("Y Spin Speed (degrees/sec)", Float) = 120
        _HoverHeight ("Hover Height", Float) = 0.25
        _HoverSpeed ("Hover Speed (cycles/sec)", Float) = 0.65
        _Phase ("Phase Offset (radians)", Float) = 0
        _Pivot ("Mesh Center (local XYZ)", Vector) = (0,0,0,0)
        _FacetShading ("Facet Shading (0 = unlit)", Range(0,1)) = 0.7
    }

    // Unity URP. Rotate in WORLD XZ around the real mesh center.
    // The center's WORLD X and Z never change; only WORLD Y bobs.
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "GemHoverSpinURP"
            Tags { "LightMode"="SRPDefaultUnlit" }
            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _BaseColor;
                float _SpinSpeed;
                float _HoverHeight;
                float _HoverSpeed;
                float _Phase;
                float4 _Pivot;
                float _FacetShading;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos   : TEXCOORD0;
                float2 uv         : TEXCOORD1;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                float3 centerWS = TransformObjectToWorld(_Pivot.xyz);
                float3 vertexWS = TransformObjectToWorld(v.positionOS);
                float3 relative = vertexWS - centerWS;

                float angle = _Time.y * _SpinSpeed * 0.0174532925199433 + _Phase;
                float s = sin(angle);
                float c = cos(angle);

                // EXCLUSIVELY rotates around WORLD Y: no X- or Z-axis rotation.
                // The pivot itself never moves sideways, even if the mesh origin is offset.
                float3 rotated = float3(
                    relative.x * c + relative.z * s,
                    relative.y,
                    -relative.x * s + relative.z * c
                );
                float3 finalWS = centerWS + rotated;
                finalWS.y += sin(_Time.y * _HoverSpeed * 6.283185307179586 + _Phase) * _HoverHeight;

                o.positionCS = TransformWorldToHClip(finalWS);
                o.worldPos = finalWS;
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * _BaseColor;
                float3 n = normalize(cross(ddx(i.worldPos), ddy(i.worldPos)) + float3(1e-6, 1e-6, 1e-6));
                float shade = 0.5 + 0.5 * abs(dot(n, normalize(float3(0.55, 0.85, 0.35))));
                col.rgb *= lerp(1.0, shade, _FacetShading);
                return half4(col.rgb, col.a);
            }
            ENDHLSL
        }
    }

    // Unity Built-in Render Pipeline: same world-Y-only movement.
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "GemHoverSpinBuiltIn"
            Cull Back
            ZWrite On

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _BaseColor;
            float _SpinSpeed;
            float _HoverHeight;
            float _HoverSpeed;
            float _Phase;
            float4 _Pivot;
            float _FacetShading;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos   : TEXCOORD0;
                float2 uv         : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 centerWS = mul(unity_ObjectToWorld, float4(_Pivot.xyz, 1.0)).xyz;
                float3 vertexWS = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 relative = vertexWS - centerWS;

                float angle = _Time.y * _SpinSpeed * 0.0174532925199433 + _Phase;
                float s = sin(angle);
                float c = cos(angle);

                float3 rotated = float3(
                    relative.x * c + relative.z * s,
                    relative.y,
                    -relative.x * s + relative.z * c
                );
                float3 finalWS = centerWS + rotated;
                finalWS.y += sin(_Time.y * _HoverSpeed * 6.283185307179586 + _Phase) * _HoverHeight;

                o.positionCS = mul(UNITY_MATRIX_VP, float4(finalWS, 1.0));
                o.worldPos = finalWS;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _BaseColor;
                float3 n = normalize(cross(ddx(i.worldPos), ddy(i.worldPos)) + float3(1e-6, 1e-6, 1e-6));
                float shade = 0.5 + 0.5 * abs(dot(n, normalize(float3(0.55, 0.85, 0.35))));
                col.rgb *= lerp(1.0, shade, _FacetShading);
                return fixed4(col.rgb, col.a);
            }
            ENDCG
        }
    }

    Fallback Off
}
