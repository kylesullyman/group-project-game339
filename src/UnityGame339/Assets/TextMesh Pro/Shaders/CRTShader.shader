Shader "Custom/CRTShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Warp ("Warp Amount", Range(0,0.2)) = 0.01
        _ScanlineIntensity ("Scanlines", Range(0,1)) = 0.18
        _NoiseIntensity ("Noise", Range(0,1)) = 0.05
        _DarkenEdges ("Darken Edges", Range(0,1)) = 0.18
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Warp;
            float _ScanlineIntensity;
            float _NoiseIntensity;
            float _DarkenEdges;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float2 centered = uv * 2.0 - 1.0;
                centered *= 1.0 + (_Warp * 0.5) * dot(centered, centered);
                uv = centered * 0.5 + 0.5;

                if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
                    return fixed4(0, 0, 0, 0);

                fixed4 col = tex2D(_MainTex, uv) * i.color;

                float scanline = sin(uv.y * 650.0) * 0.5 + 0.5;
                col.rgb -= scanline * _ScanlineIntensity;

                float noise = rand(uv * _Time.y * 60.0) * _NoiseIntensity;
                col.rgb += noise;

                float2 edge = abs(centered);
                float vignette = saturate(1.0 - dot(edge, edge) * _DarkenEdges);
                col.rgb *= vignette;

                return col;
            }
            ENDCG
        }
    }
}
