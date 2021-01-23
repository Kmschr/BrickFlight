﻿Shader "Custom/StandardVertex" {
    Properties{
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _WireColor("Color", Color)=(0,0,0,1)
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard vertex:vert fullforwardshadows
            #pragma target 3.0
            struct Input {
                float2 uv_MainTex;
                float4 vertexColor; // Vertex color stored here by vert() method
            };

            struct v2f {
              float4 pos : SV_POSITION;
              fixed4 color : COLOR;
            };

            void vert(inout appdata_full v, out Input o)
            {
                UNITY_INITIALIZE_OUTPUT(Input,o);
                o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
            }

            half _Glossiness;
            half _Metallic;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Albedo comes from a texture tinted by color
                o.Albedo = IN.vertexColor; // Combine normal color with the vertex color
                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = IN.vertexColor[3];
            }
            ENDCG
        }
        FallBack "Diffuse"

}