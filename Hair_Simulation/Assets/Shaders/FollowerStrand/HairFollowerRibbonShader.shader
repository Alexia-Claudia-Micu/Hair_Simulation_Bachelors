Shader "Unlit/HairFollowerRibbonShader"
{
    Properties
    {
        _RootThickness ("Root Thickness", Float) = 0.01
        _TipThickness ("Tip Thickness", Float) = 0.002
        _Color ("Color", Color) = (1, 0.5, 0.2, 1)
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
            int _VertexCountPerStrand; // Must be set from C# to match!

            struct appdata
            {
                uint vertexID : SV_VertexID;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
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

                // Camera-facing normal:
                float3 cameraForward = normalize(UnityWorldSpaceViewDir((p0 + p1) * 0.5));
                float3 right = normalize(cross(dir, cameraForward));
    
                // Thickness tapering
                float t = (float)segmentIndex / (_VertexCountPerStrand - 1);
                float thickness = lerp(_RootThickness, _TipThickness, t);
                float3 offset = right * thickness * 0.5;

                if (vertexInQuad == 0) o.pos = UnityObjectToClipPos(float4(p0 + offset, 1));
                if (vertexInQuad == 1) o.pos = UnityObjectToClipPos(float4(p1 + offset, 1));
                if (vertexInQuad == 2) o.pos = UnityObjectToClipPos(float4(p0 - offset, 1));
                if (vertexInQuad == 3) o.pos = UnityObjectToClipPos(float4(p1 + offset, 1));
                if (vertexInQuad == 4) o.pos = UnityObjectToClipPos(float4(p1 - offset, 1));
                if (vertexInQuad == 5) o.pos = UnityObjectToClipPos(float4(p0 - offset, 1));

                o.normal = normalize(cross(right, dir));
                return o;
            }


            float4 frag(v2f i) : SV_Target
            {
                float NdotL = saturate(dot(i.normal, normalize(float3(0.3, 1, 0.2))));
                float brightness = 0.3 + 0.7 * NdotL;
                return _Color * brightness;
            }
            ENDCG
        }
    }
}
