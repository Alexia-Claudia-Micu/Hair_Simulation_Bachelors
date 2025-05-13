
Shader "Unlit/HairFollowerShader"
{
    Properties { }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            ZWrite On
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            StructuredBuffer<float3> FollowerPositions;

            float4 vert(uint id : SV_VertexID) : SV_POSITION
            {
                float3 worldPos = FollowerPositions[id];
                return UnityObjectToClipPos(float4(worldPos, 1.0));
            }

            float4 frag() : SV_Target
            {
                return float4(1, 0.5, 0.2, 1); // Orange color
            }
            ENDCG
        }
    }
}  