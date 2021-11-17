Shader "Unlit/ToolHighlight"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _BorderWidth ("Border width", Range(0, 1)) = 0.1666
        _BorderFade ("Fade", Range(0, 1)) = 0
        _CheckeredScale ("CheckeredScale", Range(1, 20)) = 10
        _ObjectScale ("Scale", Vector) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "/Assets/Shaders/Utilities.cginc"

            struct VertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct FragInput
            {
                float4 vertex : SV_POSITION;
                float2 centeredUV : TEXCOORD0;
                float2 checkeredUV : TEXCOORD1;
            };

            float4 _Color;
            float _BorderWidth;
            float _BorderFade;
            float _CheckeredScale;
            float4 _ObjectScale;

            FragInput vert (VertexInput v)
            {
                FragInput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float aspect = _ObjectScale.x / _ObjectScale.y;
                o.centeredUV = (v.uv * 2 - 1);
                o.checkeredUV = o.centeredUV;
                o.checkeredUV.x *= max(aspect, 1.0);
                o.checkeredUV.y *= max(1 / aspect, 1.0);
                return o;
            }

            #define TAU 6.28318530718
            #define PI 3.14159265359
   
            #define CARD_WIDTH 300
            #define CARD_HEIGHT 216
            #define CARD_ASPECT (CARD_WIDTH / CARD_HEIGHT)

            fixed4 frag(FragInput i) : SV_Target
            {
                float len = saturate(length(i.centeredUV));
                float distToXEdge = 1 / 6.0;
                float distToYEdge = distToXEdge / CARD_ASPECT;
                float outerBorder = smoothstep(_BorderFade * distToXEdge, distToXEdge, 1 - abs(i.centeredUV.x));
                outerBorder *= smoothstep(_BorderFade * distToYEdge, distToYEdge, 1 - abs(i.centeredUV.y));

                float innerBorder = smoothstep(_BorderWidth + _BorderFade * distToXEdge, _BorderWidth + distToXEdge, 1 - abs(i.centeredUV.x));
                innerBorder *= smoothstep(_BorderWidth + _BorderFade * distToYEdge, _BorderWidth + distToYEdge, 1 - abs(i.centeredUV.y));
                innerBorder = 1 - innerBorder;
                float border = min(innerBorder, outerBorder);

                float xChess = smoothstep(0.5f - 0.1, 0.5f + 0.1, (sin(_CheckeredScale * TAU * i.checkeredUV.x) + 1) / 2.f);
                float yChess = smoothstep(0.5f - 0.1, 0.5f + 0.1, (sin(_CheckeredScale * TAU * i.checkeredUV.y) + 1) / 2.f);
                float checkeredPattern = saturate(abs(xChess - yChess));

                float pattern = checkeredPattern * border;
                _Color.a *= pattern * 0.8f;
                return _Color;
            }
            ENDCG
        }
    }
}
