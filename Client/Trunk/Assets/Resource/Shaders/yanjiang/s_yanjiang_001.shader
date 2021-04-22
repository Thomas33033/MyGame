// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:2,spmd:1,trmd:0,grmd:0,uamb:False,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.3511902,fgcg:0.266436,fgcb:0.5661765,fgca:1,fgde:0.01,fgrn:0,fgrf:80,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:33458,y:32262,varname:node_4795,prsc:2|diff-5283-OUT,emission-2393-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32169,y:32364,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3749717253fc967489e720ceecd78061,ntxv:0,isnm:False|UVIN-4336-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:33070,y:32422,varname:node_2393,prsc:2|A-9250-OUT,B-2053-RGB,C-797-RGB,D-7791-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32738,y:32373,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:32438,y:32664,ptovrint:True,ptlb:MainColor,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.8093306,c3:0.3088235,c4:1;n:type:ShaderForge.SFN_Panner,id:4769,x:30968,y:32312,varname:node_4769,prsc:2,spu:0,spv:0|UVIN-5758-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:5758,x:30793,y:32297,varname:node_5758,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:5844,x:31405,y:32224,ptovrint:False,ptlb:noise,ptin:_noise,varname:node_5844,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3749717253fc967489e720ceecd78061,ntxv:0,isnm:False|UVIN-5569-OUT;n:type:ShaderForge.SFN_Add,id:4336,x:31934,y:32381,varname:node_4336,prsc:2|A-6874-OUT,B-2382-OUT;n:type:ShaderForge.SFN_Panner,id:6752,x:31440,y:32650,varname:node_6752,prsc:2,spu:0,spv:0|UVIN-9339-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:9339,x:31232,y:32650,varname:node_9339,prsc:2,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:7791,x:32909,y:32567,ptovrint:False,ptlb:MainLlight,ptin:_MainLlight,varname:node_7791,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Tex2d,id:517,x:31887,y:31916,ptovrint:False,ptlb:diffuse,ptin:_diffuse,varname:node_517,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ede6be5addcbc804faf034cdca138d73,ntxv:0,isnm:False|UVIN-1144-OUT;n:type:ShaderForge.SFN_Panner,id:9104,x:31332,y:31862,varname:node_9104,prsc:2,spu:0,spv:0|UVIN-8960-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:8960,x:31172,y:31893,varname:node_8960,prsc:2,uv:0;n:type:ShaderForge.SFN_Power,id:8709,x:32437,y:32152,varname:node_8709,prsc:2|VAL-6074-RGB,EXP-1042-OUT;n:type:ShaderForge.SFN_Desaturate,id:9250,x:32786,y:32150,varname:node_9250,prsc:2|COL-8709-OUT,DES-7821-OUT;n:type:ShaderForge.SFN_Multiply,id:3746,x:33070,y:32662,varname:node_3746,prsc:2|A-3349-A,B-2053-A,C-797-A;n:type:ShaderForge.SFN_Time,id:1397,x:30308,y:31980,varname:node_1397,prsc:2;n:type:ShaderForge.SFN_Slider,id:2876,x:30197,y:32137,ptovrint:False,ptlb:speed,ptin:_speed,varname:node_2876,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.005,max:1;n:type:ShaderForge.SFN_Multiply,id:4921,x:30590,y:31977,varname:node_4921,prsc:2|A-1397-T,B-2876-OUT;n:type:ShaderForge.SFN_Vector4Property,id:1989,x:30354,y:32263,ptovrint:False,ptlb:UVnoise,ptin:_UVnoise,varname:node_1989,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_ComponentMask,id:5428,x:30566,y:32263,varname:node_5428,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-1989-XYZ;n:type:ShaderForge.SFN_Multiply,id:3033,x:30933,y:32114,varname:node_3033,prsc:2|A-4921-OUT,B-5428-OUT;n:type:ShaderForge.SFN_Add,id:5569,x:31191,y:32224,varname:node_5569,prsc:2|A-3033-OUT,B-4769-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:4294,x:31149,y:32499,varname:node_4294,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-5330-XYZ;n:type:ShaderForge.SFN_Vector4Property,id:5330,x:30890,y:32502,ptovrint:False,ptlb:UVmain,ptin:_UVmain,varname:node_5330,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Multiply,id:2736,x:31405,y:32480,varname:node_2736,prsc:2|A-4921-OUT,B-4294-OUT;n:type:ShaderForge.SFN_Add,id:2382,x:31697,y:32448,varname:node_2382,prsc:2|A-2736-OUT,B-6752-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:854,x:31393,y:32420,ptovrint:False,ptlb:size,ptin:_size,varname:node_854,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:6874,x:31649,y:32257,varname:node_6874,prsc:2|A-5844-R,B-854-OUT;n:type:ShaderForge.SFN_ComponentMask,id:7569,x:30927,y:31828,varname:node_7569,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-7043-XYZ;n:type:ShaderForge.SFN_Vector4Property,id:7043,x:30753,y:31828,ptovrint:False,ptlb:UVdiffuse,ptin:_UVdiffuse,varname:node_7043,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Multiply,id:2068,x:31109,y:31760,varname:node_2068,prsc:2|A-7569-OUT,B-4921-OUT;n:type:ShaderForge.SFN_Add,id:8976,x:31514,y:31836,varname:node_8976,prsc:2|A-2068-OUT,B-9104-UVOUT;n:type:ShaderForge.SFN_Add,id:1144,x:31699,y:31916,varname:node_1144,prsc:2|A-8976-OUT,B-6874-OUT;n:type:ShaderForge.SFN_Power,id:346,x:32092,y:31626,varname:node_346,prsc:2|VAL-517-RGB,EXP-2117-OUT;n:type:ShaderForge.SFN_Desaturate,id:5563,x:32438,y:31631,varname:node_5563,prsc:2|COL-346-OUT,DES-2851-OUT;n:type:ShaderForge.SFN_Multiply,id:5283,x:32755,y:31646,varname:node_5283,prsc:2|A-5563-OUT,B-9760-RGB;n:type:ShaderForge.SFN_Color,id:9760,x:32613,y:31800,ptovrint:False,ptlb:diffuseColor,ptin:_diffuseColor,varname:node_9760,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:7821,x:32588,y:32248,ptovrint:False,ptlb:MainDesaturate,ptin:_MainDesaturate,varname:node_7821,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:2117,x:32092,y:31789,ptovrint:False,ptlb:diffusePower,ptin:_diffusePower,varname:node_2117,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2851,x:32404,y:31786,ptovrint:False,ptlb:diffuseDesaturate,ptin:_diffuseDesaturate,varname:node_2851,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:1042,x:32227,y:32175,ptovrint:False,ptlb:MainPower,ptin:_MainPower,varname:node_1042,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Tex2d,id:3349,x:32799,y:32890,ptovrint:False,ptlb:blendTex,ptin:_blendTex,varname:node_3349,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-552-UVOUT;n:type:ShaderForge.SFN_Panner,id:552,x:32609,y:32877,varname:node_552,prsc:2,spu:0,spv:0.05|UVIN-131-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:131,x:32418,y:32848,varname:node_131,prsc:2,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:4046,x:32852,y:32795,ptovrint:False,ptlb:blend,ptin:_blend,varname:node_4046,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:6074-797-5844-7791-517-1989-2876-5330-854-7043-9760-7821-2117-2851-1042-3349-4046;pass:END;sub:END;*/

