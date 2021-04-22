Shader "ZTGame/Scene/Scene_A"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecularColor("SpecularColor",Color )=(1,1,1,1)
        _SpecularGloss("Gloss",Range(8.0,256)) = 20
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags {"Queue"="AlphaTest"}
        //LOD 100
        Cull Off
        // Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            // #include "Lighting.cginc"
            // #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldNormal :TEXCOORD1;
                float3 worldPos :TEXCOORD2;
                // SHADOW_COORDS(3)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _SpecularColor;
            float  _SpecularGloss;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos  = mul(unity_ObjectToWorld,v.vertex).xyz;
                // TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - _Cutoff);
                return col;
                // float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                // float3 N =  normalize(i.worldNormal);
                // float3 L = normalize(_WorldSpaceLightPos0.xyz);
                // float3 diffuseColor = _LightColor0.rgb * _Color.rgb * max(0.0,dot(N,L));
                // float3 V = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                // float3 H = normalize(L+V);
                // float3 specularColor = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0,dot(H,N)),_SpecularGloss);
                // fixed shadow = SHADOW_ATTENUATION(i);
                // col.rgb =  col.rgb * float3 ((ambient+diffuseColor+specularColor) * shadow);
                // return col;
            }
            ENDCG
        }
    }
    Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
