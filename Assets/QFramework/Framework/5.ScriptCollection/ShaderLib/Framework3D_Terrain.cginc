// Create  by JiepengTan@gmail.com
// https://github.com/JiepengTan/FishManShaderTutorial
// 2018-04-24
#ifndef FRAMEWORK_3D_DEFAULT_SCENE
#define FRAMEWORK_3D_DEFAULT_SCENE

#include "SDF.cginc"
#include "Framework3D.cginc"

//DEFAULT_RENDER_SKY            default sky box 



float2 TerrainL(float3 pos);
float2 TerrainM(float3 pos);
float2 TerrainH(float3 pos);

float RaycastTerrain(float3 ro, float3 rd) { 
    _MRCRO_RAY_CAST(ro,rd,10000.,TerrainL);  
}
float3 NormalTerrian( in float3 pos, float rz ){
    _MACRO_CALC_NORMAL(pos,rz,TerrainH); 
}

float SoftShadow(in float3 ro, in float3 rd,float tmax){    
    _MACRO_SOFT_SHADOW(ro,rd,tmax,TerrainM);  
}  


#endif // FRAMEWORK_3D_DEFAULT_SCENE