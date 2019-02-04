// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CooleWaterJongeke"
{
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, .5) 
		_MainTex ("Main Texture", 2D) = "white" {}
		_TextureDistort("Texture Wobble", range(0,1)) = 0.2
		_NoiseTex("Extra Wave Noise", 2D) = "white" {}
		_Speed("Wave Speed", Range(0,1)) = 0.5
		_Amount("Wave Amount", Range(0,1)) = 0.5
		_Height("Wave Height", Range(0,1)) = 0.5
		_Foam("Foamline Thickness", Range(0,5)) = 0.5
		_BumpMap("Distortion Map", 2D) = "bump" {}
		_BumpAmt("Distortion", range(0,300)) = 10
		_Color("Deep Tint", Color) = (1, 1, 1, .5)
		_ScrollingBump("Bump Map", 2D) = "White" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"  "Queue" = "Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		GrabPass{
			Name "BASE"
			Tags{ "LightMode" = "Always" }
				}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD3;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 uvgrab : TEXCOORD0;
				float2 uvbump : TEXCOORD1;
				float4 scrPos : TEXCOORD2;
				float4 worldPos : TEXCOORD4;
			};
			float _BumpAmt, _TextureDistort;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float4 _Tint;
			float4 _Color;
			sampler2D _CameraDepthTexture; // depth (gejat)
			sampler2D _MainTex, _NoiseTex, _ScrollingBump;
			sampler2D _BumpMap;
			float4 _MainTex_ST;
			float4 _BumpMap_ST;
			float _Speed, _Amount, _Height, _Foam;
			
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				float4 tex = tex2Dlod(_NoiseTex, float4(v.uv.xy, 0, 0));// extra noise voor water
				v.vertex.y += sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount * tex)) * _Height;// movement
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float scale = -1.0;
				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;
				o.scrPos = ComputeScreenPos(o.vertex); // screenpos 
				o.uvbump = TRANSFORM_TEX(v.uv, _BumpMap);
				UNITY_TRANSFER_FOG(o,o.vertex);

				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				half2 bump = UnpackNormal(tex2D(_BumpMap, i.uvbump+ (_Time.x * 0.2))).rg;   
				float2 offset = bump * (_BumpAmt * 10) * _GrabTexture_TexelSize.xy ;
				i.uvgrab.xy = offset *  UNITY_Z_0_FAR_FROM_CLIPSPACE(i.uvgrab.z) + i.uvgrab.xy;
				half4 dis = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab )) * _Color;
				half4 col = tex2D(_MainTex, i.uv + (offset * _TextureDistort)) * _Tint;// texture * tint;
				
				half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos ))); // depth
				half4 foamLine =1 - saturate(_Foam* (depth - i.scrPos.w ) ) ;// foam line (depth 'n screenpos) (gejat)
			
				col += (foamLine * _Tint); // hahahaha niet hiermee kloten het maakt alles wit als je het aan zet 
				col = (col + dis) * col.a ;
				
				return   col;
			}
			ENDCG
		}
	}
}
