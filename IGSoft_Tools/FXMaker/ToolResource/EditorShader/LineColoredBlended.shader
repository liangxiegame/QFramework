
Shader "FXMaker/Editor/LineColoredBlended" {
	SubShader {
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha BindChannels { Bind "Color",color } ZWrite On Cull Front Fog { Mode Off }
		}
	}
}

