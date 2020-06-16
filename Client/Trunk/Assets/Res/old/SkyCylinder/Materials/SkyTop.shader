Shader "Custom/SkyCylinder/SkyTop" {
	Properties {
		_State ("State",range(-0.1,3.1)) = -0.1//2=room,3=game
		_Frame ("Frame Color", Color) = (.7,.9,1,1)//180,225,255
		_RoomTex ("RoomTex", 2D) = "white" {}
		_GameTex ("GameTex", 2D) = "white" {}
		_Bright ("Bright",range(0,2)) = 1
	}
	SubShader {
		Tags { "Queue"="Background" "RenderType"="Background" }
		Cull front
		ZWrite off
	//	ZTest Always
		Fog { Mode Off }
		Lighting Off	
		
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0

		sampler2D _RoomTex;
		sampler2D _GameTex;
		float4 _Frame;
		float _State;
		float _Bright;
		struct Input {
			float2 uv_RoomTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 room = tex2D (_RoomTex, IN.uv_RoomTex);
			half4 game = tex2D (_GameTex, IN.uv_RoomTex);
			
			float r=sqrt(pow(abs(IN.uv_RoomTex.x-0.5),2)+pow(abs(IN.uv_RoomTex.y-0.5),2));
			float _offset=(3-_State)*0.5-r;
			if(_offset>0.05){//room
				o.Albedo = room.rgb;
				o.Emission= _Bright*(room.rgb*0.2+0.2);
			}
			else if(_offset>0){//room>>light
				o.Albedo = room.rgb*_offset*20+_Frame.rgb*(0.05-_offset)*20;
				o.Emission = _Bright*(room.rgb*_offset*4+_offset*4+_Frame.rgb*(0.05-_offset)*16);
			}
			else if(_offset>-0.05){
				o.Albedo = game.rgb*(-_offset)*20+_Frame.rgb*(0.05+_offset)*20;
				o.Emission = _Bright*(game.rgb*(-_offset)*10+_Frame.rgb*(0.05+_offset)*16);
			}
			else{
				o.Albedo = game.rgb;
				o.Emission= _Bright*(game.rgb*0.5);
			}
	
			//o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
