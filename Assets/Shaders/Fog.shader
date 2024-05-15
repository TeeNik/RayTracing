// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Fog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
		        float4 scrPos:TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
		        o.pos = UnityObjectToClipPos(v.vertex);
		        o.scrPos = ComputeScreenPos(o.pos);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
			fixed4 _FogColor;
			float _FogDensity;
            
            fixed4 frag (v2f i) : SV_Target
            {
                float depthValue = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);
	            half4 depth;
            
	            depth.r = depthValue;
	            depth.g =  depthValue;
	            depth.b = depthValue;
            
	            depth.a = 1;

                float4 color = 1;
                if (distance(_WorldSpaceCameraPos, i.worldPos) > 10)
                {
                    color = 0;
                }
                
	            return color;
            }
            ENDCG
        }
    }
}
