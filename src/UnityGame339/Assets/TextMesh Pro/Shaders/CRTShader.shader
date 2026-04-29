Shader "Custom/CRTShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Warp ("Warp Amount", Range(0,0.5)) = 0.15
        _ScanlineIntensity ("Scanlines", Range(0,1)) = 0.3
        _NoiseIntensity ("Noise", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Warp;
            float _ScanlineIntensity;
            float _NoiseIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy,float2(12.9898,78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // CRT curve (screen warp)
                float2 centered = uv * 2 - 1;
                centered *= 1 + _Warp * dot(centered, centered);
                uv = centered * 0.5 + 0.5;

                // Sample screen
                fixed4 col = tex2D(_MainTex, uv);

                // Scanlines
                float scan = sin(uv.y * 800) * _ScanlineIntensity;
                col.rgb -= scan;

                // Static noise
                float noise = rand(uv + _Time.y) * _NoiseIntensity;
                col.rgb += noise;

                return col;
            }
            ENDCG
        }
    }
}
