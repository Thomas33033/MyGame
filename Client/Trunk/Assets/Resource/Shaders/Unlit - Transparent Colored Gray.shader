// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Transparent Colored Gray"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}
	
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask [_ColorMask]
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			
			//ScrollView不遮挡修改 chenxi
			bool _UseClipRect;
			fixed4 _ClipRect;

			struct appdata_t
			{
				fixed4 vertex : POSITION;
				half4 color : COLOR;
				fixed2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				fixed4 vertex : POSITION;
				half4 color : COLOR;
				fixed2 texcoord : TEXCOORD0;
				fixed4 worldPosition : TEXCOORD1;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPosition = v.vertex;
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			half4 frag (v2f IN) : COLOR
			{
				// Sample the texture
				half4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
				fixed c = 0.299*col.r + 0.587*col.g + 0.184*col.b;
				col.r = col.g = col.b = c;
				if (_UseClipRect)
					col *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				return col;
			}
			ENDCG
		}
	}
}
