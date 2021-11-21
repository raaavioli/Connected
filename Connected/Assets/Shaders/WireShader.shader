Shader "Custom/WireShader"
{
    Properties
    {
        _StartColor("Start Color", Color) = (1,1,1,1)
        _EndColor("End Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _StreakTex("Streak", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        [MaterialToggle] _Connected("Connected", Float) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _StreakTex;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_StreakTex;
            };

            half _Glossiness;
            fixed4 _StartColor;
            fixed4 _EndColor;
            int _Connected;

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                fixed4 c = tex2D(_StreakTex, IN.uv_StreakTex + float2(_Time.w, 0.0f));
                o.Albedo = lerp(_StartColor, _EndColor, IN.uv_MainTex.x * 0.9);
                o.Emission = _Connected * c * abs(_SinTime.w);

                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
