Shader "Custom/MovingCablePulse"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.15, 0.7, 1.0, 1.0)
        _Speed ("Speed (cycles/second)", Range(0, 5)) = 0.6
        _Width ("Pulse Width", Range(0.01, 0.5)) = 0.15
        _Strength ("Bulge Strength (mesh units)", Range(0, 1)) = 0.12
        [Enum(UV_Y,0,Local_X,1,Local_Y,2,Local_Z,3)]
        _Direction ("Travel Along", Float) = 0
        _Start ("Travel Start", Float) = 0
        _End ("Travel End", Float) = 1
    }

    // Universal Render Pipeline (URP): no textures, no lighting dependencies.
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "Queue" = "Geometry" }
        Pass
        {
            Name "MovingPulseURP"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _Speed;
                float _Width;
                float _Strength;
                float _Direction;
                float _Start;
                float _End;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float GetTravelCoord(Attributes v)
            {
                if (_Direction < 0.5) return v.uv.y;
                if (_Direction < 1.5) return v.positionOS.x;
                if (_Direction < 2.5) return v.positionOS.y;
                return v.positionOS.z;
            }

            Varyings Vert(Attributes v)
            {
                Varyings o;
                float span = _End - _Start;
                if (abs(span) < 0.0001) span = 0.0001;

                float along = (GetTravelCoord(v) - _Start) / span;
                float width = max(_Width, 0.001);
                float center = lerp(-width, 1.0 + width, frac(_Time.y * _Speed));

                // Smooth, rounded swelling that moves from the beginning to the end.
                float bump = saturate(1.0 - abs(along - center) / width);
                bump = bump * bump * (3.0 - 2.0 * bump);

                float3 moved = v.positionOS.xyz + normalize(v.normalOS) * (bump * _Strength);
                o.positionCS = TransformObjectToHClip(moved);
                return o;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                return _BaseColor;
            }
            ENDHLSL
        }
    }

    // Built-in Render Pipeline (Unity 6 and earlier).
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        Pass
        {
            Name "MovingPulseBuiltIn"
            Cull Back
            ZWrite On

            CGPROGRAM
            #pragma target 2.0
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"

            fixed4 _BaseColor;
            float _Speed;
            float _Width;
            float _Strength;
            float _Direction;
            float _Start;
            float _End;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float GetTravelCoord(Attributes v)
            {
                if (_Direction < 0.5) return v.uv.y;
                if (_Direction < 1.5) return v.positionOS.x;
                if (_Direction < 2.5) return v.positionOS.y;
                return v.positionOS.z;
            }

            Varyings Vert(Attributes v)
            {
                Varyings o;
                float span = _End - _Start;
                if (abs(span) < 0.0001) span = 0.0001;

                float along = (GetTravelCoord(v) - _Start) / span;
                float width = max(_Width, 0.001);
                float center = lerp(-width, 1.0 + width, frac(_Time.y * _Speed));

                float bump = saturate(1.0 - abs(along - center) / width);
                bump = bump * bump * (3.0 - 2.0 * bump);

                float3 moved = v.positionOS.xyz + normalize(v.normalOS) * (bump * _Strength);
                o.positionCS = UnityObjectToClipPos(float4(moved, 1.0));
                return o;
            }

            fixed4 Frag(Varyings i) : SV_Target
            {
                return _BaseColor;
            }
            ENDCG
        }
    }

    FallBack Off
}
