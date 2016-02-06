Shader "FXMaker/Mask Texture Cutout"
{
	Properties
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Mask ("Mask", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range (0,1)) = 0
	}
	SubShader
	{
		Tags {"Queue"="Transparent"}
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest GEqual [_Cutoff]
		Pass
		{
			SetTexture [_Mask] {combine texture}
			SetTexture [_MainTex] {Combine texture * previous}
		}
	}
}
