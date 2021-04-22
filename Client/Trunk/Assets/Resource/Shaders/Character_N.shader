Shader "ZTGame/Character/Character_N" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}		
		_LightEffectColor("Light Effect Color", Color) = (1,1,1,1)
		_LightTex("LightTex 轮廓光", 2D) = "white" {}
		_SpecularMultiplier("LightTex Spread(轮廓光倍数)", Range (0, 3)) = 0
		_NormalTex("Normal", 2D) = "bump" {}		
		_ReflectTex("Reflect(反射贴图)", 2D) = "white" {}
		_ReflectColor("Reflect Color(反射颜色)", Color) = (1,1,1,1)
		_ReflectPower("Reflect Power(反射强度)", Range(0.1,5)) = 1
		_ReflectionMultiplier("Reflection Multiplier(反射倍数)", Float) = 1.5		
		_ShadowWeight("Shadow Weight(自阴影强度)", Range (0, 1)) = 0.5
		_SpecularTex("Specular Texture(权重图)", 2D) = "white" {}
		_selfColor("self Color(自发光颜色)", Color) = (1,1,1,1)
		_selfColorMultiplier("self Color Multiplier(自发光倍数)", Range (0, 2)) = 0
		_customLightDir ("LightDir(光照方向)", Vector) = (0,0,0,0)
		_customLightColor ("LightColor(光照颜色)", Color) = (1,1,1,1)
		_customLightMultiplier ("LightMultiplier(光照倍数)", Range(0,10)) = 1
		_ScatteringColor ("ScatteringColor(散射)", Color) = (0.5,0.5,0.5,1)
		_ScatteringPower ("ScatteringFW", Range(0,10)) = 0
		_ScatteringMultiplier ("ScatteringMW", Range(0,10)) = 0
		_HeightColor ("HeightColor", Color) = (1,1,1,1)
		_HeightOffset ("HeightOffset", Float) = 0
		_HeightFix ("HeightFix", Range(0,5)) = 1
		_HeightIntensity("HeightIntensity(渐变长度)", Range(0.01,10)) = 1
		_RimColor("Rim Color(闪白颜色)", Color) = (0, 0, 0, 0)
		_RimPower("Rim Power(闪白强度)", Range(0,6)) = 6
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
				float3 worldNormal : NORMAL;
				UNITY_SHADOW_COORDS(5)
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			sampler2D _SpecularTex;
			sampler2D _NormalTex;
			sampler2D _LightTex;
			sampler2D _ReflectTex;

			float4 _MainTex_ST;
			float4 _SpecularTex_ST;
			float4 _NormalTex_ST;
			float4 _LightTex_ST;
			float4 _ReflectTex_ST;

			float4 _selfColor;
			float _selfColorMultiplier;
			float3 _ReflectColor;
			float4 _LightEffectColor;
			float _ShadowWeight;
			float _SpecularMultiplier;
			float _ReflectPower;
			float _ReflectionMultiplier;
			half4 _customLightDir;
			float4 _customLightColor;
			float _customLightMultiplier;
			float4 _ScatteringColor;			
			float _ScatteringPower;
			float _ScatteringMultiplier;
			
			half _HeightOffset;
			fixed4 _HeightColor;
			fixed _HeightFix;
			half _HeightIntensity;
			float _RimPower;
			fixed4 _RimColor;

			// vertex shader
			v2f vert(appdata_full v) {
				UNITY_SETUP_INSTANCE_ID(v);
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.diffAndSpec.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.diffAndSpec.zw = TRANSFORM_TEX(v.texcoord, _SpecularTex);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
				o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);	
				o.worldNormal = worldNormal;
				UNITY_TRANSFER_SHADOW(o, v.texcoord1.xy); // pass shadow coordinates to pixel shader
				return o;
			}


			// fragment shader
			fixed4 frag(v2f IN) : SV_Target{
				UNITY_SETUP_INSTANCE_ID(IN);

				float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
				fixed3 lightDir = normalize(_customLightDir.xyz);
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				//去掉接收灯光颜色
				float3 normal = UnpackNormal(tex2D(_NormalTex, IN.diffAndSpec.xy)); 		//去掉normal
				float4 Specular = tex2D(_SpecularTex, IN.diffAndSpec.zw);

				fixed3 worldN;
				worldN.x = dot(IN.tSpace0.xyz, normal);
				worldN.y = dot(IN.tSpace1.xyz, normal);
				worldN.z = dot(IN.tSpace2.xyz, normal);
				worldN = normalize(worldN);
				float3 normal_view = mul((float3x3)UNITY_MATRIX_V, worldN);
				float4 baseTexColor = tex2D(_MainTex, IN.diffAndSpec.xy);
				baseTexColor.rgb += baseTexColor.rgb*Specular.a*_selfColor.rgb* _selfColorMultiplier;
				float3 LunkuoGuang = (baseTexColor.rgb + 0.15) * (tex2D(_LightTex, ((normal_view.xy * 0.5) + 0.5)) * 1.2).rgb * Specular.r * _SpecularMultiplier;
				float3 color_plus_noise = LunkuoGuang + baseTexColor.rgb;
				float3 fanGuang = (color_plus_noise * pow((tex2D(_ReflectTex, ((normal_view.xy * 0.5) + 0.5)).rgb * _ReflectColor), _ReflectPower)) * _ReflectionMultiplier;
				float3 liuGuangColor = lerp(color_plus_noise, fanGuang, Specular.g);
				float3 emission = (liuGuangColor * _LightEffectColor.rgb);

				// compute lighting & shadowing factor
				UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)

				//场景光源高光
				half3 h = normalize(lightDir + worldViewDir);
				float nh = max(0, dot(worldN, h));
				float spec = pow(nh, 48.0);
				fixed4 c;
				c.rgb = Specular.b * _customLightColor.rgb * _customLightMultiplier * spec * lerp(1, atten * 2, _SpecularMultiplier);//场景光源高光
				
				
				
				//暂时这样处理，为让阴影明暗明显，让光线特效也受到阴影影响
				//光效受阴影影响的程度受参数_ShadowWeight控制
				c.rgb += lerp(emission, emission * atten, _ShadowWeight);
				c.rgb += max(0.0, dot(worldN, lightDir)) * pow(1.0 - max (0.0, dot (worldN, worldViewDir)), _ScatteringPower) * _ScatteringMultiplier * _ScatteringColor.rgb;
				
				half heightSign = saturate((worldPos.y - (mul(unity_ObjectToWorld, fixed4(0.0, 0.0, 0.0, 1.0)).y + _HeightOffset)) / _HeightIntensity);
				float3 heightColor = lerp(_HeightColor.rgb * _HeightFix, fixed3(1.0, 1.0, 1.0), heightSign) ;
				c.rgb *= heightColor;

				//c.rgb = normal_view;
				UNITY_OPAQUE_ALPHA(c.a);

				//闪白
				half rim = 1.0 - saturate(dot(normalize(worldViewDir), IN.worldNormal));
				c.rgb += _RimColor.rgb * pow(rim, _RimPower);

				return c;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}