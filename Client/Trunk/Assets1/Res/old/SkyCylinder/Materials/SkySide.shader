Shader "Custom/SkyCylinder/SkySide" {
	Properties {
		_State ("State",range(-0.1,3.1)) = -0.1//1=room,2=game
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
		float _State;
		float4 _Frame;
		float _Bright;
		struct Input {
			float2 uv_RoomTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 room = tex2D (_RoomTex, IN.uv_RoomTex);
			half4 game = tex2D (_GameTex, IN.uv_RoomTex);
			
			float _offset=IN.uv_RoomTex.y+1-_State;
			
			if(_offset>0.1){//room
				o.Albedo = room.rgb;
				o.Emission= _Bright*(room.rgb*0.2+0.2);
			}
			else if(_offset>0){//room>>light
				o.Albedo = room.rgb*_offset*10+_Frame.rgb*(0.1-_offset)*10;
				o.Emission = _Bright*(room.rgb*_offset*2+_offset*2+_Frame.rgb*(0.1-_offset)*8);
			}
			else if(_offset>-0.1){
				o.Albedo = game.rgb*(-_offset)*10+_Frame.rgb*(0.1+_offset)*20;
				o.Emission = _Bright*(game.rgb*(-_offset)*5+_Frame.rgb*(0.1+_offset)*8);
			}
			else{
				o.Albedo = game.rgb;
				o.Emission= _Bright*(game.rgb*0.5);
			}
			

		}
		ENDCG
	} 
	FallBack "Diffuse"
}
