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
                float2 uv : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
		        o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            // float4x4 _CameraToWorld;
            float4x4 _CameraInverseProjection;
            
			fixed4 _FogColor;
			fixed4 _FogColor2;
			float _FogDensity;
			float _Power;

            float Remap(float value, float from1, float to1, float from2, float to2) {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r);
                fixed4 col = tex2D(_MainTex, i.uv);


				float fogDensity = saturate(depth * _FogDensity);
                fogDensity = pow(fogDensity, _Power);

                float y = Remap(i.uv.y, 0, 0.5, 1, 1);

                float4 fogColor = lerp(col, _FogColor, fogDensity);

	            return fogColor;
            }
            ENDCG
        }
    }
}
