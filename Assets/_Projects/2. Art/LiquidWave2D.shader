Shader "Custom/LiquidWave2D"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _FillAmount ("Fill Amount", Range(0,1)) = 0.6
        _FillColor ("Fill Color", Color) = (0.2, 0.6, 0.9, 1)
        _SurfaceColor ("Surface Color", Color) = (0.4, 0.8, 1, 1)
        _SurfaceThickness ("Surface Thickness", Range(0, 0.1)) = 0.02
        
        _WaveAmplitude ("Wave Amplitude", Range(0, 0.15)) = 0.03
        _WaveFrequency ("Wave Frequency", Range(0, 40)) = 12
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1.5
        _WaveSecondaryAmp ("Secondary Wave Amplitude", Range(0, 0.1)) = 0.015
        _WaveSecondaryFreq ("Secondary Wave Frequency", Range(0, 60)) = 27
        _WaveSecondarySpeed ("Secondary Wave Speed", Range(0, 8)) = 2.3
        
        _DistortionStrength ("Distortion Strength", Range(0, 0.05)) = 0.01
        _DistortionFrequency ("Distortion Frequency", Range(0, 40)) = 15
        _DistortionSpeed ("Distortion Speed", Range(0, 5)) = 1.2
        
        _EdgeSoftness ("Edge Softness", Range(0.001, 0.05)) = 0.005
        
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

            #include "UnitySprites.cginc"

            float _FillAmount;
            float4 _FillColor;
            float4 _SurfaceColor;
            float _SurfaceThickness;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            float _WaveSecondaryAmp;
            float _WaveSecondaryFreq;
            float _WaveSecondarySpeed;
            float _DistortionStrength;
            float _DistortionFrequency;
            float _DistortionSpeed;
            float _EdgeSoftness;

            float WaveHeight(float x)
            {
                float w1 = sin(x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude;
                float w2 = sin(x * _WaveSecondaryFreq - _Time.y * _WaveSecondarySpeed) * _WaveSecondaryAmp;
                return w1 + w2;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;

                float wave = WaveHeight(uv.x);
                float surfaceY = _FillAmount + wave;

                float distFromSurface = surfaceY - uv.y;
                float isLiquid = smoothstep(-_EdgeSoftness, _EdgeSoftness, distFromSurface);

                float2 distortion = float2(
                    sin(uv.y * _DistortionFrequency + _Time.y * _DistortionSpeed) * _DistortionStrength,
                    cos(uv.x * _DistortionFrequency + _Time.y * _DistortionSpeed * 0.7) * _DistortionStrength
                );
                float2 distortedUV = uv + distortion * isLiquid;

                fixed4 sprite = SampleSpriteTexture(distortedUV);

                float surfaceBand = smoothstep(_SurfaceThickness, 0.0, distFromSurface);
                surfaceBand *= smoothstep(-_EdgeSoftness, _EdgeSoftness, distFromSurface);

                float3 liquidColor = _FillColor.rgb;
                liquidColor = lerp(liquidColor, _SurfaceColor.rgb, surfaceBand);

                float4 finalColor = float4(liquidColor, _FillColor.a) * sprite;
                finalColor *= isLiquid;
                finalColor *= IN.color;

                finalColor.rgb *= finalColor.a;

                return finalColor;
            }
            ENDCG
        }
    }
    Fallback "Sprites/Default"
}