Shader "ZTGame/Scene/Scene_S" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}		
		_LightEffectColor("Light Effect Color", Color) = (1,1,1,1)		
		_ShadowWeight("Shadow Weight(自阴影强度)", Range (0, 1)) = 0.5
		_customLightDir ("LightDir(光照方向)", Vector) = (0,0,0,0)
		_customLightColor ("LightColor(光照颜色)", Color) = (1,1,1,1)
		_customLightMultiplier ("LightMultiplier(光照倍数)", Range(0,10)) = 1
		_HeightColor ("HeightColor", Color) = (1,1,1,1)
		_HeightFix ("HeightFix", Range(0,5)) = 1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" "LightMode"="ForwardBase"}
		Cull off
		Pass{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float4 diffAndSpec : TEXCOORD0; // _MainTex _SpecularTex
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_SHADOW_COORDS(5)
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;

			float4 _MainTex_ST;

			float4 _LightEffectColor;
			float _ShadowWeight;
			half4 _customLightDir;
			float4 _customLightColor;
			float _customLightMultiplier;

			half _HeightOffset;
			fixed4 _HeightColor;
			fixed _HeightFix;
			half _HeightIntensity;

			// vertex shader
			v2f vert(appdata_full v) {
				UNITY_SETUP_INSTANCE_ID(v);
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.diffAndSpec.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
				o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);				
				UNITY_TRANSFER_SHADOW(o, v.texcoord1.xy); // pass shadow coordinates to pixel shader
				return o;
			}


			// fragment shader
			fixed4 frag(v2f IN) : SV_Target{
				UNITY_SETUP_INSTANCE_ID(IN);

				float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
				fixed3 worldN = float3(IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z);
				fixed3 lightDir = normalize(_customLightDir.xyz);
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				float3 normal_view = mul((float3x3)UNITY_MATRIX_V, worldN);
				float4 baseTexColor = tex2D(_MainTex, IN.diffAndSpec.xy);
				float3 emission = (baseTexColor.rgb * _LightEffectColor.rgb);

				// compute lighting & shadowing factor
				UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)

				//场景光源高光
				half3 h = normalize(lightDir + worldViewDir);
				float nh = max(0, dot(worldN, h));
				float spec = pow(nh, 48.0);
				fixed4 c;
				c.rgb = _customLightColor.rgb * _customLightMultiplier * spec;
				
				//光效受阴影影响的程度受参数_ShadowWeight控制
				c.rgb += lerp(emission, emission * atten, _ShadowWeight);

				c.rgb *= _HeightColor.rgb * _HeightFix;

				UNITY_OPAQUE_ALPHA(c.a);
				return c;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}