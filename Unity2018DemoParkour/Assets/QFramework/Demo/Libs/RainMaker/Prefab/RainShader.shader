// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RainShader"
{
    Properties
	{
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "gray" {}
		_TintColor ("Tint Color (RGB)", Color) = (1, 1, 1, 1)
		_PointSpotLightMultiplier ("Point/Spot Light Multiplier", Range (0, 10)) = 2
		_DirectionalLightMultiplier ("Directional Light Multiplier", Range (0, 10)) = 1
		_InvFade ("Soft Particles Factor", Range(0.01, 3.0)) = 1.0
		_AmbientLightMultiplier ("Ambient light multiplier", Range(0, 1)) = 0.25
    }

    SubShader
	{
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "LightMode"="Vertex" }
		LOD 100

        Pass
		{
			ZWrite Off
			Cull Back
            Lighting On     
			AlphaTest Greater 0.01
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
						 
            CGPROGRAM

            #pragma multi_compile_particles
            #pragma vertex vert
            #pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"
			#include "Lighting.cginc"

			fixed4 _TintColor;
			float _DirectionalLightMultiplier;
			float _PointSpotLightMultiplier;
			float _AmbientLightMultiplier;

			#if defined(SOFTPARTICLES_ON)
			float _InvFade;
			#endif

			struct appdata_t
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

		    struct v2f
            {
                half2 uv_MainTex : TEXCOORD0;
                fixed4 color : COLOR0;
                float4 pos : SV_POSITION;
				#if defined(SOFTPARTICLES_ON)
                float4 projPos : TEXCOORD1;
                #endif
            };

			float3 ApplyLight(int index, float3 lightColor, float3 viewPos)
			{
				fixed3 currentLightColor = unity_LightColor[index].rgb;
				float4 lightPos = unity_LightPosition[index];

				if (lightPos.w == 0)
				{
					// directional light, the lightPos is actually the direction of the light
					// for some weird reason, Unity seems to change the directional light position based on the vertex,
					// this hack seems to compensate for that
					lightPos = mul(lightPos, UNITY_MATRIX_V);

					// depending on how the directional light is pointing, reduce the intensity (which goes to 0 as it goes below the horizon)
					fixed multiplier = clamp((lightPos.y * 2) + 1, 0, 1);
					return lightColor + (currentLightColor * multiplier * _DirectionalLightMultiplier);
				}
				else
				{
					float3 toLight = lightPos.xyz - viewPos;
	                fixed lengthSq = dot(toLight, toLight);
	                fixed atten = 1.0 / (1.0 + (lengthSq * unity_LightAtten[index].z));
					return lightColor + (currentLightColor * atten * _PointSpotLightMultiplier);
				}
			}
 
            fixed4 LightForVertex(float4 vertex)
            {
                float3 viewPos = mul(UNITY_MATRIX_MV, vertex).xyz;
                fixed3 lightColor = UNITY_LIGHTMODEL_AMBIENT.rgb * _AmbientLightMultiplier;

				lightColor = ApplyLight(0, lightColor, viewPos);
				lightColor = ApplyLight(1, lightColor, viewPos);
				lightColor = ApplyLight(2, lightColor, viewPos);
				lightColor = ApplyLight(3, lightColor, viewPos);

                return fixed4(lightColor, 1);
            }
 
            float4 _MainTex_ST;
 
            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = LightForVertex(v.vertex) * v.color * _TintColor;

				// o.color = v.color * _TintColor; // temp if you want to disable lighting

				// make sure the alpha scales down with the light
				o.color *= (min(o.color.rgb, _TintColor.a).r / _TintColor.a);

				#if defined(SOFTPARTICLES_ON)
                o.projPos = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif

                return o; 
            }
			
			#if defined(SOFTPARTICLES_ON)
			sampler2D _CameraDepthTexture;
			#endif
			
			sampler2D _MainTex;
  
            fixed4 frag (v2f i) : COLOR {
            
				#if defined(SOFTPARTICLES_ON)
                float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
                float partZ = i.projPos.z;
                i.color.a *= saturate (_InvFade * (sceneZ - partZ));
				#endif

				return tex2D(_MainTex, i.uv_MainTex) * i.color;
            }
            ENDCG
        }
    }
 
    Fallback "Particles/Alpha Blended"
}