Shader "Unlit/HairFollowerRibbonShader"
{
    Properties
    {
        _RootThickness("Root Thickness", Float) = 0.01
        _TipThickness("Tip Thickness", Float) = 0.002
        _Color("Color", Color) = (1, 0.5, 0.2, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZWrite On
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            StructuredBuffer<float3> FollowerPositions;

            float _RootThickness;
            float _TipThickness;
            float4 _Color;
            int _VertexCountPerStrand;

            struct appdata
            {
                uint vertexID : SV_VertexID;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                uint strandIndex = v.vertexID / (6 * (_VertexCountPerStrand - 1));
                uint segmentIndex = (v.vertexID / 6) % (_VertexCountPerStrand - 1);
                uint vertexInQuad = v.vertexID % 6;

                uint idx0 = strandIndex * _VertexCountPerStrand + segmentIndex;
                uint idx1 = strandIndex * _VertexCountPerStrand + segmentIndex + 1;

                float3 p0 = FollowerPositions[idx0];
                float3 p1 = FollowerPositions[idx1];

                float3 dir = normalize(p1 - p0);
                float3 worldMid = (p0 + p1) * 0.5;

                float3 camPos = _WorldSpaceCameraPos;
                float3 viewDir = normalize(camPos - worldMid);
                float3 right = normalize(cross(dir, viewDir));

                float t = (float)segmentIndex / (_VertexCountPerStrand - 1);
                float thickness = lerp(_RootThickness, _TipThickness, t);
                float3 offset = right * thickness * 0.5;

                float3 worldPos;
                if (vertexInQuad == 0) worldPos = p0 + offset;
                else if (vertexInQuad == 1) worldPos = p1 + offset;
                else if (vertexInQuad == 2) worldPos = p0 - offset;
                else if (vertexInQuad == 3) worldPos = p1 + offset;
                else if (vertexInQuad == 4) worldPos = p1 - offset;
                else worldPos = p0 - offset;

                o.pos = UnityObjectToClipPos(float4(worldPos, 1));
                o.normal = normalize(cross(right, dir));
                o.viewDir = viewDir;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = saturate(dot(i.normal, lightDir));
                float NdotV = saturate(dot(i.normal, i.viewDir));

                float diffuse = 0.3 + 0.7 * NdotL;
                float fresnel = pow(1.0 - NdotV, 3.0);
                float3 finalColor = _Color.rgb * diffuse + fresnel * 0.2;

                return float4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}