Shader "ZT/Particle/yanjiang_niuqu001" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("MainColor", Color) = (1,0.8093306,0.3088235,1)
        //_noise ("noise", 2D) = "white" {}
		_noiseTileX("noiseTileX",Float) = 0.4
		_noiseTileY ("noiseTileY",Float)=0.3
        _MainLlight ("MainLlight", Float ) = 2
        //_diffuse ("diffuse", 2D) = "white" {}
		_diffuseTileX("diffuseTileX",Float)=0.5
		_diffuseTileY("diffuseTileY",Float)=0.5
        _UVnoise ("UVnoise", Vector) = (0,0,0,0)
        _speed ("speed", Range(0, 1)) = 0.005
        _UVmain ("UVmain", Vector) = (0,0,0,0)
        _size ("size", Float ) = 0.1
        _UVdiffuse ("UVdiffuse", Vector) = (0,0,0,0)
        _diffuseColor ("diffuseColor", Color) = (1,1,1,1)
        _MainDesaturate ("MainDesaturate", Float ) = 0.5
        _diffusePower ("diffusePower", Float ) = 1
        _diffuseDesaturate ("diffuseDesaturate", Float ) = 0.5
        _MainPower ("MainPower", Float ) = 2
        _blendTex ("blendTex", 2D) = "white" {}
        _blend ("blend", Float ) = 1
    }
    SubShader {
        Tags {
            	"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
        }
		
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			uniform sampler2D _blendTex;
            uniform float4 _TintColor;
            //uniform sampler2D _noise; uniform float4 _noise_ST;
			uniform float _noiseTileX; uniform float _noiseTileY;
            uniform float _MainLlight;
            //uniform sampler2D _diffuse; uniform float4 _diffuse_ST;
			uniform float _diffuseTileX; uniform float _diffuseTileY;
            uniform float _speed;
            uniform float4 _UVnoise;
            uniform float4 _UVmain;
            uniform float _size;
            uniform float4 _UVdiffuse;
            uniform float4 _diffuseColor;
            uniform float _MainDesaturate;
            uniform float _diffusePower;
            uniform float _diffuseDesaturate;
            uniform float _MainPower;



            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 node_1397 = _Time + _TimeEditor;
                float node_4921 = (node_1397.g*_speed);
                float4 node_6236 = _Time + _TimeEditor;
                float2 node_5569 = ((node_4921*_UVnoise.rgb.rg)+(i.uv0+node_6236.g*float2(0,0)));
				node_5569 *= float2(_noiseTileX, _noiseTileY);
				//float4 _noise_var = tex2D(_noise, TRANSFORM_TEX(node_5569, _noise));
				float4 _noise_var = tex2D(_MainTex,node_5569);
                float node_6874 = (_noise_var.r*_size);
                float2 node_1144 = (((_UVdiffuse.rgb.rg*node_4921)+(i.uv0+node_6236.g*float2(0,0)))+node_6874);
				node_1144 *= float2(_diffuseTileX, _diffuseTileY);
				//float4 _diffuse_var = tex2D(_diffuse, TRANSFORM_TEX(node_1144, _diffuse));
				float4 _diffuse_var = tex2D(_MainTex,node_1144);
                float3 diffuseColor = (lerp(pow(_diffuse_var.rgb,_diffusePower),dot(pow(_diffuse_var.rgb,_diffusePower),float3(0.3,0.59,0.11)),_diffuseDesaturate)*_diffuseColor.rgb);
                float3 diffuse = directDiffuse * diffuseColor;
////// Emissive:
                float2 node_4336 = (node_6874+((node_4921*_UVmain.rgb.rg)+(i.uv0+node_6236.g*float2(0,0))));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4336, _MainTex));
                float3 emissive = (lerp(pow(_MainTex_var.rgb,_MainPower),dot(pow(_MainTex_var.rgb,_MainPower),float3(0.3,0.59,0.11)),_MainDesaturate)*i.vertexColor.rgb*_TintColor.rgb*_MainLlight);
/// Final Color:
				fixed4 alpha = tex2D(_blendTex,i.uv0);
                float3 finalColor = diffuse + emissive;
                
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
				fixed4 finalRGBA = fixed4(finalColor*alpha.a,alpha.a);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
