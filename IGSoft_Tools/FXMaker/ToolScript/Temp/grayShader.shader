Shader "GrayscaleLolTransparent" {
    Properties {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }

    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		Blend SrcAlpha One
		AlphaTest Greater .01
		ColorMask RGB

        CGPROGRAM
        #pragma surface surf Lambert alpha

            sampler2D _MainTex;

            struct Input {
                float2 uv_MainTex;
            };

            void surf (Input IN, inout SurfaceOutput o) {
                half4 c = tex2D(_MainTex, IN.uv_MainTex);
                
 				o.Albedo = dot(c.rgb, float3(0.3, 0.59, 0.11)) + 252.0;
//				fixed grayscale = Luminance(c.rgb);
//				o.Albedo = grayscale * 0.3 + 2.0;
				
				o.Alpha = (c.a * (c.r+c.g+c.b)/3);
            }

        ENDCG
    }

    Fallback "Transparent/VertexLit"
}
