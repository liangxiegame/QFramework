// Create  by JiepengTan@gmail.com
// https://github.com/JiepengTan/FishManShaderTutorial
// 2018-03-27
#ifndef FRAMEWORK_3D
#define FRAMEWORK_3D

#include "FBM.cginc"
#include "Feature.cginc"
sampler2D _MainTex;
float2 _MainTex_TexelSize;
float4x4 _FrustumCornersRay;
sampler2D _CameraDepthTexture;

struct v2f {
    float4 pos : SV_POSITION;
    half2 uv : TEXCOORD0;
    half2 uv_depth : TEXCOORD1;
    float4 interpolatedRay : TEXCOORD2;
};

v2f vert(appdata_img v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);

    o.uv = v.texcoord;
    o.uv_depth = v.texcoord;

#if UNITY_UV_STARTS_AT_TOP
    if (_MainTex_TexelSize.y < 0)
        o.uv_depth.y = 1 - o.uv_depth.y;
#endif

    int index = 0;
    if (v.texcoord.x < 0.5 && v.texcoord.y < 0.5) {
        index = 0;
    }
    else if (v.texcoord.x > 0.5 && v.texcoord.y < 0.5) {
        index = 1;
    }
    else if (v.texcoord.x > 0.5 && v.texcoord.y > 0.5) {
        index = 2;
    }
    else {
        index = 3;
    }

#if UNITY_UV_STARTS_AT_TOP
    if (_MainTex_TexelSize.y < 0)
        index = 3 - index;
#endif
    o.interpolatedRay = _FrustumCornersRay[index];
    return o;
}

//RayMarching is the main world, ignore unity sky box
void MergeUnityIntoRayMarching(inout float rz,inout float3 rCol, float unityDep,float4 unityCol){
    if(rz>unityDep && unityDep<_ProjectionParams.z-1.){// unity camera far plane 
        rCol = unityCol.xyz;
        rz = unityDep;
    }
}
//Unity scene is the main world, ignore RayMarching sky box
void MergeRayMarchingIntoUnity(inout float rz,inout float3 rCol, float unityDep,float4 unityCol){
    if(rz>unityDep ){
        rCol = unityCol.xyz;
        rz = unityDep;
    }
}           


float4 ProcessRayMarch(float2 uv,float3 ro,float3 rd,inout float sceneDep,float4 sceneCol);

float4 frag(v2f i) : SV_Target{
    float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth));
    depth *= length(i.interpolatedRay.xyz);
    fixed4 sceneCol = tex2D(_MainTex, i.uv);
    float2 uv = i.uv * float2(_ScreenParams.x/_ScreenParams.y,1.0);
    fixed3 ro = _WorldSpaceCameraPos;
    fixed3 rd = normalize(i.interpolatedRay.xyz);
    return ProcessRayMarch(uv,ro,rd,depth,sceneCol);
}


#define _MACRO_CALC_NORMAL(pos,rz, MAP_FUNC)\
    float2 e = float2(1.0,-1.0)*0.5773*0.002*rz;\
    return normalize( e.xyy*MAP_FUNC( pos + e.xyy ).x + \
                        e.yyx*MAP_FUNC( pos + e.yyx ).x + \
                        e.yxy*MAP_FUNC( pos + e.yxy ).x + \
                        e.xxx*MAP_FUNC( pos + e.xxx ).x );


#define _MACRO_SOFT_SHADOW(ro, rd, maxH,MAP_FUNC) \
    float res = 1.0;\
    float t = 0.001;\
    for( int i=0; i<80; i++ ){\
        float3  p = ro + t*rd;\
        float h = MAP_FUNC( p).x;\
        res = min( res, 16.0*h/t );\
        t += h;\
        if( res<0.001 ||p.y> maxH ) break;\
    }\
    return clamp( res, 0.0, 1.0 );


#define _MRCRO_RAY_CAST( ro, rd ,tmax,MAP_FUNC)\
    float t = .1;\
    float m = -1.0;\
    for( int i=0; i<256; i++ ) {\
        float precis = 0.0005*t;\
        float2 res = MAP_FUNC( ro+rd*t );\
        if( res.x<precis || t>tmax ) break;\
        t += 0.5*res.x;\
        m = res.y;\
    } \
    if( t>tmax ) m=-1.0;\
    return float2( t, m );




#endif