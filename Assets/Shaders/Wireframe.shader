Shader "Wireframe (Geometry Shader)"
{
    Properties
    {
        _Thickness("Thickness", Range(0, 0.2)) = 0.02
        _FrontColor("Front Color", color) = (1.0, 1.0, 1.0, 1.0)
    }
        SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex VSMain
            #pragma geometry GSMain
            #pragma fragment PSMain
            #pragma target 4.0

            float _Thickness;
            float4 _FrontColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct g2f
            {
                float4 vertex : SV_Position;
                float4 vertexOG: TEXCOORD0;
                float3 barycentric : BARYCENTRIC;
                float4 vertexColor : COLOR;
            };

            struct v2g
            {
                float4 vertex : SV_Position;
                fixed4 vertexColor : COLOR;
            };

            v2g VSMain(appdata v) {
                v2g o;
                o.vertex =v.vertex;
                o.vertexColor = fixed4(v.color.rgb, v.color.a);
                return o;
            }

            [maxvertexcount(3)]
            void GSMain(triangle v2g patch[3], inout TriangleStream<g2f> stream)
            {
                g2f o;
                // for each vertex convert to world space
                for (uint j = 0; j < 3; j++) patch[j].vertex = mul(unity_ObjectToWorld, patch[j].vertex);
               
                // get triangle side lengths
                float a = length(patch[0].vertex - patch[1].vertex);
                float b = length(patch[1].vertex - patch[2].vertex);
                float c = length(patch[2].vertex - patch[0].vertex);

                // determine which side is longest (shouldnt be rendered)
                float3 m = (a > b & a > c) ? float3(0,1,0) : (b > c & b > a) ? float3(0,0,1) : float3(1,0,0);

                // for each vertex
                for (uint i = 0; i < 3; i++)
                {
                    // convert vertex to screenspace
                    o.vertex = mul(UNITY_MATRIX_VP, patch[i].vertex);

                    // make some 2d vector (i % 2, i > 2 ? 1 : 0)
                    // results in (0, 0), (1, 0), (0, 1) for each i
                    float2 p = float2(fmod(i,2.0), step(2.0,i));

                    // get some coords
                    // (0, 0, 1) + m
                    // (1, 0, 0) + m
                    // (0, 1, 0) + m

                    // ends up as
                    // (0, 1, 1)
                    // (1, 1, 0)
                    // (0, 2, 0)
                    o.barycentric = float3(p, 1.0 - p.x - p.y) + m;
                    o.vertexColor = patch[i].vertexColor;
                    o.vertexOG = patch[i].vertex;
                    stream.Append(o);
                }
                stream.RestartStrip();
            }

            float4 PSMain(g2f PS) : SV_Target
            {
                float cameraDist = distance(PS.vertexOG, _WorldSpaceCameraPos);

                //if (cameraDist > 1000) {
                //    return PS.vertexColor;
               // }

                float thicknessFactor = (cameraDist * 0.06);

                // smoothstep between edge and edge + offset based on edge
                float3 coord = smoothstep(fwidth(PS.barycentric), fwidth(PS.barycentric) + (_Thickness), PS.barycentric * thicknessFactor);
                return float4(lerp(_FrontColor, PS.vertexColor, min(coord.x, min(coord.y, coord.z)).xxx), 1.0);
            }
            ENDCG
        }
    }
}