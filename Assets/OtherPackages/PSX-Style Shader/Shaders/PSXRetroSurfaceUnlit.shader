Shader "FX/PSX Retro Surface Unlit"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        [Header(Local Multipliers)]
        _LocalSnapStrength("Local Snap Strength", Range(0,1)) = 1
        _LocalUvSnapStrength("Local UV Snap Strength", Range(0,1)) = 1
        [HideInInspector] _LocalDepthStrength("Local Depth Strength", Range(0,1)) = 0
    }

        SubShader
        {
            Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }
            LOD 200

            Pass
            {
                

                CGPROGRAM
                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile __ PSX_AFFINE

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                float4 _Color;

                float _LocalSnapStrength;
                float _LocalUvSnapStrength;
                float _LocalDepthStrength;

                float _PSX_Enabled;

                float4 _PSX_SnapResolution;
                float _PSX_UseScreenResolution;
                float _PSX_SnapStrength;
                float _PSX_JitterSpeed;
                float _PSX_JitterPixels;

                float _PSX_DepthSteps;
                float _PSX_DepthStrength;
                float _PSX_DepthNoise;

                float _PSX_UvPrecision;
                float _PSX_UvSnapStrength;
                float _PSX_UvWarpStrength;
                float _PSX_AffineSwim;

                float _PSX_MipBias;


                float _PSX_ColorBits;
                float _PSX_DitherStrength;

                #if (defined(SHADER_API_D3D11) || defined(SHADER_API_GLCORE) || defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN))
                #define PSX_HAS_NOPERSP 1
                #else
                #define PSX_HAS_NOPERSP 0
                #endif

                #if defined(PSX_AFFINE) && PSX_HAS_NOPERSP
                #define PSX_INTERP noperspective
                #else
                #define PSX_INTERP
                #endif

                float Hash12(float2 p)
                {
                    float h = dot(p, float2(127.1, 311.7));
                    return frac(sin(h) * 43758.5453123);
                }

                float Bayer4(float2 pixel)
                {
                    float2 p = floor(pixel);
                    float x = fmod(p.x, 4.0);
                    float y = fmod(p.y, 4.0);

                    float v0 = (x == 0.0) ? 0.0 : ((x == 1.0) ? 8.0 : ((x == 2.0) ? 2.0 : 10.0));
                    float v1 = (x == 0.0) ? 12.0 : ((x == 1.0) ? 4.0 : ((x == 2.0) ? 14.0 : 6.0));
                    float v2 = (x == 0.0) ? 3.0 : ((x == 1.0) ? 11.0 : ((x == 2.0) ? 1.0 : 9.0));
                    float v3 = (x == 0.0) ? 15.0 : ((x == 1.0) ? 7.0 : ((x == 2.0) ? 13.0 : 5.0));

                    float v = (y == 0.0) ? v0 : ((y == 1.0) ? v1 : ((y == 2.0) ? v2 : v3));
                    return (v / 16.0) - 0.5;
                }

                float2 GetSnapResolution(float enabled)
                {
                    float2 baseRes = max(_PSX_SnapResolution.xy, 1.0);
                    float2 screenRes = max(_ScreenParams.xy, 1.0);
                    float useScreen = lerp(0.0, saturate(_PSX_UseScreenResolution), enabled);

                    return lerp(baseRes, screenRes, useScreen);
                }

                float2 GetFrameJitterOffsetNdc(float2 res, float enabled)
                {
                    return 0.0.xx;
                }

                float2 SnapNdc(float2 ndc, float2 res)
                {
                    float2 pixelStep = 2.0 / res;
                    return floor(ndc / pixelStep + 0.5) * pixelStep;
                }

                float Quantize01(float v, float steps)
                {
                    float s = max(steps, 1.0);
                    return floor(v * s + 0.5) / s;
                }

                float3 QuantizeRgb(float3 rgb, float bits, float dither)
                {
                    float b = clamp(bits, 1.0, 8.0);
                    float levels = (pow(2.0, b) - 1.0);

                    float3 x = saturate(rgb);
                    x = (x * levels) + dither;
                    x = floor(x + 0.5);
                    return saturate(x / levels);
                }

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float4 screenPos : TEXCOORD0;
                    PSX_INTERP float2 uv : TEXCOORD1;

                };

                v2f vert(appdata v)
                {
                    v2f o = (v2f)0;

                    float enabled = step(0.5, _PSX_Enabled);

                    float2 res = GetSnapResolution(enabled);
                    float2 jitter = GetFrameJitterOffsetNdc(res, enabled);

                    float snapStrength = saturate(_PSX_SnapStrength * _LocalSnapStrength) * enabled;

                    float depthStrength = saturate(_PSX_DepthStrength * _LocalDepthStrength) * enabled;
                    float depthSteps = max(_PSX_DepthSteps, 1.0);

                    float4 viewPos = mul(UNITY_MATRIX_MV, v.vertex);

                    float viewZ = viewPos.z;
                    float viewZ01 = saturate((-viewZ) / max(_ProjectionParams.z, 0.001));
                    float quantZ01 = Quantize01(viewZ01, depthSteps);
                    float quantZ = -quantZ01 * _ProjectionParams.z;

                    float depthNoise = (Hash12(v.vertex.xy * 17.13) - 0.5) * _PSX_DepthNoise * enabled;
                    float finalZ = lerp(viewZ, quantZ + depthNoise, depthStrength);

                    viewPos.z = finalZ;

                    float4 clip = mul(UNITY_MATRIX_P, viewPos);
                    float w = max(abs(clip.w), 1e-6);
                    float2 ndc = clip.xy / w;

                    float2 snappedNdc = SnapNdc(ndc + jitter, res);
                    float2 finalNdc = lerp(ndc, snappedNdc, snapStrength);

                    clip.xy = finalNdc * w;
                    o.pos = clip;
                    o.screenPos = ComputeScreenPos(o.pos);

                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float enabled = step(0.5, _PSX_Enabled);

                    float2 screenUv = i.screenPos.xy / max(i.screenPos.w, 1e-6);
                    float2 pixel = screenUv * _ScreenParams.xy;

                    float dither = Bayer4(pixel) * saturate(_PSX_DitherStrength);

                    float2 uv = i.uv;

                    float2 texSize = max(_MainTex_TexelSize.zw, 1.0);
                    float texScale = max(texSize.x, texSize.y) / 256.0;
                    float uvPrecision = max(_PSX_UvPrecision * texScale, 1.0);

                    float uvSnapStrength = saturate(_PSX_UvSnapStrength) * saturate(_LocalUvSnapStrength) * enabled;

                    float2 snappedUv = floor(uv * uvPrecision + 0.5) / uvPrecision;
                    uv = lerp(uv, snappedUv, uvSnapStrength);

                    float uvWarpStrength = saturate(_PSX_UvWarpStrength) * enabled;
                    float2 cell = floor(uv * uvPrecision + 0.5);
                    float2 warp = float2(Hash12(cell), Hash12(cell + 19.31)) - 0.5;
                    uv += warp * uvWarpStrength * (1.0 / uvPrecision);

                    float swim = saturate(_PSX_AffineSwim) * enabled;
                    uv += (Hash12(float2(i.screenPos.w, uv.x) * 3.1) - 0.5) * swim * (1.0 / uvPrecision);

                    float4 albedoTex = tex2Dbias(_MainTex, float4(uv, 0.0, _PSX_MipBias));
                    float3 albedo = albedoTex.rgb * _Color.rgb;

                    float3 color = albedo;

                    color = QuantizeRgb(color, _PSX_ColorBits, dither * enabled);

                    return fixed4(color, 1.0);
                }
                ENDCG
            }
        }
            FallBack Off
}
