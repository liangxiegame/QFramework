
Shader "FXMaker/Editor/LineBackground" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
		Pass {
			ZWrite on Blend SrcAlpha OneMinusSrcAlpha Colormask RGBA Lighting Off Offset 1, 1 Color[_Color]
		}
	}
}
