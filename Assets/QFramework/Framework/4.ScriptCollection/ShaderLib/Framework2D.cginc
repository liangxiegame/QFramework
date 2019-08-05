// Create  by JiepengTan@gmail.com
// https://github.com/JiepengTan/FishManShaderTutorial
// 2018-03-27

#include "FBM.cginc"
#include "Feature.cginc"

sampler2D _MainTex;
float4 _MainTex_ST;

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    return o;
}

float3 ProcessFrag(float2 uv);

float4 frag(v2f i) : SV_Target
{
	return float4(ProcessFrag(i.uv),1.0);
}