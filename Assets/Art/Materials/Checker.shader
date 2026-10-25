Shader "Unlit/Checker"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,1)
        _Tiling ("Tiling (X,Y)", Vector) = (8,8,0,0)
        _Offset ("Offset (X,Y)", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        Cull Back
        ZWrite On
        ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv  : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            float4 _ColorA;
            float4 _ColorB;
            float4 _Tiling; // xy usados
            float4 _Offset; // xy usados

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv * _Tiling.xy + _Offset.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Casilla según suma de celdas enteras
                float2 cell = floor(i.uv);
                float odd   = fmod(cell.x + cell.y, 2.0);
                return lerp(_ColorA, _ColorB, odd);
            }
            ENDCG
        }
    }
}
