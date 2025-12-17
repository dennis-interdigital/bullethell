Shader "Custom/URPBackgroundBlur"
{
    Properties
    {
        _BlurSize ("Blur Size (px)", Range(0, 8)) = 3
        _Tint     ("Tint", Color) = (1,1,1,0.6)
        _RadiusMul("Screen Radius Mult", Range(0.5,2)) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            Name "ForwardUnlit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float _BlurSize;
            float4 _Tint;
            float _RadiusMul;

            struct appdata {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float4 positionHCS : SV_POSITION;
                float4 screenPos   : TEXCOORD0; // for screen UV
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.screenPos = ComputeScreenPos(o.positionHCS);
                return o;
            }

            float2 GetScreenUV(v2f i)
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;
                #if UNITY_UV_STARTS_AT_TOP
                    uv.y = 1 - uv.y;
                #endif
                return uv;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 uv = GetScreenUV(i);

                // convert pixel radius to UV radius
                float2 texel = _BlurSize * _RadiusMul / _ScreenParams.xy;

                // 9-tap blur of the opaque texture behind
                half4 c  = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv) * 4;
                c += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2( texel.x, 0));
                c += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2(-texel.x, 0));
                c += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2(0,  texel.y));
                c += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2(0, -texel.y));
                c += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2( texel.x,  texel.y));
                c += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2(-texel.x,  texel.y));
                c += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2( texel.x, -texel.y));
                c += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2(-texel.x, -texel.y));

                c /= 13.0;
                return c * _Tint;
            }
            ENDHLSL
        }
    }
}
