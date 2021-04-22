// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ZTGame/UIMaskShader" {
	Properties{
		_MainTex("MainTex", 2D) = "white" {}
		_MaskTex("MaskTex", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_Alpha("Alpha", Range(0,1)) = 1
	}
	SubShader{
		Tags{
			"IgnoreProjector" = "True"
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"CanUseSpriteAtlas" = "True"
			"PreviewType" = "Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex; 
			float4 _MainTex_ST;
			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			float _Alpha;

			struct vi
			{
				float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;
			};
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};
			v2f vert(vi v)
			{
				v2f o;
				o.uv0 = TRANSFORM_TEX(v.texcoord0,_MainTex);
				o.uv1 = TRANSFORM_TEX(v.texcoord0,_MaskTex);

				o.vertex = UnityObjectToClipPos(v.vertex);
				
				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
				#endif

				return o;
			}
			float4 frag(v2f i) : COLOR
			{
				float4 c = tex2D(_MainTex,i.uv0);
				float4 mc = tex2D(_MaskTex,i.uv1);
				c.a=c.a*(mc.a) * _Alpha;
				return c;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
