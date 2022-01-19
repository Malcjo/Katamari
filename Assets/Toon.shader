Shader "Custom/Toon"
{
	Properties
	{
	//HDR normally rgb values set between 0 and 1
	//HDR allow for colours to be set outside that range
	//Despite screens not being able to render colours outside that range
	//Lights in unity have the ability to do this so it's good to keep it consistent'
		_Color("Color", Color) = (0.5, 0.65, 1, 1)//Set up a basic color
		_MainTex("Main Texture", 2D) = "white" {}	//ability to apply a texture
		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)//Set up a color to change the colour of the shadow

		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		_Glossiness("Glossiness", Float) = 32

		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
	}
	SubShader
	{
		Pass
		{
		Tags
		{
			"LightMode" = "ForwardBase"
			"PassFlags" = "OnlyDirectional"//Sets up that the objects light influence is only from 1 direction light object
		}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_complie_fwdbase
			
			//includes the HLSL or CG librabies from unity
			#include "UnityCG.cginc"
			//allows values from the lights within unity to be accessed
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float3 normal : NORMAL;
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float3 viewDir : TEXCOORD1;
				float3 worldNormal : NORMAL; //normal need to manually populated within the vertex shader
				float4 pos : SV_POSITION;//system value position, outputs the final transformed vertex position
				float2 uv : TEXCOORD0;
				SHADOW_COORDS(2)
			};
			//initialize properties
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Glossiness;
			float4 _SpecularColor;
			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;
			
			//Vertex shader
			v2f vert (appdata v)
			{
				v2f o;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);//populate the normal in the vertex shader
				o.viewDir = WorldSpaceViewDir(v.vertex);
				TRANSFER_SHADOW(o)
				return o;
			}
			
			float4 _Color;
			float4 _AmbientColor;

			//fragment/ pixel shader
			float4 frag (v2f i) : SV_Target
			{
				//compare world normal to light direction using a dot product
				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);

				float shadow = SHADOW_ATTENUATION(i);

				//smoothstep adds a slight fade between the light and dark
				//value is below 0 intensity returns 0
				//value is above 0.01 intensity returns 1
				float lightIntensity = smoothstep(0,0.01, NdotL * shadow);
				float4 light = lightIntensity * _LightColor0;
				
				float3 viewDir = normalize(i.viewDir);

				float3 halfVector = normalize(_WorldSpaceLightPos0 +
				viewDir);
				float NdotH = dot(normal, halfVector);

				//multiply the NdotH but the lightIntensity to ensure the reflection is only drawn when the surface is lit
				//returns the specular intensity raised to the power of _Glossiness * _Glossiness
				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness *
				_Glossiness);

				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;

				//to get the rim get the surface of the object facing away from the camera
				//dot product of the view direction and the surface normalize
				//and then inverting it with 1 - before the dot product
				float4 rimDot = 1 - dot(viewDir, normal);
				//create a threshold using the lighting normal infor and RimThreshold property
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				//toonify it by smooth stepping
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
				float4 rim = rimIntensity * _RimColor;

				float4 sample = tex2D(_MainTex, i.uv);//display the texture

				return _Color * sample * (_AmbientColor + light + specular + rim);
			}
			ENDCG
		}
		//Pass that allows the model to cast and recieve shadows
		UsePass "Legacy Shaders/VertexLit/ShadowCaster"
	}
}