Shader "Custom/AlwaysOnTopShader"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Overlay"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Cull Off
        Lighting Off

        Pass
        {
            SetTexture [_MainTex]
            {
                constantColor [_Color]
                combine texture * constant
            }
        }
    }
}